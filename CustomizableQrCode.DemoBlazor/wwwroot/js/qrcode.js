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


// Descargar SVG como PNG usando canvg
window.downloadSvgAsPng = function (svgText, fileName) {
    const canvas = document.createElement('canvas');
    canvas.width = 1024;
    canvas.height = 1024;
    canvg(canvas, svgText); // o window.Canvg.fromString para versiones modernas
    setTimeout(function () {
        const pngUrl = canvas.toDataURL("image/png");
        const a = document.createElement('a');
        a.href = pngUrl;
        a.download = fileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    }, 100);
};


// Convierte SVG a PNG usando canvg v2 (alta resolución)
// Mejor versión: retorna Promise<string> con el PNG base64
//window.generateQrPngFromSvg = function (svgString, width, height) {
//    return new Promise(function (resolve, reject) {
//        try {
//            // Normaliza el ancho y alto en el SVG
//            var parser = new DOMParser();
//            var svgDoc = parser.parseFromString(svgString, "image/svg+xml");
//            var svgElem = svgDoc.documentElement;
//            if (width && height) {
//                svgElem.setAttribute("width", width);
//                svgElem.setAttribute("height", height);
//            }
//            var serializer = new XMLSerializer();
//            var normalizedSvg = serializer.serializeToString(svgElem);

//            var canvas = document.createElement('canvas');
//            canvas.width = width;
//            canvas.height = height;
//            var ctx = canvas.getContext('2d');

//            window.canvg(canvas, normalizedSvg, {
//                ignoreMouse: true,
//                ignoreAnimation: true,
//                renderCallback: function () {
//                    var png = canvas.toDataURL("image/png");
//                    resolve(png);
//                }
//            });
//        } catch (e) {
//            reject(e);
//        }
//    });
//};

//// Descarga un PDF con la imagen PNG del QR ocupando toda la hoja
//window.downloadQrPdf = function (pngDataUrl, filename) {
//    const { jsPDF } = window.jspdf || {};
//    if (!jsPDF) { alert("jsPDF no está cargado"); return; }

//    // Usa formato A4 en puntos
//    const pdf = new jsPDF({ orientation: "portrait", unit: "pt", format: "a4" });
//    const pageWidth = pdf.internal.pageSize.getWidth();
//    const pageHeight = pdf.internal.pageSize.getHeight();

//    // Ocupa toda la hoja (ajusta si es necesario)
//    pdf.addImage(pngDataUrl, "PNG", 0, 0, pageWidth, pageHeight);
//    pdf.save(filename);
//};




//// Convierte SVG string a PNG DataURL (usado para PNG y PDF)
//window.generateQrPngFromSvg = async function (svgString) {
//    return new Promise((resolve, reject) => {
//        const img = new Image();
//        img.crossOrigin = "anonymous";
//        img.onload = function () {
//            const canvas = document.createElement('canvas');
//            canvas.width = img.width;
//            canvas.height = img.height;
//            const ctx = canvas.getContext('2d');
//            ctx.drawImage(img, 0, 0);
//            resolve(canvas.toDataURL("image/png"));
//        };
//        img.onerror = function (e) {
//            reject("Error generando PNG desde SVG: " + e);
//        };
//        // SVG string -> base64 DataURL
//        const svgBase64 = 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent(svgString)));
//        img.src = svgBase64;
//    });
//};

//// Descarga PDF con imagen PNG dentro (usa jsPDF)
//window.downloadQrPdf = function (pngDataUrl, filename) {
//    // Usa jsPDF v2+
//    const { jsPDF } = window.jspdf || {};
//    if (!jsPDF) { alert("jsPDF no está cargado"); return; }
//    const pdf = new jsPDF({ orientation: "portrait", unit: "pt", format: "a4" });
//    // Centra la imagen en la hoja
//    const imgProps = pdf.getImageProperties(pngDataUrl);
//    const pageWidth = pdf.internal.pageSize.getWidth();
//    const pageHeight = pdf.internal.pageSize.getHeight();
//    const imgWidth = Math.min(imgProps.width, 300);
//    const imgHeight = imgProps.height * (imgWidth / imgProps.width);
//    const x = (pageWidth - imgWidth) / 2;
//    const y = (pageHeight - imgHeight) / 2;
//    pdf.addImage(pngDataUrl, "PNG", x, y, imgWidth, imgHeight);
//    pdf.save(filename);
//};


// 1. SVG to PNG base64
//window.svgToPngDataUrl = async function (svgString, width = 2048, height = 2048) {
//    return new Promise(function (resolve) {
//        var canvas = document.createElement('canvas');
//        canvas.width = width;
//        canvas.height = height;
//        var ctx = canvas.getContext('2d');

//        // ⚠️ canvg 2.x es una función, NO un objeto ni tiene fromString
//        window.canvg(canvas, svgString, {
//            ignoreMouse: true,
//            ignoreAnimation: true,
//            ignoreClear: true,
//            renderCallback: function () {
//                resolve(canvas.toDataURL("image/png"));
//            }
//        });
//    });
//};

//// 2. PNG to PDF
//window.pngToPdf = function (pngDataUrl, filename) {
//    const { jsPDF } = window.jspdf;
//    const pdf = new jsPDF({ orientation: "portrait", unit: "pt", format: "a4" });
//    const pageWidth = pdf.internal.pageSize.getWidth();
//    const pageHeight = pdf.internal.pageSize.getHeight();
//    const size = Math.min(pageWidth, pageHeight);
//    const x = (pageWidth - size) / 2;
//    const y = (pageHeight - size) / 2;
//    pdf.addImage(pngDataUrl, "PNG", x, y, size, size);
//    pdf.save(filename);
//}


// Convierte SVG string a PNG DataURL (usado para PNG y PDF)
window.generateQrPngFromSvg = async function (svgString) {
    return new Promise((resolve, reject) => {
        const img = new Image();
        img.crossOrigin = "anonymous";
        img.onload = function () {
            const canvas = document.createElement('canvas');
            canvas.width = img.width;
            canvas.height = img.height;
            //canvas.width = 2048;
            //canvas.height = 2048;
            const ctx = canvas.getContext('2d');
            ctx.drawImage(img, 0, 0);
            resolve(canvas.toDataURL("image/png"));
        };
        img.onerror = function (e) {
            reject("Error generando PNG desde SVG: " + e);
        };
        // SVG string -> base64 DataURL
        const svgBase64 = 'data:image/svg+xml;base64,' + btoa(unescape(encodeURIComponent(svgString)));
        img.src = svgBase64;
    });
};

// Descarga PDF con imagen PNG dentro (usa jsPDF)
window.downloadQrPdf = function (pngDataUrl, filename) {
    // Usa jsPDF v2+
    const { jsPDF } = window.jspdf || {};
    if (!jsPDF) { alert("jsPDF no está cargado"); return; }
    const pdf = new jsPDF({ orientation: "portrait", unit: "pt", format: "a4" });
    // Centra la imagen en la hoja
    const imgProps = pdf.getImageProperties(pngDataUrl);
    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();
    const imgWidth = Math.min(imgProps.width, 300);
    const imgHeight = imgProps.height * (imgWidth / imgProps.width);
    const x = (pageWidth - imgWidth) / 2;
    const y = (pageHeight - imgHeight) / 2;
    pdf.addImage(pngDataUrl, "PNG", x, y, imgWidth, imgHeight);
    pdf.save(filename);
};