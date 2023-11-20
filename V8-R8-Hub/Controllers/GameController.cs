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
		private readonly ILogger<GameController> _logger;

		public GameController(IGameAssetService gameAssetService, IGameService gameService, ILogger<GameController> gameController) {
			_gameAssetService = gameAssetService;
			_gameService = gameService;
			_logger = gameController;
		}

		/// <summary>
		/// Creates a game with the given parameters
		/// </summary>
		/// <response code="200">Game is created and the new game guid is returned</response>
		/// <response code="415">Game or thumbnail has unsupported mime type</response>
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

		/// <summary>
		/// Get a list of all the games available in the hub
		/// </summary>
		/// <response code="200">Success</response>
		[HttpGet("", Name = "GetGames")]
		[ProducesResponseType(typeof(IEnumerable<GameBriefResponse>), 200)]
		public async Task<IActionResult> GetGames() {
			var games = await _gameService.GetGames();
			return Ok(games.Select(gameBrief => new GameBriefResponse {
				Guid = gameBrief.Guid,
				Name = gameBrief.Name,
				Description = gameBrief.Description,
				ThumbnailUrl = Url.Action("GetFile", "PublicFile", new { fileGuid = gameBrief.ThumbnailGuid }),
				GameBlobUrl = Url.Action("GetFile", "PublicFile", new { fileGuid = gameBrief.GameBlobGuid }),
				Tags = gameBrief.Tags.ToList()
			}));
		}

		/// <summary>
		/// Gets a game asset for a sepcific game
		/// </summary>
		/// <response code="200">Success and the asset blob is the response</response>
		/// <response code="404">Either the game guid or the asset path does not correspond with an asset</response>
		[HttpGet("{guid:guid}/assets/{path}", Name = "GetGameAsset")]
		[ProducesResponseType(typeof(FileContentResult), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetGameAsset(Guid guid, string path) {
			var asset = await _gameAssetService.GetGameAsset(guid, path);
			if (asset == null) {
				return NotFound();
			}
			return File(asset.ContentBlob, asset.MimeType, asset.FileName);
		}

		/// <summary>
		/// Creates game assets for a specific game
		/// </summary>
		/// <response code="200">Success</response>
		/// <response code="400">One of the assets has an unsupported mime type or the name already exists</response>
		/// <response code="404">A game with the given GUID does not exist</response>
		[HttpPost("{guid:guid}/assets", Name = "CreateGameAssets")]
		[ProducesResponseType(typeof(GameAssetBrief), 200)]
		[ProducesResponseType(typeof(string), 400)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> CreateGameAssets(Guid guid, IFormFile[] assetFiles) {
			try {
				return Ok(await _gameAssetService.AddGameAssets(guid, assetFiles.Select(VirtualFile.From)));
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

		/// <summary>
		/// Deletes a game asset for a specific game
		/// </summary>
		/// <response code="200">Success</response>
		/// <response code="404">Either the game guid or the asset path does not correspond with an asset</response>
		[HttpDelete("{guid:guid}/assets/{path}", Name = "DeleteGameAsset")]
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

		/// <summary>
		/// Adds a tag to a game
		/// </summary>
		/// <returns status="200">The tag was added successfully</returns>
		/// <returns status="404">The specified game does not exist</returns>
		/// <returns status="400">The tag contains illegal characters</returns>
		/// <returns status="409">The tag has already been added to the game</returns>
		[HttpPost("{guid:guid}/tags")]
		[ProducesResponseType(200)]
		[ProducesResponseType(typeof(string), 404)]
		[ProducesResponseType(typeof(string), 400)]
		[ProducesResponseType(typeof(string), 409)]
		public async Task<IActionResult> AddGameTag(Guid guid, [FromBody] string tag) {
			try {
				await _gameService.AddGameTag(guid, tag);
			} catch (UnknownGameException ex) {
				_logger.LogWarning("User tried to add tag to unknown game");
				_logger.LogWarning("Details: {Message}", ex.Message);
				return NotFound("The given guid does not correspond with any game");
			} catch (IllegalTagException ex) {
				_logger.LogWarning("User tried to add tag which is not valid " + ex.GivenTag);
				_logger.LogWarning("Details: {Message}", ex.Message);
				return BadRequest("The given tag is not valid");
			} catch (DuplicateTagException ex) {
				_logger.LogWarning("User tried to add a duplicate tag " + ex.GivenTag);
				_logger.LogWarning("Details: {Message}", ex.Message);
				return Conflict("The given tag is already on the game");
			}
			return Ok();
		}

		/// <summary>
		/// Removes a tag from a specific game
		/// </summary>
		/// <returns status="200">The tag was either removed or did not exist in the first place</returns>
		[HttpDelete("{guid:guid}/tags/{tag}")]
		[ProducesResponseType(200)]
		public async Task<IActionResult> RemoveTag(Guid guid, string tag) {
			await _gameService.RemoveTag(guid, tag);
			return Ok();
		} 
	}
}
