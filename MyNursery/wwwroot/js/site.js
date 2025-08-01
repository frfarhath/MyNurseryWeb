/**
 * Checks if navigator is online before form submit.
 * Shows offline validation message if offline.
 * @returns {boolean} true if online; false otherwise to block submit.
 */
function checkInternetBeforeSubmit() {
    if (!navigator.onLine) {
        showOfflineValidationMessage();
        return false;  // prevent form submission
    }
    return true;
}

/**
 * Shows offline validation alert.
 * Assumes there is a div with id 'offline-message' in the page.
 */
function showOfflineValidationMessage() {
    const msgDiv = document.getElementById('offline-message');
    if (!msgDiv || !msgDiv.classList.contains('d-none')) return; // no div or already visible

    msgDiv.classList.remove('d-none');

    // Hide message after 4 seconds
    setTimeout(() => {
        msgDiv.classList.add('d-none');
    }, 4000);
}
