using BusinessModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BusinessLayer.Interface
{
    public interface IRedisToSQL
    {
        void updateBidsFromRedis();
        List<Products> GetAllProductDetails();
    }
}
