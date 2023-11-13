class GamesLoader
{
    constructor() {
        this.Games = {};
    }

    async LoadGames() {
        await this.FetchGamesList();
    }

    getGames() {
        return Object.values(this.Games);
    }

    async FetchGamesList() {
        let response = await fetch("https://localhost:7171/api/Game",{
            headers: {
                'Access-Control-Allow-Origin': 'Origin'
            }
        });

        let Gamelist =  await response.json();
        for (const game of Gamelist) {
            this.Games[game.guid] = new Game(game);
        }
    }

    getGame(guid) {
        return this.Games[guid];
    }
}

class Game {
    constructor(game) {
        this.name = game.name;
        this.guid = game.guid;
        this.ThumbNailUrl = "https://localhost:7171" + game.thumbnailUrl; 
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

const Games = new GamesLoader();

function GetGamesLoader () {
    return Games;
}

async function SetUpGameLoader () {
    await Games.FetchGamesList();
}
