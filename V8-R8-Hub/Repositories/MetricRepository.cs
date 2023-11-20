using Dapper;
using Npgsql;
using System.Data;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Repositories {
	public interface IMetricRepository {
		Task AddMetric(string metricJsonData, string category, int userId, Guid gameGuid);
	}

	public class MetricRepository : IMetricRepository {
		IDbConnection _db;
		public MetricRepository(IDbConnector dbConnector) {
			_db = dbConnector.GetDbConnection();
		}

		public async Task AddMetric(string metricJsonData, string category, int userId, Guid gameGuid) {
			try {
				await _db.ExecuteAsync("""
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
