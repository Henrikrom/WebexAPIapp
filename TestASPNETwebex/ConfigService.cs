using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Npgsql;

namespace TestASPNETwebex
{
    public class ConfigService
    {
        public static string _clientId { get; set; }
        public static string _clientSecret { get; set; }
        public static string access_token { get; set; }
        public static string refresh_token { get; set; }

        public static void GetAuthDetails()
        {            

            using var con = new NpgsqlConnection("Host=localhost;Username=postgres;Password=postgres;Database=synergyskydb");
            con.Open();

            using var cmd1 = new NpgsqlCommand("SELECT clientid FROM public.webexconfigtable", con);
            _clientId = cmd1.ExecuteScalar().ToString();

            using var cmd2 = new NpgsqlCommand("SELECT clientsecret FROM public.webexconfigtable", con);
            _clientSecret = cmd2.ExecuteScalar().ToString();

            using var cmd3 = new NpgsqlCommand("SELECT accesstoken FROM public.webexconfigtable", con);
            access_token = cmd3.ExecuteScalar().ToString();

            using var cmd4 = new NpgsqlCommand("SELECT refreshtoken FROM public.webexconfigtable", con);
            refresh_token = cmd4.ExecuteScalar().ToString();            
        }
    }
}
