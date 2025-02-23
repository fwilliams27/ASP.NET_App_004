// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification 
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    console.log("site.js loaded and DOM fully ready.");
    initializeBootstrapModals(); // Initialize Bootstrap modals on DOM load
});

// Function to show Bootstrap modal
window.showBootstrapModal = (modalId) => {
    console.log("Opening modal: " + modalId);
    var modalElement = document.querySelector(modalId);

    if (!modalElement) {
        console.error("Modal not found in the DOM:", modalId);
        return;
    }

    try {
        // Bootstrap 5 way to initialize modal
        var modal = bootstrap.Modal.getOrCreateInstance(modalElement);
        modal.show();
    } catch (error) {
        console.error("Bootstrap modal error:", error);
    }
};

// Function to hide Bootstrap modal
window.hideBootstrapModal = (modalId) => {
    console.log("Closing modal: " + modalId);
    var modalElement = document.querySelector(modalId);

    if (!modalElement) {
        console.error("Modal not found in the DOM:", modalId);
        return;
    }

    try {
        var modal = bootstrap.Modal.getInstance(modalElement);
        if (modal) {
            modal.hide();
        }
    } catch (error) {
        console.error("Error closing Bootstrap modal:", error);
    }
};

// Function to initialize Bootstrap modals
function initializeBootstrapModals() {
    document.querySelectorAll('.modal').forEach(modal => {
        new bootstrap.Modal(modal); // Initialize each modal
    });
}