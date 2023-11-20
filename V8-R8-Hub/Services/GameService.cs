using Dapper;
using Npgsql;
using System.Data;
using System.Text.RegularExpressions;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Models.Response;
using V8_R8_Hub.Repositories;

namespace V8_R8_Hub.Services {
	public interface IGameService {
		Task AddGameTag(Guid gameGuid, string tag);
		Task<ObjectIdentifier> CreateGame(string name, string description, VirtualFile gameFile, VirtualFile thumbnailFile);
		Task<ISet<string>> GetAllowedGameMimeTypes();
		Task<ISet<string>> GetAllowedThumbnailMimeTypes();
		Task<GameBrief> GetGame(Guid guid);
		Task<IEnumerable<GameBrief>> GetGames();
		Task RemoveTag(Guid gameGuid, string tag);
	}

	public class GameService : IGameService {
		private readonly IFileService _safeFileService;
		private readonly IGameRepository _gameRepository;
		private readonly IUnitOfWorkContext _uow;

		public GameService(
			IFileService safeFileService,
			IUnitOfWorkContext uow,
			IGameRepository gameRepository
		) {
			_safeFileService = safeFileService;
			_uow = uow;
			_gameRepository = gameRepository;
		}

		public async Task<IEnumerable<GameBrief>> GetGames() {
			return (await _gameRepository.GetGames()).Select(MapGameBriefQueryModel);
		}

		public async Task<GameBrief> GetGame(Guid guid) {
			return MapGameBriefQueryModel(await _gameRepository.GetGame(guid));
		}

		private static GameBrief MapGameBriefQueryModel(GameBriefQueryModel model) => new GameBrief {
			Guid = model.Guid,
			Name = model.Name,
			Description = model.Description,
			ThumbnailGuid = model.ThumbnailGuid,
			GameBlobGuid = model.GameBlobGuid,
			Tags = model.CommaSeperatedTags?.Split(',') ?? Enumerable.Empty<string>()
		};

		public async Task<ObjectIdentifier> CreateGame(string name, string description, VirtualFile gameFile, VirtualFile thumbnailFile) {
			await _uow.Begin();
			var gameFileId = await _safeFileService.CreateFileFrom(gameFile, await GetAllowedGameMimeTypes());
			var thumbnailFileId = await _safeFileService.CreateFileFrom(thumbnailFile, await GetAllowedThumbnailMimeTypes());
			var gameId = await _gameRepository.CreateGame(new CreateGameModel() {
				Name = name,
				Description = description,
				GameFileId = gameFileId.Id,
				ThumbnailFileId = thumbnailFileId.Id
			});
			await _uow.Commit();
			return gameId;
		}

		public Task<ISet<string>> GetAllowedGameMimeTypes()
		{
			return Task.FromResult<ISet<string>>(new HashSet<string>() {
				"text/html"
			});
		}

		public Task<ISet<string>> GetAllowedThumbnailMimeTypes()
		{
			return Task.FromResult<ISet<string>>(new HashSet<string>() {
				"image/png",
				"image/jpeg",
				"image/gif",
				"image/tiff",

			});
		}

		public async Task AddGameTag(Guid gameGuid, string tag) {
			var sanitizedTag = SanitizeTag(tag);
			if (!IsTagAllowed(sanitizedTag)) {
				throw new IllegalTagException(sanitizedTag, "Tag contains illegal characters");
			}
			await _uow.Begin();
			var tagId = await _gameRepository.CreateOrGetTagId(sanitizedTag);
			await _gameRepository.AddGameTag(gameGuid, tagId);
			await _uow.Commit();
		}

		public async Task RemoveTag(Guid gameGuid, string tag) {
			var sanitizedTag = SanitizeTag(tag);

			await _uow.Begin();
			await _gameRepository.RemoveGameTag(gameGuid, sanitizedTag);
			await _uow.Commit();
		}

		private static bool IsTagAllowed(string tag) {
			return new Regex("^([A-Za-z ])+$").IsMatch(tag);
		}

		private static string SanitizeTag(string tag) {
			return tag.Trim().ToLower();
		}
	}
}
