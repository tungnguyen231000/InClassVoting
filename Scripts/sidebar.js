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
if ($('#matching-form')) {
    $("textarea").each(function () {
        this.setAttribute("style", "height:" + (this.scrollHeight) + "px;overflow-y:hidden;");
    }).on("input", function () {
        this.style.height = "auto";
        this.style.height = (this.scrollHeight) + "px";
    });
}


var addMatching = document.querySelectorAll('#add-matching');
var numberMatching = document.querySelectorAll('#number');
var letterMatching = document.querySelectorAll('#letter');
var tableSolution = document.querySelectorAll('#table-solution');
var removeMatching = document.querySelectorAll('#remove-matching');
addMatching.forEach((item) => {
    item.addEventListener("click", (e) => {
        var z = e.target;
        var parentDiv = z.parentNode;
        var a = document.querySelector('button[data-option="' + parentDiv.id + '"]');
        a.classList.remove("bg-gray-200");

        var index = Array.prototype.indexOf.call(addMatching, z);

        if (numberMatching[index].value != '' && letterMatching[index].value != '') {
            var txtLine = '<input type="text" id="matching-solution" class="text-center" name="solution" value="' + numberMatching[index].value + '-' + letterMatching[index].value + '" readonly/>';
            $(tableSolution[index]).append(txtLine);
            $(this).closest('#matching-option').find('#table-solution').append(txtLine);

        }

        var listSolution = $(tableSolution[index]).children();


        for (var z = 0; z < listSolution.length; z++) {
            listSolution[z].addEventListener('click', function (event) {

                $(removeMatching[index]).click(function (e) {
                    event.target.remove();
                });

            });
        };
        
    });
});

for (var i = 0; i < removeMatching.length; i++) {
    removeMatching[i].addEventListener("mouseup", (e) => {
        var z = e.target;
        var parentDiv = z.parentNode;
        var a = document.querySelector('button[data-option="' + parentDiv.id + '"]');

        var index = Array.prototype.indexOf.call(removeMatching, z);
        
        if (tableSolution[index].children.length == 1) {
            a.classList.add("bg-gray-200");
        } else {
            a.classList.remove("bg-gray-200");
        }
        
    });

} 
   


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


