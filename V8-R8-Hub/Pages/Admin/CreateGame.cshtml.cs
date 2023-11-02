using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Pages.Admin
{
	public class CreateGameModel : PageModel
	{
		private readonly IGameService _gameService;

		public CreateGameModel(IGameService gameService) {
			_gameService = gameService;
		}

		public async Task OnGetAsync() {
			ViewData["AllowedGameMimeTypesString"] = string.Join(", ", await _gameService.GetAllowedGameMimeTypes());
			ViewData["AllowedThumbnailMimeTypesString"] = string.Join(", ", await _gameService.GetAllowedThumbnailMimeTypes());
		}
	}
}
