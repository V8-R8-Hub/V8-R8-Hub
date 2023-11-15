console.log("test");

document.getElementById('cameraRig').setAttribute('position', '0 0 7');

function setupScene(scene) {
    var cylinder = document.createElement('a-cylinder');
    cylinder.setAttribute('color', '#FF9500');
    cylinder.setAttribute('height', '2');
    cylinder.setAttribute('radius', '0.75');
    cylinder.setAttribute('position', '-3 1 0');
    cylinder.setAttribute('rotation', '90 90 90');
    cylinder.setAttribute('src', 'https://media.tenor.com/mysjAvtx6ugAAAAM/joe-biden-wake-up.gif');
    //scene.appendChild(cylinder);

    /*var wall = document.createElement('a-box');
    wall.setAttribute('width', '5.7');
    wall.setAttribute('height', '4.65');
    wall.setAttribute('depth', '1');
    wall.setAttribute('position', '0 0.8 0');
    wall.setAttribute('src', '#brick');*/
    //scene.appendChild(wall);

    var wallLeft = document.createElement('a-box');
    wallLeft.setAttribute('width', '3');
    wallLeft.setAttribute('height', '3');
    wallLeft.setAttribute('position', '4.1 0 1.5');
    wallLeft.setAttribute('rotation', '0 125 0');
    wallLeft.setAttribute('src', '#brick');
    scene.appendChild(wallLeft);

    var wallRight = document.createElement('a-box');
    wallRight.setAttribute('width', '3');
    wallRight.setAttribute('height', '3');
    wallRight.setAttribute('position', '-4.27 0 1.2');
    wallRight.setAttribute('rotation', '0 45 0');
    wallRight.setAttribute('src', '#brick');
    scene.appendChild(wallRight);

    var floor = document.createElement('a-plane');
    floor.setAttribute('src', 'brick.jpg');
    floor.setAttribute('width', '50');
    floor.setAttribute('height', '50');
    floor.setAttribute('rotation', '-90 0 0');
    floor.setAttribute('position', '0 -1.5 0');
    scene.appendChild(floor);

    // det skulle være en basketball men det blev ronny ræs
    var basketballHoop = document.createElement('a-gltf-model');
    basketballHoop.setAttribute('src', "scene.gltf");
    basketballHoop.setAttribute('position', '0 1 5');
    //scene.appendChild(basketballHoop);

    var troldmand = document.createElement('a-plane');
    troldmand.setAttribute('position', '3 -0.2 4');
    troldmand.setAttribute('height', '2.5');
    troldmand.setAttribute('width', '2');
    //troldmand.setAttribute('src', 'troldmand1.png');
    troldmand.setAttribute('transparent', 'true');

    RegisterAFRAMEComponent('slides', {
        init: function () {
            let loader = new THREE.TextureLoader();
            this.array = [];
            this.array.push(loader.load("maler.dir_212.png"));
            this.array.push(loader.load("maler.dir_213.png"));
            this.array.push(loader.load("maler.dir_214.png"));
            this.array.push(loader.load("maler.dir_215.png"));
            this.array.push(loader.load("maler.dir_216.png"));
            this.array.push(loader.load("maler.dir_217.png"));
            this.array.push(loader.load("maler.dir_218.png"));

            this.el.addEventListener('loaded', e => {
                let mesh = this.el.getObject3D('mesh');
                this.material = mesh.material;
                let i = 0;
                setInterval(e => {
                    if (i >= this.array.length)
                        i = 0;
                    this.material.map = this.array[i++];
                    this.material.needsUpdate = true;
                }, 500)
            })
        }
    });
    troldmand.setAttribute('slides', '');

    var descriptionText = document.createElement('a-text');
    descriptionText.setAttribute('id', 'descriptionText');
    descriptionText.setAttribute('value', 'Hvor meget maling skal der s\\å bruges til denne v\æg?\n\nHusk, at håndværkeren skal bruge 2 deciliter maling\nfor at male EN kvadratmeter.');
    descriptionText.setAttribute('position', '0 1 0');
    descriptionText.setAttribute('font', 'roboto');
    troldmand.appendChild(descriptionText);

    // tilføj lortet
    scene.appendChild(troldmand);
}

