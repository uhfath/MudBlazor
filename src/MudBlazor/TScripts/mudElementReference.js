// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

class MudElementReference {
    constructor() {
        this.listenerId = 0;
        this.eventListeners = {};
    }

    focus (element) {
        if (element)
        {
            element.focus();
        }
    }

    blur(element) {
        if (element) {
            element.blur();
        }
    }

    focusFirst (element, skip = 0, min = 0) {
        if (element)
        {
            const tabbables = getTabbableElements(element);
            if (tabbables.length <= min)
                element.focus();
            else
                tabbables[skip].focus();
        }
    }

    focusLast (element, skip = 0, min = 0) {
        if (element)
        {
            const tabbables = getTabbableElements(element);
            if (tabbables.length <= min)
                element.focus();
            else
                tabbables[tabbables.length - skip - 1].focus();
        }
    }

    focusPrevious (element, from) {
        if (element)
        {
            const tabbables = [...getAllTabbableElements(element)];
            const current = tabbables.indexOf(from || document.activeElement);
            let previous = (current === 0 || current === -1) ? tabbables.length - 1 : current - 1;

            previous = tabbables.slice(0, previous + 1).findLastIndex((e) => e.tabIndex >= 0);
            if (previous === -1) {
                previous = tabbables.findLastIndex((e) => e.tabIndex >= 0);
            }

            if (previous >= 0) {
                tabbables[previous].focus();
            }
        }
    }

    focusNext (element, from) {
        if (element)
        {
            const tabbables = [...getAllTabbableElements(element)];
            const current = tabbables.indexOf(from || document.activeElement);
            let next = (current === tabbables.length || current === -1) ? 0 : current + 1;

            next = tabbables.slice(next).findIndex((e) => e.tabIndex >= 0);
            if (next === -1) {
                next = tabbables.findIndex((e) => e.tabIndex >= 0);
            }

            if (next >= 0) {
                tabbables[next].focus();
            }
        }
    }

    saveFocus (element) {
        if (element)
        {
            element['mudblazor_savedFocus'] = document.activeElement;
        }
    }

    clearSavedFocus (element) {
        if (element) {
            delete element['mudblazor_savedFocus']
        }
    }

    restoreFocus (element) {
        if (element)
        {
            const saved = element['mudblazor_savedFocus'];
            this.clearSavedFocus(element);

            if (saved)
                saved.focus();
        }
    }

    restoreFocusToPrevious (element, from) {
        if (element)
        {
            const saved = element['mudblazor_savedFocus'];
            this.clearSavedFocus(element);

            if (saved)
                this.focusPrevious(from || document, saved);
        }
    }

    restoreFocusToNext (element, from) {
        if (element)
        {
            const saved = element['mudblazor_savedFocus'];
            this.clearSavedFocus(element);

            if (saved)
                this.focusNext(from || document, saved);
        }
    }

    selectRange(element, pos1, pos2) {
        if (element)
        {
            if (element.createTextRange) {
                const selRange = element.createTextRange();
                selRange.collapse(true);
                selRange.moveStart('character', pos1);
                selRange.moveEnd('character', pos2);
                selRange.select();
            } else if (element.setSelectionRange) {
                element.setSelectionRange(pos1, pos2);
            } else if (element.selectionStart) {
                element.selectionStart = pos1;
                element.selectionEnd = pos2;
            }
            element.focus();
        }
    }

    select(element) {
        if (element)
        {
            element.select();
        }
    }

    getBoundingClientRect(element) {
        if (!element) return;

        const rect = JSON.parse(JSON.stringify(element.getBoundingClientRect()));

        rect.scrollY = window.scrollY || document.documentElement.scrollTop;
        rect.scrollX = window.scrollX || document.documentElement.scrollLeft;

        rect.windowHeight = window.innerHeight;
        rect.windowWidth = window.innerWidth;
        return rect;
    }

    changeCss (element, css) {
        if (element)
        {
            element.className = css;
        }
    }

    dispatchEvent (element, event) {
        if (element) {
            element.dispatchEvent(new Event(event, { bubbles: true }));
        }
    }

    removeEventListener (element, event, eventId) {
        element.removeEventListener(event, this.eventListeners[eventId]);
        delete this.eventListeners[eventId];
    }

    addDefaultPreventingHandler(element, eventName) {
        const listener = function(e) {
            e.preventDefault();
        }
        element.addEventListener(eventName, listener, { passive: false });
        this.eventListeners[++this.listenerId] = listener;
        return this.listenerId;
    }

    removeDefaultPreventingHandler(element, eventName, listenerId) {
        this.removeEventListener(element, eventName, listenerId);
    }

    addDefaultPreventingHandlers(element, eventNames) {
        const listeners = [];

        for (const eventName of eventNames) {
            const listenerId = this.addDefaultPreventingHandler(element, eventName);
            listeners.push(listenerId);
        }

        return listeners;
    }

    removeDefaultPreventingHandlers(element, eventNames, listenerIds) {
        for (const index = 0; index < eventNames.length; ++index) {
            const eventName = eventNames[index];
            const listenerId = listenerIds[index];
            this.removeDefaultPreventingHandler(element, eventName, listenerId);
        }
    }
};
window.mudElementRef = new MudElementReference();
