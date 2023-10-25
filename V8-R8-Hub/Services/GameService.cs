using Dapper;
using System.Data;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Models.Response;

namespace V8_R8_Hub.Services {
	public interface IGameService {
		Task<ObjectIdentifier> CreateGame(string name, string description, VirtualFile gameFile, VirtualFile thumbnailFile);
		Task<ISet<string>> GetAllowedGameMimeTypes();
		Task<ISet<string>> GetAllowedThumbnailMimeTypes();
        Task<GameBrief> GetGame(Guid guid);
        Task<IEnumerable<GameBrief>> GetGames();
	}

	public class GameService : IGameService {
		private readonly IDbConnection _connection;
		private readonly ISafeFileService _safeFileService;

		public GameService(IDbConnector connector, ISafeFileService safeFileService) {
			_connection = connector.GetDbConnection();
			_safeFileService = safeFileService;
		}

		public async Task<IEnumerable<GameBrief>> GetGames() {
			return await _connection.QueryAsync<GameBrief>("""
				SELECT 
					g.public_id AS guid,
					g.name,
					g.description,
					tf.public_id AS thumbnail_guid,
					gf.public_id AS game_blob_guid
					FROM games g
					INNER JOIN public_files tf ON g.thumbnail_file_id = tf.id
					INNER JOIN public_files gf ON g.game_file_id = gf.id
			""");
		}

		public async Task<GameBrief> GetGame(Guid guid) {
			return await _connection.QuerySingleAsync<GameBrief>("""
				SELECT 
					g.public_id AS guid,
					g.name,
					g.description,
					tf.public_id AS thumbnail_guid,
					gf.public_id AS game_blob_guid
					FROM games g
					INNER JOIN public_files tf ON g.thumbnail_file_id = tf.id
					INNER JOIN public_files gf ON g.game_file_id = gf.id
				WHERE
					g.public_id = @Guid
				""", new {
				Guid = guid
			});
        }

		public async Task<ObjectIdentifier> CreateGame(string name, string description, VirtualFile gameFile, VirtualFile thumbnailFile) {
			_connection.Open();
			using var transaction = _connection.BeginTransaction();
			var gameFileId = await _safeFileService.CreateFileFrom(gameFile, await GetAllowedGameMimeTypes());
			var thumbnailFileId = await _safeFileService.CreateFileFrom(thumbnailFile, await GetAllowedThumbnailMimeTypes());

			ObjectIdentifier gameId = await _connection.QuerySingleAsync<ObjectIdentifier>("""
					INSERT INTO games
						(name, description, game_file_id, thumbnail_file_id)
					VALUES 
						(@Name, @Description, @GameFileId, @ThumbnailFileId)
					RETURNING 
						id, public_id as guid;
				""", new {
					Name = name,
					Description = description,
					GameFileId = gameFileId.Id,
					ThumbnailFileId = thumbnailFileId.Id
				}
			);
			transaction.Commit();
			return gameId;

		}

		public Task<ISet<string>> GetAllowedGameMimeTypes() {
			return Task.FromResult<ISet<string>>(new HashSet<string>() {
				"text/javascript"
			});
		}

		public Task<ISet<string>> GetAllowedThumbnailMimeTypes() {
			return Task.FromResult<ISet<string>>(new HashSet<string>() {
				"image/png",
				"image/jpeg",
				"image/gif",
				"image/tiff"
			});
		}

	}
}
