function ClearMenu() {
    let menu = document.querySelector(`#menu`);

    menu.remove();
}


function CreateText(parent, string, color, horizontal_align, vertical_align) {


    let text = document.createElement('a-troika-text');
    text.setAttribute("max-width", 4);

    // text.setAttribute("height", 0.5);
    text.setAttribute("anchor", "left");
    let y_pos = ConvertYPos(vertical_align, parent.getAttribute("height"));

    let x_pos = ConvertXPos(horizontal_align, parent.getAttribute("width"));
    console.log(`height: ${parent.getAttribute("height")}  y_pos: ${y_pos}`);
    text.setAttribute("position", { x: x_pos, y: y_pos, z: 0.005 });

    // text.setAttribute("scale", { x: 3, y: 3, z: 3 });

    text.setAttribute("value", string);
    text.setAttribute('color', color);

    return text;
}
function ConvertYPos(option, height) {

    let correctedHeight = height;
    switch (option) {
        case "Bottom": return 0;

        case "center": return correctedHeight / 2;
        case "top": return correctedHeight - 0.2;

        default: return correctedHeight / 2;
    }
}


function ConvertXPos(option, width) {

    let CorrectedWidth = width - 0.2;
    switch (option) {
        case "left": return 0;
        case "center": return CorrectedWidth / 2;
        case "right": return CorrectedWidth + 0.2;
        default: return correctedXPos / 2;
    }
}