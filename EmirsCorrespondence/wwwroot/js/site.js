
    let dropzone = document.getElementById('dropzone');
    let fileInput = document.getElementById('fileInput');
    let fileList = document.getElementById('fileList');

    dropzone.addEventListener('click', () => fileInput.click());

    dropzone.addEventListener('dragover', (e) => {
        e.preventDefault();
        dropzone.style.border = "2px dashed #007bff";
    });

    dropzone.addEventListener('dragleave', () => {
        dropzone.style.border = "2px dashed #ccc";
    });

    dropzone.addEventListener('drop', (e) => {
        e.preventDefault();
        dropzone.style.border = "2px dashed #ccc";
        let files = e.dataTransfer.files;
        fileInput.files = files;

        for (let file of files) {
            let listItem = document.createElement('li');
            listItem.textContent = file.name;
            fileList.appendChild(listItem);
        }
    });



    .dropzone {
        border: 2px dashed #ccc;
        padding: 20px;
        text-align: center;
        cursor: pointer;
}


//Image Previews, File Size, and Cancel Upload Button
    document.getElementById("dropZone").addEventListener("click", () => {
        document.getElementById("fileInput").click();
    });

    document.getElementById("fileInput").addEventListener("change", function() {
        displayFiles(this.files);
    });

    document.getElementById("dropZone").addEventListener("dragover", function(e) {
        e.preventDefault();
    this.classList.add("dragging");
    });

    document.getElementById("dropZone").addEventListener("dragleave", function() {
        this.classList.remove("dragging");
    });

    document.getElementById("dropZone").addEventListener("drop", function(e) {
        e.preventDefault();
    this.classList.remove("dragging");
    document.getElementById("fileInput").files = e.dataTransfer.files;
    displayFiles(e.dataTransfer.files);
    });

    function displayFiles(files) {
        let fileList = document.getElementById("fileList");
    fileList.innerHTML = "";
        Array.from(files).forEach(file => {
        let listItem = document.createElement("li");
    listItem.textContent = file.name;
    fileList.appendChild(listItem);
        });
    }

    document.getElementById("uploadForm").addEventListener("submit", function(e) {
        e.preventDefault();
    uploadFiles();
    });

    function uploadFiles() {
        let files = document.getElementById("fileInput").files;
    if (files.length === 0) return alert("No files selected.");

    let formData = new FormData();
        Array.from(files).forEach(file => formData.append("files", file));

    let xhr = new XMLHttpRequest();
    xhr.open("POST", "/Document/BulkUpload", true);

    xhr.upload.onprogress = function(e) {
            if (e.lengthComputable) {
        let percentComplete = (e.loaded / e.total) * 100;
    document.getElementById("progressBar").style.width = percentComplete + "%";
            }
        };

    xhr.onload = function() {
            if (xhr.status === 200) {
        showNotification("Upload completed successfully!", "success");
            } else {
        showNotification("Upload failed. Please try again.", "error");
            }
        };

    xhr.onerror = function() {
        showNotification("An error occurred during the upload.", "error");
        };

    xhr.send(formData);
    }

    function showNotification(message, type) {
        let notification = document.getElementById("notification");
    notification.textContent = message;
    notification.className = type;
        setTimeout(() => {notification.textContent = ""; }, 5000);
}


var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.on("ReceiveNotification", function(message) {
    showNotification(message, "success");
});

connection.start().catch(function(err) {
    console.error(err.toString());
});



    document.getElementById("dropZone").addEventListener("click", () => {
        document.getElementById("fileInput").click();
    });

    document.getElementById("fileInput").addEventListener("change", function() {
        displayFiles(this.files);
    });

    function displayFiles(files) {
        let previewContainer = document.getElementById("previewContainer");
    let fileList = document.getElementById("fileList");
    previewContainer.innerHTML = "";
    fileList.innerHTML = "";

        Array.from(files).forEach(file => {
        let listItem = document.createElement("li");
    listItem.textContent = `${file.name} - ${formatBytes(file.size)}`;
            fileList.appendChild(listItem);

            if (file.type.startsWith("image/")) {
                let imgPreview = document.createElement("img");
                imgPreview.src = URL.createObjectURL(file);
                imgPreview.classList.add("img-preview");
                previewContainer.appendChild(imgPreview);
            }
        });
    }

    function formatBytes(bytes) {
        let sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
        if (bytes === 0) return '0 Byte';
        let i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
        return Math.round(bytes / Math.pow(1024, i), 2) + ' ' + sizes[i];
    }

    document.getElementById("uploadForm").addEventListener("submit", function(e) {
        e.preventDefault();
        uploadFiles();
    });

    document.getElementById("cancelUpload").addEventListener("click", function() {
        if (xhr) {
            xhr.abort();
            showNotification("Upload canceled.", "error");
            resetProgressBar();
        }
    });

    function uploadFiles() {
        let files = document.getElementById("fileInput").files;
        if (files.length === 0) return alert("No files selected.");

        let formData = new FormData();
        Array.from(files).forEach(file => formData.append("files", file));

        xhr = new XMLHttpRequest();
        xhr.open("POST", "/Document/BulkUpload", true);

        xhr.upload.onprogress = function(e) {
            if (e.lengthComputable) {
                let percentComplete = (e.loaded / e.total) * 100;
                document.getElementById("progressBar").style.width = percentComplete + "%";
                document.getElementById("cancelUpload").style.display = "inline";
            }
        };

        xhr.onload = function() {
            if (xhr.status === 200) {
                showNotification("Upload completed successfully!", "success");
            } else {
                showNotification("Upload failed. Please try again.", "error");
            }
            resetProgressBar();
        };

        xhr.onerror = function() {
            showNotification("An error occurred during the upload.", "error");
            resetProgressBar();
        };

        xhr.send(formData);
    }

    function showNotification(message, type) {
        let notification = document.getElementById("notification");
        notification.textContent = message;
        notification.className = type;
        setTimeout(() => { notification.textContent = ""; }, 5000);
    }

    function resetProgressBar() {
        document.getElementById("progressBar").style.width = "0%";
        document.getElementById("cancelUpload").style.display = "none";
}


//Advanced Filtering, Sorting, and Search for Documents
function filterDocuments() {
    const searchInput = document.getElementById("searchInput").value.toLowerCase();
    const categoryFilter = document.getElementById("categoryFilter").value;
    const documents = document.querySelectorAll("#documentList li");

    documents.forEach(doc => {
        const title = doc.querySelector(".doc-title").textContent.toLowerCase();
        const category = doc.dataset.category;

        if (title.includes(searchInput) && (categoryFilter === "" || category === categoryFilter)) {
            doc.style.display = "block";
        } else {
            doc.style.display = "none";
        }
    });

    function generateShareLink(docId, expiry) {
        fetch(`/Document/GenerateShareLink?documentId=${docId}&expiryHours=${expiry}`, { method: 'POST' })
            .then(res => res.json())
            .then(data => alert(`Share Link: ${data.link}`));
    }




