using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Repositories;

namespace V8_R8_Hub.Services {
	public interface IGameAssetService {
		Task<IEnumerable<GameAssetBrief>> AddGameAssets(Guid gameId, IEnumerable<VirtualFile> assetFiles);
		Task DeleteGameAsset(Guid gameId, string filePath);
		Task<ISet<string>> GetAllowedGameAssetMimeTypes();
		Task<FileData?> GetGameAsset(Guid gameGuid, string path);
		Task<IEnumerable<GameAssetBrief>> GetGameAssets(Guid gameGuid);
	}

	public class GameAssetService : IGameAssetService {
		private readonly IFileService _safeFileService;
		private readonly IFileRepository _publicFileService;
		private readonly IGameAssetRepository _gameAssetRepository;
		private readonly IUnitOfWorkContext _uow;
		private readonly IGameRepository _gameRepository;
		public GameAssetService(IFileService safeFileService, IGameAssetRepository gameAssetService, IUnitOfWorkContext uow, IGameRepository gameRepository, IFileRepository publicFileService) {
			_safeFileService = safeFileService;
			_gameAssetRepository = gameAssetService;
			_uow = uow;
			_gameRepository = gameRepository;
			_publicFileService = publicFileService;
		}

		public async Task<IEnumerable<GameAssetBrief>> AddGameAssets(Guid gameGuid, IEnumerable<VirtualFile> assetFiles) {
			await _uow.Begin();

			int gameId = await _gameRepository.GetGameId(gameGuid) 
				?? throw new UnknownGameException(gameGuid, "Could not find game corresponding with the given guid");

			var gameAssetBriefs = new List<GameAssetBrief>();
			foreach (var assetFile in assetFiles) {
				var assetFileId = await _safeFileService.CreateFileFrom(assetFile, await GetAllowedGameAssetMimeTypes());
				gameAssetBriefs.Add(await _gameAssetRepository.AddGameAsset(gameId, assetFileId.Id, assetFile.FileName));
			}

			await _uow.Commit();
			return gameAssetBriefs;
		}

		public async Task DeleteGameAsset(Guid gameId, string filePath) {
			await _uow.Begin();
			using var transaction = _gameAssetRepository.DeleteGameAsset(gameId, filePath);
			await _uow.Commit();
		}

		public async Task<FileData?> GetGameAsset(Guid gameGuid, string path) {
			await _uow.Begin();
			var id = await _gameAssetRepository.GetGameAssetFileId(gameGuid, path);
			if (id == null)
				return null;
			return await _publicFileService.GetFile(id.Value);
		}

		public async Task<IEnumerable<GameAssetBrief>> GetGameAssets(Guid gameGuid) {
			return await _gameAssetRepository.GetGameAssets(gameGuid);
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
