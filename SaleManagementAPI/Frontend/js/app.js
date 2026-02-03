const API_URL = "https://localhost:7150/api/auth";

//Function register
document.getElementById('register-form')?.addEventListener('submit',
    async function (e) {
        e.preventDefault();

        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;

        const response = await fetch(`${API_URL}/register`,
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password })
            }
        );

        const data = await response.json();

        if (response.ok) {
            window.location.href = 'index.html';
        } else {
            document.getElementById('error-message').textContent = data.message || 'Error';
            document.getElementById('error-message').style.display = 'block';
        }

    })