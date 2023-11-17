class GameRepository {
    #games = {};

    constructor(gameList) {
        this.#games = gameList;
    }

    static async FromServer() {
        return new GameRepository(await GameRepository.FetchGameDictionary());
    }

    getGames() {
        return Object.values(this.#games);
    }

    static async FetchGameDictionary() {
        let gameList = await (await fetch("/api/Game")).json();

        let gamesDictionary = {};
        for (const game of gameList) {
            gamesDictionary[game.guid] = new Game(game);
        }

        return gamesDictionary;
    }

    getGame(guid) {
        return this.#games[guid];
    }
}

class Game {
    constructor(game) {
        this.name = game.name;
        this.guid = game.guid;
        this.ThumbNailUrl = game.thumbnailUrl;
        this.description = game.description;

        this.htmlUrl = game.gameBlobUrl;
    }

    GetDescription() {
        return this.description;
    }

    async GetHtmlFile() {
        let htmlFile = await fetch(this.htmlUrl);
        return htmlFile;
    }
}

let gameRepository;

function GetGamesLoader() {
    if (gameRepository == null) {
        throw new Error("Tried to use game repository before it has been setup")
    }
    return gameRepository;
}

async function SetUpGameLoader() {
    gameRepository = await GameRepository.FromServer();
}
