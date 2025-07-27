// wwwroot/js/utils.js
window.getWindowWidth = () => {
    return window.innerWidth;
};


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