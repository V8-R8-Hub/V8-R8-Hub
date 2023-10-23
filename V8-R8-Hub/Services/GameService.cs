using Dapper;
using System.Data;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Models.Response;

namespace V8_R8_Hub.Services {
	public interface IGameService {
		Task<IEnumerable<GameBrief>> GetGames();
	}

	public class GameService : IGameService {
		private readonly IDbConnection _connection;

		public GameService(IDbConnector connector) {
			_connection = connector.GetDbConnection();
		}

		public async Task<IEnumerable<GameBrief>> GetGames() {
			return await _connection.QueryAsync<GameBrief>(@"
				SELECT 
					g.public_id AS guid,
					g.name
					g.description,
					tf.public_id AS thumbnail_guid
					gf.public_id AS game_blob_guid
					FROM games g
					INNER JOIN public_files tf ON g.thumbnail_file_id = tf.id
					INNER JOIN public_files gf ON g.game_file_id = gf.id
			");
		}
	}
}
