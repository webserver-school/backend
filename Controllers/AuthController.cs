using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;

namespace App.Controllers
{
    [ApiController]
    public class RegisterController : Controller
    {
        public string SPOTIFY_CLIENT_ID;
        public string SPOTIFY_CLIENT_SECRET;

        public RegisterController()
        {
            var SPOTIFY_CLIENT_ID = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
            var SPOTIFY_CLIENT_SECRET = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");

            if (SPOTIFY_CLIENT_ID == null || SPOTIFY_CLIENT_SECRET == null)
            {
                throw new System.Exception("SPOTIFY_CLIENT_ID or SPOTIFY_CLIENT_SECRET is null");
            }

            this.SPOTIFY_CLIENT_ID = SPOTIFY_CLIENT_ID;
            this.SPOTIFY_CLIENT_SECRET = SPOTIFY_CLIENT_SECRET;
        }

        [HttpGet]
        [Route("/Login")]
        public object Login()
        {
            var loginRequest = new LoginRequest(
                new Uri("http://localhost:5171/Login/Callback"),
                this.SPOTIFY_CLIENT_ID,
                LoginRequest.ResponseType.Code
            ) {
                Scope = new[] { Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative }
            };

            var uri = loginRequest.ToUri();
            
            Console.WriteLine(uri);

            return Redirect(uri.ToString());
        }

        [HttpGet]
        [Route("/Login/Callback")]
        public async Task<object> Callback(string code)
        {
            var response = await new OAuthClient().RequestToken(
                new AuthorizationCodeTokenRequest(this.SPOTIFY_CLIENT_ID, this.SPOTIFY_CLIENT_SECRET, code, new Uri("http://localhost:5171/Login/Callback"))
            );

            var spotify = new SpotifyClient(response.AccessToken);
            
            Response.Cookies.Append("access_token", response.AccessToken);

            Console.WriteLine(Environment.GetEnvironmentVariable("HOME_URL"));
            return Redirect(Environment.GetEnvironmentVariable("HOME_URL") ?? "/");
        }

        [HttpGet]
        [Route("/api/User")]
        public async Task<object> GetUser()
        {
            var accessToken = Request.Cookies["access_token"];

            if (accessToken == null)
            {
                return StatusCode(401, new { error = "Unauthorized" });
            }

            var spotify = new SpotifyClient(accessToken);

            var user = await spotify.UserProfile.Current();

            return user;
        }
    
        [HttpGet]
        [Route("/Logout")]
        public object Logout()
        {
            Response.Cookies.Delete("access_token");

            return Redirect(Environment.GetEnvironmentVariable("HOME_URL") ?? "/");
        }
    }
}