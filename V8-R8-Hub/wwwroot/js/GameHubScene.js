function setupScene() {
    let scene = document.querySelector("a-gamescene");
    scene.remove();

    let template = document.querySelector("#overallTemplate");
    template.appendChild(document.createElement("a-gamescene"));
}

RegisterAFRAMEComponent('gamescene', {
    init: function () {
        CreateImgAsset("ground", "/img/cool-gray-61nfwad1bullevu4.jpg");
        CreateSceneWall(this.el, { x: 0, y: 5, z: -15 }, { x: 0, y: 0, z: 0 }, "30", "30", "#ground");

        CreateSceneWall(this.el, { x: 0, y: 5, z: 15 }, { x: 0, y: 0, z: 0 }, "30", "20", "#ground");

        CreateSceneWall(this.el, { x: -15, y: 5, z: 0 }, { x: 0, y: 90, z: 0 }, "30", "20", "#ground");

        CreateSceneWall(this.el, { x: 15, y: 5, z: 0 }, { x: 0, y: 90, z: 0 }, "30", "30", "#ground");

        CreateSceneWall(this.el, { x: 0, y: 15, z: 0 }, { x: 90, y: 0, z: 0 }, "30", "30", "#ground");
        CreateSceneFloor(this.el, "30", "30", "#ground")
    }
})


function CreateImgAsset(id, src) {
    let scene = document.querySelector("a-scene");
    let wallAsset = document.createElement("a-assets");
    let img = document.createElement("img");
    img.setAttribute("id", id);
    img.setAttribute("src", src);
    wallAsset.appendChild(img);
    scene.appendChild(wallAsset);
}

function CreateSceneWall(el, pos, rot, width, height, src) {
    let wall = document.createElement("a-box");
    wall.setAttribute("position", { x: pos.x, y: pos.y, z: pos.z });

    wall.setAttribute("rotation", { x: rot.x, y: rot.y, z: rot.z });

    wall.setAttribute("depth", 2);
    wall.setAttribute("width", width);
    wall.setAttribute("height", height);
    wall.setAttribute("src", src);

    el.appendChild(wall)
}

function CreateSceneFloor(el, width, height, src) {
    let plane = document.createElement("a-plane");
    plane.setAttribute("src", src);
    plane.setAttribute("width", width);
    plane.setAttribute("height", height);
    plane.setAttribute("rotation", "-90 0 0");
    el.appendChild(plane);
}

RegisterAFRAMEPrimitive('a-gamescene', {
    defaultComponents: {
        gamescene: {
        }
    },
})

setupScene();
