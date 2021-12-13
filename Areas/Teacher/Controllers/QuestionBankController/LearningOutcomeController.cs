using InClassVoting.Filter;
using InClassVoting.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.Teacher.Controllers.QuestionBankController
{
    [AccessAuthenticationFilter]
    [UserAuthorizeFilter("Teacher")]
    public class LearningOutcomeController : Controller
    {
        private DBModel db = new DBModel();

        private bool checkCourserIdAvailbile(string cid)
        {
            bool check = true;
            int courseId;
            bool isInt = int.TryParse(cid, out courseId);
            //check if chapter id is int
            if (isInt == false)
            {
                check = false;
            }
            else
            {
                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                var course = db.Courses.Find(courseId);
                //check if chapter exist in db
                if (course == null)
                {
                    check = false;
                }
                else
                {
                    //check if chapter belong to teacher
                    if (course.TeacherID != teacherId)
                    {
                        check = false;
                    }
                }
            }
            return (check);
        }

        [HandleError]
        public ActionResult ViewLearningOutcome(string cid, string searchText, int? i)
        {
            //check if course is availble
            if (checkCourserIdAvailbile(cid) == false)
            {
                return Redirect("~/Teacher/Question/QuestionBank");
            }
            else
            {

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                int courseID = int.Parse(cid);
                Course course = db.Courses.Find(courseID);

                var loList = db.LearningOutcomes.Where(lo => lo.CourseID == courseID).ToList();

                if (searchText != null && !searchText.Trim().Equals(""))
                {
                    loList = loList.Where(lo => lo.LO_Description.ToLower().Trim().Contains(searchText.ToLower().Trim()) ||
                    lo.LO_Name.ToLower().Trim().Contains(searchText.ToLower().Trim())).ToList();
                }
                int totalLo = db.LearningOutcomes.Count(lo => lo.CourseID == courseID);
                ViewBag.Course = course;
                ViewBag.CountLO = totalLo;
                ViewBag.Search = searchText;
                //if there is no page number return
                if (i == null)
                {
                    i = 1;
                }
                else
                {
                    if (totalLo % 10 == 0)
                    {
                        if (i > (totalLo / 10))
                        {
                            i = totalLo / 10;
                        }
                    }

                }

                ViewBag.LONo = (i - 1) * 10;
                Session["SelectedChapter"] = null;
                Session["SelectedCourse"] = courseID;
                return View(loList.ToPagedList(i ?? 1, 10));
            }
        }

        // Create New LO
        [HandleError]
        [HttpPost]
        public ActionResult CreateLearningOutcome(string cid, string loName, string loDes)
        {
            try
            {
                LearningOutcome lo = new LearningOutcome();
                int courseID = int.Parse(cid);
                lo.CourseID = courseID;
                lo.LO_Name = loName.ToUpper();
                lo.LO_Description = loDes;
                db.LearningOutcomes.Add(lo);
                db.SaveChanges();
                int lastPage = 0;
                int countLo = db.LearningOutcomes.Count(l => l.CourseID == courseID);
                if ((countLo % 10) != 0)
                {
                    lastPage = (countLo / 10) + 1;

                }
                else
                {
                    lastPage = countLo / 10;
                }
                return Redirect("~/Teacher/LearningOutcome/ViewLearningOutcome?cid=" + cid + "&i=" + lastPage);

            }
            catch
            { return Redirect("~Error/NotFound"); }
        }

        //Check New LO Name
        [HttpPost]
        public JsonResult CheckDuplicateNewLO(string text, string cid)
        {
            string dataInput = text;
            int courseId = int.Parse(cid);
            string check = "";
            string message = "";


            if (dataInput.Trim().Equals("") || dataInput == null)
            {
                check = "0";
                message = "Please enter learning outcome name !";
            }
            else
            {
                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                var lo = db.LearningOutcomes.Where(l => l.Course.TeacherID == teacherId &&
                l.CourseID == courseId &&
                l.LO_Name.ToLower().Trim().Equals(dataInput.ToLower().Trim())).FirstOrDefault();
                /*var currentCourse = db.Courses.Find(courseId);*/

                if (lo != null)
                {
                    check = "0";
                    message = "This learning outcome name already existed!";
                }
                else
                {
                    check = "1";
                    message = "";
                }
            }

            return Json(new { mess = message, check = check });

        }


        //Update LO
        [HandleError]
        [HttpPost]
        public ActionResult EditLearningOutcome(string cid, string newLoName, string newLoDes, string loid)
        {
            try
            {
                var lo = db.LearningOutcomes.Find(int.Parse(loid));
                lo.LO_Name = newLoName.ToUpper();
                lo.LO_Description = newLoDes;
                db.Entry(lo).State = EntityState.Modified;
                db.SaveChanges();
                return Redirect(Request.UrlReferrer.ToString());
            }
            catch
            { return Redirect("~Error/NotFound"); }
        }


        //Check edit LO Name
        [HttpPost]
        public JsonResult CheckDuplicateEditLO(string text, string cid, string loid)
        {
            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
            string dataInput = text;
            int courseId = int.Parse(cid);
            int loId = int.Parse(loid);
            string check = "";
            string message = "";


            if (dataInput.Trim() == "")
            {
                check = "0";
                message = "Please enter learning outcome name !";
            }
            else
            {
                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                var lo = db.LearningOutcomes.Where(l => l.Course.TeacherID == teacherId &&
                l.CourseID == courseId && l.LOID != loId &&
                l.LO_Name.ToLower().Trim().Equals(dataInput.ToLower().Trim())).FirstOrDefault();
                var curentLO = db.LearningOutcomes.Find(loId);

                if (lo != null)
                {
                    check = "0";
                    message = "This learning outcome name already existed!";
                }
                else
                {
                    if (dataInput.Trim().ToLower().Equals(curentLO.LO_Name.Trim().ToLower()))
                    {
                        check = "0";
                        message = "Your learning outcome name is unchange!";
                    }
                    else
                    {
                        check = "1";
                        message = "";
                    }
                }
            }


            return Json(new { mess = message, check = check });

        }

        [HandleError]
        public ActionResult DeleteLearningOutcome(string cid, FormCollection collection)
        {

            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
            int courseID = int.Parse(cid);
            var loListSelected = collection["loid"];
            if (loListSelected == null)
            {

                return Redirect(Request.UrlReferrer.ToString());

            }
            else
            {
                string[] loIdList = collection["loid"].Split(new char[] { ',' });
                foreach (string lo in loIdList)
                {
                    int loID = int.Parse(lo);

                    var questionContainLO = db.QuestionLOes.Where(ql => ql.LearningOutcomeID == loID).ToList();
                    //delete LO from question
                    foreach (var questionLO in questionContainLO)
                    {
                        db.QuestionLOes.Remove(questionLO);
                    }

                    var questionDoneContainLO = db.QuestionDoneLOes.Where(ql => ql.LearningOutcomeID == loID).ToList();
                    //delete LO from question that is done
                    foreach (var questionLO in questionDoneContainLO)
                    {
                        db.QuestionDoneLOes.Remove(questionLO);
                    }

                    var learnOC = db.LearningOutcomes.Find(loID);
                    //delete LO
                    db.LearningOutcomes.Remove(learnOC);
                }
                db.SaveChanges();

                return Redirect(Request.UrlReferrer.ToString());
            }



        }
    }
}