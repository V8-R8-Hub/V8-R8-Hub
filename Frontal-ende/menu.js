//scene til hele lortet
var Ascene = document.querySelector('a-scene');
var cameraRig = document.getElementById('cameraRig');

var laser = document.createElement('a-entity')

//seje hånd controls til vr brug 
function createRightHand() {
    var rightHand = document.createElement('a-entity');
    rightHand.setAttribute('id', 'rightHand');
    rightHand.setAttribute('mixin', 'hand');
    rightHand.setAttribute('oculus-touch-controls', { hand: 'right' });
    cameraRig.appendChild(rightHand);
}

function createLeftHand() {
    var leftHand = document.createElement('a-entity');
    leftHand.setAttribute('id', 'leftHand');
    leftHand.setAttribute('mixin', 'hand');
    leftHand.setAttribute('oculus-touch-controls', { hand: 'left' });
    cameraRig.appendChild(leftHand);
}

function removeHands() {
    document.querySelector('#rightHand').remove();
    document.querySelector('#leftHand').remove();
}
createRightHand()
document.querySelector('#rightHand').setAttribute('laser-controls', '')
createLeftHand()

//sej menu 
var menu = document.createElement('a-rounded');
Ascene.appendChild(menu);
menu.setAttribute("position", { x: -1.5, y: 0.5, z: -3 });
menu.setAttribute('width', 3);
menu.setAttribute('height', 4)

//nem måde at add menu items
function AddMenuItemsToMenu(menu, itemarr, supMenuItemWidth) {
    var y = 3;
    for (let i = 0; i <= itemarr.length - 1; i++) {
        var menuitem = document.createElement('a-rounded')
        menu.appendChild(menuitem);
        menuitem.setAttribute('height', 0.8);
        menuitem.setAttribute("position", { x: 0.25, y: y, z: 0.01 });
        y -= 0.9
        if (i === 0) {
            menuitem.setAttribute('width', 2.5);
            menuitem.setAttribute('color', 'black');
            AddText(menuitem, itemarr[i], '#FFF');
        }
        else {
            menuitem.setAttribute('width', supMenuItemWidth);
            menuitem.setAttribute('color', 'grey');
            AddText(menuitem, itemarr[i], 'black');
        }
        menuitem.setAttribute('id', itemarr[i]);
    }
    AddEvents()
}

//add test til menu items
function AddText(item, string, color) {
    var text = document.createElement('a-text')
    item.appendChild(text)
    text.setAttribute("position", { x: 0.05, y: 0.4, z: 0.01 });
    text.setAttribute("scale", { x: 3, y: 3, z: 3 });
    text.setAttribute("value", string);
    text.setAttribute('color', color);
}

//add eventlisteners til menu items
function AddEvents() {
    // document.querySelector('#Quit').addEventListener('mousedown', function (evt){
    //     menu.setAttribute('animation', {property: 'position', to: {x: -1.5, y: -10, z: -3}, dur: 2000, loop: 'false'})
    // });

    // document.querySelector('#Games').addEventListener('mousedown', function(evt){
    //     menu.setAttribute('animation', {property: 'position', to: {x: -20, y: 0.5, z: -3}, dur: 2000, loop: 'false'})
    // })  

    document.querySelector('#Options').addEventListener('mousedown', function (evt) {
        ClearMenu();
        var arr = ['Options', 'Right', 'Left', 'Back'];
        AddMenuItemsToMenu(menu, arr, 1.7);
        var back = document.querySelector('#Back');
        var left = document.querySelector('#Left');
        var rightHand = document.querySelector('#rightHand')
        var leftHand = document.querySelector('#leftHand')
        var right = document.querySelector('#Right');
        back.setAttribute('width', 2.5)

        if (rightHand.hasAttribute('laser-controls')) {
            right.setAttribute('color', 'green');
            right.firstChild.setAttribute('color', '#FFF');
        }
        else {
            left.setAttribute('color', 'green');
            left.firstChild.setAttribute('color', '#FFF');
        }

        right.addEventListener('mousedown', function (evt) {
            // removeHands();
            // createRightHand();
            // rightHand.setAttribute('laser-controls', { hand: 'right' })
            // createLeftHand();
            left.setAttribute('color', 'grey');
            left.firstChild.setAttribute('color', 'black');
            right.setAttribute('color', 'green');
            right.firstChild.setAttribute('color', '#FFF');
        });

        left.addEventListener('mousedown', function (evt) {
            // removeHands();
            // createLeftHand();
            // leftHand.setAttribute('laser-controls', { hand: 'left' })
            // createRightHand();
            right.setAttribute('color', 'grey');
            right.firstChild.setAttribute('color', 'black');
            left.setAttribute('color', 'green');
            left.firstChild.setAttribute('color', '#FFF');
        });

        back.addEventListener('mousedown', function (evt) {
            ClearMenu();
            AddMenuItemsToMenu(menu, menuItemArr, 2.5);
        });

    });
}

function ClearMenu() {
    while (menu.firstChild) {
        menu.removeChild(menu.lastChild);
    }
}

var menuItemArr = ['Menu', 'Games', 'Options', 'Quit']
AddMenuItemsToMenu(menu, menuItemArr, 2.5);
