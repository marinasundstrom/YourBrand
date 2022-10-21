const dropdownToggle = ".dropdown-toggle";
const dropdownMenu = ".dropdown-menu";
const showClass = "show";

window.initDropDown = function () {
    function hoverfunc() {

        var bigger = window.matchMedia("(min-width: 768px)").matches;

        Array.from(document.querySelectorAll(".navbar .dropdown")).forEach(dropdown => {

            if (bigger) {
                dropdown
                    .querySelector(dropdownToggle)
                    .setAttribute("data-bs-toggle", "");

                // Show menu when hovering
                dropdown.addEventListener("mouseover",
                    function onMouseover() {
                        dropdown.classList.add(showClass);
                        dropdown.querySelector(dropdownToggle).setAttribute("aria-expanded", "true");
                        dropdown.querySelector(dropdownMenu).classList.add(showClass);
                    });

                // Hide menu when leaving
                dropdown.addEventListener("mouseout",
                    function onMouseout() {
                        dropdown.classList.remove(showClass);
                        dropdown.querySelector(dropdownToggle).setAttribute("aria-expanded", "false");
                        dropdown.querySelector(dropdownMenu).classList.remove(showClass);
                    }
                );

                // Close menu when items are being clicked
                Array.from(dropdown.querySelectorAll(".dropdown-item")).forEach(dropdownItem => {
                    dropdownItem.addEventListener("click",
                        function () {
                            dropdown.querySelector(dropdownMenu).classList.remove(showClass);
                        });
                });
            } else {
                dropdown
                    .querySelector(dropdownToggle)
                    .setAttribute("data-bs-toggle", "dropdown");

                //dropdown.removeEventListener("onMouseover");
                //dropdown.removeEventListener("onMouseout");
            }
        });
    }

    window.addEventListener("load", hoverfunc);
    window.addEventListener("resize", hoverfunc);

    hoverfunc();
};