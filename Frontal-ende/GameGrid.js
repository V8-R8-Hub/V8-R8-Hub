import { ClearMenu } from "./UtilityFunctions.js";
AFRAME.registerComponent('gameGrid', {
    schema: {
        games: { type: "array", default: [] },
        id: { type: "string", default: 'menu' }
    },

    init: function () {

        this.gamesPerPage = 9;
        this.gamesPerRow = 3;
        this.games_on_page = [];
        this.page_number = 0;

        this.number_of_pages = Math.ceil(this.data.games.length / this.gamesPerPage);
        this.Ascene = document.querySelector("a-scene");

        this.CreateGamePagesArray();
        this.CreateStuff();

    },

    CreateStuff: function () {

        let poscontainer = document.createElement("a-entity");
        poscontainer.setAttribute("id", this.data.id);
        poscontainer.setAttribute("position", "0 6 0");
        let container = this.CreateFullcontainer();

        poscontainer.appendChild(container);
        this.el.appendChild(poscontainer);
    },




    CreateFullcontainer: function () {
        let container = document.createElement("a-xycontainer");
        container.setAttribute("direction", "row");

        let leftButton = this.CreateButton("left", () => {
            if (this.page_number != 0) {
                this.page_number -= 1;
                this.CreateStuff();

            }
        });
        let gameContainer = this.CreateGameContainer();

        let rightButton = this.CreateButton("right", () => {
            if (this.page_number + 1 < this.number_of_pages) {
                this.page_number += 1;
                this.CreateStuff();

            }
        });
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
        let gameContainer = document.createElement("a-xycontainer");
        gameContainer.setAttribute("direction", "column");
        let backButton = this.CreateButton("back", () => {

            this.GoToMenu();

        });

        gameContainer.appendChild(backButton);

        return this.CreatePage(gameContainer);


    },
    CreateGamePagesArray: function () {
        for (let i = 0; i < this.number_of_pages; i++) {
            let start_idx = i * this.gamesPerPage;

            let games = this.data.games.slice(start_idx, start_idx + this.gamesPerPage);
            this.games_on_page.push(games);

        }
    },
    CreatePage: function (gameContainer) {
        let games_on_page = this.games_on_page[this.page_number];

        console.log(this.games_on_page);
        for (let i = 0; i < games_on_page.length; i += this.gamesPerRow) {

            let games_on_row = games_on_page.slice(i, i + this.gamesPerRow);

            let row_element = document.createElement("a-xycontainer");
            row_element.setAttribute("direction", "row");
            row_element.setAttribute("spacing", "0.2");
            row_element.setAttribute("padding", "0.2");

            for (const game of games_on_row) {
                this.RenderGameElement(row_element, game);
            }

            gameContainer.appendChild(row_element);

        }
        return gameContainer;
    },

    RenderGameElement: function (row, game) {
        let gameElement = document.createElement("a-image");

        gameElement.setAttribute("width", "3");
        gameElement.setAttribute("height", "3");
        gameElement.setAttribute("src", `#${game}`);
        row.appendChild(gameElement);

    },

})

//skyd mig :)
// class Games {
//     constructor(game_list) {
//         let
//     }
// }

// function GetGamesList() {

// }





AFRAME.registerPrimitive('a-GameGrid', {
    defaultComponents: {
        gameGrid: {}
    },
    mappings: {
        games: "gameGrid.games",
    }


})