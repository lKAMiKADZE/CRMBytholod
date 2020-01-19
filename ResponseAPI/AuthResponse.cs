using CRMBytholod.Models;
using CRMBytholod.RequestAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace CRMBytholod.ResponseAPI
{
    public class AuthResponse
    {
        [JsonPropertyName("Login")]
        public bool isAuth { get; private set; }
        public string Sessionid { get; private set; }

        public AuthResponse(AuthRequest authRequest)
        {
            isAuth = false;
            Sessionid = "";
            User user = new User();

            if (user.AuthMaster(authRequest.Login, authRequest.Passw))
            {
                isAuth = true;
                Sessionid = user.Sessionid;
            }
        }





    }
}
