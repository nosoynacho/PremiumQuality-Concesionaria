// Función para mostrar notificación de bienvenida en la primera visita
export function showWelcomeNotification() {
    const hasVisited = localStorage.getItem('hasVisitedBefore');
    
    if (!hasVisited) {
        // Primera visita del usuario
        localStorage.setItem('hasVisitedBefore', 'true');
        
        // Mostrar notificación
        const notification = document.createElement('div');
        notification.className = 'welcome-notification';
        notification.innerHTML = `
            <div class="alert alert-success alert-dismissible fade show" role="alert">
                <h4 class="alert-heading">¡Bienvenido!</h4>
                <p>Nos alegra mucho verte por primera vez en nuestra aplicación.</p>
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;
        
        document.body.insertAdjacentElement('afterbegin', notification);
        
        // Eliminar automáticamente después de 5 segundos
        setTimeout(() => {
            const alert = notification.querySelector('.alert');
            if (alert) {
                alert.classList.remove('show');
                setTimeout(() => notification.remove(), 150);
            }
        }, 5000);
    }
}
