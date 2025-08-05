document.addEventListener('DOMContentLoaded', async function() {
    fetch('/cities.json')
        .then(response => response.json())
        .then(cities => {
            const select = document.getElementById("citySelect");

            cities.forEach(city => {
                const option = document.createElement("option");
                option.value = city;
                option.textContent = city;
                select.appendChild(option);
            });
        })
        .catch(error => console.error('Ошибка загрузки городов:', error));
});