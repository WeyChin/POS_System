using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace POS_System.Models
{
    public class ProductDetails
    {
        [Key] public int ProductID { get; set; }
        public int StatusID { get; set; }
        public int? PromotionID { get; set; }
        public bool Active { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    public class ProductQuantityUpdateRequest
    {
        public List<ProductQuantityUpdateItem> Items { get; set; }
    }

    public class ProductQuantityUpdateItem
    {
        public string SKU { get; set; }
        public int Quantity { get; set; }
    }


}
