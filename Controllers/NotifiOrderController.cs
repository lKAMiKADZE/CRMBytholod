using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.RequestAPI;
using CRMBytholod.ResponseAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CRMBytholod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifiOrderController : ControllerBase
    {

        [HttpPost]
        public NotifiNewOrderResponse Post([FromBody] NotifiNewOrderRequest req)
        {

            NotifiNewOrderResponse resp = new NotifiNewOrderResponse(req);
            //json = JsonConvert.SerializeObject(resp);


            return resp;
        }


    }
}