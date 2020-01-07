using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CRMBytholod.RequestAPI;
using CRMBytholod.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CRMBytholod.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class _MobileController : ControllerBase
    {
        // GET: api/Mobile
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "2222" };
        }

        // GET: api/Mobile/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Mobile
        //[HttpPost]
        //public string Post([FromBody] reqTest req)
        //{

        //    return $"{req.Login} - {req.Passw}";
        //}

        [HttpPost]
        public string Post()
        {

            return "{req.Login} - {req.Passw}";
        }


    }

    public class reqTest
    {
        public string Login { get; set; }
        public string Passw { get; set; }
    }
}
