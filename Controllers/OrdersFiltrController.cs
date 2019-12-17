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
    public class OrdersFiltrController : ControllerBase
    {
        [HttpPost]
        public OrdersFiltrResponse Post([FromBody] OrdersFiltrRequest req)
        {
            string json = "";

            OrdersFiltrResponse resp = new OrdersFiltrResponse(req);
            //json = JsonConvert.SerializeObject(resp);
            

            return resp;
        }
    }
}