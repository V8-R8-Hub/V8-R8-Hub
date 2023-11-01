using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using V8_R8_Hub.Models.Internal;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Pages.Admin {
    public class GameModel : PageModel {
        [BindProperty(SupportsGet = true)]
        public Guid Guid { get; set; }

        public GameBrief GameBrief { get; set; }
        public List<GameAssetBrief> GameAssets { get; set; }

        private readonly IGameService _gameService;
        private readonly IGameAssetService _gameAssetService;

        public GameModel(IGameService gameService, IGameAssetService gameAssetService) {
            _gameService = gameService;
            _gameAssetService = gameAssetService;
        }

        public async Task OnGetAsync(Guid guid) {
            Guid = guid;
            GameBrief = await _gameService.GetGame(guid);
            GameAssets = (await _gameAssetService.GetGameAssets(guid)).ToList();
        }
    }
}
