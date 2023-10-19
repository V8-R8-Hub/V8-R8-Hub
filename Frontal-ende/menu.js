var Ascene = document.querySelector('a-scene');
var menu = document.createElement('a-rounded');

Ascene.appendChild(menu);
menu.setAttribute("position", { x: -1.5, y: 0.5, z: -3 });
menu.setAttribute('width', 3);
menu.setAttribute('height', 4)

function AddMenuItemsToMenu(menu) {
    var arr = ['Menu', 'Games', 'Options', 'Quit'];
    var y = 3;
    for (let i = 0; i <= arr.length-1; i++) {
        var menuitem = document.createElement('a-rounded')
        menu.appendChild(menuitem);
        menuitem.setAttribute('width', 2.5);
        menuitem.setAttribute('height', 0.8);
        menuitem.setAttribute("position", { x: 0.25, y: y, z: 0.01 });
        y -= 0.9
        if (arr[i] === 'Menu') {
            menuitem.setAttribute('color', 'black')
            AddTextMenuitem(menuitem, arr[i], '#FFF')
        }
        else{
            menuitem.setAttribute('color', 'grey')
            AddTextMenuitem(menuitem, arr[i], 'black')
        }
    }
}
function AddTextMenuitem(menuitem, string, color) {
    var text = document.createElement('a-text')
    menuitem.appendChild(text)
    text.setAttribute("position", { x: 0.25, y: 0.4, z: 0.01 });
    text.setAttribute("scale", { x: 3, y: 3, z: 3 });
    text.setAttribute("value", string);
    text.setAttribute('color', color);
}

AddMenuItemsToMenu(menu);

document.querySelector('#box').addEventListener('mousedown', function (evt) {
    clickgames();
});


function clickgames() {
    Ascene.parentNode.removeChild(menu);
    console.log("sus")
}
