using Dapper;
using Npgsql;
using System.Data;
using System.Data.Common;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Repositories {
	public interface IGameSessionRepository {
		public Task AddGameSession(int userId, Guid gameGuid, DateTimeOffset start, DateTimeOffset stop);
	}

	public class GameSessionRepository : IGameSessionRepository {
		private readonly IDbConnection _db;

		public GameSessionRepository(IDbConnector connector) {
			_db = connector.GetDbConnection();
		}

		public async Task AddGameSession(int userId, Guid gameGuid, DateTimeOffset start, DateTimeOffset stop) {
			try {
				await _db.ExecuteAsync("""
				INSERT INTO user_game_sessions (user_id, game_id, start, stop)
					VALUES (@UserId, (SELECT id FROM games WHERE public_id = @GameGuid), @Start, @Stop)
				""", new {
					UserId = userId,
					GameGuid = gameGuid,
					Start = start.ToUniversalTime(),
					Stop = stop.ToUniversalTime()
				});
			} catch (PostgresException ex) {
				if (ex.SqlState == PostgresErrorCodes.NotNullViolation) {
					throw new UnknownGameException(gameGuid, "Unknown game guid");
				}
				throw ex;
			}
		}

	}
}
