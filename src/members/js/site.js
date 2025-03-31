'use strict';

async function fetchUserInfo() {
    // Get DOM elements once at the beginning
    const nameElement = document.getElementById('name');
    const emailElement = document.getElementById('email');
    const userIdElement = document.getElementById('userId');
    
    try {
        const response = await fetch('/api/userinfo');
        
        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        
        const userDetails = await response.json();

        console.log('User details:', userDetails);

        // Update elements using the saved references
        nameElement.value = userDetails.name || 'N/A';
        emailElement.value = userDetails.email || 'N/A';
        userIdElement.value = userDetails.userId || 'N/A';

    } catch (error) {
        console.error('Error fetching user information:', error);
        
        // Update elements in case of error using the saved references
        nameElement.value = 'Error loading user data';
        emailElement.value = '';
        userIdElement.value = '';
    }
}

// Wait for DOM to be ready before executing
document.addEventListener('DOMContentLoaded', fetchUserInfo);
