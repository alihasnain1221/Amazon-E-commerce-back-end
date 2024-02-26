using E_commerce_FYP_backend.Data;
using E_commerce_FYP_backend.Models.Users;
using E_commerce_FYP_backend.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_FYP_backend.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ECommerceDbContext dbContext;
        private IConfiguration _configuration;

        public UsersController(ECommerceDbContext dbContext, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await dbContext.Users.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetUserById([FromRoute] Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);

            if (user != null)
            {
                string test = Password.EncryptPassword("test12");
                UserResponse res = Utils.Utils.ConvertUserToUserResponse(user);
                return Ok(res);
            }
            return NotFound("No such account found!");
        }

        [HttpGet]
        [Route("getUserByEmailPass")]
        public async Task<IActionResult> login(string email, string password)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user != null)
            {
                bool isVerified = Password.VerifyPassword(password, user.Password);
                if (isVerified)
                {
                    UserResponse res = Utils.Utils.ConvertUserToUserResponse(user);
                    string token = Utils.Utils.CreateToken(user, _configuration);
                    var orders = await dbContext.Orders.Where(order => order.User.Id == user.Id).ToListAsync();
                    foreach (var order in orders)
                    {
                        await dbContext.Entry(order).Collection(o => o.Products).LoadAsync();
                    }

                    return Ok(new { user = res, token, orders });
                }
                return Unauthorized("Invalid Credentials!");
            }
            return NotFound("No such account found!");
        }

        [HttpGet]
        [Route("getUserByEmailPhone")]
        public async Task<IActionResult> getUserByEmailPhone(string email, string phone)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Email == email && user.Phone == phone);
            if (user != null)
            {
                string token = Utils.Utils.CreateToken(user, _configuration);
                return Ok(new { user, token });
            }
            return NotFound("No such user exists!");
        }

        [HttpPost]
        public async Task<IActionResult> AddNewUser(AddUserRequest user)
        {
            var alreadyExist = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == user.Email || u.Phone == user.Phone);

            if (alreadyExist != null) return Conflict("Email or phone is already in use!");

            var newUser = new Users()
            {
                Id = new Guid(),
                Username = user.Username,
                Email = user.Email,
                Password = Password.EncryptPassword(user.Password),
                Role = user.Role,
                Address = user.Address,
                Phone = user.Phone,
            };

            await dbContext.Users.AddAsync(newUser);
            await dbContext.SaveChangesAsync();

            string emailSubject = "Welcome to " + Constants.Constants.PROJECT_NAME + "! 🎉";

            string emailBody =
                "<h2>Dear " + newUser.Username + ",</h2><br />" +
                "Thank you for registering with " + Constants.Constants.PROJECT_NAME + "! We're thrilled to have you join our growing community of happy shoppers. Get ready to embark on a delightful online shopping experience with us.<br /><br />" +
                "Your new account is all set and ready to go. Here are a few key details about your registration:<br /><br />" +
                "<h2>Account Details</h2>" +
                "<b>Username</b>: " + newUser.Username + "<br />" +
                "<b>Email Address</b>: " + newUser.Email + "<br />" +
                "<b>Contact number</b>: " + newUser.Phone + "<br /><br />" +
                "Now that you have an account with us, you can enjoy the following benefits:<br /><br />" +
                "1. Seamless Shopping: Explore our wide range of products, conveniently categorized for your browsing pleasure. From trendy fashion to cutting-edge electronics, we've got you covered.<br /><br />" +
                "2. Personalized Recommendations: As you browse and make purchases, our intelligent system will learn your preferences and tailor product suggestions specifically to your taste.<br /><br />" +
                "3. Fast and Secure Checkout: With our streamlined checkout process, you can complete your orders quickly and securely, ensuring a smooth and hassle-free experience.<br /><br />" +
                "4. Order Tracking: Stay informed about your purchases every step of the way. You'll receive regular updates on the status of your orders, from packaging to dispatch and delivery.<br /><br />" +
                "5. Exclusive Offers: Be the first to know about our exciting promotions, discounts, and special events. As a registered member, you'll have access to exclusive deals curated just for you.<br /><br />" +
                "Should you have any questions, concerns, or need assistance, our friendly customer support team is here to help. Don't hesitate to reach out to us by replying to this email or visiting our support page on the website.<br /><br />" +
                "We hope you find everything you're looking for and more at " + Constants.Constants.PROJECT_NAME + ". Happy shopping!<br /><br />" +
                "Best regards," +
                Constants.Constants.EMAIL_FOOTER;

            Utils.Utils.SendEmail(newUser.Email, emailSubject, emailBody);

            UserResponse res = Utils.Utils.ConvertUserToUserResponse(newUser);
            return Ok(res);
        }

        [HttpPut]
        [Route("{id:guid}"), Authorize]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, UpdateUserRequest newUserToUpdate)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {
                user.Username = newUserToUpdate.Username;
                user.Email = newUserToUpdate.Email;
                if (newUserToUpdate.Password != "")
                {
                    user.Password = Password.EncryptPassword(newUserToUpdate.Password);
                }
                user.Role = newUserToUpdate.Role;
                user.Address = newUserToUpdate.Address;

                await dbContext.SaveChangesAsync();
                UserResponse res = Utils.Utils.ConvertUserToUserResponse(user);
                return Ok(res);
            }
            return NotFound("No such account found!");
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
        {
            var user = await dbContext.Users.FindAsync(id);
            if (user != null)
            {
                dbContext.Remove(user);
                await dbContext.SaveChangesAsync();
                UserResponse res = Utils.Utils.ConvertUserToUserResponse(user);
                return Ok(res);
            }
            return NotFound("No such account found!");
        }
    }
}
