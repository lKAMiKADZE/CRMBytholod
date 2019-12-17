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
    public class MasterController : ControllerBase
    {
        [HttpPost]
        public MasterResponse Post([FromBody] MasterRequest req)
        {
            string json = "";

            MasterResponse resp = new MasterResponse(req);
            //json = JsonConvert.SerializeObject(resp);
           

            return resp;
        }
    }
}