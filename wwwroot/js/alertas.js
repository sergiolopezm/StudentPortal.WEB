window.StudentPortal = {
    mostrarAlerta: (mensaje) => {
        alert(mensaje);
    },

    showToast: (titulo, mensaje, tipo) => {
        // Si quisieras implementar toasts m�s elegantes 
        // podr�as usar una librer�a como Bootstrap Toasts
        // Por ahora usamos alerts b�sicos
        alert(`${titulo}: ${mensaje}`);
    }
};