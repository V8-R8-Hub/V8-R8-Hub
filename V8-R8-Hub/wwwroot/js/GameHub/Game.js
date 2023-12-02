let EventHandler = new ToggleAbleEventHandler();
RegisterAFRAMEComponent('game', {
    schema: {
        guid: { type: "string", default: "" }

    },

    init: function () {
        this.game = GetGamesLoader().getGame(this.data.guid);

        this.gameElement = document.createElement("a-image");
        this.gameElement.setAttribute("id", this.game.guid);
        this.gameElement.setAttribute('class', 'Game');
        this.gameElement.setAttribute("width", "3");
        this.gameElement.setAttribute("height", "3");

        this.gameElement.setAttribute("src", `#${this.game.guid}-thumb`);
        this.ShowDescriptionOnClick();
        this.el.appendChild(this.gameElement);
    },

    ShowDescriptionOnClick: function () {

        EventHandler.AddEventListener(this.gameElement, "mousedown", () => {
            let descriptionElement = this.CreateDescriptionElement(this.game.guid);

            this.el.appendChild(descriptionElement);
            EventHandler.DeactivateEventlisteners();
        });
    },

    CreateDescriptionElement: function (guid) {
        let descriptionElement = document.createElement("a-Description");
        descriptionElement.setAttribute("guid", guid);

        return descriptionElement;
    }
});

RegisterAFRAMEPrimitive('a-game', {
    defaultComponents: {
        game: {}
    },

    mappings: {
        guid: "game.guid",
    },
})

RegisterAFRAMEComponent('description', {
    schema: {
        guid: { type: "string", default: "" }
    },

    init: function () {
        this.game = GetGamesLoader().getGame(this.data.guid);
        console.log(this.data.guid);
        "/a2d966b3-8600-4ecd-9bb9-cedeed54920a";
        this.RenderDescriptionElemment();
    },

    RenderDescriptionElemment: function () {
        let descriptionElement = document.createElement("a-rounded");
        this.descriptionElement = descriptionElement;
        this.descriptionElement.setAttribute('height', 5);
        descriptionElement.setAttribute('width', 5);
        descriptionElement.setAttribute('position', { x: -2.5, y: 2.5, z: 1 })

        let playButton = this.CreatePlayButton();
        let textElement = this.CreateDescriptionTextElement();
        let exitButton = this.CreateExitButton();
        this.descriptionElement.appendChild(playButton)
        this.descriptionElement.appendChild(exitButton)

        this.descriptionElement.appendChild(textElement)
        document.querySelector("#menu").appendChild(this.descriptionElement);
    },

    CreateDescriptionTextElement() {
        let textElement = document.createElement("a-rounded");
        textElement.setAttribute("color", "grey");
        textElement.setAttribute('height', 3);
        textElement.setAttribute('width', 4);

        textElement.setAttribute('position', { x: 0, y: 2, z: 0.05 })
        let game = this.game.description;
        console.log(game);
        let text = CreateText(textElement, this.game.description, "black", "left", "top");
        textElement.appendChild(text);
        return textElement;
    },



    CreatePlayButton: function () {
        let PlayButton = document.createElement('a-rounded');
        PlayButton.setAttribute('position', { x: 0, y: 0, z: 0.05 })
        PlayButton.setAttribute('height', 1);
        PlayButton.setAttribute('width', 5);
        PlayButton.setAttribute("color", "grey");

        PlayButton.addEventListener('mousedown', () => {
            EventHandler.ActivateEventlisteners();
            switchScene(this.game.htmlUrl, this.game.guid, "");
        });

        PlayButton.setAttribute("position", { x: 0.05, y: 0.4, z: 0.01 });
        let buttonText = CreateText(PlayButton, `play ${this.game.name}`, "black", "left", "center");
        PlayButton.appendChild(buttonText);

        return PlayButton;
    },

    CreateExitButton: function () {

        let exitButton = document.createElement('a-rounded');

        exitButton.setAttribute('height', 0.5);
        exitButton.setAttribute('width', 0.5);
        exitButton.setAttribute('color', 'black');
        exitButton.setAttribute('position', { x: 4.5, y: 4.5, z: 0.01 })
        exitButton.setAttribute('radius', 0.08);
        let exitText = CreateText(exitButton, "X", "white", "center", "center");


        exitButton.addEventListener("mousedown", () => {
            this.descriptionElement.remove();
            EventHandler.ActivateEventlisteners();
        });

        exitButton.appendChild(exitText);
        return exitButton;
    },

    CreateDescriptionThumbnail: function () {
        let gameElement = document.createElement("a-image");

        gameElement.setAttribute("width", "3");
        gameElement.setAttribute("height", "3");
        gameElement.setAttribute("src", `#${this.game.guid}-thumb`);

        return gameElement;
    },
})

RegisterAFRAMEPrimitive('a-description', {
    defaultComponents: {
        description: {}
    },
    mappings: {
        guid: "description.guid",
    },
})

RegisterAFRAMEComponent('gamegrid', {
    schema: {
        games: { type: "array", default: [{ x: 2, y: 4 }] },
        id: { type: "string", default: 'menu' }
    },

    init: function () {
        let getGames = async () => {
            await SetUpGameLoader();
            this.games = GetGamesLoader();

            this.gamesPerPage = 9;
            this.gamesPerRow = 3;
            this.games_on_page = [];
            this.page_number = 0;
            this.number_of_pages = Math.ceil(this.games.getGames().length / this.gamesPerPage);
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
                ClearMenu();

                this.CreateStuff();
            }
        });

        leftButton.setAttribute("position", { x: -3, y: 4, z: 0 });
        let gameContainer = this.CreateGameContainer();

        let rightButton = this.CreateButton("right", () => {
            if (this.page_number + 1 < this.number_of_pages) {

                this.page_number += 1;
                ClearMenu();

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
        EventHandler.AddEventListener(button, 'mousedown', onclick);
        return button;
    },

    GoToMenu: function () {
        ClearMenu();
        let menu = document.createElement("a-menu");
        this.el.sceneEl.appendChild(menu);
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

            let y_coordinates = 8 - ((i * 4) / this.gamesPerRow);
            let row_element = document.createElement("a-entity");

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
