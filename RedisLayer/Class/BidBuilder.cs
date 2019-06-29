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

                    string bidHashAllKey = RedisCacheManager.GetRedisKey("BidHashKey", redisObj);
                    string bidSortedSetAllKey = RedisCacheManager.GetRedisKey("AllBidsSortedSetKey", redisObj);
                    string bidSoertedSetSupplierKey = RedisCacheManager.GetRedisKey("AllBidsSortedSetForSupplierKey", redisObj);

                    var trans = RedisCacheManager.RedisCon.CreateTransaction();
                    trans.HashSetAsync(bidHashAllKey, RedisCacheManager.ConvertIntToStringWithLeftPadding(bid.BidId), JsonConvert.SerializeObject(bid));
                    trans.SortedSetAddAsync(bidSortedSetAllKey, RedisCacheManager.ConvertIntToStringWithLeftPadding(bid.BidId), Convert.ToDouble(bid.Amount));
                    trans.SortedSetAddAsync(bidSoertedSetSupplierKey, RedisCacheManager.ConvertIntToStringWithLeftPadding(bid.BidId), Convert.ToDouble(bid.Amount));
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
          return Convert.ToInt32( RedisCacheManager.RedisCon.StringGet("BidIdSqlKey"))+1;
        }

        public bool SaveBidIdFromSQl(int BidId)
        {
            return RedisCacheManager.RedisCon.StringSet("BidIdSqlKey", BidId);
        }
    }
}
