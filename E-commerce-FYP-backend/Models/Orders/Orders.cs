namespace E_commerce_FYP_backend.Models.Orders
{
    public class Orders
    {
        public Guid Id { get; set; }
        public Users.Users User { get; set; }
        public ICollection<Product.OrderedProduct> Products { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
