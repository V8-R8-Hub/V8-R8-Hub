using Dapper;
using System.Data;
using V8_R8_Hub.Models.DB;
using V8_R8_Hub.Models.Internal;

namespace V8_R8_Hub.Services
{
	public interface IGameAssetService {
		Task<int> AddGameAsset(int gameId, string mimetype, string path, byte[] content);
		Task<FileData?> GetGameAsset(Guid gameGuid, string path);
	}

	public class GameAssetService : IGameAssetService {
		private readonly IDbConnection _connection;
		private readonly IPublicFileService _fileService;

		public GameAssetService(IDbConnector connector) {
			_connection = connector.GetDbConnection();
		}

		public async Task<int> AddGameAsset(int gameId, string mimetype, string path, byte[] content) {
			_connection.Open();
			using var transaction = _connection.BeginTransaction();
			ObjectIdentifier fileReference = await _fileService.CreateFile(path, mimetype, content);
			var id = await _connection.ExecuteScalarAsync<int>(@"
				INSERT INTO game_assets (game_id, file_id, path)
					VALUES (@GameId, @FileId, @Path)
					RETURNING id;
			", new {
				GameId = gameId,
				FileId = fileReference.Id,
				Path = path
			});
			transaction.Commit();
			return id;
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
				GameName = gameGuid,
				Path = path
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
