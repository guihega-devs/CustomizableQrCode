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

window.getWindowWidth = () => window.innerWidth;


window.downloadFileFromUrl = (filename, url) => {
    var link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};


/*Descarga sin librerias externas */
window.downloadSvgNative = function (svgString, filename) {
    const blob = new Blob([svgString], { type: "image/svg+xml" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    setTimeout(() => {
        document.body.removeChild(a);
        URL.revokeObjectURL(url);
    }, 100);
}


window.svgStringToPng = function (svgString, width, height, filename) {
    const svgBlob = new Blob([svgString], { type: "image/svg+xml" });
    const url = URL.createObjectURL(svgBlob);
    const img = new Image();
    img.onload = function () {
        const canvas = document.createElement("canvas");
        canvas.width = width || img.width;
        canvas.height = height || img.height;
        const ctx = canvas.getContext("2d");
        ctx.drawImage(img, 0, 0, canvas.width, canvas.height);

        // Descarga el PNG
        canvas.toBlob(function (blob) {
            const url2 = URL.createObjectURL(blob);
            const a = document.createElement("a");
            a.href = url2;
            a.download = filename || "codigoQR.png";
            document.body.appendChild(a);
            a.click();
            setTimeout(() => {
                document.body.removeChild(a);
                URL.revokeObjectURL(url2);
            }, 100);
        }, "image/png");
        URL.revokeObjectURL(url);
    };
    img.onerror = function (e) {
        alert("No se pudo convertir SVG a PNG");
        URL.revokeObjectURL(url);
    };
    img.src = url;
};

// Función principal para exportar QR como PDF
window.exportQrAsPdf = async function (svgString, width, height, filename) {
    // 1. Convierte SVG a PNG DataURL con tamaño exacto
    const pngDataUrl = await window.svgStringToPngDataUrl(svgString, width, height);

    // 2. Usa jsPDF (asegúrate de tener jsPDF cargado)
    const { jsPDF } = window.jspdf || {};
    if (!jsPDF) { alert("jsPDF no está cargado"); return; }
    // Ajusta el tamaño de la hoja exactamente al del PNG (en puntos: 1 px ≈ 0.75 pt)
    // Si quieres A4, ajusta formato, si quieres tamaño personalizado, usa [w, h]
    const pdfWidthPt = width * 0.75;
    const pdfHeightPt = height * 0.75;

    const pdf = new jsPDF({
        orientation: pdfWidthPt > pdfHeightPt ? "landscape" : "portrait",
        unit: "pt",
        format: [pdfWidthPt, pdfHeightPt]
    });

    // 3. Agrega la imagen ocupando toda la hoja
    pdf.addImage(pngDataUrl, "PNG", 0, 0, pdfWidthPt, pdfHeightPt);

    // 4. Descarga el PDF
    pdf.save(filename);
};

window.svgStringToPngDataUrl = function (svgString, width, height) {
    return new Promise(function (resolve, reject) {
        const svgBlob = new Blob([svgString], { type: "image/svg+xml" });
        const url = URL.createObjectURL(svgBlob);
        const img = new Image();
        img.onload = function () {
            const canvas = document.createElement("canvas");
            canvas.width = width;
            canvas.height = height;
            const ctx = canvas.getContext("2d");
            ctx.drawImage(img, 0, 0, width, height);
            resolve(canvas.toDataURL("image/png"));
            URL.revokeObjectURL(url);
        };
        img.onerror = function (e) {
            reject("No se pudo convertir SVG a PNG");
            URL.revokeObjectURL(url);
        };
        img.src = url;
    });
};

window.resizeListeners = [];

// Permite a C# suscribirse al evento resize
window.subscribeResize = (dotNetObjRef) => {
    console.log('UpdateScreenWidth');
    function onResize() {
        dotNetObjRef.invokeMethodAsync('UpdateScreenWidth', window.innerWidth);
    }
    console.log(window.innerWidth); 
    window.addEventListener('resize', onResize);

    // Guarda para posibles limpiezas futuras
    window.resizeListeners.push({ ref: dotNetObjRef, handler: onResize });
};
