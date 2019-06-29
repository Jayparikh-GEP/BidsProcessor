using BusinessModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Interface
{
   public interface IBidBusiness
    {
        string SaveBidDetails(BidDetails bidDetails);
    }
}
