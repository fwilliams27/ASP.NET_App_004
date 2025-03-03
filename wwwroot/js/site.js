// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification 
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Wait for Blazor to start before initializing client-side scripts
document.addEventListener("DOMContentLoaded", function () {
    console.log("site.js loaded and DOM fully ready.");
    initializeMatrixEffect(); // Initialize Matrix digital rain effect
});

// Function to initialize the Matrix digital rain effect with retry mechanism
function initializeMatrixEffect() {
    function tryInitializeCanvas(attempts = 10, delay = 1000) {
        const canvas = document.getElementById('matrixCanvas');
        if (!canvas) {
            if (attempts <= 0) {
                console.error('Matrix canvas not found after multiple attempts!');
                return;
            }
            console.warn('Matrix canvas not found, retrying in ' + delay + 'ms... Attempts left: ' + attempts);
            setTimeout(() => tryInitializeCanvas(attempts - 1, delay), delay);
            return;
        }

        const ctx = canvas.getContext('2d');
        if (!ctx) {
            console.error('Failed to get 2D context for canvas!');
            return;
        }

        // Set initial canvas size
        function resizeCanvas() {
            canvas.height = window.innerHeight;
            canvas.width = window.innerWidth;
            console.log(`Canvas resized to ${canvas.width}x${canvas.height}`);
        }
        resizeCanvas();

        // Update canvas size on window resize
        window.addEventListener('resize', resizeCanvas);

        const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%^&*()_+-=[]{}|;:,.<>?';
        const fontSize = 14;
        let columns = canvas.width / fontSize;
        let drops = [];

        // Initialize drops array
        function initializeDrops() {
            columns = canvas.width / fontSize;
            drops = [];
            for (let x = 0; x < columns; x++) {
                drops[x] = 1;
            }
        }
        initializeDrops();

        function draw() {
            try {
                ctx.fillStyle = 'rgba(0, 0, 0, 0.05)';
                ctx.fillRect(0, 0, canvas.width, canvas.height);

                ctx.fillStyle = '#00FF66';
                ctx.font = fontSize + 'px Courier New';

                for (let i = 0; i < drops.length; i++) {
                    const text = chars.charAt(Math.floor(Math.random() * chars.length));
                    ctx.fillText(text, i * fontSize, drops[i] * fontSize);

                    if (drops[i] * fontSize > canvas.height && Math.random() > 0.975) {
                        drops[i] = 0;
                    }
                    drops[i]++;
                }
                console.log('Digital rain frame rendered');
            } catch (error) {
                console.error('Error in digital rain draw loop:', error);
            }
        }

        // Start the drawing loop
        setInterval(() => {
            try {
                draw();
            } catch (error) {
                console.error('Error starting digital rain draw loop:', error);
            }
        }, 33);
    }

    // Start the canvas initialization with retries
    tryInitializeCanvas();
}