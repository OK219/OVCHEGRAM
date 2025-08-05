const userEdit = document.getElementById("userEdit");
const userInfo = document.getElementById("userInfo");
const editBtn = document.getElementById("editBtn");
const cancelBtn = document.getElementById("cancelBtn");


editBtn.addEventListener('click', async () => {
    userInfo.style.display = 'none';
    userEdit.style.display = 'block';
})

cancelBtn.addEventListener('click', async () => {
    userEdit.style.display = 'none';
    userInfo.style.display = 'block';
})
