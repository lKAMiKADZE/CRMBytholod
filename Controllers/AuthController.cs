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


        [HttpGet]
        public string Get(string Login, string Passw)
        {
            string json = "";
            var req = new AuthRequest
            {
                Login = Login,
                Passw = Passw
            };

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

        // POST: api/Auth
        [HttpPost]
        public AuthResponse Post([FromBody] AuthRequest req)
        {
            string json = "";

            AuthResponse resp = new AuthResponse(req);
            //json = JsonConvert.SerializeObject(resp);
            


            return resp;
        }

       

    }
}
