window.StudentPortal = {
    mostrarAlerta: (mensaje) => {
        alert(mensaje);
    },

    showToast: (titulo, mensaje, tipo) => {
        // Crear un toast de Bootstrap
        const toastId = 'toast-' + new Date().getTime();
        const toastHTML = `
            <div id="${toastId}" class="toast align-items-center text-white bg-${tipo} border-0" role="alert" aria-live="assertive" aria-atomic="true">
                <div class="d-flex">
                    <div class="toast-body">
                        <strong>${titulo}</strong>: ${mensaje}
                    </div>
                    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Cerrar"></button>
                </div>
            </div>
        `;

        // Si no existe el contenedor de toasts, crearlo
        let toastContainer = document.getElementById('toast-container');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toast-container';
            toastContainer.className = 'position-fixed bottom-0 end-0 p-3';
            toastContainer.style.zIndex = '11';
            document.body.appendChild(toastContainer);
        }

        // Añadir el toast al contenedor
        toastContainer.innerHTML += toastHTML;

        // Inicializar y mostrar el toast
        const toastElement = document.getElementById(toastId);
        const bsToast = new bootstrap.Toast(toastElement, { autohide: true, delay: 5000 });
        bsToast.show();

        // Eliminar el toast del DOM cuando se oculte
        toastElement.addEventListener('hidden.bs.toast', () => {
            toastElement.remove();
        });
    },

    // Método específico para alertas en la interfaz
    mostrarAlertaUI: (tipo, titulo, mensaje) => {
        // Crear la estructura del alert
        const alertId = 'alert-' + new Date().getTime();
        const iconClass = tipo === 'danger' ? 'exclamation-triangle-fill' :
            tipo === 'success' ? 'check-circle-fill' :
                tipo === 'warning' ? 'exclamation-triangle-fill' : 'info-circle-fill';

        const alertHTML = `
            <div id="${alertId}" class="alert alert-${tipo} alert-dismissible fade show" role="alert">
                <i class="bi bi-${iconClass} me-2"></i>
                <strong>${titulo}</strong> ${mensaje}
                <button type="button" class="btn-close position-absolute top-0 end-0 mt-2 me-2" data-bs-dismiss="alert" aria-label="Cerrar"></button>
            </div>
        `;

        // Añadir el alert al contenedor proporcionado o crear uno temporal
        const alertContainer = document.getElementById('alert-container') || document.createElement('div');
        if (!document.getElementById('alert-container')) {
            alertContainer.id = 'alert-container-temp';
            document.body.insertBefore(alertContainer, document.body.firstChild);
        }

        alertContainer.innerHTML += alertHTML;

        // Auto-destruir después de un tiempo
        setTimeout(() => {
            const alertElement = document.getElementById(alertId);
            if (alertElement) {
                alertElement.classList.remove('show');
                setTimeout(() => alertElement.remove(), 150);
            }
        }, 5000);
    }
};