using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace POS_System.DA.ProductDA
{
    public class ProductDAL : DbContext
    {
        public ProductDAL(DbContextOptions<ProductDAL> options) : base(options)
        {
        }

        public DbSet<Models.ProductDetails> ProductDetails { get; set; }

        public Models.ProductDetails GetProductDetailsByProductSKU(string productSKU)
        {
            try
            {
                return ProductDetails.FirstOrDefault(u => u.SKU == productSKU);
            }
            catch
            {
                throw;
            }
        }

        public void UpdateStockQtyByProductSKU(string productSKU, int quantity, int userID)
        {

            try
            {
                string sql = "UPDATE stk SET stk.quantity = stk.quantity - @Qty, stk.ModifiedBy = @userID, stk.ModifiedDate = GETDATE() FROM dbo.stockqty AS stk INNER JOIN dbo.productdetails AS pd ON stk.productid = pd.productid WHERE pd.sku = @SKU AND stk.StatusID = 3";

                // Create SqlConnection object (assuming your connection string is defined elsewhere)
                using (SqlConnection connection = new SqlConnection(Database.GetDbConnection().ConnectionString))
                {
                    // Open the connection
                    connection.Open();

                    // Create SqlCommand object with the SQL command and connection
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        // Add parameters to the command
                        command.Parameters.AddWithValue("@Qty", quantity);
                        command.Parameters.AddWithValue("@userID", userID);
                        command.Parameters.AddWithValue("@SKU", productSKU);

                        // Execute the SQL command
                        command.ExecuteNonQuery();
                    }

                    connection.Close();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
