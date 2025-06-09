window.addEventListener('scroll', () => {
    const navbar = document.querySelector('.bg-navbar');
    if (window.scrollY > 50) {
        navbar.classList.add('scrolled');
    } else {
        navbar.classList.remove('scrolled');
    }
});
