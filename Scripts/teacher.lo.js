var currentLoID = "";
var currentLoName = "";
var currentLoDes = "";
//get course id from the course user want to create/edit/delete new Chapter

function updateLO(loID, loName, loDes) {
   /* console.log(loID +"==" + loName + loDes)*/
    currentLoID = loID;
    currentLoName = loName;
    currentLoDes = loDes;
    document.getElementById("loIdToUpdate").value = currentLoID;
    document.getElementById("loNameToUpdate").value = currentLoName;
    document.getElementById("loDesToUpdate").value = currentLoDes;
}

function submitDeleteForm() {
    /*console.log("hihihihihi");*/
    document.getElementById("formDeleteLO").submit();
}

$(document).ready(function () {

    //check new lo 
    var loName = document.querySelector('#newLoNameCreate');
    var courseIDNewLo = document.querySelector('#newLoCId');
    /*var loDes = document.querySelector('#createLoDes');*/

    $('#createLO').click(function (e) {
        $('.image-error').remove();
        var newLoName = $(loName).val();
        var courseIDCreateLO = $(courseIDNewLo).val();
        console.log("jihihi1");
        $.ajax({
            url: '/LearningOutcome/CheckDuplicateNewLO',
            dataType: "json",
            data: { text: newLoName, cid: courseIDCreateLO/*, descrip: loDes */},
            type: "POST",
            success: function (data) {
                if (data.check == "0") {
                    if ($('.image-error').length == 0) {
                        $(loName).after('<div class="image-error">*' + data.mess + '</div>');
                        $('.image-error').css("color", "red");
                        $('.image-error').css("font-weight", "bold");
                    }
                }
                if (data.check == "1") {
                    $('.image-error').remove();
                }
                $('#form-create-lo').submit();



            },
            error: function (xhr) {
                alert('Error by LO');
            }
        });

        //Để không tự submit nữa
        e.preventDefault();
        return false;
    });

    //Khi submit sẽ check xem có class .image-error hay không
    $('#form-create-lo').submit(function (e) {
        if ($('.image-error').length != 0) {
            e.preventDefault();
            return false;
        }
    });


    //check edit lo
    var loNameUpdate = document.querySelector('#loNameToUpdate');
    var courseIDEditLo = document.querySelector('#loCIdToUpdare');
    var loIdToEdit = document.querySelector('#loIdToUpdate');

    $('#editLO').click(function (e) {
        $('.image-error-4').remove();
        var newLOName = $(loNameUpdate).val();
        var courseIDToUpdate = $(courseIDEditLo).val();
        var loIDToUpdate = $(loIdToEdit).val();
        $.ajax({
            url: '/LearningOutcome/CheckDuplicateEditLO',
            dataType: "json",
            data: { text: newLOName, cid: courseIDToUpdate, loid: loIDToUpdate },
            type: "POST",
            success: function (data) {
                if (data.check == "0") {
                    if ($('.image-error-4').length == 0) {
                        $(loNameUpdate).after('<div class="image-error-4">*' + data.mess + '</div>');
                        $('.image-error-4').css("color", "red");
                        $('.image-error-4').css("font-weight", "bold");
                    }
                }
                if (data.check == "1") {
                    $('.image-error-4').remove();
                }

                $('#form-edit-lo').submit();


            },
            error: function (xhr) {
                alert('Error by edit lo');
            }
        });

        //Để không tự submit nữa
        e.preventDefault();
        return false;
    });

    //Khi submit sẽ check xem có class .image-error hay không
    $('#form-edit-lo').submit(function (e) {
        if ($('.image-error-4').length != 0) {
            e.preventDefault();
            return false;
        }
    });
});
