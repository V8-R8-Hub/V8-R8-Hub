using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Pages.Admin
{
	public class GameListModel : PageModel
	{
		public IEnumerable<GameBrief> GameBriefs { get; set; }
		private readonly IGameService _gameService;

		public GameListModel(IGameService gameService) {
			_gameService = gameService;
		}

		public async Task OnGetAsync(Guid guid) {
			GameBriefs = await _gameService.GetGames();
		}
	}
}
