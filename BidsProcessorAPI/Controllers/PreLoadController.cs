using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BidsProcessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreLoadController : ControllerBase
    {
        IRedisToSQL _iRedisToSQL;
        public PreLoadController(IRedisToSQL iRedisToSQL)
        {
            _iRedisToSQL = iRedisToSQL;

        }

       
        public IActionResult PreloadData()
        {
            try
            {
                _iRedisToSQL.GetAllProductDetails();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
        public IActionResult SaveSql()
        {
            try
            {
                _iRedisToSQL.updateBidsFromRedis();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}