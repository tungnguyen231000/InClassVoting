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

        });
    }

    $('#add-matching').click(function () {
        if ($('#number').val() != '' && $('#letter').val() != '') {
            $('#number').val();
            $('#letter').val();
            var txtLine = '<input type="text" id="matching-solution" class="text-center" name="solution" value="' + $('#number').val() + '-' + $('#letter').val() + '" readonly/>';
            $('#table-solution').append(txtLine);
            var txtLine2 = '<input type="text" id="matching-solution-preview" class="text-center" name="solution" value="' + $('#number').val() + '-' + $('#letter').val() + '" readonly/>';
            $('#table-solution-preview').append(txtLine2);
            $('#number').val('');
            $('#letter').val('');


            var listSolution = document.querySelectorAll('#matching-solution');

            listSolution.forEach((item) => {

                $(item).click(function (event) {


                    $('#remove-matching').removeAttr("disabled");
                    $('#remove-matching').click(function () {
                        event.target.remove();
                        $('#remove-matching').attr("disabled", true);
                    });

                });


            });
        }
    });

    $('#remove-matching').attr("disabled", true);

    //===============Phần Poll====================
    $('.option-btn').click(function () {

        var txtLine2 = '<tr id="poll-option">' +
            '<td><span></span></td>' +
            '<td class="w-full"><input type="text" class="w-full p-3 mt-2 option" name="option" placeholder="text..."></td>' +
            '<td class="w-2"><i id="remove-option-poll" class="far fa-times-circle my-3 p-2"></i></td>' +
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

    //===============Phần Reading====================
    $('#more-question-reading').click(function () {
        //Thêm Câu hỏi vào Reading
        var txtLine4 = '<div id="reading-question" class="w-full items-center bg-gray p-6 border border-black-400">' +
            '<div class="ms-error"></div>' +
            '<div class="m-2">' +
            '<i id="remove-question-reading" class="fas fa-times h4 delete float-right"></i>' +
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
            item.addEventListener('click', function () {
                var list = document.querySelectorAll('#reading-question');
                for (var index = 0; index < list.length; index++) {
                    list[index].querySelector("#remove-question-reading").addEventListener("click",
                        function () {
                            this.closest("#reading-question").remove();
                            note3();
                        });
                }

            });
        });

    });

    if ($('#reading-form').length != 0) {

        var moreOptionReading = document.querySelectorAll('#more-option-reading');
        //Thêm option trước khi add thêm
        moreOptionReading[moreOptionReading.length - 1].addEventListener('click', function () {

            var txtLine3 = '<tr  id="reading-option">' +
                '<td class="w-2"><input type="checkbox" id="optionReading-checkbox" id="cb-option" name="cb-option"></td>' +
                '<td><input type="text" id="optionReading-text" class="w-full pl-3" name="option" placeholder="Option"></td>' +
                '<td class="w-2"><i id="remove-option-reading" class="far fa-times-circle p-2"></i></td>' +
                '</tr>';

            $(this).closest('#reading-question').find('#table-reading > tbody').append(txtLine3);


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
        });

        //Xoá Câu Hỏi Ban dau
        var removeQuestion = document.querySelectorAll('#remove-question-reading');
        removeQuestion.forEach((item) => {
            item.addEventListener('click', function () {
                var list = document.querySelectorAll('#reading-question');
                for (var index = 0; index < list.length; index++) {
                    list[index].querySelector("#remove-question-reading").addEventListener("click",
                        function () {
                            this.closest("#reading-question").remove();
                            note3();
                        });
                }

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