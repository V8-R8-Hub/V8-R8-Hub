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
		public IDbConnection GetDbConnection() {
			if (_connection == null) {
				_connection = new NpgsqlConnection(DatabaseUtils.CreateConnectionString(new ConnectionStringModel {
					Host = "localhost",
					Username = "v8_r8_api_user",
					Password = "bobby",
					DatabaseName = "v8r8hub"
				}));
			}
			return _connection;
		}
	}
}
