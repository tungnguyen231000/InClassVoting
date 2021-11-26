function isCheckedMixQ() {
    if (document.getElementById("rdQuestion").checked) {
        document.getElementById("rdQuestionNum").style.display = "block";
    } else {
        document.getElementById("rdQuestionNum").style.display = "none";
    }
}
function checkMask() {
    if (document.getElementById("checkBoxRight").checked) {
        document.getElementById("checkBoxMask").checked = "true";
    }
}

var quizLink = document.getElementById("quizLink").value;
if (quizLink.indexOf("DoQuizPaperTest") == -1) {
    document.getElementById("qQuiz").checked = "true";
} else {
    document.getElementById("papeQuiz").checked = "true";
}

function changeLinkqQuiz() {
    document.getElementById("linkQ").innerHTML = "https://localhost:44350/Student/Quiz/DoQuestionByQTest?qzID=MQ==";
}
function changeLinkpQuiz() {
    document.getElementById("linkQ").innerHTML = "https://localhost:44350/Student/Quiz/DoQuizPaperTest?qzID=MQ==";
}

function reloadModalAddQuestion() {
    var chid = document.getElementById("ddlChapter").value;
    var cid = document.getElementById("cidAddModal").value;
    var qzid = document.getElementById("qzidAddModal").value;
    var qtype = document.getElementById("ddlQtype").value;
    var searchText = document.getElementById("searchString").value;
    $("#divShowQuestions").load('@Url.Action("ShowQuestionForEditQuiz", "Quiz")' +
        '?chid=' + chid + '&cid=' + cid + '&qzid=' + qzid + '&qtype=' + qtype + '&searchText=' + searchText);
}

function copyLink() {
    var copyText = document.getElementById("quizLink");

    copyText.select();
    navigator.clipboard.writeText(copyText.value);

}

function reloadModalShowQuestion() {
    var chid = document.getElementById("ddlChapter").value;
    var cid = document.getElementById("cidAddModal").value;
    var questions = document.getElementById("questSet").value;
    var qtype = document.getElementById("ddlQtype").value;
    var searchText = document.getElementById("searchString").value;
    console.log("jhhh" + chid + cid + questions + qtype);
    $("#divShowQuestionsForNewQuiz").load('@Url.Action("ShowQuestionForNewQuiz", "Quiz")' +
        '?chid=' + chid + '&cid=' + cid + '&qtype=' + qtype + '&searchText=' + searchText + '&questions=' + questions);
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