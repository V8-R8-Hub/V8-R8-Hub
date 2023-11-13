RegisterAFRAMEComponent('game',{
    schema: {
        guid : {type : "string", default: ""}

    },
    
    init : function() {
        this.game = GetGamesLoader().getGame(this.data.guid);
        
        console.log("HAHAH");

        this.gameElement = document.createElement("a-image");
        this.gameElement.setAttribute('class','Game');
        this.gameElement.setAttribute("width", "3");
        this.gameElement.setAttribute("height", "3");
        this.gameElement.setAttribute("src", `#${this.game.ThumbNailUrl}`);
        this.ShowDescriptionOnClick();
        this.el.appendChild(this.gameElement);
    },

    ShowDescriptionOnClick: function() {
        AddGameEventListener(this.gameElement,"mousedown",() => {
            let descriptionElement = this.CreateDescriptionElement();

            this.el.appendChild(descriptionElement); 
            DeactivateGameEventlisteners();
        });
   
    },

    CreateDescriptionElement : function () {
        let descriptionElement = document.createElement("a-Description");
        descriptionElement.setAttribute("guid",this.game.guid);

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
        guid : {type : "string", default: ""}

    },

    init : function() {
        this.game = GetGamesLoader().getGame(this.data.guid);
        
        this.RenderDescriptionElemment();
    },

    RenderDescriptionElemment: function () {
        let descriptionElement = document.createElement("a-rounded");
        this.descriptionElement = descriptionElement;
        this.descriptionElement.setAttribute('height', 5);
        descriptionElement.setAttribute('width', 5);
        descriptionElement.setAttribute('position', {x : -2.5, y : 2.5, z : 1    })

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
        textElement.setAttribute("color","grey");
        textElement.setAttribute('height', 3);
        textElement.setAttribute('width', 4);
        textElement.setAttribute('position',{x : 0, y : 2,  z : 0.05})
        let text = this.CreateText(this.game.description,"black");
        textElement.appendChild(text);
        return textElement;
    },

    CreateText: function (string, color) {
        let text = document.createElement('a-text')
        text.setAttribute("position", { x: 0.05, y: 0.4, z: 0.01 });
        text.setAttribute("scale", { x: 3, y: 3, z: 3 });
        text.setAttribute("value", string);
        text.setAttribute('color', color);

        return text;
    },

    CreatePlayButton: function() {
        let PlayButton = document.createElement('a-rounded');
        PlayButton.setAttribute('position',{x : 0, y : 0,  z : 0.05})
        PlayButton.setAttribute('height', 1);
        PlayButton.setAttribute('width', 5);
        PlayButton.setAttribute("color","grey");

        PlayButton.addEventListener('mousedown',()=> {
            switchScene("maler.html","");
        });

        PlayButton.setAttribute("position", { x: 0.05, y: 0.4, z: 0.01 });
        let buttonText = this.CreateText(`play ${this.game.name}`,"black");
        PlayButton.appendChild(buttonText);

        return PlayButton;
    },

    CreateExitButton : function () {
        let exitButton = document.createElement('a-entity');

        exitButton.setAttribute('height', 2);
        exitButton.setAttribute('width', 2);

        exitButton.setAttribute('position',{x: 4.5, y : 4.5, z : 0.05})
        let exitText =  this.CreateText("X","black");
        console.log("SUS");

        exitButton.addEventListener("mousedown", ()=> {
            this.descriptionElement.remove();
            ActivateGameEventlisteners();
        });

        exitButton.appendChild(exitText);
        return exitButton;
    },

    CreateDescriptionThumbnail: function() {
        let gameElement = document.createElement("a-image");
       
        gameElement.setAttribute("width", "3");
        gameElement.setAttribute("height", "3");
        gameElement.setAttribute("src", `#${this.game.ThumbNailUrl}`);

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
