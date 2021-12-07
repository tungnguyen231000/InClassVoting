const dragArea = document.querySelector(".drag-area");
const header = document.querySelector(".header"),
	btnBrowser = document.querySelector(".btn-image"),
	input = document.querySelector(".file-input");

let file;
//Thêm files và checkImage -new
let files;
let checkImage = false;


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


	//Thêm files -new
	files = event.dataTransfer.files;

	show(file);

	//Check ảnh -new
	if (checkImage == true) {
		input.files = event.dataTransfer.files;
	}
	if (checkImage == false) {
		$(".file-input").val(null);
	}
});

function show(file) {
	let validImage = ['image/jpeg', 'image/jpg', 'image/png'];
	if (validImage.includes(file.type)) {
		let fileReader = new FileReader();
		fileReader.onload = () => {
			let fileURL = fileReader.result;
			let imgTag = '<img src="' + fileURL + '" alt="">';
			dragArea.innerHTML = imgTag;


			//validate ảnh và vất vào preview - new
			if (file.size > 2 * 1024 * 1024) {
				if ($('.image-error').length == 0) {
					$(".file-input").after('<div class="image-error">*Image size exceeds 2MB</div>');
					$('.image-error').css("color", "red");
					$('.image-error').css("font-weight", "bold");
				}
				$(".file-input").val(null);
				checkImage = false;
			} else {
				$('.image-error').remove();
				input.files = files;
				checkImage = true;
			}

			if ($(".drag-area-preview").length != 0 && checkImage == true) {
				const dragArea2 = document.querySelector(".drag-area-preview");
				dragArea2.innerHTML = imgTag;
			} else {
				const dragArea2 = document.querySelector(".drag-area-preview");
				dragArea2.innerHTML = "";
			}



		}
		fileReader.readAsDataURL(file);
	} else {
		alert('This is not a Image File');
	}
}