function isCheckedMixQ() {
    if (document.getElementById("rdQuestion").checked) {
        document.getElementById("rdQuestionNum").style.display = "block";
    } else {
        document.getElementById("rdQuestionNum").style.display = "none";
    }
    saveChanges();
}
function checkMask() {
    if (document.getElementById("checkBoxMask").checked == false) {
        document.getElementById("checkBoxAnswer").checked = false;
    }
    saveChanges();
}
function checkAnswer() {
    if (document.getElementById("checkBoxAnswer").checked) {
        document.getElementById("checkBoxMask").checked = true;
    }
    publishChange();
}

/*var quizLink = document.getElementById("quizLink").value;
if (quizLink.indexOf("DoQuizPaperTest") == -1) {
    document.getElementById("qQuiz").checked = "true";
} else {
    document.getElementById("papeQuiz").checked = "true";
}*/

function changeLinkqQuiz() {
    let link = document.getElementById("linkQ").innerHTML;
    /*document.getElementById("linkQ").innerHTML = link.replace("DoQuizPaperTest", "DoQuizQuestionByQuestion");
    document.getElementById("quizLink").value = link.replace("DoQuizPaperTest", "DoQuizQuestionByQuestion");*/
    let quizType = "ShowQuestionByQuestion";
    document.getElementById("qtype").value = quizType;
    document.getElementById("qtypeChange").value = quizType;
    console.log(quizType);
}

function changeLinkpQuiz() {
    let link = document.getElementById("linkQ").innerHTML;
   /* document.getElementById("linkQ").innerHTML = link.replace("DoQuizQuestionByQuestion", "DoQuizPaperTest");
    document.getElementById("quizLink").value = link.replace("DoQuizQuestionByQuestion", "DoQuizPaperTest");*/
    let quizType = "ShowAllQuestion";
    document.getElementById("qtype").value = quizType;
    document.getElementById("qtypeChange").value = quizType;
    console.log(quizType);
    
}


function copyLink() {
    var copyText = document.getElementById("quizLink");

    copyText.select();
    navigator.clipboard.writeText(copyText.value);

}


function getNewQuizInfo() {
    var newName = document.getElementById("newName").value;
    document.getElementById("quizName").value = newName;
    var checkedQuizMode = null;
    var listCheckBox = document.getElementsByClassName('rdQuizMode');
    for (var i = 0; listCheckBox[i]; ++i) {
        if (listCheckBox[i].checked) {
            checkedQuizMode = listCheckBox[i].value;
            break;
        }
    }

    document.getElementById("quizMode").value = checkedQuizMode;
}

function getTempQuizInfo() {
    var tempName = document.getElementById("newName").value;
    document.getElementById("tempName").value = tempName;
    var checkedTempMod = null;
    var listCheckBox = document.getElementsByClassName('rdQuizMode');
    for (var i = 0; listCheckBox[i]; ++i) {
        if (listCheckBox[i].checked) {
            checkedTempMod = listCheckBox[i].value;
            break;
        }
    }
    document.getElementById("tempMode").value = checkedTempMod;
}

function saveChanges() {
   
    document.getElementById("saveOption").hidden = false;
}