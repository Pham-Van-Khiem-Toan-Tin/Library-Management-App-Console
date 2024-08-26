using MySql.Data.MySqlClient;
using System.Data;

namespace LibraryManagement.Config
{
    public class ApplicationDBContext
    {
        private readonly string _connectionString;
        private MySqlConnection _connection;

        public ApplicationDBContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public MySqlConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new MySqlConnection(_connectionString);
                }
                return _connection;
            }
        }
        public void OpenConnection()
        {
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
            }
        }
        public void CloseConnection()
        {
            if (Connection.State == ConnectionState.Open)
            {
                Connection.Close();
            }
        }
        public MySqlCommand CreateCommand(string query)
        {
            var command = new MySqlCommand(query, Connection);
            return command;
        }

        public void Dispose()
        {
            CloseConnection();
            _connection?.Dispose();
        }
    }
}
