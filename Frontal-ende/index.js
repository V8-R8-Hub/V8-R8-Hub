
let registeredComponents = [];
let registeredPrimitives = {};
function createButtons()
{
    // tilføj hele lortet til scenen
    var rightHand = document.createElement('a-entity');
    rightHand.setAttribute('id', 'rightHand');
    rightHand.setAttribute('mixin', 'hand');
    rightHand.setAttribute('oculus-touch-controls', {hand: 'right'});
    rightHand.setAttribute('laser-controls', '');

    var leftHand = document.createElement('a-entity');
    leftHand.setAttribute('id', 'leftHand');
    leftHand.setAttribute('mixin', 'hand');
    leftHand.setAttribute('oculus-touch-controls', {hand: 'left'});

    var cameraRig = document.getElementById('cameraRig');
    cameraRig.appendChild(rightHand);
    cameraRig.appendChild(leftHand);
}

function switchScene(htmlFilename, jsFilename)
{
    // enable loading screen
    document.getElementById('loadingScreen').setAttribute('visible', 'true');

    // slet alle registered components

    

    registeredComponents.forEach((x) => delete AFRAME.components[x]);


    console.log("AMOGUS AMOGUS");


    // skift HTML ud
    document.getElementById('overallTemplate').setAttribute('template', 'src: ' + htmlFilename);
    setTimeout(() =>
    {
        // eksekvere JS
        console.log(jsFilename);
        let files =  document.getElementById('overallTemplate').querySelectorAll("script");
        console.log(files);


        files.forEach((file)=> {
            fetch(file.attributes.src.value).then(r => r.text()).then(eval)

        });
        

        setTimeout(() =>
        {
            // disable loading screen
            document.getElementById('loadingScreen').emit('hide');
            document.getElementById('loadingScreen').setAttribute('visible', 'false');
        }, "5000");
    }, "5000");
}

function RegisterAFRAMEComponent(componentName, componentFunction)
{
    AFRAME.registerComponent(componentName, componentFunction);
    registeredComponents.push(componentName);
}



function RegisterAFRAMEPrimitive(primitiveName, PrimitiveObj)
{


    if (primitiveName in registeredPrimitives) {


        return;
    }
    AFRAME.registerPrimitive(primitiveName, PrimitiveObj);
    registeredPrimitives[primitiveName] = true;
}

createButtons();

switchScene("menu.html", "");
