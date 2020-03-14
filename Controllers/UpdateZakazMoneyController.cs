using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CRMBytholod.RequestAPI;
using CRMBytholod.ResponseAPI;
using Microsoft.AspNetCore.Mvc;

namespace CRMBytholod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateZakazMoneyController : Controller
    {
        [HttpPost]
        public UpdateZakazMoneyResponse Post([FromBody] UpdateZakazMoneyRequest req)
        {
            UpdateZakazMoneyResponse resp = new UpdateZakazMoneyResponse(req);            

            return resp;
        }
    }
}