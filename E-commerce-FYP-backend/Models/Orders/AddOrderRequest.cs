namespace E_commerce_FYP_backend.Models.Orders
{
    public class AddOrderRequest
    {
        public Guid UserId { get; set; }
        public ICollection<Product.AddOrderedProductRequest> Products { get; set; }
    }
}
