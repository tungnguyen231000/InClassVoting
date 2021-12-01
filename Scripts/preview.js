$(document).ready(function () {

    //=========Multiple Choice PreView=========

    $('#previewMultiple').click(function () {

        const getText = document.querySelectorAll('#optionMultiple-text');
        const getQuestion = document.querySelector('#questionMultiple').value;

        var item = '';

        for (var i = 0; i < getText.length; i++) {
            if (getText[i].value != '') {

                item +=     '<tr>' +
                                '<td class="w-2"><input type="checkbox" id="multiple-q1" name="cb-option1" value="1" disabled></td>' +
                                '<td><input type="text" class="w-full pl-3 bg-gray-100" name="option1" placeholder="Option 1" value="' + getText[i].value+'" readonly></td>' +
                            '</tr>';


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
                            '<td><input type="text" class="w-full pl-3 bg-gray-100" name="option1" placeholder="Option 1" value="' + listText[z].value + '" readonly></td>' +
                        '</tr>';
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

    $('#previewShort').click(function () {

        const getQuestion = document.querySelector('#questionShort').value;


        $('#questionShort-preview').text(getQuestion);

    });
    //=========End Short PreView=========

    //=========Matching PreView=========

    $('#previewMatching').click(function () {
        $("#matching-left-preview").val($("#matching-left"));
        $("#matching-right-preview").val($("#matching-right"));

        $("#matching-left-preview").height($("#matching-left").height());
        $("#matching-right-preview").height($("#matching-right").height());
    });
    //=========End Matching PreView=========
    

});