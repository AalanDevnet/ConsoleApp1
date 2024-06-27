using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=IDJKT02KEX001;Database=CashControlDB;User Id=sa;Password=P@ssw0rd;";

            int month = 6;
            int year = 2023;

            string query = @"
            SELECT TOP 3 
                c.CustomerCode,
                c.CustomerName,
                c.CustomerAddress,
                SUM(p.Price) AS TotalPrice,
                FORMAT(DATEFROMPARTS(@Year, @Month, 1), 'MMM-yy') AS PurchasePeriod
            FROM Purchase p
            INNER JOIN Customer c ON p.CustomerCode = c.CustomerCode
            WHERE MONTH(p.PurchaseDate) = @Month AND YEAR(p.PurchaseDate) = @Year
            GROUP BY c.CustomerCode, c.CustomerName, c.CustomerAddress
            ORDER BY TotalPrice DESC";

            List<CustomerPurchaseSummary> customerSummaries = new List<CustomerPurchaseSummary>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CustomerPurchaseSummary summary = new CustomerPurchaseSummary();

                          
                            if (!reader.IsDBNull(reader.GetOrdinal("CustomerCode")))
                                summary.CustomerCode = Convert.ToInt32(reader["CustomerCode"]);

                          
                            if (!reader.IsDBNull(reader.GetOrdinal("CustomerName")))
                                summary.CustomerName = reader["CustomerName"].ToString();

                           
                            if (!reader.IsDBNull(reader.GetOrdinal("CustomerAddress")))
                                summary.CustomerAddress = reader["CustomerAddress"].ToString();

                           
                            if (!reader.IsDBNull(reader.GetOrdinal("TotalPrice")))
                                summary.TotalPrice = Convert.ToDecimal(reader["TotalPrice"]);

                           
                            if (!reader.IsDBNull(reader.GetOrdinal("PurchasePeriod")))
                                summary.PurchasePeriod = reader["PurchasePeriod"].ToString();

                            customerSummaries.Add(summary);
                        }
                    }
                }
            }

           
            Console.WriteLine($"Top 3 Customers with Highest Purchase in {month}-{year}:");
            foreach (var customer in customerSummaries)
            {
                Console.WriteLine($"Customer Code: {customer.CustomerCode}");
                Console.WriteLine($"Customer Name: {customer.CustomerName}");
                Console.WriteLine($"Customer Address: {customer.CustomerAddress}");
                Console.WriteLine($"Total Price: {customer.TotalPrice:C}");
                Console.WriteLine($"Purchase Period: {customer.PurchasePeriod}");
                Console.WriteLine();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }

   
    public class CustomerPurchaseSummary
    {
        public int CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public decimal TotalPrice { get; set; }
        public string PurchasePeriod { get; set; }
    }
}
