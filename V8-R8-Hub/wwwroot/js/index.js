let registeredComponents = [];
let registeredPrimitives = {};
let gameGuid = null;
let acceptedCookies = false;
let previousLeftPosition = null;
let previousRightPosition = null;

function SetGameGuid(guid) {
    gameGuid = guid;
}

function createButtons() {
    // tilføj hele lortet til scenen
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
}

async function switchScene(htmlFilename, guid, jsFilename) {
    SetGameGuid(guid);
    await SetUpGameLoader();

    let game = GetGamesLoader().getGame(guid);
    // enable loading screen
    // ja det her shit er nødvendigt fordi .hide ikke forhindre lasers i at intersect med boksen :D
    let loadingScreen = document.createElement('a-box');
    loadingScreen.setAttribute('id', 'loadingScreen');
    loadingScreen.setAttribute('material', 'side: double');
    loadingScreen.setAttribute('color', '#000000');
    document.getElementById('camera').appendChild(loadingScreen);

    // slet alle registered components
    registeredComponents.forEach((x) => delete AFRAME.components[x]);
    SetHistory(game);

    // skift HTML ud
    document.getElementById('overallTemplate').setAttribute('template', 'src: ' + htmlFilename);
    setTimeout(() => {
        // eksekvere JS

        let files = document.getElementById('overallTemplate').querySelectorAll("script");


        files.forEach((file) => {
            fetch(file.attributes.src.value).then(r => r.text()).then(eval)
        });

        setTimeout(() => {
            // disable loading screen
            loadingScreen.parentNode.removeChild(loadingScreen);
        }, "5000");
    }, "5000");
}

function SetHistory(game) {
    if (game == null) {
        window.history.pushState("", "GameHub", "/components/");
        return;
    }
    window.history.pushState("", game.name, "/api/Game/" + game.guid + "/assets/play");
}

function RegisterAFRAMEComponent(componentName, componentFunction) {
    AFRAME.registerComponent(componentName, componentFunction);
    registeredComponents.push(componentName);
}

function RegisterAFRAMEPrimitive(primitiveName, PrimitiveObj) {
    if (primitiveName in registeredPrimitives) {
        return;
    }

    AFRAME.registerPrimitive(primitiveName, PrimitiveObj);
    registeredPrimitives[primitiveName] = true;
}

function RemoveNonVRElements() {
    // fjern hitmarker
    let cursor = document.querySelector('a-cursor');
    cursor.parentNode.removeChild(cursor);

    // idk om det er nødvendigt men fuck it
    document.getElementById('camera').removeAttribute('look-controls');

    // cookies
    RemovePopup();
}

const cookiePopup = document.getElementById('cookiepopup');
function RemovePopup() {
    cookiePopup.remove();
}

function DismissCookiePopup(accepted) {
    RemovePopup();
    if (accepted) {
        document.cookie = "V8R8HubCookieAccept=; Max-Age=34560000";
    } else {
        document.cookie = "V8R8HubCookieAccept=; expires=Thu, 01 Jan 1970 00:00:00 UTC;";
    }
    acceptedCookies = true;
}

if (document.cookie.split(";").find(x => x.startsWith("V8R8HubCookieAccept") != null)) {
    RemovePopup();
}


createButtons();

document.querySelector('a-scene').addEventListener('enter-vr', RemoveNonVRElements);

switchScene("/components/menu.html ", null, "");

AFRAME.registerComponent('camera-tracking', {
    init: function () {
        this.threshold = 0.01;
        this.updateInterval = 1000;
        this.previousCameraData = null;
        this.updateCameraData();
    },
    updateCameraData: function () {
        const camera = this.el.object3D;
        const position = camera.position;
        const rotation = camera.rotation;
        const currentCameraData = {
            position: { x: position.x, y: position.y, z: position.z },
            rotation: { x: rotation.x, y: rotation.y, z: rotation.z },
        };
        if (!this.previousCameraData ||
            thresholdCheck(this.previousCameraData.position, currentCameraData.position, this.threshold) ||
            thresholdCheck(this.previousCameraData.rotation, currentCameraData.rotation, this.threshold)){
            this.previousCameraData = currentCameraData;
            this.el.emit('camera-updated', currentCameraData);
        }
        setTimeout(this.updateCameraData.bind(this), this.updateInterval);
    },
});

document.querySelector('a-scene').addEventListener('camera-updated', function (event) {
    if(gameGuid == null)
        return;
    const cameraData = event.detail;
    const cameraPosition = cameraData.position;
    const cameraRotation = cameraData.rotation;
    sendMetricData(cameraPosition, "CameraPositionMetrics");
    sendMetricData(cameraRotation, "CameraRotationMetrics");
});

function listenToRightHandEvents() {
    const threshold = 0.01;
    const rightController = document.getElementById('rightHand');
    const rightControllerPosition = rightController.getAttribute('position');
    const currentRightHandPosition = {
        x: rightControllerPosition.x,
        y: rightControllerPosition.y,
        z: rightControllerPosition.z
    };
    if (!previousRightPosition ||
        thresholdCheck(previousRightPosition, currentRightHandPosition, threshold)) {
        previousRightPosition = currentRightHandPosition;
        rightController.emit('rightControllerMoved', currentRightHandPosition);
    }
    setTimeout(listenToRightHandEvents, 1000);
}

function listenToLeftHandEvents() {
    const threshold = 0.01;
    const leftController = document.getElementById('leftHand');
    const leftControllerPosition = leftController.getAttribute('position');
    const currentLeftHandPosition = {
        x: leftControllerPosition.x,
        y: leftControllerPosition.y,
        z: leftControllerPosition.z
    };
    if (!previousLeftPosition ||
        thresholdCheck(previousLeftPosition, currentLeftHandPosition, threshold)) {
        previousLeftPosition = currentLeftHandPosition;
        leftController.emit('leftControllerMoved', currentLeftHandPosition);
    }
    setTimeout(listenToLeftHandEvents, 1000);
}

listenToRightHandEvents();
listenToLeftHandEvents();

document.getElementById('rightHand').addEventListener('rightControllerMoved', function (event) {
    const rightControllerData = event.detail;
    sendMetricData(rightControllerData, "rightControllerPosition");

});

document.getElementById('leftHand').addEventListener('leftControllerMoved', function (event) {
    const leftControllerData = event.detail;
    sendMetricData(leftControllerData, "leftControllerPosition");
});

function thresholdCheck(previousData, currentData, threshold) {
    for (let i in currentData) {
        if (Math.abs(currentData[i] - previousData[i]) > threshold) {
            return true;
        }
    }
    return false;
}

function sendMetricData(metricJsonData, metricCategory)
{
    if(!acceptedCookies)
        return;
    const metricRequest = {
        MetricJsonData: JSON.stringify(metricJsonData),
        MetricCategory: metricCategory
    }
    fetch('/api/User/metrics/'+ gameGuid, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(metricRequest),
    });
}