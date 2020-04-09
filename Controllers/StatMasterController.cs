using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CRMBytholod.RequestAPI;
using CRMBytholod.ResponseAPI;

namespace CRMBytholod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatMasterController : ControllerBase
    {

        [HttpPost]
        public StatMasterResponse Post([FromBody] StatMasterRequest req)
        {
            StatMasterResponse resp = new StatMasterResponse(req);

            return resp;
        }
    }
}