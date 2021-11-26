var currentLoID = "";
var currentLoName = "";
var currentLoDes = "";
//get course id from the course user want to create/edit/delete new Chapter

function updateLO(loID, loName, loDes) {
    console.log(loID +"==" + loName + loDes)
    currentLoID = loID;
    currentLoName = loName;
    currentLoDes = loDes;
    document.getElementById("loIdToUpdate").value = currentLoID;
    document.getElementById("loNameToUpdate").value = currentLoName;
    document.getElementById("loDesToUpdate").value = currentLoDes;
}

function submitDeleteForm() {
    console.log("hihihihihi")
    document.getElementById("formDeleteLO").submit();
}