function updateMeasurements(greenValue, greenHeight, redValue, redPos, redRot, redHeight, redTextPos, redTextRot) {
    var measureGreen = document.getElementById('measureGreen');
    measureGreen.setAttribute('height', greenHeight);

    var measureRed = document.getElementById('measureRed');
    measureRed.setAttribute('position', redPos);
    measureRed.setAttribute('rotation', redRot);
    measureRed.setAttribute('height', redHeight);

    var measureRedText = document.getElementById('measureRedText');
    measureRedText.setAttribute('value', redValue);
    measureRedText.setAttribute('position', redTextPos);
    measureRedText.setAttribute('rotation', redTextRot);

    var measureGreenText = document.getElementById('measureGreenText');
    measureGreenText.setAttribute('value', greenValue);
}

function createMeasurements(scene, width, height) {
    var measureGreen = document.createElement('a-cylinder');
    measureGreen.setAttribute('id', 'measureGreen');
    measureGreen.setAttribute('radius', '0.05');
    measureGreen.setAttribute('position', '-2.8 0 0.5');
    measureGreen.setAttribute('height', '3');
    measureGreen.setAttribute('color', '#39FF14');
    measureGreen.setAttribute('emissive', '#39FF14');

    var measureRed = document.createElement('a-cylinder');
    measureRed.setAttribute('id', 'measureRed');
    measureRed.setAttribute('radius', '0.05');
    measureRed.setAttribute('position', '-3.85 1.5 1.55');
    measureRed.setAttribute('height', '3');
    measureRed.setAttribute('color', '#FF3131');
    measureRed.setAttribute('rotation', '-90 -45 0');
    measureRed.setAttribute('emissive', '#FF3131');

    var measureRedText = document.createElement('a-text');
    measureRedText.setAttribute('id', 'measureRedText');
    measureRedText.setAttribute('value', width);
    measureRedText.setAttribute('width', '20');
    measureRedText.setAttribute('height', '200');
    measureRedText.setAttribute('rotation', '45 90 90');

    var measureGreenText = document.createElement('a-text');
    measureGreenText.setAttribute('id', 'measureGreenText');
    measureGreenText.setAttribute('value', width);
    measureGreenText.setAttribute('width', '20');
    measureGreenText.setAttribute('height', '20');

    measureRed.appendChild(measureRedText);
    measureGreen.appendChild(measureGreenText);

    scene.appendChild(measureGreen);
    scene.appendChild(measureRed);
}

function setInnerHTML(elm, html) {
    elm.innerHTML = html;

    Array.from(elm.querySelectorAll("script"))
        .forEach(oldScriptEl => {
            const newScriptEl = document.createElement("script");

            Array.from(oldScriptEl.attributes).forEach(attr => {
                newScriptEl.setAttribute(attr.name, attr.value)
            });

            const scriptText = document.createTextNode(oldScriptEl.innerHTML);
            newScriptEl.appendChild(scriptText);

            oldScriptEl.parentNode.replaceChild(newScriptEl, oldScriptEl);
        });
}

