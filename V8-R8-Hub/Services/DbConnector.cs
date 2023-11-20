using Npgsql;
using Shared.Models;
using Shared.Util;
using System.Data;

namespace V8_R8_Hub.Services {
	public interface IDbConnector {
		IDbConnection GetDbConnection();
	}

	public class DbConnector : IDbConnector {
		NpgsqlConnection? _connection;
		IConfigProvider _configProvider;
		public DbConnector(IConfigProvider configProvider) {
			_configProvider = configProvider;
		}
		public IDbConnection GetDbConnection() {
			if (_connection == null) {
				_connection = new NpgsqlConnection(DatabaseUtils.CreateConnectionString(new ConnectionStringModel {
					Host = _configProvider.DatabaseHost,
					Username = _configProvider.DatabaseUser,
					Password = _configProvider.DatabasePassword,
					DatabaseName = _configProvider.DatabaseName
				}));
			}
			return _connection;
		}
	}
}
