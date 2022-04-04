using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using System.Net;

namespace App.Controllers
{
    public class PageViewController : Controller
    {
        [HttpGet]
        [Route("/PageViews")]
        public object Index(string page)
        {
            IPAddress? ip = HttpContext.Connection.RemoteIpAddress;

            if (page == null || ip == null)
            {
                return StatusCode(400, "Page and IP are required");
            }

            MySqlConnection connection = new MySqlConnection(Environment.GetEnvironmentVariable("ASPNETCORE_DB_STRING"));

            connection.Open();

            // Insert the page view into the database
            connection.Execute("INSERT INTO page_views (page, ip) VALUES (@page, @ip)", new { @page = page, @ip = ip.ToString() });

            // Get page views
            var result = connection.QueryFirst("SELECT count(*) as views FROM page_views WHERE page = @page", new { @page = page });

            return StatusCode(200, result);
        }
    }
}