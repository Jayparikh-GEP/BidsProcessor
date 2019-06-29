using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BusinessModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using BusinessLayer.Interface;

namespace BidsProcessorAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BidDetailsController : ControllerBase
    {
        IBidBusiness _bidBusiness;
        public BidDetailsController(IBidBusiness bidBusiness)
        {
            _bidBusiness = bidBusiness;

        }
        public IActionResult SaveBidDetails() {
            HttpRequestMessage request = new HttpRequestMessage();
            var headers = request.Headers;

            if (headers.Contains("BidDetails"))
            {
                BidDetails bidDetails = JsonConvert.DeserializeObject<BidDetails>(headers.GetValues("BidDetails").FirstOrDefault());
                string result = _bidBusiness.SaveBidDetails(bidDetails);
                return Ok(result);
            }
            else { return BadRequest(); }
        }
    }
}