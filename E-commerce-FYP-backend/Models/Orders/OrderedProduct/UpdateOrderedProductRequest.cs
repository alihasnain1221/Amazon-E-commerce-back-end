namespace E_commerce_FYP_backend.Models.Orders.Product
{
    public class UpdateOrderedProductRequest
    {
        public string Asin { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
