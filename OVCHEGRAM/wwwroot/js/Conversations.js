const userIds = new Set();
document.addEventListener('DOMContentLoaded', function () {
    const modal = document.getElementById('modalOverlay');
    const openBtn = document.getElementById('openModalBtn');
    const closeBtn = document.querySelector('.close-modal');
    const userInput = document.getElementById("user-input");
    const groupTitle = document.getElementById("group-Title");
    const userList = document.getElementById("user-list");
    const addedUsers = document.getElementById("addedUsers");
    const confirmBtn = document.getElementById("confirm-btn");
    const fileUpload = document.getElementById("fileLoad");
    confirmBtn.disabled = true;
    let userNames;

    confirmBtn.addEventListener('click', async (e) => {
        let file;
        if (fileUpload.files.length > 0) {
            file = fileUpload.files[0];
        }
        const formData = new FormData();
        formData.append('userIds', Array.from(userIds).map(Number));
        formData.append('groupTitle', groupTitle.value);
        if (fileUpload.files.length > 0) {
            file = fileUpload.files[0];
            formData.append('file', file);
        }

        const response = await fetch('/ME/CreateGroupChat', {
            method: 'POST',
            body: formData
        })
            .then(response => response.json())
            .then(data => window.location.href = data.redirectUrl);
        modal.style.display = 'none';
    });


    groupTitle.addEventListener('input', () => {
        confirmBtn.disabled = groupTitle.value <= 0;
    })

    openBtn.addEventListener('click', function () {
        modal.style.display = 'flex';
    });

    closeBtn.addEventListener('click', function () {
        modal.style.display = 'none';
    });

    modal.addEventListener('click', function (e) {
        if (e.target === modal) {
            modal.style.display = 'none';
        }
    });

    userInput.addEventListener('input', async () => {
        if (userInput.value.length > 0) {
            const response = await fetch(`/ME/GetUsersNamesHtml?page=${1}&filter=${userInput.value}`);
            userList.innerHTML = await response.text();
            userNames = document.querySelectorAll('.user');
            userNames.forEach(user => {
                user.addEventListener('click', () => {
                    userClick(user);
                })
            });
        } else {
            userList.innerHTML = '';
        }
    });

    function userClick(user) {
        const userId = user.querySelector('.userId').textContent;
        if (userIds.has(userId)) {
            userList.innerHTML = '';
            return;
        }
        const userName = user.querySelector('.userName').textContent;
        userIds.add(userId);
        userInput.value = '';
        userList.innerHTML = '';
        const profilePicSrc = user.querySelector('.profilePic').src;
        const userDiv = document.createElement('div');
        userDiv.id = `addedUser${userId}`;
        userDiv.innerHTML = `
        <img class="profilePic" src="${profilePicSrc}" alt="oi"/>
        <p>${userName}</p>
        <span id="deleteUser${userId}">&times;</span>`;
        addedUsers.appendChild(userDiv);
        document.getElementById(`deleteUser${userId}`).addEventListener('click', () => {
            userDelete(userId);
        })
        console.log(userIds);
    }

    function userDelete(userId) {
        userIds.delete(userId);
        document.getElementById(`addedUser${userId}`).remove();
        console.log(userIds);
    }
});