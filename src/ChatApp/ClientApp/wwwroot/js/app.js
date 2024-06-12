
window.blazorCulture = {
    get: () => window.localStorage['BlazorCulture'],
    set: (value) => window.localStorage['BlazorCulture'] = value
};

window.helpers = {
    scrollIntoView: function (id) {
        const element = document.getElementById(id);
        if (element) {
            element.scrollIntoView({
                behavior: 'smooth'
            });
        }
    },
    scrollToBottom: function () {
        return new Promise((resolve, reject) => {
            window.scrollTo({ left: 0, top: document.body.scrollHeight, behavior: "smooth" });
            resolve();
        });
    },
    attachScrollEventHandler: function (objRef) {
        window.addEventListener("scroll", () => {
            return objRef.invokeMethodAsync('OnScroll', { X: window.scrollX, Y: window.scrollY });
        });
    }
}

window.isDarkMode = () => {
    if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
        return true;
    }
    return false;
};

window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', async event => {
    await DotNet.invokeMethodAsync("ClientApp", "OnDarkModeChanged", event.matches);
});

function splashscreen() {
    let preferredColorScheme = JSON.parse(window.localStorage["preferredColorScheme"] ?? "null");
    let colorScheme = preferredColorScheme ?? (window.isDarkMode() ? 1 : 0);

    if (colorScheme == 1) {
        const elem = document.getElementById("splashscreen");
        elem.classList.toggle("dark");
    }
}

splashscreen();