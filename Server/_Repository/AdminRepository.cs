using Server._Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server._Repository
{
    internal class AdminRepository : Base
    {
        public AdminRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public bool GetAdmin(string name, string password)
        {
            string pass = "";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand())
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "Select * from Admins where name = @name";
                command.Parameters.Add("name", SqlDbType.VarChar).Value= name;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        pass = reader.GetString(2);
                        break;
                    }
                }

                Logs.SaveAdmin(command.CommandText, name);
            }
          
            return (pass == password);
        }        
    }
}
