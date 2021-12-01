function isCheckedMixQ() {
    if (document.getElementById("rdQuestion").checked) {
        document.getElementById("rdQuestionNum").style.display = "block";
    } else {
        document.getElementById("rdQuestionNum").style.display = "none";
    }
    saveChanges();
}
function checkMask() {
    if (document.getElementById("checkBoxRight").checked) {
        document.getElementById("checkBoxMask").checked = "true";
    }
    saveChanges();
}

var quizLink = document.getElementById("quizLink").value;
if (quizLink.indexOf("DoQuizPaperTest") == -1) {
    document.getElementById("qQuiz").checked = "true";
} else {
    document.getElementById("papeQuiz").checked = "true";
}

function changeLinkqQuiz() {
    let link = document.getElementById("linkQ").innerHTML;
    document.getElementById("linkQ").innerHTML = link.replace("DoQuizPaperTest", "DoQuestionByQTest");
}
function changeLinkpQuiz() {
    let link = document.getElementById("linkQ").innerHTML;
    document.getElementById("linkQ").innerHTML = link.replace("DoQuestionByQTest", "DoQuizPaperTest");
}


function copyLink() {
    var copyText = document.getElementById("quizLink");

    copyText.select();
    navigator.clipboard.writeText(copyText.value);

}


function getQuizName() {
    var newName = document.getElementById("newName").value;
    document.getElementById("quizName").value = newName;
}

function getTempQuizInfo() {
    console.log("hihih");

    var tempName = document.getElementById("newName").value;
    document.getElementById("tempName").value = tempName;
    console.log(tempName + "===");
}

function saveChanges() {
    console.log("hihi");
    document.getElementById("saveOption").hidden = false;
}