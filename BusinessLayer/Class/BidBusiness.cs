using BusinessLayer.Interface;
using BusinessModel;
using RedisLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace BusinessLayer.Class
{
    public class BidBusiness : IBidBusiness
    {
        IBidBuilder _bidBuilder;
        public BidBusiness(IBidBuilder bidBuilder)
        {
            _bidBuilder = bidBuilder;
        }
        public string SaveBidDetails(BidDetails bidDetails)
        {
            try
            {
                bidDetails.BidId = _bidBuilder.GetNewBidIdFromSQl();
                Products products = _bidBuilder.GetProductDetails(bidDetails.ProductId);
                if (bidDetails.Amount > products.BasePrice)
                {

                    _bidBuilder.SaveBid(bidDetails);
                    _bidBuilder.SaveBidIdFromSQl(bidDetails.BidId);
                    return "Successfully saved";
                }
                else
                {
                    return "Amount is less than Bid Base price";
                }
            }
            catch (Exception ex) {

                return ex.Message;
            }

        }


        public List<int> TopBidders (int productId,int num)
        {
            try{
              return  _bidBuilder.GetTopBidders(productId, num);
            }
            catch
            {
                return null;
            }
        }


        public string GetUserDetails(int produtId,int userid)
        {
            try
            {
                int rank= _bidBuilder.GetRankUser(produtId, userid);
             BidDetails bidDetails=   _bidBuilder.LastBidOfUser(userid, produtId);
                var result = new
                {
                    rank= rank,
                    bidDetails= bidDetails
                };

                return JsonConvert.SerializeObject(result);
            }
            catch
            {
                return null;
            }
        }


    }
}
