using BusinessModel;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisLayer.Class
{
  
        internal static class RedisCacheManager
        {
            private static ConnectionMultiplexer _connectionMultiplier = null;
            private static IDatabase _iDatabase = null;

            static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {


                if (_connectionMultiplier == null)
                {
                    var configOptions = ConfigurationOptions.Parse(GetRedisConfig());
                    _connectionMultiplier = ConnectionMultiplexer.Connect(configOptions);
                }

                return _connectionMultiplier;


            });
            private static string GetRedisConfig()
            {
                string redisConfig = string.Empty;
            redisConfig = "";
                return redisConfig;
            }



            public static IDatabase RedisCon
            {
                get
                {
                    if (_iDatabase == null)
                        _iDatabase = lazyConnection.Value.GetDatabase();

                    return _iDatabase;

                }
            }
            public static bool PutJson<T>(string key, T data)
            {
                try
                {
                    var trans = RedisCon.CreateTransaction();
                    trans.StringSetAsync(key, JsonConvert.SerializeObject(data));
                    var exec = trans.ExecuteAsync();
                    return RedisCon.Wait(exec);

                }
                catch (Exception ex)
                {
                    return false;
                }


            }

            public static string GetJson(string key)
            {
                if (RedisCon.KeyExists(key))
                    return RedisCon.StringGet(key);

                return "";
            }

            public static bool SortedSetStore<T>(string key, T value, double scoringField)
            {
                try
                {
                    var trans = RedisCon.CreateTransaction();
                    trans.SortedSetAddAsync(key, JsonConvert.SerializeObject(value), scoringField);
                    var exec = trans.ExecuteAsync();
                    return RedisCon.Wait(exec);
                }
                catch (Exception ex)
                {
                    return false;
                }
                //var result = CacheDatabase.Wait(exec);
            }

            public static bool PutHashSetAsync<T>(string key, string hashField, T value)
            {
                var trans = RedisCon.CreateTransaction();
                trans.HashSetAsync(key, hashField, JsonConvert.SerializeObject(value));
                var exec = trans.ExecuteAsync();
                return RedisCon.Wait(exec);

            }

            public static List<T> GetHashAllValues<T>(string Key)
            {
                var hashValues = RedisCon.HashValues(Key);
                List<T> ListT = new List<T>();
                if (hashValues != null && hashValues.Length > 0)
                {
                    foreach (var value in hashValues)
                    {
                        ListT.Add(JsonConvert.DeserializeObject<T>(value));
                    }
                    return ListT;
                }
                return null;


            }

            public static RedisValue[] GetHashAllValueAsRedisArray(string key)
            {
                if (RedisCon.KeyExists(key))
                {
                    var hashValues = RedisCon.HashValues(key);
                    return hashValues;
                }
                return null;
            }
            public static HashEntry[] GetHashAllValueAsKeyValuePair(string key)
            {
                if (RedisCon.KeyExists(key))
                {
                    var hashPairValues = RedisCon.HashGetAll(key);
                    return hashPairValues;
                }
                return null;
            }
            public static long GetHashLength(string key)
            {
                return RedisCon.HashLength(key);
            }
            public static long GetSortedSetLength(string key)
            {
                return RedisCon.SortedSetLength(key);
            }
            public static T GetHashItemValue<T>(string Key, string hashField)
            {
                T value = default(T);
                if (RedisCon.KeyExists(Key))
                    value = JsonConvert.DeserializeObject<T>(RedisCon.HashGet(Key, hashField));

                return value;

            }

            public static bool RemoveKey(string key)
            {
                bool success = false;
                if (RedisCon.KeyExists(key))
                    success = RedisCon.KeyDelete(key);

                return success;
            }
            public static bool DeleteHashField(string hashKey, RedisValue hashField)
            {
                bool success = false;
                if (RedisCon.KeyExists(hashKey))
                    success = RedisCon.HashDelete(hashKey, hashField);
                return success;
            }

            public static string[] GetHashItemAllKeys(string key)
            {
                if (RedisCon.KeyExists(key)) { return RedisCon.HashKeys(key).ToStringArray(); }
                return new string[0];

            }
            public static RedisValue[] GetSortedSetRangeByRankAscending(string key, int start = 0, int stop = -1)
            {
                if (RedisCon.KeyExists(key))
                    return RedisCon.SortedSetRangeByRank(key, start, stop, order: Order.Ascending);

                return null;
            }
            public static RedisValue[] GetSortedSetRangeByRankDescending(string key, int start = 0, int stop = -1)
            {
                if (RedisCon.KeyExists(key))
                    return RedisCon.SortedSetRangeByRank(key, start, stop, order: Order.Descending);

                return null;
            }

            public static bool PutSetData<T>(string key, T data)
            {
                try
                {
                    var trans = RedisCon.CreateTransaction();
                    trans.SetAddAsync(key, JsonConvert.SerializeObject(data));
                    return trans.Execute();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            public static string[] GetSetData(string key)
            {

                if (RedisCon.KeyExists(key))
                {
                    return RedisCon.SetMembers(key).ToStringArray();
                }
                return new string[0];

            }

            public static RedisValue[] GetSortedSetRangeByScore(string key, long startValue, long stopValue)
            {
                if (RedisCon.KeyExists(key))
                    return RedisCon.SortedSetRangeByScore(key, startValue, stopValue);

                return null;
            }
            public static string ConvertIntToStringWithLeftPadding(int value, int paddingCount = 16)
            {
                return value.ToString().PadLeft(paddingCount, '0');
            }

            public static string GetRedisKey(string KeyType, RedisKeyParameters redisParam)
            {

            //switch (KeyType)
            //{
            //    //case RedisConstants.EventsHashKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:Hash:Events";

            //    //case RedisConstants.LotsHashKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:Hash:Lots:{redisParam.EventCode}";

            //    //case RedisConstants.InvitedSupplierHashKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:Hash:InvitedSuppliers:{redisParam.LotId}";

            //    //case RedisConstants.InvitedSupplierAttributesHashKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:Hash:InvitedSupplierAttributes:{redisParam.LotId}";

            //    //case RedisConstants.LotAllItemsHashkey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:Hash:Items:{redisParam.LotId}";

            //    //case RedisConstants.PreBidHashKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:Hash:PreBids:{redisParam.LotId}";

            //    //case RedisConstants.AllPreBidsSortedSetKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:SortedSet:PreBids:{redisParam.LotId}";

            //    //case RedisConstants.PreBidSupplierHashKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:HashSupplier:PreBids:{redisParam.LotId}";

            //    //case RedisConstants.BidHashKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:Hash:Bids:{redisParam.LotId}";

            //    //case RedisConstants.AllBidsSortedSetKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:SortedSet:Bids:{redisParam.LotId}";

            //    //case RedisConstants.AllBidsSortedSetForSupplierKey:
            //    //    return $"AuctionEngine2:{redisParam.BuyerPartnerCode}:SortedSetSupplier:Bids:{redisParam.LotId}:{redisParam.ContactCode}";



            //    //default:
            //    //    return "";

            //}
            return "";
            }
        }
    }

