using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data.SqlClient;

namespace EventTicket.App_Code
{
    public class StockLoginAuthorization
    {
        int UserID;
        int AccessLevel;
        public Boolean checkUser(string username, string password)
        {
            Boolean isUser = false;
            var connectionString = ConfigurationManager.ConnectionStrings["constrStock"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Login WHERE Username = '" + username + "' and Password = '" + password + "'", connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["ID"] != DBNull.Value)
                        {
                            UserID = Convert.ToInt32(reader["AllID"]);
                            AccessLevel = Convert.ToInt32(reader["AccessLevel"]);
                            isUser = true;
                        }
                    }
                }
            }
            return isUser;
        }

        public int getUserID()
        {
            return UserID;
        }
        public int getAccessLevel()
        {
            return AccessLevel;
        }
    }
}