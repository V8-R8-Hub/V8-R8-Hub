function SetUpMenu() {
    let menu = document.querySelector("a-menu");
    let scene = document.querySelector("a-gamescene");

    menu.remove();
    scene.remove();
    let template = document.querySelector("#overallTemplate");

    template.appendChild(document.createElement("a-menu"));

    template.appendChild(document.createElement("a-gamescene"));
}

RegisterAFRAMEComponent('menu', {
    schema: {
        id: { type: "string", default: "menu" }
    },

    init: function () {
        this.menu = document.createElement('a-rounded');
        this.el.appendChild(this.menu);
        document.getElementById('cameraRig').setAttribute('position', '0 1 7');

        this.menu.setAttribute('id', this.data.id);
        this.menu.setAttribute("position", { x: -1.5, y: 0.5, z: -3 });
        this.menu.setAttribute('width', 3);
        this.menu.setAttribute('height', 4);
        this.menuItemArr = ['Menu', 'Games', 'Options', 'Quit'];
        this.AddMenuItemsToMenu(this.menu, this.menuItemArr, 2.5);
        this.AddEvents()
    },

    AddMenuItemsToMenu: function (menu, itemarr, supMenuItemWidth) {
        let y = 3;
        for (let i = 0; i <= itemarr.length - 1; i++) {
            let menuitem = document.createElement('a-rounded')
            menu.appendChild(menuitem);
            menuitem.setAttribute('height', 0.8);
            menuitem.setAttribute("position", { x: 0.25, y: y, z: 0.01 });
            y -= 0.9
            if (i === 0) {
                menuitem.setAttribute('width', 2.5);
                menuitem.setAttribute('color', 'black');
                this.AddText(menuitem, itemarr[i], '#FFF');
            }
            else {
                menuitem.setAttribute('width', supMenuItemWidth);
                menuitem.setAttribute('color', 'grey');
                this.AddText(menuitem, itemarr[i], 'black');
            }
            menuitem.setAttribute('id', itemarr[i]);
        }
    },

    AddText: function (item, string, color) {
        let text = document.createElement('a-text')
        item.appendChild(text)
        text.setAttribute("position", { x: 0.05, y: 0.4, z: 0.01 });
        text.setAttribute("scale", { x: 3, y: 3, z: 3 });
        text.setAttribute("value", string);
        text.setAttribute('color', color);
    },

    AddText: function (item, string, color) {
        let text = document.createElement('a-text')
        item.appendChild(text)
        text.setAttribute("position", { x: 0.05, y: 0.4, z: 0.01 });
        text.setAttribute("scale", { x: 3, y: 3, z: 3 });
        text.setAttribute("value", string);
        text.setAttribute('color', color);
    },

    SetGameHub: function () {
        let game_grid = document.createElement('a-GameGrid');
        game_grid.setAttribute("games", "mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity,mooncity");
        this.el.appendChild(game_grid);
    },

    AddEvents: function () {
        document.querySelector('#Games').addEventListener('mousedown', (evt) => {
            ClearMenu();
            this.SetGameHub();
        });

        document.querySelector('#Options').addEventListener('mousedown', (evt) => {
            this.ClearMenu();

            console.log(this)
            let arr = ['Options', 'Right', 'Left', 'Back'];
            this.AddMenuItemsToMenu(this.menu, arr, 1.7);
            let back = document.querySelector('#Back');
            let left = document.querySelector('#Left');
            let rightHand = document.querySelector('#rightHand')
            let right = document.querySelector('#Right');
            back.setAttribute('width', 2.5)

            if (rightHand.hasAttribute('laser-controls')) {
                right.setAttribute('color', 'green');
                right.firstChild.setAttribute('color', '#FFF');
            }
            else {
                left.setAttribute('color', 'green');
                left.firstChild.setAttribute('color', '#FFF');
            }

            right.addEventListener('mousedown', (evt) => {
                // removeHands();
                // createRightHand();
                // rightHand.setAttribute('laser-controls', { hand: 'right' })
                // createLeftHand();
                left.setAttribute('color', 'grey');
                left.firstChild.setAttribute('color', 'black');
                right.setAttribute('color', 'green');
                right.firstChild.setAttribute('color', '#FFF');
            });

            left.addEventListener('mousedown', (evt) => {
                // removeHands();
                // createLeftHand();
                // leftHand.setAttribute('laser-controls', { hand: 'left' })
                // createRightHand();
                right.setAttribute('color', 'grey');
                right.firstChild.setAttribute('color', 'black');
                left.setAttribute('color', 'green');
                left.firstChild.setAttribute('color', '#FFF');
            });

            back.addEventListener('mousedown', (evt) => {
                this.ClearMenu();
                this.AddMenuItemsToMenu(this.menu, this.menuItemArr, 2.5);
                this.AddEvents();
            });
        });
    },

    ClearMenu: function () {
        while (this.menu.firstChild) {
            this.menu.removeChild(this.menu.lastChild);
        }
    },
})

RegisterAFRAMEPrimitive('a-menu', {
    defaultComponents: {
        menu: {}
    },
    mappings: {
        id: "menu.id",
    }
})

SetUpMenu();
