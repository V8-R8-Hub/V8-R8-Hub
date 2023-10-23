using Microsoft.AspNetCore.Mvc;
using V8_R8_Hub.Models.Response;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Controllers {
	[Route("api/[Controller]")]
	[ApiController]
	public class GameController : ControllerBase {
		private readonly IGameAssetService _gameAssetService;
		private readonly IGameService _gameService;

		public GameController(IGameAssetService gameAssetService, IGameService gameService) {
			_gameAssetService = gameAssetService;
			_gameService = gameService;
		}

		[HttpGet("benis", Name = "HelloWorld")]
		public IActionResult HelloWorld() {
			return Ok("Hello World!");
		}

		[HttpPost("Game/{uuid:guid}", Name = "CreateGame")]
		public IActionResult CreateGame(Guid uuid, IFormFile gameFile) {
			return Ok("Hello World!");
		}

		[HttpGet("Game/{guid:guid}/{path}", Name = "GetGameAsset")]
		[ProducesResponseType(typeof(FileContentResult), 200)]
		[ProducesResponseType(404)]
		public async Task<IActionResult> GetGameAsset(Guid guid, string path) {
			var asset = await _gameAssetService.GetGameAsset(guid, path);
			if (asset == null) {
				return NotFound();
			}
			return File(asset.ContentBlob, asset.MimeType, asset.FileName);
		}

		[HttpGet("Game", Name = "GetGames")]
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
	}
}
