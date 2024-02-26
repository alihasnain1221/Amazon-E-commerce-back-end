using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace E_commerce_FYP_backend.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class ScrapperController : Controller
    {
        private string scrapperApi = "http://127.0.0.1:5000//";

        [HttpGet]
        [Route("/getCategoryResults")]
        public async Task<IActionResult> GetCategoryResults(string categoryId, string page)
        {
            if (categoryId == null) return BadRequest("categoryId is required");
            if (page == null) return BadRequest("page is required");
            string categoryUrl = "https://www.amazon.com/s?bbn=" + categoryId + "&rh=n:" + categoryId + "&page=" + page; // Replace with the desired category URL

            HttpClient client = new HttpClient();

            try
            {
                string encodedUrlParameter = WebUtility.UrlEncode(categoryUrl);
                string link = scrapperApi + "scrapeProducts?url=" + encodedUrlParameter;
                HttpResponseMessage response = await client.GetAsync(link);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(responseBody);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                client.Dispose();
            }

            return BadRequest(new { message = "scraping Products failed!" });
        }

        [HttpGet]
        [Route("/getSearchResults")]
        public async Task<IActionResult> GetSearchResults(string searchTerm, string page)
        {
            if (searchTerm == null) return BadRequest("searchTerm is required");
            if (page == null) return BadRequest("page is required");
            string searchUrl = "https://www.amazon.com/s?k=" + searchTerm + "&rh=n:" + searchTerm + "&page=" + page; // Replace with the desired category URL

            HttpClient client = new HttpClient();

            try
            {
                string encodedUrlParameter = WebUtility.UrlEncode(searchUrl);
                string link = scrapperApi + "scrapeProducts?url=" + encodedUrlParameter;
                HttpResponseMessage response = await client.GetAsync(link);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(responseBody);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                client.Dispose();
            }

            return BadRequest(new { message = "scraping Products failed!" });
        }

        [HttpGet]
        [Route("/getProductDetails")]
        public async Task<IActionResult> GetProductDetails(string productAsin)
        {
            if (productAsin == null) return BadRequest("productAsin is required");
            string productUrl = "https://www.amazon.com/dp/" + productAsin + "/"; // Replace with the desired category URL

            HttpClient client = new HttpClient();

            try
            {
                HttpResponseMessage response = await client.GetAsync(scrapperApi + "scrapeProductDetails?url=" + productUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return Ok(responseBody);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                client.Dispose();
            }
            return BadRequest(new { message = "scraping Product details failed!" });
        }
    }
}
