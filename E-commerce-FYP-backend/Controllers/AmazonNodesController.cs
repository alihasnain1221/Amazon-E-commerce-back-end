using E_commerce_FYP_backend.Data;
using E_commerce_FYP_backend.Models.AmazonNodes;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_FYP_backend.Controllers
{
    [EnableCors]
    [ApiController]
    [Route("api/[controller]")]
    public class AmazonNodesController : Controller
    {
        private readonly ECommerceDbContext dbContext;

        public AmazonNodesController(ECommerceDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAmazonNodes()
        {
            var nodeList = await dbContext.AmazonNodes.ToListAsync();
            var nodeListToSend = new Object[nodeList.Count];

            for (int i = 0; i < nodeList.Count; i++)
            {
                var products = await dbContext.NodeProducts.Where(product => product.ParentNodeId == nodeList[i].Id).ToListAsync();
                var WeeklySalesEstimation = 0;
                var MonthlySalesEstimation = 0;
                var productsWithEstimations = 0;

                foreach (var product in products)
                {
                    WeeklySalesEstimation += product.WeeklySalesEstimation ?? 0;
                    MonthlySalesEstimation += product.MonthlySalesEstimation ?? 0;
                    if (product.WeeklySalesEstimation > 0 || product.MonthlySalesEstimation > 0)
                    {
                        productsWithEstimations++;
                    }
                }
                if (productsWithEstimations > 0)
                {
                    WeeklySalesEstimation /= productsWithEstimations;
                    MonthlySalesEstimation /= productsWithEstimations;
                }

                nodeListToSend[i] = new
                {
                    Node = nodeList[i],
                    Estimations = new
                    {
                        WeeklySalesEstimation,
                        MonthlySalesEstimation
                    }
                };
            }

            return Ok(nodeListToSend);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetAmazonNode([FromRoute] Guid id)
        {
            var node = await dbContext.AmazonNodes.FindAsync(id);
            if (node != null)
            {
                return Ok(node);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> AddAmazonNode(AddAmazonNodeRequest addAmazonNodeRequest)
        {
            var node = new AmazonNodes()
            {
                Id = Guid.NewGuid(),
                Name = addAmazonNodeRequest.Name,
                NodeId = addAmazonNodeRequest.NodeId,
                AmazonNodeId = addAmazonNodeRequest.AmazonNodeId,
                Domain = addAmazonNodeRequest.Domain,
                Visible = addAmazonNodeRequest.Visible,
            };

            await dbContext.AmazonNodes.AddAsync(node);
            await dbContext.SaveChangesAsync();

            return Ok(node);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateAmazonNode([FromRoute] Guid id, UpdateAmazonNodeRequest updateAmazonNodeRequest)
        {
            var nodeToBeUpdated = await dbContext.AmazonNodes.FindAsync(id);
            if (nodeToBeUpdated != null)
            {
                nodeToBeUpdated.Name = updateAmazonNodeRequest.Name;
                nodeToBeUpdated.NodeId = updateAmazonNodeRequest.NodeId;
                nodeToBeUpdated.AmazonNodeId = updateAmazonNodeRequest.AmazonNodeId;
                nodeToBeUpdated.Domain = updateAmazonNodeRequest.Domain;
                nodeToBeUpdated.Visible = updateAmazonNodeRequest.Visible;

                await dbContext.SaveChangesAsync();

                return Ok(nodeToBeUpdated);
            }
            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteAmazonNode([FromRoute] Guid id)
        {
            var node = await dbContext.AmazonNodes.FindAsync(id);
            if (node != null)
            {
                dbContext.Remove(node);
                await dbContext.SaveChangesAsync();

                return Ok(node);
            }
            return NotFound();
        }
    }
}
