$(document).ready(function () {

    
    const alphabet = ["A.", "B.", "C.", "D.", "E.", "F.", "G.", "H.", "I.", "J.", "K.", "L.", "M.", "N.", "O.", "P.", "Q.", "R.", "S.", "T.", "U.", "V.", "W.", "X.", "Y.", "Z."];
    
    //=========Multiple Choice PreView=========
    $('#previewMultiple').click(function () {

        const getText = document.querySelectorAll('#optionMultiple-text');
        const getQuestion = document.querySelector('#questionMultiple').value;
        var indexAlphabet = 0;

        var item = '';

        for (var i = 0; i < getText.length; i++) {
            if (getText[i].value != '') {

                item +=     '<tr>' +
                            '<td class="w-2"><input type="checkbox" id="multiple-q1" name="cb-option1" value="1" disabled></td>' +
                    '<td class="w-2">' + alphabet[indexAlphabet] + '</td><td><input type="text" class="w-full bg-gray-100" name="option1" placeholder="Option 1" value="' + getText[i].value + '" readonly></td>' +
                            '</tr>';

                ++indexAlphabet;
                $('#table-multiple-preview').html(item);
            }
        }

        $('#questionMultiple-preview').text(getQuestion);
        
    });
    //=========End Multiple Choice PreView=========

    //=========Reading PreView=========
    $('#previewReading').click(function () {
        const getParagraph = document.querySelector('#paragraphReading').value;
        const getQuestion = document.querySelectorAll('#questionReading');
        const getListOption = document.querySelectorAll('#table-reading');
        var item1 = '';
        //Add số Question tương ứng
        for (var i = 0; i < getQuestion.length; i++) {

            var listText = getListOption[i].querySelectorAll('#optionReading-text');
            var indexAlphabet = 0;

            item1 += '<div id="reading-test-preview">' +
                '<div class="w-full items-center bg-gray p-6">' +
                '<span>Question ' + (i + 1) + ':</span>' +
                '</div>' +
                '<div class="w-full items-center bg-gray ">' +
                '<div class="">' +
                '<div contenteditable="false" id="questionReading-preview" class="w-full items-center bg-gray p-6 font-sans font-semibold ">' + getQuestion[i].value +
                '</div>' +
                '</div>' +
                '</div>' +
                '<div class="w-full items-center bg-gray ">' +
                '<table id="table-reading-preview" class="w-full border-spacing-2">';
            for (var z = 0; z < listText.length; z++) {
                if (listText[z].value != '') {
                item1 +='<tr>' +
                            '<td class="w-2"><input type="checkbox" id="multiple-q1" name="cb-option1" value="1" disabled></td>' +
                            '<td class="w-2">' + alphabet[indexAlphabet] + '</td>' +
                            '<td><input type="text" class="w-full pl-3 bg-gray-100" name="option1" placeholder="Option 1" value="' + listText[z].value + '" readonly></td>' +
                        '</tr>';

                    ++indexAlphabet;
                }
            }

            item1 += '</table>' +
                '</div>' +
                '</div>';
        }
        $('#table-reading-question-preview').html(item1);
        $('#paragraphReading-preview').text(getParagraph);

    });
    //=========End Reading PreView=========

    //=========Short PreView=========
    if ($('#previewShort').length != 0) {
        $('#previewShort').click(function () {

            const getQuestion = document.querySelector('#questionShort').value;

            

          

            $('#questionShort-preview').text(getQuestion);

            //Hidden
            //$('#answerShort-preview').val($('#answerShort').val());

        });
    }
    //=========End Short PreView=========

    //=========Matching PreView=========

    $('#previewMatching').click(function () {
        $("#matching-left-preview").val($("#matching-left"));
        $("#matching-right-preview").val($("#matching-right"));

        $("#matching-left-preview").height($("#matching-left").height());
        $("#matching-right-preview").height($("#matching-right").height());
    });
    //=========End Matching PreView=========
    
    //=========Indicate PreView=========
    if ($('#previewIndicate').length != 0) {
        $('#previewIndicate').click(function () {

            var getQuestion = $('#questionIndicate').val();

            var regexIndicateQuestion = /\([a-zA-Z]{1}-[aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆ fFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTu UùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ1234567890]*\)/g;
            var list = getQuestion.match(regexIndicateQuestion);

            var list2 = [];
            if (list != null) {
                for (let i = 0; i < list.length; i++) {
                    let currentString = list[i];
                    let answer = currentString.split("-")[1].replace(")", "");
                    let option = currentString.split("-")[0].replace("(", "");
                    list2.push('<span style="color:blue">(' + option + ')</span> <u>' + answer + '</u>');

                }



                for (let i = 0; i < list.length; i++) {
                    let currentString = list[i];
                    getQuestion = getQuestion.replace(currentString, list2[i]);
                }
            }

            


            $('#questionIndicate-preview').html(getQuestion);


            $('#answerIndicate-preview').val($('#answerIndicate').val());

        });
    }
    //=========End Indicate PreView=========

    //=========Fill Blank PreView=========
    if ($('#previewFill').length != 0) {
        $('#previewFill').click(function () {

            const regexGroup = /\(\~[^\(\)]+\)/g;
            var fillText = $("#questionFill").val();
            var arrGroup = fillText.match(regexGroup);

            if ($("#givenFill").prop("checked") == true) {
                var arrGroup3 = fillText.match(regexGroup);

                if (arrGroup != null) {
                    for (var i = 0; i < arrGroup.length; i++) {
                        let selectString = '<select id="" style="color: blue" name="fillBankGivenAnswer" class="border border-black-400 w-2/12">' + '<option label=""></option>';

                        arrGroup[i] = arrGroup[i].replace("(", "");
                        arrGroup[i] = arrGroup[i].replace(")", "");
                        var listOption = arrGroup[i].match(/\~[^,. =][aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆfFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTuUùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ1234567890,. ]+/g);
                        var listAnswer = arrGroup[i].match(/\~=[^,. ][aAàÀảẢãÃáÁạẠăĂằẰẳẲẵẴắẮặẶâÂầẦẩẨẫẪấẤậẬbBcCdDđĐeEèÈẻẺẽẼéÉẹẸêÊềỀểỂễỄếẾệỆfFgGhHiIìÌỉỈĩĨíÍịỊjJkKlLmMnNoOòÒỏỎõÕóÓọỌôÔồỒổỔỗỖốỐộỘơƠờỜởỞỡỠớỚợỢpPqQrRsStTuUùÙủỦũŨúÚụỤưƯừỪửỬữỮứỨựỰvVwWxXyYỳỲỷỶỹỸýÝỵỴzZ1234567890,. ]+/g);
                        if (listOption != null) {
                            for (let j = 0; j < listOption.length; j++) {
                                selectString += '<option label="">' + listOption[j].replace("~", "") + '</option>';
                            }
                        }
                        if (listAnswer != null) {
                            for (let z = 0; z < listAnswer.length; z++) {
                                selectString += '<option label="">' + listAnswer[z].replace("~=", "") + '</option>';
                            }
                        }
                        
                        selectString += '</select>';

                        fillText = fillText.replace(arrGroup3[i], selectString);
                    }
                }

                $('#questionFill-preview').html(fillText);

            }
            if ($("#givenFill").prop("checked") == false) {

                const regexGroup2 = /\([^\(\)]+\)/g;
                var arrGroup2 = fillText.match(regexGroup2);

                for (var i = 0; i < arrGroup2.length; i++) {
                    let selectString = '<input type="text" id="" value="" style="width: 15%; color:blue" class="fill-input border border-black-400" name="fillBankNotGivenAnswer" placeholder="..." disabled>';
                    fillText = fillText.replace(arrGroup2[i], selectString);
                }

                $('#questionFill-preview').html(fillText);

            }

        });
    }
    //=========End Fill Blank PreView=========


    //=========Multiple Choice PreView=========
    $('#previewPoll').click(function () {

        const getText = document.querySelectorAll('#txtPoll');
        const getQuestion = document.querySelector('#questionPoll').value;

        var item = '';

        for (var i = 0; i < getText.length; i++) {
            if (getText[i].value != '') {

                item += '<tr>' +
                    '<td class="w-2"><input type="checkbox"  disabled></td>' +
                    '<td><input type="text" class="w-full bg-gray-100" value="' + getText[i].value + '" readonly></td>' +
                    '</tr>';

                $('#table-poll-preview').html(item);
            }
        }

        $('#questionPoll-preview').text(getQuestion);

    });
    //=========End Multiple Choice PreView=========
});