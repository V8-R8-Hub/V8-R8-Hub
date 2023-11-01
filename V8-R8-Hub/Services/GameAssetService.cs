using Dapper;
using Npgsql;
using System.Data;
using V8_R8_Hub.Models.DB;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;

namespace V8_R8_Hub.Services {
	public interface IGameAssetService {
		Task<GameAssetBrief> AddGameAsset(Guid gameId, VirtualFile assetFile);
		Task DeleteGameAsset(Guid gameId, string filePath);
		Task<FileData?> GetGameAsset(Guid gameGuid, string path);
        Task<IEnumerable<GameAssetBrief>> GetGameAssets(Guid gameGuid);
    }

	public class GameAssetService : IGameAssetService {
		private readonly IDbConnection _connection;
		private readonly IPublicFileService _fileService;
        private readonly ISafeFileService _safeFileService;

        public GameAssetService(IDbConnector connector, ISafeFileService safeFileService) {
            _connection = connector.GetDbConnection();
            _safeFileService = safeFileService;
        }

        public async Task<GameAssetBrief> AddGameAsset(Guid gameId, VirtualFile assetFile) {
			try {
				_connection.Open();

				using var transaction = _connection.BeginTransaction();
				var assetFileId = await _safeFileService.CreateFileFrom(assetFile, await GetAllowedGameAssetMimeTypes());

				var inserted = await _connection.QuerySingleAsync<GameAssetBrief>(@"
				INSERT INTO game_assets (game_id, file_id, path)
					VALUES ((SELECT id FROM games WHERE public_id = @GameId), @FileId, @Path)
					RETURNING path;
			", new {
					GameId = gameId,
					FileId = assetFileId.Id,
					Path = assetFile.FileName
				});
				transaction.Commit();
				return inserted;
			} catch (PostgresException ex) {
				if (ex.SqlState == PostgresErrorCodes.UniqueViolation && ex.ConstraintName == "game_assets_game_id_path_key")
					throw new DuplicateAssetException("Asset with that name already exists");
				if (ex.SqlState == PostgresErrorCodes.NotNullViolation && ex.ColumnName == "game_id")
					throw new UnknownGameException("Could not find game corresponding with the given guid");
				throw ex;
			}
		}

		public async Task DeleteGameAsset(Guid gameId, string filePath) {
			_connection.Open();
			using var transaction = _connection.BeginTransaction();
			var deleted = await _connection.QueryAsync<int>("""
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
				throw new UnknownGameException("No game assets were deleted");
			}
			transaction.Commit();
		}

		public async Task<FileData?> GetGameAsset(Guid gameGuid, string path) {
			return await _connection.QuerySingleOrDefaultAsync<FileData>(@"
				SELECT f.mime_type, f.file_name, f.content_blob 
					FROM game_assets ga
					INNER JOIN games g ON ga.game_id = g.id
					INNER JOIN public_files f ON ga.file_id = f.id
					WHERE g.public_id = @GameGuid AND ga.path = @Path
					LIMIT 1
			", new {
				GameGuid = gameGuid,
				Path = path
			});
		}

		public async Task<IEnumerable<GameAssetBrief>> GetGameAssets(Guid gameGuid) {
			return await _connection.QueryAsync<GameAssetBrief>(@"
				SELECT ga.path
					FROM game_assets ga
					INNER JOIN games g ON ga.game_id = g.id
					WHERE g.public_id = @GameGuid
			", new {
				GameGuid = gameGuid
			});
		}

		public Task<ISet<string>> GetAllowedGameAssetMimeTypes() {
			return Task.FromResult<ISet<string>>(new HashSet<string>() {
				"text/javascript",
				"text/plain",
				"text/css",
				"image/png",
				"image/jpeg",
				"image/gif",
				"image/tiff"
			});
		}
	}
}
