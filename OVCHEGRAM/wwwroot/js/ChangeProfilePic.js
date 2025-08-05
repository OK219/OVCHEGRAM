const fileUpload = document.getElementById("fileLoad");
fileUpload.addEventListener('change', (e) => {
    const imagePreview = document.getElementById("profileEditPic");
    const file = e.target.files[0];
    if (!file) return;
    if (!file.type.startsWith('image/')) {
        alert('Нужна картинка');
        return;
    }
    const reader = new FileReader();
    reader.onload = function(event) {
        const newImg = document.createElement('img');
        newImg.id = 'imgPreview';
        newImg.src = event.target.result;
        imagePreview.innerHTML = '';
        imagePreview.appendChild(newImg);
    };

    reader.readAsDataURL(file);
})