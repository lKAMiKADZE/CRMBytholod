﻿using System;
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
    public class OrderController : ControllerBase
    {
        [HttpPost]
        public OrderResponse Post([FromBody] OrderRequest req)
        {
            string json = "";

            OrderResponse resp = new OrderResponse(req);
            //json = JsonConvert.SerializeObject(resp);
            
           

            return resp;
        }

    }
}