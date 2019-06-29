using BusinessModel;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RedisLayer.Class
{
    public class BidBuilder
    {
        public bool SaveBid(BidDetails bid)
        {
            try
            {
                if (bid != null)
                {

                    RedisKeyParameters redisObj = new RedisKeyParameters();
                    redisObj.UserId = bid.UserId;
                    redisObj.ProductId = bid.ProductId;

                    string bidHashAllKeyByProduct = RedisCacheManager.GetRedisKey("BidHashKey", redisObj);
                    string bidSortedSetAllKeyByProduct = RedisCacheManager.GetRedisKey("AllBidsSortedSetKey", redisObj);
                    string bidSoertedSetSupplierKeyByProduct = RedisCacheManager.GetRedisKey("AllBidsSortedSetForSupplierKey", redisObj);
                    string bidAllHash = RedisCacheManager.GetRedisKey("AllBidHashKey", redisObj);

                    string BidAllBidAllProduct = RedisCacheManager.GetRedisKey("", redisObj);

                    var trans = RedisCacheManager.RedisCon.CreateTransaction();
                    string jsonBid = JsonConvert.SerializeObject(bid);
                    trans.HashSetAsync(bidHashAllKeyByProduct, RedisCacheManager.ConvertIntToStringWithLeftPadding(bid.BidId), jsonBid);
                    trans.SortedSetAddAsync(bidSortedSetAllKeyByProduct, RedisCacheManager.ConvertIntToStringWithLeftPadding(bid.BidId), Convert.ToDouble(bid.Amount));
                    trans.SortedSetAddAsync(bidSoertedSetSupplierKeyByProduct, RedisCacheManager.ConvertIntToStringWithLeftPadding(bid.BidId), Convert.ToDouble(bid.Amount));
                    trans.HashSetAsync(bidAllHash, RedisCacheManager.ConvertIntToStringWithLeftPadding(bid.BidId), jsonBid);
                    var exec = trans.ExecuteAsync();

                    return RedisCacheManager.RedisCon.Wait(exec);
                }
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public List<int> GetTopBidders(int productId, int topNum, int userId)
        {
            RedisKeyParameters redisObj = new RedisKeyParameters
            {
                ProductId = productId,
                UserId = userId

            };
            string bidHashAllKey = RedisCacheManager.GetRedisKey("BidHashKey", redisObj);
            string bidSortedSetKey = RedisCacheManager.GetRedisKey("AllBidsSortedSetKey", redisObj);
            var redisValuesSortedSet = RedisCacheManager.GetSortedSetRangeByRankDescending(bidSortedSetKey);
            HashEntry[] redisKeyValuesHash = RedisCacheManager.GetHashAllValueAsKeyValuePair(bidHashAllKey);
            List<int> listBids = new List<int>();
            if (redisKeyValuesHash != null && redisValuesSortedSet != null && redisValuesSortedSet.Length > 0 && redisKeyValuesHash.Length > 0)
            {
                foreach (var value in redisValuesSortedSet)
                {

                    string _bidObj = redisKeyValuesHash.Where(x => x.Name == value.ToString()).Select(x => x.Value).FirstOrDefault();
                    listBids.Add(JsonConvert.DeserializeObject<BidDetails>(_bidObj).UserId);

                }
            }
            return listBids.Take(topNum).ToList() ?? null;

        }

        public Products GetProductDetails(int productId)
        {
            RedisKeyParameters redisObj = new RedisKeyParameters();

            string ProductKey = RedisCacheManager.GetRedisKey("ProductKey", redisObj);
            return RedisCacheManager.GetHashItemValue<Products>(ProductKey, productId.ToString());
        }

        public int GetRankUser(int productId, int userId)
        {
            RedisKeyParameters redisObj = new RedisKeyParameters
            {
                ProductId = productId,
                UserId = userId

            };
            string bidHashAllKey = RedisCacheManager.GetRedisKey("BidHashKey", redisObj);
            string bidSortedSetKey = RedisCacheManager.GetRedisKey("AllBidsSortedSetKey", redisObj);
            var redisValuesSortedSet = RedisCacheManager.GetSortedSetRangeByRankDescending(bidSortedSetKey);
            HashEntry[] redisKeyValuesHash = RedisCacheManager.GetHashAllValueAsKeyValuePair(bidHashAllKey);
            List<int> listBids = new List<int>();
            if (redisKeyValuesHash != null && redisValuesSortedSet != null && redisValuesSortedSet.Length > 0 && redisKeyValuesHash.Length > 0)
            {
                foreach (var value in redisValuesSortedSet)
                {

                    string _bidObj = redisKeyValuesHash.Where(x => x.Name == value.ToString()).Select(x => x.Value).FirstOrDefault();
                    listBids.Add(JsonConvert.DeserializeObject<BidDetails>(_bidObj).UserId);

                }
            }
            return listBids.IndexOf(userId) + 1;

        }

        public BidDetails LastBidOfUser(int userId, int productId)
        {
            BidDetails latestBid = null;
            RedisKeyParameters redisObj = new RedisKeyParameters
            {
                UserId = userId,
                ProductId = productId
            };
            string bidHashAllKey = RedisCacheManager.GetRedisKey("BidHashKey", redisObj);
            string bidSortedSetSupplierKey = RedisCacheManager.GetRedisKey("AllBidsSortedSetForSupplierKey", redisObj);
            RedisValue[] redisValuesSortedSet;

            redisValuesSortedSet = RedisCacheManager.GetSortedSetRangeByRankDescending(bidSortedSetSupplierKey, 0, 0);
            if (redisValuesSortedSet != null && redisValuesSortedSet.Any())
                latestBid = RedisCacheManager.GetHashItemValue<BidDetails>(bidHashAllKey, redisValuesSortedSet[0]);
            return latestBid ?? null;
        }

        public int GetNewBidIdFromSQl()
        {
            return Convert.ToInt32(RedisCacheManager.RedisCon.StringGet("BidIdSqlKey")) + 1;
        }

        public bool SaveBidIdFromSQl(int BidId)
        {
            return RedisCacheManager.RedisCon.StringSet("BidIdSqlKey", BidId);
        }
        public List<BidDetails> GetAllRedisBid()
        {
            RedisKeyParameters redisObj = new RedisKeyParameters();
            string bidAllHash = RedisCacheManager.GetRedisKey("AllBidHashKey", redisObj);
            return RedisCacheManager.GetHashAllValues<BidDetails>(bidAllHash);
        }

        public bool SaveProducts(List<Products> products)
        {
            try
            {
                if (products != null && products.Count > 0)
                {
                    var trans = RedisCacheManager.RedisCon.CreateTransaction();
                    foreach (var item in products)
                    {
                        RedisKeyParameters redisObj = new RedisKeyParameters();

                        string lotKey = RedisCacheManager.GetRedisKey("ProductKey", redisObj);
                        trans.HashSetAsync(lotKey, item.ProductId.ToString(), JsonConvert.SerializeObject(item));

                        //// Creating eventid set. Which will have all the eventIds in the set
                        //string eventIdsKey = RedisCacheManager.GetRedisKey(RedisConstants.RedisKeys.EventIdsKey.ToString(), redisObj);
                        //RedisCacheManager.PutSetData<string>(eventIdsKey, item.EventID.ToString());
                    }

                    var exec = trans.ExecuteAsync();

                    return RedisCacheManager.RedisCon.Wait(exec);
                }
                return false;
            }
            catch (Exception ex)
            { return false; }

        }

        public List<Products> GetAllProducts()
        {
            RedisKeyParameters redisObj = new RedisKeyParameters();
            string allProducts = RedisCacheManager.GetRedisKey("ProductKey", redisObj);
            return RedisCacheManager.GetHashAllValues<Products>(allProducts);
        }
    }
}
