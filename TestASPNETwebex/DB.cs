using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;
using WebexAPITestApp.Models;
using Newtonsoft.Json;


namespace TestASPNETwebex
{
    class DB
    {
        private static string cs = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";
        private static string newCs = "Host=localhost;Username=postgres;Password=postgres;Database=webexapidb";

        public static void DbSetup()
        {
            bool DBexists = true;

            try
            {
                using var con = new NpgsqlConnection(newCs);
                con.Open();
            }
            catch (Exception)
            {
                DBexists = false;
            }

            if (!DBexists)
            {
                using var conDefault = new NpgsqlConnection(cs);
                conDefault.Open();

                string createDB = "CREATE DATABASE webexapidb;";

                using var cmd1 = new NpgsqlCommand(createDB, conDefault);
                cmd1.ExecuteScalar();

                using var conNewDB = new NpgsqlConnection(newCs);
                conNewDB.Open();

                string createSchema = "CREATE SCHEMA webexschema;";

                using var cmd2 = new NpgsqlCommand(createSchema, conNewDB);
                cmd2.ExecuteScalar();
            }

            using var newCon = new NpgsqlConnection(newCs);
            newCon.Open();

            string createAuthTokenTable = "CREATE TABLE IF NOT EXISTS webexschema.webextable (accesstoken TEXT, createdat TEXT, refreshtoken TEXT);";
            WriteToDB(createAuthTokenTable);

            string createWebexPeopleTable = "CREATE TABLE IF NOT EXISTS webexschema.webexpeople (id TEXT, emails TEXT, displayname TEXT);";
            WriteToDB(createWebexPeopleTable);       

            string getCount = "SELECT COUNT(*) FROM webexschema.webextable;";

            using var cmd3 = new NpgsqlCommand(getCount, newCon);
            var count = cmd3.ExecuteScalar().ToString();
            var auth = GetAuthDetails();

            if(Convert.ToInt32(count) == 0)
            {
                string query1 = $"INSERT INTO webexschema.webextable VALUES ('{auth[0]}','{DateTime.Now}','{auth[1]}');";
                DB.WriteToDB(query1);   
            }

        }

        public static void WriteToDB(string query)
        {
            using var con = new NpgsqlConnection(newCs);
            con.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand(query, con);

            cmd.ExecuteScalar();
        }

        public static string ReadDB(string query)
        {
            using var con = new NpgsqlConnection(newCs);
            con.Open();

            using NpgsqlCommand cmd = new NpgsqlCommand(query, con);

            var result = cmd.ExecuteScalar().ToString();

            return result;
        }        

        public static void WriteWebexPeople(string people)
        {
            WebexPeople content = JsonConvert.DeserializeObject<WebexPeople>(people);
            foreach (var item in content.items)
            {
                var id = item.id;
                var emails = item.emails[0];
                var displayName = item.displayName;
                
                string insertString = $"INSERT INTO webexschema.webexpeople VALUES ('{id}', '{emails}', '{displayName}');";
                WriteToDB(insertString);
            }                    
        }
    }
}
