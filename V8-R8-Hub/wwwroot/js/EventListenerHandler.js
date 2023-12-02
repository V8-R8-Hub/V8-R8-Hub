

class ToggleAbleEventHandler {
    #Clickable;
    constructor() {
        this.#Clickable = true;
    }
    DeactivateEventlisteners() {
        this.#Clickable = false;
    }
    ActivateEventlisteners() {
        this.#Clickable = true;
    }
    AddEventListener(element, eventName, onClick) {

        element.addEventListener(eventName, () => {
            if (this.#Clickable) {
                onClick();
            }
        })
    }


}