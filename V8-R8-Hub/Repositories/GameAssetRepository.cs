using Dapper;
using Npgsql;
using System.Data;
using System.Data.Common;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Repositories {
	public interface IGameAssetRepository {
		Task<GameAssetBrief> AddGameAsset(int gameId, int fileId, string path);
		Task DeleteGameAsset(Guid gameId, string filePath);
		Task<IEnumerable<GameAssetBrief>> GetGameAssets(Guid gameGuid);
		Task<ObjectIdentifier?> GetGameAssetFileId(Guid gameGuid, string path);
	}

	public class GameAssetRepository : IGameAssetRepository {
		private readonly IDbConnection _db;

		public GameAssetRepository(IDbConnector connector) {
			_db = connector.GetDbConnection();
		}

		public async Task<IEnumerable<GameAssetBrief>> GetGameAssets(Guid gameGuid) {
			return await _db.QueryAsync<GameAssetBrief>("""
				SELECT ga.path
					FROM game_assets ga
					INNER JOIN games g ON ga.game_id = g.id
					WHERE g.public_id = @GameGuid
			""", new {
				GameGuid = gameGuid
			});
		}

		public async Task<ObjectIdentifier?> GetGameAssetFileId(Guid gameGuid, string path) {
			return await _db.QuerySingleOrDefaultAsync<ObjectIdentifier?>("""
				SELECT pf.id as "id", pf.public_id as "guid"
					FROM game_assets ga
					INNER JOIN games g ON ga.game_id = g.id
					INNER JOIN public_files pf ON ga.file_id = pf.id
					WHERE g.public_id = @GameGuid AND ga.path = @Path
					LIMIT 1
				""", new {
				GameGuid = gameGuid,
				Path = path
			});
		}

		public async Task DeleteGameAsset(Guid gameId, string filePath) {
			var deleted = await _db.QueryAsync<int>("""
				DELETE FROM game_assets 
					WHERE 
					game_id = (SELECT id FROM games WHERE public_id = @GameId)
					AND path = @Path
				RETURNING 1;
				""", new {
				GameId = gameId,
				Path = filePath
			});

			if (deleted.Count() != 1) {
				throw new UnknownGameException(gameId, "No game assets were deleted");
			}
		}

		public async Task<GameAssetBrief> AddGameAsset(int gameId, int fileId, string path) {
			try {
				return await _db.QuerySingleAsync<GameAssetBrief>("""
					INSERT INTO game_assets (game_id, file_id, path)
						VALUES (@GameId, @FileId, @Path)
						RETURNING path;
					""", new {
					GameId = gameId,
					FileId = fileId,
					Path = path
				});
			} catch (PostgresException ex)
				when (ex.SqlState == PostgresErrorCodes.UniqueViolation && ex.ConstraintName == "game_assets_game_id_path_key") {
				throw new DuplicateAssetException("Asset with that name already exists");
			}
		}
	}
}
