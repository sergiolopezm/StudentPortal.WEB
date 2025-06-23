window.scrollToTop = function () {
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
};

window.scrollToElement = function (elementId) {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollIntoView({ 
            behavior: 'smooth', 
            block: 'start'
        });
    }
};

// Interceptar errores HTTP
window.addEventListener('error', function (e) {
    // Capturar errores de red y de recursos
    console.error('Error capturado por el manejador global:', e);
    return false; // No mostrar el mensaje de error nativo del navegador
});