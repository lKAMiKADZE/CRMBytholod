using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CRMBytholod.RequestAPI
{
    public class AuthRequest
    {
        [JsonPropertyName("Login")]
        public string Login { get; set; }
        [JsonPropertyName("Passw")]
        public string Passw { get; set; }


    }
}
