var Ascene = document.querySelector('a-scene');

var rightHand = document.createElement('a-entity');
rightHand.setAttribute('id', 'rightHand');
rightHand.setAttribute('mixin', 'hand');
rightHand.setAttribute('oculus-touch-controls', { hand: 'right' });
rightHand.setAttribute('laser-controls', '');

var leftHand = document.createElement('a-entity');
leftHand.setAttribute('id', 'leftHand');
leftHand.setAttribute('mixin', 'hand');
leftHand.setAttribute('oculus-touch-controls', { hand: 'left' });

var cameraRig = document.getElementById('cameraRig');
cameraRig.appendChild(rightHand);
cameraRig.appendChild(leftHand);

var menu = document.createElement('a-rounded');
Ascene.appendChild(menu);
menu.setAttribute("position", { x: -1.5, y: 0.5, z: -3 });
menu.setAttribute('width', 3);
menu.setAttribute('height', 4)


function AddMenuItemsToMenu(menu) {
    var arr = ['Menu', 'Games', 'Options', 'Quit'];
    var y = 3;
    for (let i = 0; i <= arr.length - 1; i++) {
        var menuitem = document.createElement('a-rounded')
        menu.appendChild(menuitem);
        menuitem.setAttribute('width', 2.5);
        menuitem.setAttribute('height', 0.8);
        menuitem.setAttribute("position", { x: 0.25, y: y, z: 0.01 });
        y -= 0.9
        if (arr[i] === 'Menu') {
            menuitem.setAttribute('color', 'black');
            AddText(menuitem, arr[i], '#FFF');
        }
        else {
            menuitem.setAttribute('color', 'grey');
            AddText(menuitem, arr[i], 'black');
        }
        menuitem.setAttribute('id', arr[i]);
    }
}

function AddText(item, string, color) {
    var text = document.createElement('a-text')
    item.appendChild(text)
    text.setAttribute("position", { x: 0.05, y: 0.4, z: 0.01 });
    text.setAttribute("scale", { x: 3, y: 3, z: 3 });
    text.setAttribute("value", string);
    text.setAttribute('color', color);
}

AddMenuItemsToMenu(menu);

// document.querySelector('#Quit').addEventListener('mousedown', function (evt){
//     menu.setAttribute('animation', {property: 'position', to: {x: -1.5, y: -10, z: -3}, dur: 2000, loop: 'false'})
// });

// document.querySelector('#Games').addEventListener('mousedown', function(evt){
//     menu.setAttribute('animation', {property: 'position', to: {x: -20, y: 0.5, z: -3}, dur: 2000, loop: 'false'})
// })  

document.querySelector('#Options').addEventListener('mousedown', function (evt) {
    ClearMenu();
    var optionitem = document.createElement('a-rounded');
    menu.appendChild(optionitem);
    optionitem.setAttribute('color', 'black');
    AddText(optionitem, 'Options', '#FFF');
    optionitem.setAttribute('width', 2.5);
    optionitem.setAttribute('height', 0.8);
    optionitem.setAttribute("position", { x: 0.25, y: 3, z: 0.01 });

    var left = document.createElement('a-rounded');
    left.setAttribute('width', 1)
    left.setAttribute('height', 0.8);
    left.setAttribute("position", { x: 0.25, y: 3, z: 0.01 });
    


});

function ClearMenu() {
    while (menu.firstChild) {
        menu.removeChild(menu.lastChild)
    }
}