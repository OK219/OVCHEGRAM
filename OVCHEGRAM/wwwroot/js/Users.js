let page = 1;
const loading = document.getElementById('loading');
const filter = document.getElementById('filter');

async function loadUsers() {
    page++;
    const response = await fetch(`/ME/GetUsersHtml?page=${page}&filter=${filter.value}`);
    const html = await response.text();
    if (html.length < 10) {
        observer.unobserve(loading)
    }
    document.getElementById('items-container').insertAdjacentHTML('beforeend', html);
}
const observer = new IntersectionObserver(async (entries) => {
    if (entries[0].isIntersecting) {
        loadUsers();
    }
}, { threshold: 0.1 });

observer.observe(loading);

filter.addEventListener('input', function() {
    page = 0
    loading.style.display = 'none';
    document.querySelectorAll('.userLink').forEach(element => {
        element.remove();
    });
    setTimeout(() => {
        loading.style.display = 'block';
    }, 200)
    loadUsers();
    
})