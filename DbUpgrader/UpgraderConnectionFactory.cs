using DBUpgrader.Interfaces;
using DBUpgrader.Models;
using DBUpgrader.Repositories;
using Npgsql;
using Shared.Models;
using Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DBUpgrader {
    internal class UpgraderConnectionFactory : IConnectionFactory {
        private NpgsqlConnection? _connection;
        private readonly IConfigRepository _configRepository;

        public UpgraderConnectionFactory(
           IConfigRepository configRepository
        ) {
            _configRepository = configRepository;
        }

        public async Task<NpgsqlConnection> GetConnection() {
            if (_connection == null) {
                var config = await _configRepository.GetConfig();
                var connectionString = DatabaseUtils.CreateConnectionString(new ConnectionStringModel {
                    Host = config.Host,
                    Username = config.Username,
                    Password = config.Password,
                    Port = config.Port,
                    DatabaseName = config.DatabaseName
                });
                _connection = new NpgsqlConnection(connectionString);
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
