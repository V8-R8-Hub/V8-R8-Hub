using Dapper;
using Npgsql;
using System.Data;
using V8_R8_Hub.Models.Exceptions;

namespace V8_R8_Hub.Services {
	public interface IMetricService {
		public Task AddMetric(string metricJsonData, string category, int userId, Guid gameGuid);
	}

	public class MetricService : IMetricService {
		private readonly IDbConnection _dbConnection;

		public MetricService(IDbConnector dbConnector) {
			_dbConnection = dbConnector.GetDbConnection();
		}

		public async Task AddMetric(string metricJsonData, string category, int userId, Guid gameGuid) {
			try {
				var rowsAffected = await _dbConnection.ExecuteAsync("""
				INSERT INTO game_metrics (metric, category, user_id, game_id)
					VALUES (@Metric::json, @Category, @UserId, (SELECT id FROM games WHERE public_id = @GameGuid))
				""", new {
					Metric = metricJsonData,
					Category = category,
					UserId = userId,
					GameGuid = gameGuid
				});
			} catch (PostgresException ex)
				when (ex.SqlState == PostgresErrorCodes.InvalidTextRepresentation) {
				throw new InvalidJsonException("Given json was invalid");
			} catch (PostgresException ex) 
				when (ex.SqlState == PostgresErrorCodes.NotNullViolation) {
				throw new UnknownGameException(gameGuid, "Game not found");
			}
		}
	}
}
