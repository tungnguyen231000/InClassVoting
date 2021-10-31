var currentCouseID = "";
var currentCourseName = "";
//get course id from the course user want to edit Course Name
function getCourseIdToUpdate(cid, name) {
    console.log(cid + "========");
    console.log(name + "========");
    currentCouseID = cid;
    currentCourseName = name;
    document.getElementById("cIdToUpdate").value = currentCouseID;
    document.getElementById("newCourseName").value = currentCourseName;
    console.log("ji1111111jij");
}

function getCourseIdToDelete(cid, cname) {
    console.log(cid + "========");
    console.log(cname + "========");
    currentCouseID = cid;
    document.getElementById("cIdToDelete").value = currentCouseID;
    currentCourseName = cname;
    document.getElementById("courseNameDelete").innerHTML = currentCourseName;
    console.log("jij1222222222222ij");
}
