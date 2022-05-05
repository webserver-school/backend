using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;

namespace App.Controllers
{
    public class UserLoginDto
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }

    public class UserRegisterDto
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public string? confirm_password { get; set; }
    }

    [ApiController]
    public class RegisterController : Controller
    {
        [HttpPost]
        [Route("/Register")]
        public object Register([FromBody] UserRegisterDto userLogin)
        {
            if (userLogin.username == null || userLogin.password == null)
            {
                return StatusCode(400, "Please provide a username and password");
            }

            if (userLogin.password != userLogin.confirm_password)
            {
                return StatusCode(400, "Passwords do not match");
            }

            MySqlConnection connection = new MySqlConnection(Environment.GetEnvironmentVariable("ASPNETCORE_DB_STRING"));

            connection.Open();

            // Insert the page view into the database
            connection.Execute("INSERT INTO users (username, password) VALUES (@username, @password)", new { @username = userLogin.username, @password = userLogin.password });

            return StatusCode(200);
        }

        [HttpPost]
        [Route("/Login")]
        public object Login([FromBody] UserLoginDto userLogin)
        {
            if (userLogin.username == null || userLogin.password == null)
            {
                return StatusCode(400, "Please provide a username and password");
            }

            MySqlConnection connection = new MySqlConnection(Environment.GetEnvironmentVariable("ASPNETCORE_DB_STRING"));

            connection.Open();

            // Insert the page view into the database
            var user = connection.QueryFirst("SELECT * FROM users WHERE username = @username AND password = @password", new { @username = userLogin.username, @password = userLogin.password });

            if (user == null)
            {
                return StatusCode(400, "Invalid username or password");
            }

            HttpContext.Session.SetString("username", userLogin.username);

            return StatusCode(200, new { username = userLogin.username });
        }

        [HttpGet]
        [Route("/User")]
        public object GetUser()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                return StatusCode(400, new { message = "Not logged in" });
            }

            return StatusCode(200, new { username = HttpContext.Session.GetString("username") });
        }
    }
}