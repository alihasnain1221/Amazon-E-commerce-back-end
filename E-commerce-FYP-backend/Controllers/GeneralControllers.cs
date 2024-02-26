using E_commerce_FYP_backend.Models.Generals;
using E_commerce_FYP_backend.Utils;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce_FYP_backend.Controllers
{
    public class GeneralControllers : Controller
    {
        [HttpPost]
        [Route("/contactUs")]
        public async Task<IActionResult> contactUs(ContactUsRequest contactUsRequest)
        {
            if (contactUsRequest.name != null || contactUsRequest.email != null || contactUsRequest.message != null)
            {
                string emailMessage = 
                    "<b>Name: " + contactUsRequest.name + "</b><br />" +
                    "<b>Email: " + contactUsRequest.email + "</b><br /><br />" +
                    "<p>Message: " + contactUsRequest.message + "</p>";

                Utils.Utils.SendEmail("adminxyz@yopmail.com", "Contact from user in " + Constants.Constants.PROJECT_NAME, emailMessage);
                return Ok("done");
            }
            return BadRequest("name, email, message shouldn't be empty!");
        }
    }
}
