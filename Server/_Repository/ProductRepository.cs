using Server.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server._Repository
{
    internal class ProductRepository : Base
    {
        public ProductRepository(string connectionString)
        {
            Delay = 7000;
            this.connectionString = connectionString;
        }

        public int Delay { get; }

        public bool AddProduct(Product productModel)
        {            
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "insert into Products values (@name, @description, @price, @status)";
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = productModel.Name;
                command.Parameters.Add("@description", SqlDbType.VarChar).Value = productModel.Description;
                command.Parameters.Add("@price", SqlDbType.Int).Value = productModel.Price;
                command.Parameters.Add("@status", SqlDbType.VarChar).Value = productModel.Status;
               
                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        command.Transaction = transaction;
                        Logs.SaveProduct(command.CommandText, productModel);
                        command.ExecuteNonQuery();
                        Thread.Sleep(Delay);
                        transaction.Commit();
                        return true;
                    }
                    catch(Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }                
            }
        }

        public bool EditProduct(Product product)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "update Products set name=@name, description=@description, price=@price, status=@status where id_product=@id";
                command.Parameters.Add("@id", SqlDbType.Int).Value = product.IdProduct;
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = product.Name;
                command.Parameters.Add("@description", SqlDbType.VarChar).Value = product.Description;
                command.Parameters.Add("@price", SqlDbType.Int).Value = product.Price;
                command.Parameters.Add("@status", SqlDbType.VarChar).Value = product.Status;

                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        command.Transaction = transaction;
                        Logs.SaveProduct(command.CommandText, product);
                        command.ExecuteNonQuery();
                        Thread.Sleep(Delay);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }                
            }
        }

        public bool DeleteProduct(int IdProduct)
        {
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;                              

                command.CommandText = "delete from Products where id_product=@id";
                command.Parameters.Add("@id", SqlDbType.Int).Value = IdProduct;

                using (var transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        command.Transaction = transaction;
                        Logs.SaveProduct(command.CommandText);
                        command.ExecuteNonQuery();
                        Thread.Sleep(Delay);
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        return false;
                    }
                }                            
            }
        }

        public List<Product> GetProducts()
        {
            var ProductList = new List<Product>();

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "Select * from Products order by id_product desc";
                
                Logs.SaveProduct(command.CommandText);
                using(var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Product Product = new Product();
                        Product.IdProduct = reader.GetInt32(0);
                        Product.Name = reader.GetString(1);
                        Product.Description = reader.GetString(2);
                        Product.Price = reader.GetInt32(3);
                        Product.Status = reader.GetString(4);

                        ProductList.Add(Product);
                    }
                }
            }

            return ProductList;
        }

        public List<Product> GetProductsByValue(string value)
        {
            var ProductList = new List<Product>();
            int ProductId = int.TryParse(value, out _) ? Convert.ToInt32(value) : 0;
            string Name = value;

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "Select * from Products where id_product=@id or name like '%'+@name+'%' order by id_product desc";

                command.Parameters.Add("@id", SqlDbType.Int).Value = ProductId;
                command.Parameters.Add("@name", SqlDbType.VarChar).Value = Name;

                Logs.SaveProduct(command.CommandText);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var ProductModel = new Product();
                        ProductModel.IdProduct = reader.GetInt32(0);
                        ProductModel.Name = reader.GetString(1);
                        ProductModel.Description = reader.GetString(2);
                        ProductModel.Price = reader.GetInt32(3);
                        ProductModel.Status = reader.GetString(4);

                        ProductList.Add(ProductModel);
                    }
                }
            }

            return ProductList;
        }
    }
}
