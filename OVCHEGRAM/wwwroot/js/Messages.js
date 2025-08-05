const urlParams = new URLSearchParams(window.location.search);
const id = urlParams.get("conversationId");
const oldMessageLoading= document.getElementById('message-old-load');
const uploadFile = document.getElementById('uploadFile');
const modalSendBtn = document.getElementById('modalSendMessageBtn');
const imgPreview = document.getElementById('imgPreview');
const modalWindow = document.getElementById('modalOverlay');
const modalInput = document.getElementById('modalInput');

let file;

document.addEventListener('DOMContentLoaded', function () {
    const showUsers = document.getElementById('showUsers');
    const users = document.getElementById('users');
    function scrollDown() {
        messageField.scrollTop = messageField.scrollHeight;
    }
    
    const messageField = document.getElementById("message-field");
    scrollDown();
    let lastNewMessageId = undefined;
    let lastOldMessageId = undefined;
    let isLoading = false;
    
    document.getElementById("send-form").addEventListener("submit", function (e) {
        e.preventDefault();
    });

    const input = document.getElementById("message-input");
    const sendBtt = document.getElementById("send-message-btt");

    input.addEventListener("input", () => {
        sendBtt.disabled = input.value.length <= 0 || input.value.length >= 1000;
    });

    sendBtt.addEventListener("click", async (e) => {
        e.preventDefault();
        const formData = new FormData();
        formData.append('conversationId', id);
        formData.append('message', input.value);
        const response = fetch('/Message/SendMessage', {
            method: 'POST',
            body: formData
        }).then(r => {
            input.value = "";
            sendBtt.disabled = true;
            loadMoreMessages().then(r => scrollDown());
        });
    });

    oldMessageLoading.style.display = 'block';
    
    const oldMessageObserver = new IntersectionObserver(async (entries) => {
        if (entries[0].isIntersecting) {
            await loadMoreMessages(true);
        }

    }, {
        threshold: 0.01,
    });

    setTimeout(() => {
        oldMessageObserver.observe(oldMessageLoading);
    }, 2000)
    

    async function loadMoreMessages(takeOld = false) {
        if (isLoading) return;
        isLoading = true;

        try {
            const newDateElements = document.querySelectorAll('.messageId');
            if (newDateElements.length > 0) {
                lastNewMessageId = newDateElements[newDateElements.length - 1].textContent;
                lastOldMessageId = newDateElements[0].textContent;
            }
            const response = await fetch(`/Message/LoadMessages?conversationId=${id}&lastMessageId=${takeOld ? lastOldMessageId || "" : lastNewMessageId || ""}&takeOld=${takeOld}`);
            const html = await response.text();
            if (html.length < 10 && takeOld) {
                oldMessageObserver.unobserve(oldMessageLoading);
            }
            if (!takeOld) {
                document.getElementById('message-field').insertAdjacentHTML('beforeend', html);
            } else {
                document.getElementById("message-old-load").remove();
                document.getElementById('message-field').insertAdjacentHTML('afterbegin', html);
                document.getElementById('message-field').insertAdjacentHTML('afterbegin', '<p id="message-old-load"></p>');
                oldMessageObserver.observe(document.getElementById("message-old-load"));
            }
        } finally {
            isLoading = false;
        }
    }

    setInterval(loadMoreMessages, 3000);

    modalSendBtn.addEventListener('click', async (e) => {
        e.preventDefault();
        const formData = new FormData();
        formData.append('conversationId', id);
        formData.append('message', modalInput.value);
        formData.append('file', file);
        formData.append('isImage', file.type.startsWith('image/'));

        const response = await fetch('/Message/SendMessage', {
            method: 'POST',
            body: formData
        });

        modalWindow.style.display = 'none';
        loadMoreMessages().then(r => scrollDown());
    });

    const closeBtn = document.getElementById('close-modal');
    closeBtn.addEventListener('click', function () {
        modalWindow.style.display = 'none';
        input.value = modalInput.value;
        modalInput.value = '';
    });
    
    uploadFile.addEventListener('change', async (e) => {
        file = e.target.files[0];
        let isImage;
        if (!file) return;
        if (!file.type.startsWith('image/')) {
            imgPreview.innerHTML = `<p>${file.name}</p>`;
        } else {
            const reader = new FileReader();
            reader.onload = function(event) {
                const newImg = document.createElement('img');
                newImg.id = 'imgPreview';
                newImg.src = event.target.result;
                imgPreview.innerHTML = '';
                imgPreview.appendChild(newImg);
            };

            reader.readAsDataURL(file);
        }
        
        
        modalWindow.style.display = 'block';
        const text = input.value;
        input.value = '';
        modalInput.value = text;
    })
    
    showUsers.addEventListener('click', async () => {
        if (users.style.display === 'block') {
            users.style.display = 'none';
        } else {
            users.style.display = 'block';
        }
    })
});