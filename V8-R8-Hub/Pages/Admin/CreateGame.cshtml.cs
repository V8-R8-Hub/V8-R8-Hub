using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Pages.Admin
{
	public class CreateGameModel : PageModel
	{
		private readonly IGameService _gameService;
		public string AllowedGameMimeTypesString { get; set; }
		public string AllowedThumbnailMimeTypesString { get; set; }

		public CreateGameModel(IGameService gameService) {
			_gameService = gameService;
		}

		public async Task OnGetAsync() {
			await Task.WhenAll(
				Task.Run(async () => AllowedGameMimeTypesString = string.Join(", ", await _gameService.GetAllowedGameMimeTypes())),
				Task.Run(async () => AllowedThumbnailMimeTypesString = string.Join(", ", await _gameService.GetAllowedThumbnailMimeTypes()))
			);
		}
	}
}
