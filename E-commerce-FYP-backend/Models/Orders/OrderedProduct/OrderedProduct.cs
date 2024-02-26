using E_commerce_FYP_backend.Models.Users;

namespace E_commerce_FYP_backend.Models.Orders.Product
{
    public class OrderedProduct
    {
        public Guid Id { get; set; }
        public string Asin { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
    }
}
