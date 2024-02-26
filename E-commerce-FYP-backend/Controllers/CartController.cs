using E_commerce_FYP_backend.Data;
using E_commerce_FYP_backend.Models.CartProducts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_FYP_backend.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly ECommerceDbContext dbContext;

        public CartController(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> getCarts()
        {
            return Ok(await dbContext.CartProducts.ToListAsync());
        }

        [HttpGet]
        [Route("getUserCart/")]
        public async Task<IActionResult> getUserCart(Guid userId)
        {
            var products = await dbContext.CartProducts.Where(cart => cart.UserId == userId).ToListAsync();
            if (products != null)
            {
                return Ok(products);
            }
            return NoContent();
        }

        [HttpGet]
        [Route("getSingleProduct/")]
        public async Task<IActionResult> getSingleCartProduct(Guid id)
        {
            var product = await dbContext.CartProducts.FirstOrDefaultAsync(product => product.Id == id);
            if (product != null)
            {
                return Ok(product);
            }
            return NotFound("No Product found!");
        }

        [HttpPost]
        public async Task<IActionResult> addProductToCart(AddCartProductRequest addCartProductRequest)
        {
            var alreadyExist = await dbContext.CartProducts
                .FirstOrDefaultAsync(product => product.ProductId == addCartProductRequest.ProductId &&
                product.UserId == addCartProductRequest.UserId);

            if (alreadyExist == null)
            {
                var newProduct = new CartProducts()
                {
                    Id = new Guid(),
                    Name = addCartProductRequest.Name,
                    ProductId = addCartProductRequest.ProductId,
                    ImageLink = addCartProductRequest.ImageLink,
                    Quantity = addCartProductRequest.Quantity,
                    Price = addCartProductRequest.Price,
                    UserId = addCartProductRequest.UserId,
                };

                await dbContext.CartProducts.AddAsync(newProduct);
                await dbContext.SaveChangesAsync();
                return Ok(newProduct);
            }
            return BadRequest("Product already exists!");
        }

        [HttpPut]
        public async Task<IActionResult> updateCartProduct(UpdateCartProductRequest updateCartProductRequest)
        {
            var product = await dbContext.CartProducts.FirstOrDefaultAsync(product =>
                product.Id == updateCartProductRequest.Id
            );

            if (product != null)
            {
                if (updateCartProductRequest.Quantity == 0)
                {
                    product.Quantity = updateCartProductRequest.Quantity;
                    dbContext.Remove(product);
                }
                else
                {
                    product.ProductId = updateCartProductRequest.ProductId;
                    product.Name = updateCartProductRequest.Name;
                    product.ImageLink = updateCartProductRequest.ImageLink;
                    product.Price = updateCartProductRequest.Price;
                    product.UserId = updateCartProductRequest.UserId;
                    product.Quantity = updateCartProductRequest.Quantity;
                }
                await dbContext.SaveChangesAsync();
                return Ok(product);

            }
            return NotFound("Product Not Found!");
        }

        [HttpDelete]
        public async Task<IActionResult> removeCartProduct(RemoveCartProductRequest removeCartProductRequest)
        {
            var product = await dbContext.CartProducts.FirstOrDefaultAsync(product =>
                product.ProductId == removeCartProductRequest.ProductId
            );

            if (product != null)
            {
                dbContext.Remove(product);
                await dbContext.SaveChangesAsync();
                return Ok(product);
            }
            return NotFound("Product not found!");
        }

        [HttpDelete]
        [Route("removeProductsFromUser/{id:guid}")]
        public async Task<IActionResult> removeProductsFromUser([FromRoute] Guid id)
        {
            var products = await dbContext.CartProducts.Where(product => product.UserId == id).ToListAsync();

            if (products.Count > 0)
            {
                foreach (var product in products)
                {
                    dbContext.Remove(product);
                }

                await dbContext.SaveChangesAsync();
                return Ok("Deleted Successfully!");
            }
            return NotFound("No Products Available!");
        }
    }
}
