using MySql.Data.MySqlClient;

namespace BookManage
{
    public interface ISqlService
    {
        public MySqlConnection GetConnection();
    }

    public class SqlService : ISqlService
    {
        private readonly string _connectionString;

        public SqlService(string connectionString) => _connectionString = connectionString;

        public MySqlConnection GetConnection() => new MySqlConnection(_connectionString);
    }
}