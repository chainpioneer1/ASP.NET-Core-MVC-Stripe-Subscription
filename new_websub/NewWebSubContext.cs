using MySql.Data.MySqlClient;
using new_websub.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace new_websub
{
    public class NewWebSubContext
    {
        public string ConnectionString { get; set; }

        public NewWebSubContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(ConnectionString);
        }

        
    }
}
