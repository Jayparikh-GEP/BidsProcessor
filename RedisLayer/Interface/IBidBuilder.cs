using BusinessModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedisLayer.Interface
{
   public interface IBidBuilder
    {
        BidDetails LastBidOfUser(int userId, int productId);
        bool SaveBid(BidDetails bid);
        List<int> GetTopBidders(int productId, int topNum, int userId);
        Products GetProductDetails(int productId);
        int GetRankUser(int productId, int userId);
    }
}
