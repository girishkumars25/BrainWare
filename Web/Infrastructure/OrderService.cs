using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Infrastructure
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using Models;

    public class OrderService : IDisposable
    {
        private readonly SqlConnection _connection;
        public List<Order> GetOrdersForCompany(int companyId)
        {
            using (var database = new Database())
            {
                var orders = GetOrders(database, companyId);
                var orderProducts = GetOrderProducts(database);

                MapOrderProductsToOrders(orders, orderProducts);

                return orders;
            }
        }

        private List<Order> GetOrders(Database database, int companyId)
        {
            var storedProcedureName = "GetOrdersForCompany";
            var parameters = new SqlParameter("@CompanyId", companyId);

            using (var reader = database.ExecuteStoredProcedure(storedProcedureName, new[] { parameters }))
            {
                var orders = new List<Order>();

                while (reader.Read())
                {
                    orders.Add(new Order()
                    {
                        CompanyName = reader.GetString(0),
                        Description = reader.GetString(1),
                        OrderId = reader.GetInt32(2),
                        OrderProducts = new List<OrderProduct>(),
                        OrderTotal = 0
                    });
                }

                reader.Close();
                return orders;
            }
        }

        private List<OrderProduct> GetOrderProducts(Database database)
        {
            var storedProcedureName = "GetOrderProducts";

            using (var reader = database.ExecuteStoredProcedure(storedProcedureName, null))
            {
                var orderProducts = new List<OrderProduct>();

                while (reader.Read())
                {
                    orderProducts.Add(new OrderProduct()
                    {
                        OrderId = reader.GetInt32(1),
                        ProductId = reader.GetInt32(2),
                        Price = reader.GetDecimal(0),
                        Quantity = reader.GetInt32(3),
                        Product = new Product()
                        {
                            Name = reader.GetString(4),
                            Price = reader.GetDecimal(5)
                        }
                    });
                }

                reader.Close();
                return orderProducts;
            }
        }



        private void MapOrderProductsToOrders(List<Order> orders, List<OrderProduct> orderProducts)
        {
            foreach (var order in orders)
            {
                var orderProductsWithSameOrderId = orderProducts.Where(op => op.OrderId == order.OrderId);

                foreach (var orderProduct in orderProductsWithSameOrderId)
                {
                    order.OrderProducts.Add(orderProduct);
                    order.OrderTotal += orderProduct.Price * orderProduct.Quantity;
                }
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Close();
                    _connection.Dispose();
                }
            }
        }

    }
}