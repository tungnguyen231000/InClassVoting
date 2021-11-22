//Check Student Checked Test
var x = document.querySelectorAll('input[type="checkbox"]');
x.forEach((item) => {
    item.addEventListener("change", () => {
        var z = item.getAttribute('id');
        var y = document.querySelectorAll('#'+z);
        var checked = 0;
        y.forEach((item2) => {
            if (item2.checked == true) {
                ++checked;
            }
        });

        if (checked > 0) {
            var a = document.querySelector('button[data-option="' + z + '"]');
            a.classList.remove("bg-gray-200");
        } else {
            var a = document.querySelector('button[data-option="' + z + '"]');
            a.classList.add("bg-gray-200");
        }

    });
});

//Check Student Input Test
var textbox = document.querySelectorAll('.txt');
textbox.forEach((item) => {
    item.addEventListener("keyup", () => {
        var z = item.getAttribute('id');
        var y = document.querySelector('#' + z);
        var vvv = $('#' + z).val().length;
        var checked = 0;
        if (vvv > 0) {
            ++checked;
        };

        if (checked > 0) {
            var a = document.querySelector('button[data-option="' + z + '"]');
            a.classList.remove("bg-gray-200");
        } else {
            var a = document.querySelector('button[data-option="' + z + '"]');
            a.classList.add("bg-gray-200");
        }

    });
});

//Check Student Change Matching Test
var tag = document.querySelectorAll('#matching-change-table li');
tag.forEach((item) => {
    item.addEventListener("mouseup", () => {
        var z = item.getAttribute('id');
        var a = document.querySelector('button[data-option="' + z + '"]');
        a.classList.remove("bg-gray-200");

    });
});


var fill1 = document.querySelectorAll('#fill-content input');
fill1.forEach((item) => {
    item.addEventListener("keyup", () => {
        var z = item.getAttribute('id');

        var vvv = item.value.length;
        var checked = 0;
        if (vvv > 0) {
            ++checked;
        };

        if (checked > 0) {
            var a = document.querySelector('button[data-option="' + z + '"]');
            a.classList.remove("bg-gray-200");
        } else {
            var a = document.querySelector('button[data-option="' + z + '"]');
            a.classList.add("bg-gray-200");
        }

    });
});

var fill2 = document.querySelectorAll('#fill-content select');
var checkfill = 0;
fill2.forEach((item) => {
    item.addEventListener("change", () => {
        var z = item.getAttribute('id');
        var a = document.querySelector('button[data-option="' + z + '"]');
        a.classList.remove("bg-gray-200");

        var y = document.querySelectorAll('#' + z);
        y.forEach((item2) => {
            if (item2.value != '') {
                checkfill = 0;
            } else {
                ++checkfill;
            }
            console.log(item2.value);
        });
        
        console.log(checkfill);

        if (checkfill >= y.length) {
            a.classList.add("bg-gray-200");
        }
    });
});




const link = document.querySelectorAll("#scroll-to");
link.forEach((item) => {
    item.addEventListener('click', () => {
        const element = document.getElementById(item.getAttribute('data-link'));
        element.scrollIntoView({ behavior: "smooth", block: "center" });
    })

});


