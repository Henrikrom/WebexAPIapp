using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Npgsql;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebexAPITestApp.Models;
using System.Threading;
using Microsoft.Extensions.Hosting;

namespace TestASPNETwebex
{
    class API : IHostedService
    {
        private HttpClient _client { get; }
        private string _clientId { get; set; }
        private string _clientSecret { get; set; } 
        public string access_token { get; set; }         
        public string refresh_token { get; set; }

        public API()
        {
            _client = new HttpClient();
            //_clientId = auth[0];
            //_clientSecret = auth[1];
            //access_token = auth[2];
            //refresh_token = auth[3];

            _clientId = "dsgdff";
            _clientSecret = "dsgdff";
            access_token = "dsgdff";
            refresh_token = "dsgdff";
        }

        public async Task<AuthToken> GetNewToken()
        {
            string requestUrl = "https://webexapis.com/v1/access_token";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUrl);
            request.Content = new StringContent($"grant_type=refresh_token&client_id={_clientId}&client_secret={_clientSecret}&refresh_token={refresh_token}", Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await _client.SendAsync(request);
            string newToken = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AuthToken>(newToken);

        }

        public async Task<AuthToken> TokenRefresh()
        {
            AuthToken newToken = null;
            
            string tokenCreatedAt = DB.ReadDB("SELECT createdat FROM webexschema.webextable ORDER BY createdat DESC;");
            DateTime created = DateTime.Parse(tokenCreatedAt);
            int diff = DateTime.Now.Day - created.AddDays(-8).Day;
            
            if (diff > 7)
            {
                newToken = await GetNewToken();                
            }

            return newToken;
        }

        public async Task<string> GetPeople()
        {       
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.ciscospark.com/v1/people");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

            var response = await _client.SendAsync(request);            
            string peopleResponse = await response.Content.ReadAsStringAsync();                  

            return peopleResponse;
        }

        public static async Task mainMethod()
        {
            string[] APIAuth = DB.GetAuthDetails(); //Get webex api authentification details from DB

            //API obj = new API(APIAuth);
            API obj = new API();

            DB.DbSetup(); //Create database, schema and table(s)

            //string people = await obj.GetPeople();
            //DB.WriteWebexPeople(people);       

            while (true) // Loop updating access token when necessary
            {
                AuthToken newToken = await obj.TokenRefresh();
                try
                {
                    DB.WriteToDB($"UPDATE webexschema.webextable SET accesstoken = '{newToken.Access_token}', createdat = '{DateTime.Now}', refreshtoken = '{newToken.Refresh_token}'");
                }
                catch (Exception)
                {
                    Console.WriteLine("Token not stored in DB");
                }

                Thread.Sleep(10000);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Service started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
