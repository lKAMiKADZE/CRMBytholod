using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CRMBytholod.ResponseAPI;
using CRMBytholod.RequestAPI;
using Newtonsoft.Json;

namespace CRMBytholod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // GET: api/Auth
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };    
        }


        // POST: api/Auth
        [HttpPost]
        public string Post([FromBody] AuthRequest req)
        {
            string json = "";

            try
            {
                AuthResponse resp = new AuthResponse(req);
                json = JsonConvert.SerializeObject(resp);
            }
            catch (Exception ex)
            {
                json += " err " + ex.Message;
            }


            return json;
        }

       

    }
}
