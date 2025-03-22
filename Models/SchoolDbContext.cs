using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace Cummulative1_schooldb.Models
{
    /// <summary>
    /// Represents the context for connecting to the school database.
    /// Provides methods to access and interact with the database.
    /// </summary>
    public class SchoolDbContext
    {
        // Database connection credentials
        private static string User { get { return "root"; } } // Database username
        private static string Password { get { return ""; } } // Database password (empty)
        private static string Database { get { return "school"; } } // Name of the database
        private static string Server { get { return "localhost"; } } // Database server address (local)
        private static string Port { get { return "3306"; } } // Port number used for MySQL

        // Connection string used to establish a connection to the database
        // This string contains all necessary details like server, user, database, and password.
        protected static string ConnectionString
        {
            get
            {
                return "server=" + Server
                    + ";user=" + User
                    + ";database=" + Database
                    + ";port=" + Port
                    + ";password=" + Password
                    + ";convert zero datetime=True";
            }
        }

        /// <summary>
        /// Establishes and returns a connection to the school database using the provided connection string.
        /// </summary>
        /// <returns>A MySqlConnection object that can be used to interact with the database.</returns>
        public MySqlConnection AccessDatabase()
        {
            // Creates a new MySqlConnection object using the connection string
            // This connection object will be used to communicate with the school database on localhost:3306
            return new MySqlConnection(ConnectionString);
        }
    }
}