const dragArea = document.querySelector(".drag-area");
const header = document.querySelector(".header"),
	btnBrowser = document.querySelector(".btn-image"),
	input = document.querySelector(".file-input");

let file;


/*
btn.onclick = () => {
	input.click();
};
*/

input.addEventListener("change", function () {
	file = this.files[0];
	show(file);
});


// di chuyển bên trong
dragArea.addEventListener("dragover", (event) => {
	event.preventDefault();
	dragArea.classList.add('active');
	header.textContent = 'Release to Upload File';
});

// di chuyển bên ngoài
dragArea.addEventListener("dragleave", () => {
	dragArea.classList.remove('active');
	header.textContent = 'Drag and Drop to Upload File';
});

// thả bên trong
dragArea.addEventListener("drop", (event) => {
	event.preventDefault();
	file = event.dataTransfer.files[0];

	input.files = event.dataTransfer.files;

	show(file);
});

function show(file) {
	let validImage = ['image/jpeg', 'image/jpg', 'image/png'];
	if (validImage.includes(file.type)) {
		let fileReader = new FileReader();
		fileReader.onload = () => {
			let fileURL = fileReader.result;
			let imgTag = '<img src="' + fileURL + '" alt="">';
			dragArea.innerHTML = imgTag;
		}
		fileReader.readAsDataURL(file);
	} else {
		alert('This is not a Image File');
	}
}