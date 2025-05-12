window.StudentPortal = {
    mostrarAlerta: (mensaje) => {
        alert(mensaje);
    },

    showToast: (titulo, mensaje, tipo) => {
        // Si quisieras implementar toasts más elegantes 
        // podrías usar una librería como Bootstrap Toasts
        // Por ahora usamos alerts básicos
        alert(`${titulo}: ${mensaje}`);
    }
};