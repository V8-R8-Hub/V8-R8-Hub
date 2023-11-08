

class Games {
    constructor() {
        this.Games = [];

    }
    LoadGames() {
        let gamesList = GetGamesList();


    }
    async GetGamesList() {
        let Gamelist = await fetch("https://localhost:8080");
        for (const game of Gamelist) {
            this.Games.push(new Game(game));
        }
    }
}


class Game {
    constructor(game) {
        this.name = game.name;
        this.Guid = game.Guid;
        this.thumbnail = game.thumbnail;
        this.description = game.description;
        this.gameBlobUrl = game.gameBlobUrl;

    }
}