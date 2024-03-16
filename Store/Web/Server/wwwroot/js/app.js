window.scrollToTop = () => {
    window.scrollTo({ top: 0, left: 0, behavior: 'smooth' });
};

function initScrollToTop() {
    var scrollToTop = document.querySelector("#scrollToTop");

    if (!scrollToTop) return;

    scrollToTop.addEventListener("click", ev => {
        ev.preventDefault();

        window.scrollToTop();
    });
}

initScrollToTop();

let fullNav = null;
let mobileNav = null;
let cartDisplay = null;

function updateCartDisplay() {
    fullNav = fullNav || document.querySelector("#fullNav");
    mobileNav = mobileNav || document.querySelector("#mobileNav");
    cartDisplay = cartDisplay || document.querySelector("#cartDisplay");

    let mql = window.matchMedia("(max-width: 992px)");
    if (mql.matches) {
        const cartDisplay0 = mobileNav.querySelector("#cartDisplay");
        if (!cartDisplay0) {
            mobileNav.innerHTML = "<!--!-->";
            mobileNav.appendChild(cartDisplay);
        }
    } else {
        const cartDisplay0 = fullNav.querySelector("#cartDisplay");
        if (!cartDisplay0) {
            fullNav.innerHTML = "<!--!-->";
            fullNav.appendChild(cartDisplay);
        }
    }
}

window.addEventListener("resize", ev => {
    updateCartDisplay();
});

setTimeout(() => {
    updateCartDisplay();
}, 2000);

const navbarOffCanvas = document.querySelector("#offcanvasNavbar");

function hideNavbarOffcanvas() {

    const offcanvas = bootstrap.Offcanvas.getInstance(navbarOffCanvas);
    if (!offcanvas) {
        return;
    }
    offcanvas.hide();
}

const navbarLinks = document.querySelectorAll("#offcanvasNavbar .nav-link, #offcanvasNavbar .dropdown-item")
for (let navbarLink of navbarLinks) {
    if (navbarLink.classList.contains("dropdown-toggle")) {
        continue;
    }

    navbarLink.addEventListener("click", hideNavbarOffcanvas)
}

Blazor.addEventListener('enhancedload', () => {
    window.scrollTo({ top: 0, left: 0, behavior: 'instant' });
});

window.changeUrl = function (url) {
    history.pushState(null, '', url);
}

window.getReferrer = function () {
    return document.referrer;
};