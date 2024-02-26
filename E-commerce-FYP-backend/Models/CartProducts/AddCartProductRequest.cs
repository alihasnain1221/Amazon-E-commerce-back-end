using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce_FYP_backend.Models.CartProducts
{
    public class AddCartProductRequest
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string ImageLink { get; set; }
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(6,3)")]
        public decimal Price { get; set; }
        public Guid UserId { get; set; }
    }
}
