using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

namespace App.Controllers
{
    public class HelloWorldController : Controller
    {
        // 
        // GET: /HelloWorld/

        public async Task<object> Index()
        {
            MySqlConnection connection = new MySqlConnection(Environment.GetEnvironmentVariable("ASPNETCORE_DB_STRING"));

            connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT * FROM users;", connection);
            using var reader = await command.ExecuteReaderAsync();

            List<object> users = new List<object>();

            while (reader.Read())
            {
                users.Add(new {
                    id = reader.GetInt32(0),
                });
            }

            return string.Join(",", users);
        }
    }
}