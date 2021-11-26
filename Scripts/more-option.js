$(document).ready(function () {
    $("#matching-change-table").sortable();

    function note() {
        var count = document.querySelectorAll('#matching-option div span');
        for (var i = 0; i < count.length; i++) {
            count[i].innerHTML = i + 1;
        };
    }

    function note2() {
        var count = document.querySelectorAll('#poll-option td span');
        for (var i = 0; i < count.length; i++) {
            count[i].innerHTML = i + 1;
        };
    }

    $('#add-matching').click(function () {
        

        var txtLine = '<li id="matching-option" class="w-full flex">'+
            '<div class="w-1/12 my-2 p-2 font-bold"><span></span></div>'+
            '<input type="text" class="text-center my-2 py-2 w-5/12 border border-black-400 bg-blue-200" name="name" value="" placeholder="Question here..." />'+
            '<input type="text" class="text-center my-2 py-2 w-5/12 border border-black-400 bg-green-200" name="name" value="" placeholder="Answer here..." />'+
            '<i id="remove-option" class="far fa-times-circle my-3 p-2"></i>'+
            '</li>';
        $('#table-matching').append(txtLine);

        note();

        var remove = document.querySelectorAll('#remove-option');

        remove.forEach((item) => {
            item.addEventListener('click', function () {
                var list = document.querySelectorAll('#matching-option');

                for (var index = 0; index < list.length; index++) {
                    list[index].querySelector("#remove-option").addEventListener("click",
                        function () {
                            this.closest("#matching-option").remove();
                            note();
                        });
                }
                
            });
        });
    });

    

    $('.option-btn').click(function () {


        var txtLine2 = '<tr id="poll-option">' +
                       '<td><span></span></td>'+
                       '<td class="w-full"><input type="text" class="w-full p-3 mt-2" name="option2" placeholder="Option"></td>'+
                       '<td class="w-2"><i id="remove-option-poll" class="far fa-times-circle my-3 p-2"></i></td>'+
                       '</tr>';
        $('#table-poll').append(txtLine2);

        note2();

        var remove = document.querySelectorAll('#remove-option-poll');

        remove.forEach((item) => {
            item.addEventListener('click', function () {
                var list = document.querySelectorAll('#poll-option');

                for (var index = 0; index < list.length; index++) {
                    list[index].querySelector("#remove-option-poll").addEventListener("click",
                        function () {
                            this.closest("#poll-option").remove();
                            note2();
                        });
                }

            });
        });
    });


    
    
});