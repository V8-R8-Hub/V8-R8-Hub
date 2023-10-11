using DBUpgrader.Interfaces;
using DBUpgrader.Models;
using Npgsql;
using Shared.Models;
using Shared.Util;

namespace DBUpgrader {
    internal class AdminConnectionFactory : IConnectionFactory {
        private NpgsqlConnection? _connection;
        private string _connectionString;

        public AdminConnectionFactory(ConnectionStringModel model) {
            _connectionString = DatabaseUtils.CreateConnectionString(model);
        }

        public async Task<NpgsqlConnection> GetConnection() {
            if (_connection == null) {
                _connection = new NpgsqlConnection(_connectionString);
                await _connection.OpenAsync();
            }

            return _connection;
        }

        public string GetDatabaseName() {
            if (_connection == null) {
                throw new Exception("Not connected");
            }
            return _connection.Database;
        }
    }
}
