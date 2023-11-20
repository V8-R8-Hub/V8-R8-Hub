using Dapper;
using Npgsql;
using Swashbuckle.Swagger;
using System.Data;
using System.Data.Common;
using System.Xml.Linq;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Repositories {
	public interface IGameRepository {
		Task<IEnumerable<GameBriefQueryModel>> GetGames();
		Task<ObjectIdentifier> CreateGame(CreateGameModel model);
		Task AddGameTag(Guid gameGuid, int tagId);
		Task RemoveGameTag(Guid gameGuid, string tag);
		Task<GameBriefQueryModel> GetGame(Guid guid);
		Task<int> CreateOrGetTagId(string tagName);
		Task<int?> GetGameId(Guid guid);
	}

	public class GameRepository : IGameRepository {
		private readonly IDbConnection _db;

		public GameRepository(IDbConnector connector) {
			_db = connector.GetDbConnection();
		}

		public async Task AddGameTag(Guid gameGuid, int tagId) {
			try {
				await _db.ExecuteAsync("""
				INSERT INTO game_tags (game_id, tag_id)
					VALUES ((SELECT id FROM games WHERE public_id = @GameGuid), @TagId)
				""", new {
					GameGuid = gameGuid,
					TagId = tagId
				});
			} catch (PostgresException ex)
				when (ex.SqlState == PostgresErrorCodes.UniqueViolation && ex.ConstraintName == "game_tags_pkey") {
				throw new DuplicateTagException(null, "The given game already has the given tag");
			} catch (PostgresException ex)
				when (ex.SqlState == PostgresErrorCodes.NotNullViolation && ex.ColumnName == "game_id") {
				throw new UnknownGameException(gameGuid, "Unknown game");

			}
		}

		public async Task<ObjectIdentifier> CreateGame(CreateGameModel model) {
			return await _db.QuerySingleAsync<ObjectIdentifier>("""
					INSERT INTO games
						(name, description, game_file_id, thumbnail_file_id)
					VALUES 
						(@Name, @Description, @GameFileId, @ThumbnailFileId)
					RETURNING 
						id, public_id as guid;
				""", new {
				Name = model.Name,
				Description = model.Description,
				GameFileId = model.GameFileId,
				ThumbnailFileId = model.ThumbnailFileId
			}
			);
		}

		public async Task<IEnumerable<GameBriefQueryModel>> GetGames() {
			return await _db.QueryAsync<GameBriefQueryModel>("""
				SELECT 
					g.public_id AS guid,
					g.name,
					g.description,
					tf.public_id AS thumbnail_guid,
					gf.public_id AS game_blob_guid,
					(
						SELECT string_agg(t.name, ',') 
							FROM game_tags gt 
							LEFT JOIN tags t 
								ON t.id = gt.tag_id 
							WHERE gt.game_id = g.id
					) AS comma_seperated_tags
				FROM games g
				INNER JOIN public_files tf ON g.thumbnail_file_id = tf.id
				INNER JOIN public_files gf ON g.game_file_id = gf.id
			""");
		}

		public async Task<GameBriefQueryModel?> GetGame(Guid guid) {
			return await _db.QuerySingleOrDefaultAsync<GameBriefQueryModel?>("""
				SELECT 
					g.public_id AS guid,
					g.name,
					g.description,
					tf.public_id AS thumbnail_guid,
					gf.public_id AS game_blob_guid,
					(
						SELECT string_agg(t.name, ',') 
							FROM game_tags gt 
							LEFT JOIN tags t 
								ON t.id = gt.tag_id 
							WHERE gt.game_id = g.id
					) AS comma_seperated_tags
				FROM games g
				INNER JOIN public_files tf ON g.thumbnail_file_id = tf.id
				INNER JOIN public_files gf ON g.game_file_id = gf.id
				WHERE g.public_id = @Guid
			""", new {
				Guid = guid
			});
		}

		public async Task<int?> GetGameId(Guid guid) {
			return await _db.QuerySingleOrDefaultAsync<int?>("""
				SELECT 
					g.id
				FROM games g
				WHERE g.public_id = @Guid
			""", new {
				Guid = guid
			});
		}

		public async Task RemoveGameTag(Guid gameGuid, string tag) {
			await _db.QuerySingleOrDefaultAsync<int?>("""
					DELETE FROM game_tags 
						WHERE game_id = (SELECT id FROM games WHERE public_id = @GameGuid)
						AND tag_id = (SELECT id FROM tags WHERE name = @Tag)
				""", new {
				GameGuid = gameGuid,
				Tag = tag
			});
		}

		public async Task<int> CreateOrGetTagId(string tagName) {
			var tagId = await _db.QuerySingleOrDefaultAsync<int?>("SELECT id FROM tags WHERE name=@Tag", new {
				Tag = tagName
			});

			return tagId ?? await _db.QuerySingleAsync<int>("""
					INSERT INTO tags (name)
						VALUES (@Tag)
					RETURNING id
					""", new {
				Tag = tagName
			});
		}
	}
}
