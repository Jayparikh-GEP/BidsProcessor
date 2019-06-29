using BusinessLayer.Interface;
using BusinessModel;
using RedisLayer.Interface;
using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
