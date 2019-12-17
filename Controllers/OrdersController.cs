using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.RequestAPI;
using CRMBytholod.ResponseAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CRMBytholod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        [HttpPost]
        public OrdersResponse Post([FromBody] OrdersRequest req)
        {
            string json = "";

            OrdersResponse resp = new OrdersResponse(req);
            //json = JsonConvert.SerializeObject(resp);
            

            return resp;
        }
    }
}