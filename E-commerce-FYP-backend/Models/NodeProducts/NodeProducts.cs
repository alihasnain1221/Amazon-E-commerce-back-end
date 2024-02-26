namespace E_commerce_FYP_backend.Models.NodeProducts
{
    public class NodeProducts
    {
        public Guid Id { get; set; }
        public string Asin { get; set; }
        public string Name { get; set; }
        public Guid ParentNodeId { get; set; }
        public int? MonthlySalesEstimation { get; set; } = 0;
        public int? WeeklySalesEstimation { get; set; } = 0;

    }
}
