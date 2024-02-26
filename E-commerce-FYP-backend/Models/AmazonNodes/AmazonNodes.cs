using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce_FYP_backend.Models.AmazonNodes
{
    public class AmazonNodes
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string NodeId { get; set; }
        public string AmazonNodeId {  get; set; }
        public string Domain { get; set; }
        public bool Visible { get; set; }
    }
}
