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
    public class StatusOrderController : ControllerBase
    {
        [HttpPost]
        public StatusOrderResponse Post([FromBody] StatusOrderRequest req)
        {
            string json = "";

            StatusOrderResponse resp = new StatusOrderResponse(req);
            //json = JsonConvert.SerializeObject(resp);
            
            return resp;
        }
    }
}