const button = document.querySelector("#menu-button");
const menu = document.querySelector("#menu");

button.addEventListener("click", () => {
  menu.classList.toggle("hidden");
});

// function shortened thanks to ChatGPT
function showWordsLeft() {
  var cheepTextAreaElement = document.getElementById("cheepTextArea");
  var charactersLeftElement = document.getElementById("charactersLeft");
  var cheepButtonElement = document.getElementById("cheepButton");
  var textFromTextArea = cheepTextAreaElement.value;

  cheepTextAreaElement.addEventListener("input", () => {
    if (textFromTextArea.length > 0) {
      cheepButtonElement.disabled = false;
    } else {
      cheepButtonElement.disabled = true;
    }
  });

  cheepTextAreaElement.addEventListener("change", () => {
    if (textFromTextArea.length > 0) {
      cheepButtonElement.disabled = false;
    } else {
      cheepButtonElement.disabled = true;
    }
  });

  charactersLeftElement.textContent =
    "Character left: " + (160 - textFromTextArea.length);
}

// TO UPLOAD profile pics
const inputDiv = document.getElementById("input-div");
const uploadButton = document.getElementById("upload-button");

inputDiv.addEventListener("dragover", (e) => {
  e.preventDefault();
  inputDiv.classList.add("border-blue-500");
});

inputDiv.addEventListener("dragleave", () => {
  inputDiv.classList.remove("border-blue-500");
});

inputDiv.addEventListener("drop", (e) => {
  e.preventDefault();
  inputDiv.classList.remove("border-blue-500");

  const files = e.dataTransfer.files;

  console.log("Dropped files:", files);
});

inputDiv.addEventListener("dragenter", (e) => {
  e.preventDefault();
});

const fileSelect = document.getElementById("fileSelect");
const fileElem = document.getElementById("fileElem");

fileSelect.addEventListener(
  "click",
  (e) => {
    if (fileElem) {
      fileElem.click();
    }
    const files = fileElem.files;
    uploadButton.classlist.remove("hidden");
    console.log("Selected files:", files);
  },
  false
);