function createButtons(scene) {
    let controlPanelWidth = 5;
    let controlPanelHeight = 3;
    let controlPanelDepth = 0.2;

    var controlPanel = document.createElement('a-box');
    controlPanel.setAttribute('id', 'controlPanel');
    controlPanel.setAttribute('width', controlPanelWidth);
    controlPanel.setAttribute('height', controlPanelHeight);
    controlPanel.setAttribute('depth', controlPanelDepth);
    controlPanel.setAttribute('rotation', '0 0 0'); // 270 remember
    controlPanel.setAttribute('scale', '0.1 0.1 0.1');
    controlPanel.setAttribute('color', '#929292');

    var measureNumber = document.createElement('a-text');
    measureNumber.setAttribute('id', 'counter');
    measureNumber.setAttribute('position', '0 1 0.2');
    measureNumber.setAttribute('value', '0');
    measureNumber.setAttribute('width', 50);
    measureNumber.setAttribute('color', '#00FFFF');
    controlPanel.appendChild(measureNumber);

    RegisterAFRAMEComponent('pis', {
        schema: {
            addition: { type: 'int', default: 0 }
        },

        init: function () {
            let fuckAf = this.data.addition;

            this.el.addEventListener('mousedown', function () {
                counter = document.getElementById('counter');

                let currentValue = parseInt(counter.getAttribute('value'));
                if (isNaN(currentValue))
                    currentValue = 0;

                currentValue += fuckAf;

                counter.setAttribute('value', currentValue);

                //var entity = document.getElementById('ambient');
                //entity.components.sound.playSound();
            });
        }
    })

    var posButtons = document.createElement('a-entity');
    posButtons.setAttribute('position', -(controlPanelWidth / 4) + ' ' + controlPanelHeight / 8 + ' 0.1');
    console.log("yo what");
    var buttonPlus = document.createElement('a-box');
    buttonPlus.setAttribute('rotation', '0 90 0');
    buttonPlus.setAttribute('width', '0.2');
    buttonPlus.setAttribute('pis', 'addition: 1');
    buttonPlus.setAttribute('src', '#plus');
    buttonPlus.setAttribute('class', 'clickable');
    buttonPlus.setAttribute('transparent', 'true');

    var buttonPlus5 = document.createElement('a-box');
    buttonPlus5.setAttribute('id', 'buttonPlus5');
    buttonPlus5.setAttribute('position', '0.8 0.2 0');
    buttonPlus5.setAttribute('width', '0.5');
    buttonPlus5.setAttribute('height', '0.5');
    buttonPlus5.setAttribute('depth', '0.5');
    buttonPlus5.setAttribute('pis', 'addition: 5');
    buttonPlus5.setAttribute('transparent', 'true');
    // buttonPlus5.setAttribute('src', 'http://localhost:7171/api/Game/af513586-36e8-4f74-b1f1-27e46e4fb8d8/plus5.png');

    var minusButtons = document.createElement('a-entity');
    minusButtons.setAttribute('position', -(controlPanelWidth / 4) + ' ' + '-1' + ' 0.1');

    var buttonMinus = document.createElement('a-box');
    buttonMinus.setAttribute('rotation', '0 90 0');
    buttonMinus.setAttribute('width', '0.2');
    buttonMinus.setAttribute('pis', 'addition: -1');
    buttonMinus.setAttribute('src', '#minus');
    buttonMinus.setAttribute('transparent', 'true');

    var buttonMinus5 = document.createElement('a-box');
    buttonMinus5.setAttribute('position', '0.8, -0.2, 0');
    buttonMinus5.setAttribute('width', '0.5');
    buttonMinus5.setAttribute('height', '0.5');
    buttonMinus5.setAttribute('depth', '0.5');
    buttonMinus5.setAttribute('pis', 'addition: -5');
    buttonMinus5.setAttribute('src', '#minus5');
    buttonMinus5.setAttribute('transparent', 'true');

    posButtons.appendChild(buttonPlus);
    posButtons.appendChild(buttonPlus5);
    minusButtons.appendChild(buttonMinus);
    minusButtons.appendChild(buttonMinus5);

    // smid lortet ind
    controlPanel.appendChild(posButtons);
    controlPanel.appendChild(minusButtons);

    RegisterAFRAMEComponent("cock",
        {
            init: function () {
                this.el.addEventListener('mousedown', function () {
                    var elem = document.getElementById('controlPanel');
                    elem.parentNode.removeChild(elem);
                    let shit = document.getElementById("overallTemplate");
                    switchScene("menu.html", "sus.js");
                    /*
                    counter = document.getElementById('counter');
                    if (correctAnswer == parseInt(counter.getAttribute('value')))
                    {
                        counter.setAttribute('value', '0');
                        updateSceneToBackWall();
                    }d
                    else
                        counter.setAttribute('value', 'No!');
                    */
                });
            }
        })

    var button0 = document.createElement('a-box');
    button0.setAttribute('position', '1 -1 0.1');
    button0.setAttribute('height', '0.5');
    button0.setAttribute('width', '0.2');
    button0.setAttribute('rotation', '0 90 0');
    button0.setAttribute('src', '#ok');
    button0.setAttribute('transparent', 'true');
    button0.setAttribute('cock', '');
    controlPanel.appendChild(button0);

    // tilføj hele lortet til scenen
    var leftHand = document.getElementById('leftHand');
    leftHand.appendChild(controlPanel);
}

let correctAnswer = 0;

function generateRandomNumber(min, max) {
    return Math.floor(Math.random() * (max - min + 1) + min);
}

function updateMeasurementValues(measure) {
    // beregn de korrekte tal.
    let deciliterPrSquareMeter = 2; // TODO mellem 1-4
    correctAnswer = (measure * measure) * deciliterPrSquareMeter;
}

function updateSceneToBackWall() {
    document.getElementById('maler_370').components.sound.playSound();

    let measure = generateRandomNumber(2, 5);
    updateMeasurementValues(measure);
    updateMeasurements(measure, '6.2', measure, '0 3.1 0.5', '0 0 90', '5.7', '0.4 0 0', '0 0 270');
    document.getElementById('descriptionText').setAttribute('value', 'Hvad så med bagvæggen? Den har disse mål');
}

//var scene = document.querySelector('a-scene');
var scene = document.getElementById('overallTemplate');
setupScene(scene);

let measure = generateRandomNumber(2, 4);
updateMeasurementValues(measure);
createMeasurements(scene, measure, measure);
createButtons(scene);
