using E_commerce_FYP_backend.Data;
using E_commerce_FYP_backend.Models.NodeProducts;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_FYP_backend.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class NodeProductsController : Controller
    {
        private readonly ECommerceDbContext dbContext;

        public NodeProductsController(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> NodeProducts()
        {
            return Ok(await dbContext.NodeProducts.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetProduct([FromRoute] Guid id)
        {
            var product = await dbContext.NodeProducts.FindAsync(id);
            if (product != null)
            {
                return Ok(product);
            }
            return NotFound("No Product Found!");
        }

        [HttpGet]
        [Route("get-products/{id:guid}")]
        public async Task<IActionResult> GetNodeProductsForId([FromRoute] Guid id)
        {
            var node = await dbContext.AmazonNodes.FirstOrDefaultAsync(x => x.Id == id);

            if (node != null)
            {
                List<NodeProducts> products = dbContext.NodeProducts.ToList().FindAll((product) => product.ParentNodeId == node.Id);
                return Ok(products);
            }
            return NotFound("No Node Found!");
        }

        [HttpPost]
        public async Task<IActionResult> NodeProducts(AddNodeProductsRequest product)
        {
            var parentNode = await dbContext.AmazonNodes.FirstOrDefaultAsync((node) => node.NodeId == product.ParentNodeId);

            if (parentNode != null)
            {
                var newProduct = new NodeProducts()
                {
                    Id = Guid.NewGuid(),
                    ParentNodeId = parentNode.Id,
                    Asin = product.Asin,
                    Name = product.Name,
                    MonthlySalesEstimation = product?.MonthlySalesEstimation ?? 0,
                    WeeklySalesEstimation = product?.WeeklySalesEstimation ?? 0,
                };

                await dbContext.NodeProducts.AddAsync(newProduct);
                await dbContext.SaveChangesAsync();
                return Ok(newProduct);

            }
            return NotFound("No parent node found!");
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> NodeProducts([FromRoute] Guid id, UpdateNodeProductsRequest newProductData)
        {
            var product = await dbContext.NodeProducts.FirstOrDefaultAsync(product => product.Id == id);
            if (product != null)
            {
                var parentNode = await dbContext.AmazonNodes.FirstOrDefaultAsync((node) => node.Id.ToString() == newProductData.ParentNodeId);

                if (parentNode != null)
                {
                    product.ParentNodeId = parentNode.Id;
                    product.Name = newProductData.Name;
                    product.Asin = newProductData.Asin;
                    product.WeeklySalesEstimation = newProductData?.WeeklySalesEstimation ?? 0;
                    product.MonthlySalesEstimation = newProductData?.MonthlySalesEstimation ?? 0;


                    await dbContext.SaveChangesAsync();
                    return Ok(product);
                }
                return NotFound("No Such node found.");
            }
            return NotFound("No Such product found.");
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> NodeProducts([FromRoute] Guid id)
        {
            var product = await dbContext.NodeProducts.FindAsync(id);
            if (product != null)
            {
                dbContext.Remove(product);
                await dbContext.SaveChangesAsync();
                return Ok(product);
            }
            return NotFound("No product found!");
        }

        [HttpDelete]
        [Route("remove-products/{id:guid}")]
        public async Task<IActionResult> RemoveProductsFromNode([FromRoute] Guid id)
        {
            var node = await dbContext.AmazonNodes.FindAsync(id);
            if (node != null)
            {
                var productsArr = dbContext.NodeProducts.Where(product => product.ParentNodeId == node.Id);

                foreach (var product in productsArr)
                {
                    dbContext.Remove(product);
                }

                await dbContext.SaveChangesAsync();
                return Ok(node);
            }
            return NotFound("No node found!");
        }
    }
}
