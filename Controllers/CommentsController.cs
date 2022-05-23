using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using SpotifyAPI.Web;

namespace App.Controllers
{
    public class CommentsController : Controller
    {
        [HttpGet]
        [Route("/api/comments")]
        public object IndexTrack(string trackId)
        {
            if (trackId == null)
            {
                return StatusCode(400, new {
                    message = "Missing track id."
                });
            }

            MySqlConnection connection = new MySqlConnection(Environment.GetEnvironmentVariable("ASPNETCORE_DB_STRING"));

            connection.Open();

            var comments = connection.Query("SELECT * FROM comments WHERE track_id = @track_id", new { track_id = trackId });

            return StatusCode(200, comments);
        }

        [HttpPost]
        [Route("/api/comments")]
        public async Task<object> CreateComment(string trackId, string body)
        {
            if (trackId == null)
            {
                return StatusCode(400, new {
                    message = "Missing track id."
                });
            }

            if (body == null)
            {
                return StatusCode(400, new {
                    message = "Missing comment body."
                });
            }

            var accessToken = Request.Cookies["access_token"];

            if (accessToken == null)
            {
                return StatusCode(401, new {
                    message = "Unauthorized."
                });
            }

            MySqlConnection connection = new MySqlConnection(Environment.GetEnvironmentVariable("ASPNETCORE_DB_STRING"));

            connection.Open();

            var spotify = new SpotifyClient(accessToken);
            var author = await spotify.UserProfile.Current();

            connection.Execute("INSERT INTO comments (author_name, author_spotify_id, track_id, body) VALUES (@author_name, @author_spotify_id, @track_id, @body)", new { author_name = author.DisplayName, author_spotify_id = author.Id, track_id = trackId, body = body });

            return StatusCode(200, new {
                message = "Comment created."
            });
        }
    }
}