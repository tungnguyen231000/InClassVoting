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
        // Create New LO

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
                return View(loList.ToPagedList(i ?? 1, 10));
            }
        }

        //Update LO
        [HttpPost]
        public ActionResult CreateLearningOutcome(string cid, string loName, string loDes)
        {
            LearningOutcome lo = new LearningOutcome();
            int courseID = int.Parse(cid);
            lo.CourseID = courseID;
            lo.LO_Name = loName;
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

        [HttpPost]
        public ActionResult EditLearningOutcome(string cid, string newLoName, string newLoDes, string loid)
        {
            var lo = db.LearningOutcomes.Find(int.Parse(loid));
            lo.LO_Name = newLoName;
            lo.LO_Description = newLoDes;
            db.Entry(lo).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

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