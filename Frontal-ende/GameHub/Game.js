import { GetGamesLoader } from "../GameLoader.js";




AFRAME.registerComponent('game',{
    schema: {
        guid : {type : "string", default: ""}

    },
    init : function() {

        this.game = GetGamesLoader().getGame(this.data.guid);
        
        console.log("HAHAH");

        this.gameElement = document.createElement("a-image");
       
        this.gameElement.setAttribute("width", "3");
        this.gameElement.setAttribute("height", "3");
        this.gameElement.setAttribute("src", `#${this.game.ThumbNailUrl}`);

        this.el.appendChild(gameElement);

    },
    ShowDescriptionOnClick: function() {
        this.gameElement.addEventListener("mousedown",() => {
            let descriptionElement = this.CreateDescriptionElement();
            this.el.sceneEl.appendChild(descriptionElement); 
        });

    },
    CreateDescriptionElement : function () {
        let descriptionElement = document.createElement("a-Description");
        descriptionElement.setAttribute("guid",this.game.guid);

        return descriptionElement;
    } 
    





});


AFRAME.registerPrimitive('a-Description', {
    defaultComponents: {
        game: {}
    },
    mappings: {
        guid: "game.guid",
    },
 


})



AFRAME.registerComponent('description', {
    schema: {
        guid : {type : "string", default: ""}

    },
    init : function() {
        this.game = GetGamesLoader().getGame(this.data.guid);
        
    },
    RenderDescriptionElemment: function () {
        let descriptionElement = document.createElement("a-arounded");
        descriptionElement.setAttribute('height', 5);
        descriptionElement.setAttribute('width', 5);


        let playButton = this.CreatePlayButton();
        let textElement = this.CreateDescriptionTextElement();

        descriptionElement.appendChild(playButton)

        descriptionElement.appendChild(textElement)




    } ,
    CreateDescriptionTextElement() {
        let textElement = document.createElement("a-arounded");
        textElement.setAttribute("color","grey");
        textElement.setAttribute('height', 0.5);
        textElement.setAttribute('width', 0.5);
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
        button.addEventListener('mousedown',()=> {
            //luscus stuff??
        });
        button.setAttribute("position", { x: 0.05, y: 0.4, z: 0.01 });
        let buttonText = this.CreateText(`play ${this.game.name}`,"white");
        PlayButton.appendChild(buttonText);

        return PlayButton;


    },

    CreateExitButton : function () {
        let exitButton = document.createElement('a-entity');
        let exitText =  this.CreateText("X","black");
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



AFRAME.registerPrimitive('a-Description', {
    defaultComponents: {
        description: {}
    },
    mappings: {
        guid: "description.guid",
    },
 
})
