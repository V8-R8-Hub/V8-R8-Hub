//For at qoute en vis mand "Globale variabler er roden til alt ondt i verdenen" 
let Clickable = true;

function AddGameEventListener(element, eventName, onClick) {
    console.log("AMONGUS")

    element.addEventListener(eventName,()=> {
        if (Clickable) {

            onClick(); 
        }
    })
}

function ActivateGameEventlisteners()
{
    Clickable = true;
}

function DeactivateGameEventlisteners()
{
    Clickable = false;
}
