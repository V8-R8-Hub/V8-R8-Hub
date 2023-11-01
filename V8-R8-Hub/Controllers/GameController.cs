using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Headers;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Models.Request;
using V8_R8_Hub.Models.Response;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Controllers {
	[Route("api/[Controller]")]
	[ApiController]
	public class GameController : ControllerBase {
		private readonly IGameAssetService _gameAssetService;
		private readonly IGameService _gameService;
		private readonly IPublicFileService _publicFileService;
		private readonly ISafeFileService _safeFileService;
		private readonly ILogger<GameController> _logger;

		public GameController(IGameAssetService gameAssetService, IGameService gameService, IPublicFileService publicFileService, ISafeFileService safeFileService, ILogger<GameController> gameController) {
			_gameAssetService = gameAssetService;
			_gameService = gameService;
			_publicFileService = publicFileService;
			_safeFileService = safeFileService;
			_logger = gameController;
		}

		[HttpGet("benis", Name = "HelloWorld")]
		public IActionResult HelloWorld() {
			return Ok("Hello World!");
		}

		[HttpPost("", Name = "CreateGame")]
		[ProducesResponseType(415)]
		[ProducesResponseType(typeof(Guid), 200)]
		public async Task<IActionResult> CreateGame([FromForm] CreateGameRequestModel requestModel) {
			try {
				var gameId = await _gameService.CreateGame(requestModel.Name, requestModel.Description, VirtualFile.From(requestModel.GameFile), VirtualFile.From(requestModel.ThumbnailFile));
				return Ok(gameId.Guid);
			} catch (DisallowedMimeTypeException ex) {
				_logger.LogWarning("User tried to upload game with unsupported mime type");
				_logger.LogWarning("Details: {Message}", ex.Message);
				return StatusCode(415);
			}
		}

		[HttpGet("", Name = "GetGames")]
		[ProducesResponseType(typeof(IEnumerable<GameBriefResponse>), 200)]
		public async Task<IActionResult> GetGames() {
			var games = await _gameService.GetGames();
			return Ok(games.Select(gameBrief => new GameBriefResponse {
				Guid = gameBrief.Guid,
				Name = gameBrief.Name,
				Description = gameBrief.Description,
				ThumbnailUrl = Url.Action("GetFile", "PublicFile", new { fileGuid = gameBrief.ThumbnailGuid }),
				GameBlobUrl = Url.Action("GetFile", "PublicFile", new { fileGuid = gameBrief.GameBlobGuid })
			}));
		}

		[HttpGet("{guid:guid}/{path}", Name = "GetGameAsset")]
		[ProducesResponseType(typeof(FileContentResult), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetGameAsset(Guid guid, string path) {
			var asset = await _gameAssetService.GetGameAsset(guid, path);
			if (asset == null) {
				return NotFound();
			}
			return File(asset.ContentBlob, asset.MimeType, asset.FileName);
		}

		[HttpPost("{guid:guid}", Name = "CreateGameAsset")]
		[ProducesResponseType(typeof(GameAssetBrief), 200)]
		[ProducesResponseType(typeof(string), 400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> CreateGameAsset(Guid guid, IFormFile assetFile) {
			try {
				return Ok(await _gameAssetService.AddGameAsset(guid, VirtualFile.From(assetFile)));
			} catch (DisallowedMimeTypeException ex) {
				_logger.LogWarning("User tried to upload game with unsupported mime type");
				_logger.LogWarning("Details: {Message}", ex.Message);
				return BadRequest($"Mime type of '" + ex.GivenMimeType + "' is not allowed");
			} catch (DuplicateAssetException ex) {
				_logger.LogWarning("User tried to upload game asset with duplicate file name");
				_logger.LogWarning("Details: {Message}", ex.Message);
				return BadRequest("An asset with that filename already exists");
			} catch (UnknownGameException ex) {
				_logger.LogWarning("User tried to upload game asset for unknown game");
				_logger.LogWarning("Details: {Message}", ex.Message);
				return NotFound("The given guid does not correspond with any game");
			}
		}

		[HttpDelete("{guid:guid}/{path}", Name = "DeleteGameAsset")]
		[ProducesResponseType(typeof(GameAssetBrief), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> DeleteGameAsset(Guid guid, string path) {
			try {
				await _gameAssetService.DeleteGameAsset(guid, path);
				return Ok();
			} catch (UnknownGameException ex) {
				_logger.LogWarning("User tried to delete game asset for unknown game");
				_logger.LogWarning("Details: {Message}", ex.Message);
				return NotFound("The given guid does not correspond with any game");
			}
		}
	}
}
