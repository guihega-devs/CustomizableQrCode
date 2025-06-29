document.addEventListener('DOMContentLoaded', function () {
    var burger = document.getElementById('burgerBtn');
    var navMenu = document.getElementById('navMenu');

    function closeMenu() {
        navMenu.classList.remove('open');
        document.body.classList.remove('menu-open');
    }

    burger.addEventListener('click', function () {
        // Alternar clase 'open' en el menú
        navMenu.classList.toggle('open');
        const opened = navMenu.classList.contains('open');

        // Actualizar body y atributos aria
        document.body.classList.toggle('menu-open', opened);
        burger.setAttribute('aria-expanded', opened ? 'true' : 'false');

        if (opened) {
            // Focus en el primer link del menú solo al abrir
            setTimeout(() => navMenu.querySelector('a')?.focus(), 180);
        }
    });

    // Cerrar al hacer click en un link
    navMenu.querySelectorAll('a').forEach(function (link) {
        link.addEventListener('click', closeMenu);
    });

    // Cerrar al tocar fuera del menú
    document.addEventListener('click', function (e) {
        if (navMenu.classList.contains('open') && !navMenu.contains(e.target) && e.target !== burger) {
            closeMenu();
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape' && navMenu.classList.contains('open')) {
            closeMenu();
        }
    });
});

// wwwroot/js/site.js
window.restartRadialPulse = function (element) {
    console.log(element);
    if (!element) return;
    element.classList.remove('pulse');
    // Forzar reflow para reiniciar animación
    void element.offsetWidth;
    element.classList.add('pulse');
}