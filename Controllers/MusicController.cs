using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;

namespace App.Controllers
{
    public class MusicController : Controller
    {
        [HttpGet]
        [Route("/api/MusicList")]
        public async Task<object> Index()
        {
            var accessToken = Request.Cookies["access_token"];

            if (accessToken == null)
            {
                return StatusCode(401, new { error = "Unauthorized" });
            }

            var spotify = new SpotifyClient(accessToken);

            var tracks = await spotify.Personalization.GetTopTracks();

            return tracks;
        }
    }
}