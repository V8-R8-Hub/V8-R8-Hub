RegisterAFRAMEComponent('gamegrid', {
    schema: {
        games: { type: "array", default: [{ x: 2, y: 4 }] },
        id: { type: "string", default: 'menu' }
    },

    init: function () {
        console.log("HUH?");
        let getGames = async () => {
            await SetUpGameLoader();
            this.games = GetGamesLoader();
            console.log(this.games);
            this.gamesPerPage = 9;
            this.gamesPerRow = 3;
            this.games_on_page = [];
            this.page_number = 0;
            this.number_of_pages = Math.ceil(this.data.games.length / this.gamesPerPage);

            this.menu = document.createElement('a-menu');
            this.CreateAssets();
            this.CreateGamePagesArray();
            this.CreateStuff();
        }

        getGames();
    },

    CreateAssets: function () {
        let imageAssets = document.createElement("a-assets");
        let gameList = this.games.getGames();
        for (const game of gameList) {
            let game_img = document.createElement("img");
            game_img.setAttribute("id", game.guid + "-thumb");
            game_img.setAttribute("src", game.ThumbNailUrl);

            imageAssets.appendChild(game_img);
        }
        this.el.sceneEl.appendChild(imageAssets);
    },

    CreateStuff: function () {
        let poscontainer = document.createElement("a-entity");
        poscontainer.setAttribute("id", this.data.id);
        poscontainer.setAttribute("position", { x: -4, y: 2, z: 0 });
        let container = this.CreateFullcontainer();

        poscontainer.appendChild(container);
        this.el.appendChild(poscontainer);
    },

    CreateFullcontainer: function () {
        let container = document.createElement("a-entity");

        let leftButton = this.CreateButton("left", () => {
            if (this.page_number != 0) {
                this.page_number -= 1;
                this.CreateStuff();
            }
        });

        leftButton.setAttribute("position", { x: -3, y: 4, z: 0 });
        let gameContainer = this.CreateGameContainer();

        let rightButton = this.CreateButton("right", () => {
            if (this.page_number + 1 < this.number_of_pages) {
                this.page_number += 1;
                this.CreateStuff();
            }
        });

        rightButton.setAttribute("position", { x: 11, y: 4, z: 0 });

        container.appendChild(leftButton);
        container.appendChild(gameContainer);

        container.appendChild(rightButton);
        return container;

    },

    CreateButton: function (label, onclick) {
        let button = document.createElement("a-xybutton");
        button.setAttribute("label", label);
        button.addEventListener('mousedown', onclick);
        return button;
    },

    GoToMenu: function () {
        ClearMenu();
        let menu = document.createElement("a-menu");

        this.Ascene.appendChild(menu);
    },

    CreateGameContainer: function () {
        let gameContainer = document.createElement("a-entity");

        let backButton = this.CreateButton("back", () => {
            this.GoToMenu();
        });
        backButton.setAttribute("position", { x: 4, y: 10, z: 0 })

        gameContainer.appendChild(backButton);

        return this.CreatePage(gameContainer);
    },

    CreateGamePagesArray: function () {
        let game_list = this.games.getGames();
        for (let i = 0; i < this.number_of_pages; i++) {
            let start_idx = i * this.gamesPerPage;

            let games = game_list.slice(start_idx, start_idx + this.gamesPerPage);
            this.games_on_page.push(games);
        }
    },

    CreatePage: function (gameContainer) {
        let games_on_page = this.games_on_page[this.page_number];

        for (let i = 0; i < games_on_page.length; i += this.gamesPerRow) {
            let games_on_row = games_on_page.slice(i, i + this.gamesPerRow);
            let y_coordinates = (i * 4) / this.gamesPerRow;
            let row_element = document.createElement("a-entity");
            console.log(y_coordinates);
            row_element.setAttribute("position", { x: 0, y: y_coordinates, z: 0 })

            for (const [idx, game] of games_on_row.entries()) {
                let x_coordinates = idx * 4;

                this.RenderGameElement(row_element, game, x_coordinates);
            }

            gameContainer.appendChild(row_element);
        }

        return gameContainer;
    },

    RenderGameElement: function (row, game, x_coordinates) {
        let gameElement = document.createElement("a-game");
        gameElement.setAttribute("position", { x: x_coordinates, y: 0, z: 0 })
        console.log("HUHU")
        gameElement.setAttribute("guid", game.guid);

        row.appendChild(gameElement);
    },
})

RegisterAFRAMEPrimitive('a-gamegrid', {
    defaultComponents: {
        gamegrid: {}
    },
    mappings: {
        games: "gameGrid.games",
    }
})
