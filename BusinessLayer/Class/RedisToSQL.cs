using BusinessLayer.Interface;
using BusinessModel;
using RedisLayer.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace BusinessLayer.Class
{
   public class RedisToSQL: IRedisToSQL
    {

        IBidBuilder _bidBuilder;
        public RedisToSQL(IBidBuilder bidBuilder)
        {
            _bidBuilder = bidBuilder;
        }
        public void updateBidsFromRedis()
        {
            SqlCommand cmd;
            SqlParameter objSqlParameter;
            string connetionString = @"Data Source=hxbidauction.database.windows.net;Initial Catalog=dev_auction;User ID=AZSQL;Password=Password@123";
            SqlConnection con = new SqlConnection(connetionString);
            con.Open();
            try
            {
                cmd = new SqlCommand("USP_UpdateBidsFromCache", con);
                cmd.CommandType = CommandType.StoredProcedure;
                objSqlParameter = new SqlParameter("@bidData", SqlDbType.Structured)
                {
                    TypeName = "tvp_biddetails",
                    Value = FillBidDetails()
                };

                cmd.Parameters.Add(objSqlParameter);
                cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        private DataTable FillBidDetails()
        {
            DataTable dtBidDetails = new DataTable() { TableName = "tvp_biddetails" };
            dtBidDetails.Columns.Add("bidid", typeof(long));
            dtBidDetails.Columns.Add("userid", typeof(long));
            dtBidDetails.Columns.Add("product_id", typeof(long));
            dtBidDetails.Columns.Add("bidprice", typeof(decimal));


            List<BidDetails> bidDetails = _bidBuilder.GetAllRedisBid();

            foreach (BidDetails bid in bidDetails)
            {
                DataRow dr = dtBidDetails.NewRow();
                dr["bidid"] = bid.BidId;
                dr["userid"] = bid.UserId;
                dr["product_id"] = bid.ProductId;
                dr["bidprice"] = bid.Amount;
            }
            return dtBidDetails;
        }

        public List<Products> GetAllProductDetails()
        {
            SqlCommand cmd;
            string connetionString = @"Data Source=hxbidauction.database.windows.net;Initial Catalog=dev_auction;User ID=AZSQL;Password=Password@123";
            SqlConnection con = new SqlConnection(connetionString);
            con.Open();
            try
            {
                cmd = new SqlCommand("USP_GetProductDetails", con);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader sqlDr = cmd.ExecuteReader();
                List<Products> lstProducts = new List<Products>();
                while (sqlDr.Read())
                {
                    lstProducts.Add(new Products
                    {
                        ProductId = Convert.ToInt32(sqlDr["product_id"]),
                        ProductName = Convert.ToString(sqlDr["product_name"]),
                        Quantity = Convert.ToInt32(sqlDr["quantity"]),
                        BasePrice = Convert.ToDecimal(sqlDr["baseprice"])
                    });
                }
                return lstProducts;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
        }

    }

}
