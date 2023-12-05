using Dapper;
using System.Data;
using System.Security.Cryptography;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Repositories {
	public interface IUserRepository {
		Task<string> CreateAuthKey(int userId);
		Task<User> CreateUser();
		Task<User?> GetUserFromAuthKey(string authKey);
	}

	public class UserRepository : IUserRepository {
		private readonly IDbConnection _db;

		public UserRepository(IDbConnector connector) {
			_db = connector.GetDbConnection();
		}
	
		public async Task<User> CreateUser() {
			var userId = await _db.QuerySingleAsync<int>("INSERT INTO users DEFAULT VALUES RETURNING id");
			return new User {
				Id = userId
			};
		}

		public async Task<string> CreateAuthKey(int userId) {
			var key = Convert.ToHexString(RandomNumberGenerator.GetBytes(8));

			await _db.ExecuteAsync("INSERT INTO auth_keys (user_id, key) VALUES (@UserId, @Key)", new {
				UserId = userId,
				Key = key
			});
			return key;
		}

		public async Task<User?> GetUserFromAuthKey(string authKey) {
			int? userId = await _db.QuerySingleOrDefaultAsync<int?>("""
					SELECT user_id
						FROM auth_keys
						WHERE key = @Key
					""", new {
				Key = authKey
			});
			if (userId == null) {
				return null;
			}
			return new User {
				Id = userId.Value
			};
		}
	}
}
