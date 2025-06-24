document.addEventListener('DOMContentLoaded', function() {
    // Create particles container
    const particlesContainer = document.createElement('div');
    particlesContainer.className = 'particles-container';
    document.querySelector('.admin-panel').appendChild(particlesContainer);

    // Create particles
    const particleCount = 15; // Reduced from 20
    const colors = [
        'rgba(100, 255, 218, 0.4)',
        'rgba(100, 255, 218, 0.2)',
        'rgba(100, 255, 218, 0.1)'
    ];

    for (let i = 0; i < particleCount; i++) {
        createParticle();
    }

    function createParticle() {
        const particle = document.createElement('div');
        particle.className = 'particle';
        
        // Random size between 1px and 3px
        const size = Math.random() * 2 + 1;
        
        // Random position
        const posX = Math.random() * 100;
        const posY = Math.random() * 100;
        
        // Random animation duration (slower and less movement)
        const duration = Math.random() * 30 + 15;
        const delay = Math.random() * -30;
        
        // Random color
        const color = colors[Math.floor(Math.random() * colors.length)];
        
        // Apply styles
        Object.assign(particle.style, {
            width: `${size}px`,
            height: `${size}px`,
            left: `${posX}%`,
            top: `${posY}%`,
            background: color,
            animation: `floatParticle ${duration}s ${delay}s infinite ease-in-out`,
            opacity: 0.7 // Make particles more subtle
        });
        
        particlesContainer.appendChild(particle);
    }

    // Add hover effect to cards
    const cards = document.querySelectorAll('.uk-card');
    cards.forEach(card => {
        card.addEventListener('mousemove', (e) => {
            const rect = card.getBoundingClientRect();
            const x = e.clientX - rect.left;
            const y = e.clientY - rect.top;
            
            const centerX = rect.width / 2;
            const centerY = rect.height / 2;
            
            const angleX = (y - centerY) / 20;
            const angleY = (centerX - x) / 20;
            
            card.style.transform = `perspective(1000px) rotateX(${angleX}deg) rotateY(${angleY}deg) scale(1.03) translateY(-12px)`;
        });
        
        card.addEventListener('mouseleave', () => {
            card.style.transform = 'perspective(1000px) rotateX(0) rotateY(0) scale(1) translateY(0)';
        });
    });
});
