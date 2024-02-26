namespace E_commerce_FYP_backend.Models.Orders
{
    public class UpdateOrderRequest
    {
        public Guid UserId { get; set; }
        public ICollection<Product.UpdateOrderedProductRequest> Products { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
