$(document).ready(function () {

    //===============Các hàm đánh số thứ tự====================


    function note2() {
        var count = document.querySelectorAll('#poll-option td span');
        for (var i = 0; i < count.length; i++) {
            count[i].innerHTML = i + 1;
        };
    }

    function note3() {
        var count = document.querySelectorAll('#reading-note');
        for (var i = 0; i < count.length; i++) {
            var x = (i + 1);
            count[i].innerHTML = 'Question ' + x + ':';
        };
    }

    //===============Phần Matching====================
    if ($('#matching-form').length != 0) {
        $("#matching-left").height(300);
        $("#matching-right").height(300);

        $('#previewMatching').click(function () {

            $("#matching-left-preview").val($("#matching-left").val());
            $("#matching-left-preview").height(300);

            $("#matching-right-preview").val($("#matching-right").val());
            $("#matching-right-preview").height(300);

            $("#table-solution-preview").html($("#table-solution").html());
        });
    }

    var matchingleft = document.querySelectorAll("#matching-left");
    var matchingright = document.querySelectorAll("#matching-right");
    matchingleft.forEach((item) => { $(item).height(300); });
    matchingright.forEach((item) => { $(item).height(300); });

    if ($("#matching-test").length == 0) {
    var addMatching = document.querySelectorAll('#add-matching');
    var numberMatching = document.querySelectorAll('#number');
    var letterMatching = document.querySelectorAll('#letter');
    var tableSolution = document.querySelectorAll('#table-solution');
    var removeMatching = document.querySelectorAll('#remove-matching');
    addMatching.forEach((item) => {
        $(item).click(function (e) {
            var z = e.target;

            var index = Array.prototype.indexOf.call(addMatching, z);

            if (numberMatching[index].value != '' && letterMatching[index].value != '') {
                var txtLine = '<input type="text" id="matching-solution" class="text-center" name="solution" value="' + numberMatching[index].value + '-' + letterMatching[index].value + '" readonly/>';

                $(tableSolution[index]).append(txtLine);

                if ($("#table-solution-preview").length != 0) {
                    $("#table-solution-preview").append(txtLine);
                }
            }

           

            var listSolution = $(tableSolution[index]).children();


            for (var z = 0; z < listSolution.length; z++) {
                listSolution[z].addEventListener('click', function (event) {

                    $(removeMatching[index]).click(function (e) {
                        event.target.remove();
                        if ($("#table-solution-preview").length != 0) {
                            var listSolution = $("#table-solution-preview").children();
                            $(listSolution[z]).remove();
                        }
                    });

                });
            };

        });
    });
    }

    //===============Phần Poll====================
    $('.option-btn').click(function () {

        var txtLine2 = '<tr id="poll-option">' +
            '<td><span></span></td>' +
            '<td class="w-full"><input type="text" id="txtPoll" class="w-full p-3 mt-2 option" name="option" placeholder="text..."></td>' +
            '<td class="w-2"><i id="remove-option-poll" class="far fa-times-circle my-3 p-2"></i></td>' +
            '</tr>';
        $('#table-poll').append(txtLine2);

        note2();

        var remove = document.querySelectorAll('#remove-option-poll');

        //Không cho xóa dưới 1
        if (remove.length == 1) {
            $('#remove-option-poll').hide();
        } else {
            $('#remove-option-poll').show();
        }

        
        remove[remove.length-1].addEventListener("click",function (e) {
            var z = e.target;
            var list = document.querySelectorAll('#poll-option');
            remove = document.querySelectorAll('#remove-option-poll');

            var index = Array.prototype.indexOf.call(remove, z);
                
            console.log(index);

            list[index].remove();
            note2();

            console.log(index);

            remove = document.querySelectorAll('#remove-option-poll');

            //Không cho xóa dưới 1
            var removecheck = document.querySelectorAll('#remove-option-poll');
            if (removecheck.length <= 1) {
            $('#remove-option-poll').hide();
            } else {
            $('#remove-option-poll').show();
            }
        });
        
    });
    //Xóa Poll ban đầu
    var remove = document.querySelectorAll('#remove-option-poll');

    remove.forEach((item) => {
         item.addEventListener("click", function (e) {
            var z = e.target;
            var list = document.querySelectorAll('#poll-option');

            var index = Array.prototype.indexOf.call(remove, z);

             console.log(index);
            
            list[index].remove();

            note2();

            remove = document.querySelectorAll('#remove-option-poll');

            //Không cho xóa dưới 1
            var removecheck = document.querySelectorAll('#remove-option-poll');
            if (removecheck.length <= 1) {
                $('#remove-option-poll').hide();
            } else {
                $('#remove-option-poll').show();
            }

        });
    });
    //===============End Phần Poll====================

    //===============Phần Reading====================
    $('#more-question-reading').click(function () {
        //Thêm Câu hỏi vào Reading
        var txtLine4 = '<div id="reading-question" class="w-full items-center bg-gray p-6 border border-black-400">' +
            '<div class="ms-error"></div>' +
            '<div class="m-2">' +
            '<i id="remove-question-reading" class=" h4 delete float-right">x</i>' +
            '</div>' +
            '<div class="m-4">' +
            '<span id="reading-note" class="question-num-2" style="margin-right:20px"></span>' +
            '<span>Time: </span>' +
            '<input id="timeReading" class="border border-black-400 pl-1 w-1/12" type="number" name="time" placeholder="...">' +
            '<span>Mark: </span>' +
            '<input id="markReading" class="border border-black-400 pl-1 w-1/12" type="number" name="mark" placeholder="...">' +
            '</div>' +
            '<div class="mx-4">' +
            '<textarea class="p-2 w-full" id="questionReading" name="question" rows="3" placeholder=" ..."></textarea>' +
            '</div>' +
            '<div class="mx-4 div-margin-20">' +
            '<input type="checkbox" class="form-check-input" id="cb-option2" name="mixChoice" value="1" style="margin-top:6px">' +
            '<input type="hidden" name="mixChoice" value="0">' +
            '&nbsp;&nbsp;<span class="shuffle-lable">Shuffle Answer</span>' +
            '</div>' +
            '<table id="table-reading" class="w-full border-spacing-2">' +
            '<tr id="reading-option">' +
            '<td class="w-2"><input id="optionReading-checkbox" type="checkbox" name="cbOption" value="1"></td>' +
            '<input type="hidden" name="cboption" value="0">'+
            ' <td> A.&nbsp;&nbsp;<input style="width:95%" id="optionReading-text" type="text" class="w-full pl-3" name="option" placeholder="...."></td>' +
            '</tr>' +
            '<tr id="reading-option">' +
            '<td class="w-2"><input id="optionReading-checkbox" type="checkbox" name="cbOption" value="1"></td>' +
            '<input type="hidden" name="cboption" value="0">' +
            ' <td> B.&nbsp;&nbsp;<input style="width:95%" id="optionReading-text" type="text" class="w-full pl-3" name="option" placeholder="...."></td>' +
            '</tr>' +
            '<tr id="reading-option">' +
            '<td class="w-2"><input id="optionReading-checkbox" type="checkbox" name="cbOption" value="1"></td>' +
            '<input type="hidden" name="cboption" value="0">' +
            ' <td> C.&nbsp;&nbsp;<input style="width:95%" id="optionReading-text" type="text" class="w-full pl-3" name="option" placeholder="...."></td>' +
            '</tr>' +
            '<tr id="reading-option">' +
            '<td class="w-2"><input id="optionReading-checkbox" type="checkbox" name="cbOption" value="1"></td>' +
            '<input type="hidden" name="cboption" value="0">' +
            ' <td> D.&nbsp;&nbsp;<input style="width:95%" id="optionReading-text" type="text" class="w-full pl-3" name="option" placeholder="...."></td>' +
            '</tr>' +
            '<tr id="reading-option">' +
            '<td class="w-2"><input id="optionReading-checkbox" type="checkbox" name="cbOption" value="1"></td>' +
            '<input type="hidden" name="cboption" value="0">' +
            ' <td> E.&nbsp;&nbsp;<input style="width:95%" id="optionReading-text" type="text" class="w-full pl-3" name="option" placeholder="...."></td>' +
            '</tr>' +
            '<tr id="reading-option">' +
            '<td class="w-2"><input id="optionReading-checkbox" type="checkbox" name="cbOption" value="1"></td>' +
            '<input type="hidden" name="cboption" value="0">' +
            '<td> F.&nbsp;&nbsp;<input style="width:95%" id="optionReading-text" type="text" class="w-full pl-3" name="option" placeholder="...."></td>' +
            '</tr>' +
            '</table>' +
            '</div>';

        //Thêm Câu hỏi vào Reading
        $('#table-question-reading').append(txtLine4);

        //Dánh index câu
        note3();

        //Remove Question sau khi add thêm
        var removeQuestion = document.querySelectorAll('#remove-question-reading');
        removeQuestion.forEach((item) => {
            item.addEventListener('click', function (e) {
                var z = e.target;
                var list = document.querySelectorAll('#reading-question');
                 removeQuestion = document.querySelectorAll('#remove-question-reading');

                var index = Array.prototype.indexOf.call(removeQuestion, z);

                list[index].remove();

                note3();

            });
        });

    });

    if ($('#reading-form').length != 0) {

        //Xoá Câu Hỏi Ban dau
        var removeQuestion = document.querySelectorAll('#remove-question-reading');
        removeQuestion.forEach((item) => {
            item.addEventListener('click', function (e) {
                var z = e.target;
                var list = document.querySelectorAll('#reading-question');
                removeQuestion = document.querySelectorAll('#remove-question-reading');

                var index = Array.prototype.indexOf.call(removeQuestion, z);

                list[index].remove();

                note3();

            });
        });

        //Xoá option ban đầu- fix cung roi thi khong can
        var removeOption = document.querySelectorAll('#remove-option-reading');
        removeOption.forEach((item) => {
            item.addEventListener('click', function () {
                var list = document.querySelectorAll('#reading-option');

                for (var index = 0; index < list.length; index++) {
                    list[index].querySelector("#remove-option-reading").addEventListener("click",
                        function () {
                            this.closest("#reading-option").remove();
                        });
                }

            });
        });

    }


});