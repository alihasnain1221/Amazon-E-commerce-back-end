using E_commerce_FYP_backend.Data;
using E_commerce_FYP_backend.Models.Orders;
using E_commerce_FYP_backend.Models.Orders.Product;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace E_commerce_FYP_backend.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : Controller
    {
        private readonly ECommerceDbContext dbContext;
        private IConfiguration _configuration;

        public OrdersController(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await dbContext.Orders.ToListAsync();
            foreach (var order in orders)
            {
                await dbContext.Entry(order).Reference(o => o.User).LoadAsync();
                order.User.Password = "";
                await dbContext.Entry(order).Collection(order => order.Products).LoadAsync();
            }
            return Ok(orders);
        }

        [HttpGet]
        [Route("GetUserOrders/")]
        public async Task<IActionResult> GetUserOrder(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                var orders = await dbContext.Orders.Where(order => order.User.Id == user.Id).ToListAsync();
                if (orders.Count != 0)
                {
                    foreach (var order in orders)
                    {
                        await dbContext.Entry(order).Reference(o => o.User).LoadAsync();
                        await dbContext.Entry(order).Collection(o => o.Products).LoadAsync();
                    }
                    return Ok(orders);
                }
                return NoContent();
            }

            return NotFound("No such user found!");
        }

        [HttpGet]
        [Route("GetSingleOrder/")]
        public async Task<IActionResult> GetSingleOrder(Guid orderId)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(order => order.Id == orderId);
            if (order != null)
            {
                await dbContext.Entry(order).Reference(o => o.User).LoadAsync();
                await dbContext.Entry(order).Collection(o => o.Products).LoadAsync();
                return Ok(order);
            }
            return NotFound("No such order exists!");
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder(AddOrderRequest addOrderRequest)
        {
            ICollection<OrderedProduct> newOrderedProducts = new List<OrderedProduct>();

            foreach (var newOrderedProduct in addOrderRequest.Products)
            {
                OrderedProduct product = new OrderedProduct()
                {
                    Id = new Guid(),
                    Asin = newOrderedProduct.Asin,
                    ImageUrl = newOrderedProduct.ImageUrl,
                    Name = newOrderedProduct.Name,
                    Quantity = newOrderedProduct.Quantity,
                };

                newOrderedProducts.Add(product);
            }
            var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == addOrderRequest.UserId);
            var newOrder = new Orders()
            {
                Id = new Guid(),
                User = user,
                Products = newOrderedProducts,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
            };

            await dbContext.Orders.AddAsync(newOrder);
            await dbContext.SaveChangesAsync();

            var emailSubject = "Congratulations on Creating a New Order from Our " + Constants.Constants.PROJECT_NAME + " 🎉!";
            var emailMessage =
                "<h2>Dear " + newOrder.User.Username + ",</h2><br/><br/>" +
                "We are absolutely thrilled to extend our warmest congratulations on successfully creating a new order through our cutting-edge " + Constants.Constants.PROJECT_NAME + ". Your support and trust in our platform are sincerely appreciated, and we are delighted to have you as a valued customer.<br/><br/>" +
                "At " + Constants.Constants.PROJECT_NAME + ", we strive to provide an exceptional online shopping experience, and your decision to place an order with us reaffirms our commitment to delivering quality products and outstanding service. We have received your order details, and our dedicated team is already working diligently to ensure a seamless fulfillment process.<br/><br/>" +
                "<h2>Order Details:</h2><br/>" +
                "<b>Order Id</b>: " + newOrder.Id + "<br/>" +
                "<b>Order Date</b>: " + newOrder.CreatedAt + "<br/><br/>" +
                "Rest assured that your order is in good hands, and we will keep you updated every step of the way. You can expect regular notifications regarding the progress of your order, including shipping information and estimated delivery dates.<br/><br/>" +
                "Should you have any questions, concerns, or specific requirements regarding your order, please don't hesitate to reach out to our customer support team. We are here to assist you and make your shopping experience as smooth as possible.<br/><br/>" +
                "Once again, congratulations on creating a new order with us! We sincerely appreciate your business and look forward to exceeding your expectations with our exceptional products and services. We value your continued support and hope to serve you again in the future.<br/><br/>" +
                "Warmest regards," +
                Constants.Constants.EMAIL_FOOTER;
            Utils.Utils.SendEmail(newOrder.User.Email, emailSubject, emailMessage);

            return Ok(newOrder);
        }

        [HttpPut]
        [Route("{orderId:guid}")]
        public async Task<IActionResult> UpdateSingleOrder([FromRoute] Guid orderId, UpdateOrderRequest updateOrderRequest)
        {
            var order = await dbContext.Orders.FirstOrDefaultAsync(order => order.Id == orderId);
            foreach (var product in updateOrderRequest.Products)
            {
                await dbContext.Entry(order).Reference(o => o.User).LoadAsync();
                await dbContext.Entry(order).Collection(o => o.Products).LoadAsync();
            }

            if (order != null)
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == updateOrderRequest.UserId);
                if (user == null) return BadRequest("No Such User Exists!");

                order.User = user;

                order.Products.Clear();
                foreach (var newOrderedProduct in updateOrderRequest.Products)
                {
                    OrderedProduct product = new OrderedProduct()
                    {
                        Id = new Guid(),
                        Asin = newOrderedProduct.Asin,
                        ImageUrl = newOrderedProduct.ImageUrl,
                        Name = newOrderedProduct.Name,
                        Quantity = newOrderedProduct.Quantity,
                    };
                    order.Products.Add(product);
                }

                order.CreatedAt = updateOrderRequest.CreatedAt;
                order.Status = updateOrderRequest.Status;

                await dbContext.SaveChangesAsync();

                var emailSubject = "";
                var emailMessage = "";

                switch (order.Status)
                {
                    case Constants.Constants.ORDER_STATUS_APPROVED:
                        //string googleMapsImageUrl = $"https://maps.googleapis.com/maps/api/staticmap?center={Uri.EscapeDataString(location)}&zoom=14&size=600x400&maptype=roadmap&markers=color:red%7C{Uri.EscapeDataString(location)}&key=YOUR_API_KEY";
                        //< img src =\"{googleMapsImageUrl}\" alt=\"Google Map\" />
                        emailSubject = "Congratulations, your order is ready and good to go 🎉!";
                        emailMessage =
                            "<h2>Dear " + order.User.Username + ",</h2><br /> <br />" +
                            "We hope this email finds you well. We are reaching out to remind you that your ordered products are now ready for pick up from our " + Constants.Constants.PROJECT_NAME + ". We are excited to fulfill your order and provide you with the items you have selected.<br /> <br />" +
                            "<h2>Order Details:</h2><br />" +
                            "<b>Order Number:</b> " + order.Id + "<br />" +
                            "<b>Pick-up Location:</b> " + Constants.Constants.ORDER_PICK_UP_LOCATION + "<br />" +
                            "<b>Pick-up Date/Time:</b> " + new DateTime().ToLocalTime() + "<br /> <br />" +
                            "Please make sure to bring a valid identification document and your order confirmation email or the order number when you arrive for pick up. This will help us ensure a smooth and efficient process.<br /> <br />" +
                            "If you are unable to pick up your order on the specified date and time, please let us know as soon as possible so that we can make alternative arrangements or hold your order for a limited period. You can reach out to our customer support team at " + Constants.Constants.CUSTOMER_SUPPORT_EMAIL + " to discuss any adjustments needed.<br /> <br />" +
                            "We kindly request that you pick up your order within " + Constants.Constants.ORDER_PICK_UP_TIME_FRAME + " to avoid any inconvenience or storage limitations at our pick-up location. If you have any questions or concerns, please do not hesitate to contact us.<br /> <br />" +
                            "Once again, we would like to express our gratitude for choosing our " + Constants.Constants.PROJECT_NAME + " for your purchase. We appreciate your trust in our brand, and we strive to provide you with the best possible service.<br /> <br />" +
                            "We look forward to assisting you with the pick-up of your order and ensuring a seamless experience for you. Thank you for being a valued customer, and we hope you enjoy your new products.<br /> <br />" +
                            "Best regards," +
                            Constants.Constants.EMAIL_FOOTER;
                        break;
                    case Constants.Constants.ORDER_STATUS_REJECTED:
                        emailSubject = "Order Rejection Notification - " + order.Id;
                        emailMessage =
                            "<h2>Dear " + order.User.Username + ",</h2><br /> <br />" +
                            "We hope this email finds you well. We appreciate your interest in our products and thank you for choosing our company for your recent order. However, we regret to inform you that your order, with the reference number " + order.Id + ", has been rejected. We understand that this news may be disappointing, and we sincerely apologize for any inconvenience caused.<br /><br />" +
                            "After careful consideration, we have identified several reasons for the rejection of your order. We would like to provide you with a detailed explanation of each factor to ensure transparency and avoid any confusion:<br /><br />" +
                            "1. <b>Product Unavailability</b>: Unfortunately, one or more items from your order are currently out of stock or discontinued. Our inventory management system failed to update the stock levels accurately at the time of your purchase. We apologize for the oversight and any inconvenience this may have caused.<br />" +
                            "2. <b>Fraud Prevention Measures</b>: Our rigorous security protocols flagged your order as potentially high-risk based on various factors, such as billing address mismatch, suspicious activity, or inconsistencies in the provided information. To protect our customers and maintain a secure environment, we have opted to reject the order. We suggest reviewing the information provided during the ordering process to ensure accuracy.<br /><br />" +
                            "We understand that these reasons may not cover all possible scenarios, but we assure you that our team has thoroughly assessed your order to identify the underlying cause. If you have any further questions or require clarification regarding the rejection, please don't hesitate to contact our customer support team at " + Constants.Constants.CUSTOMER_SUPPORT_EMAIL + ". They will be more than happy to assist you.<br /><br />" +
                            "Once again, we apologize for the inconvenience caused by the rejection of your order. We value your business and appreciate your understanding. Should you decide to place a new order or require any assistance in the future, we will be delighted to serve you.<br /><br />" +
                            "Thank you for considering our products, and we hope to have the opportunity to fulfill your requirements in the near future.<br /><br />" +
                            "Best regards," +
                            Constants.Constants.EMAIL_FOOTER;
                        break;
                    case Constants.Constants.ORDER_STATUS_DONE:
                        emailSubject = "Thank You for Your Successful Order 🎉!";
                        emailMessage =
                            "<h2>Dear " + order.User.Username + ",</h2><br /> <br />" +
                            "I hope this email finds you well. On behalf of " + Constants.Constants.PROJECT_NAME + ", I would like to extend our sincerest thanks for your recent order on our " + Constants.Constants.PROJECT_NAME + ". We are thrilled to inform you that your order has been successfully processed.<br /> <br />" +
                            "We understand that there are countless options available when it comes to online shopping, and we are truly honored that you chose our website for your purchase. Your support is invaluable to us, and we are committed to providing you with an exceptional shopping experience.<br /> <br />" +
                            "Our dedicated team has carefully taken great care to provide you with the necessary tracking information, which you will find in a separate email. This will allow you to monitor the progress of your shipment every step of the way.<br /> <br />" +
                            "If you have any questions or concerns regarding your order, our customer support team is readily available to assist you. Feel free to reach out to us at " + Constants.Constants.CUSTOMER_SUPPORT_EMAIL + ", and we will be more than happy to help.<br /> <br />" +
                            "Once again, we want to express our heartfelt appreciation for choosing " + Constants.Constants.PROJECT_NAME + ". Your trust in our brand means a lot to us, and we will continue to work hard to meet and exceed your expectations.<br /> <br />" +
                            "We hope you enjoy your new purchase, and we look forward to serving you again in the future. Thank you for being a valued customer.<br /> <br />" +
                            "Best regards," +
                            Constants.Constants.EMAIL_FOOTER;
                        break;
                }
                Utils.Utils.SendEmail(order.User.Email, emailSubject, emailMessage);
                return Ok(order);
            }
            return NotFound("No such Order Found!");
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteSingleOrder(Guid id)
        {
            var order = await dbContext.Orders
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null)
            {
                // Delete associated products
                foreach (var product in order.Products)
                {
                    dbContext.Remove(product);
                }

                dbContext.Remove(order);
                await dbContext.SaveChangesAsync();
                return Ok(order);
            }

            return NotFound("No such order exists!");
        }

    }
}
