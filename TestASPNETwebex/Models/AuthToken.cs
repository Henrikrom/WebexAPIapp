using System;
using System.Collections.Generic;
using System.Text;

namespace WebexAPITestApp.Models
{

    public class AuthToken
    {
        public string Access_token { get; set; }
        public int Expires_in { get; set; }
        public string Refresh_token { get; set; }
        public int Refresh_token_expires_in { get; set; }
    }
}
    
