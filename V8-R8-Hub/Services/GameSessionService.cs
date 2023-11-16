using Dapper;
using Npgsql;
using System.Data;
using V8_R8_Hub.Models.Exceptions;

namespace V8_R8_Hub.Services {
	public interface IGameSessionService {
		Task AddGameSession(int userId, Guid gameGuid, DateTimeOffset start, DateTimeOffset stop);
	}
	public class GameSessionService : IGameSessionService {
		private readonly IDbConnection _connection;

		public GameSessionService(IDbConnector connector) {
			_connection = connector.GetDbConnection();
		}

		public async Task AddGameSession(int userId, Guid gameGuid, DateTimeOffset start, DateTimeOffset stop) {
			if (start > stop) {
				throw new ArgumentException("Start date must be before stop date", nameof(start));
			}
			try {
				await _connection.ExecuteAsync("""
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
