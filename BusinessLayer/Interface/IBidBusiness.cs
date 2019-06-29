using BusinessModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interface
{
   public interface IBidBusiness
    {
        string SaveBidDetails(BidDetails bidDetails);
        List<int> TopBidders(int productId, int num);
        string GetUserDetails(int produtId, int userid);
    }
}
