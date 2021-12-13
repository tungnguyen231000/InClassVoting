using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.teacher.Controllers
{
    public class CourseController : Controller
    {
        private DBModel db = new DBModel();

        [HandleError]
        public PartialViewResult ShowCourseListForQuestionBank()
        {
            int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
            List<Course> courseList = db.Courses.Where(c => c.TeacherID == teacherId).ToList();
            ViewBag.CourseList = courseList;
            ViewBag.ChapterList = db.Chapters.Where(ch=>ch.Course.TeacherID==teacherId).ToList();
            ViewBag.CourseCount = courseList.Count;
            ViewBag.SelectedCourse = Convert.ToInt32(HttpContext.Session["SelectedCourse"]);
            ViewBag.SelectedChapter = Convert.ToInt32(HttpContext.Session["SelectedChapter"]);
            return PartialView("_ShowCourseListForQuestionBank");
        }

        [HandleError]
        public PartialViewResult ShowCourseListForQuizLibrary()
        {
            int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
            List<Course> courseList = db.Courses.Where(c => c.TeacherID == teacherId).ToList();
            ViewBag.CourseList = courseList;
            ViewBag.ChapterList = db.Chapters.Where(ch => ch.Course.TeacherID == teacherId).ToList();
            ViewBag.CourseCount = courseList.Count;
            return PartialView("_ShowCourseListForQuizLibrary");
       
        }

        [HandleError]
        public PartialViewResult ShowCourseListForReport()
        {
            int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
            List<Course> courseList = db.Courses.Where(c => c.TeacherID == teacherId).ToList();
            ViewBag.CourseList = courseList;
            ViewBag.ChapterList = db.Chapters.Where(ch => ch.Course.TeacherID == teacherId).ToList();
            ViewBag.CourseCount = courseList.Count;
            return PartialView("_ShowCourseListForReport");

        }

        [HandleError]
        //Create New Course
        [HttpPost]
        public ActionResult CreateCourse(string newcourseName)
        {
            try { 
            Course course = new Course();
            int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
            course.TeacherID = teacherId;
            course.Name = newcourseName.ToUpper().Trim();
            db.Courses.Add(course);
            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());

            }
            catch
            { return Redirect("~Error/NotFound"); }
        }

        //Check New Course Name
        [HttpPost]
        public JsonResult CheckDuplicateCourse(string text)
        {
            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);

            string dataInput = text;
            string check = "";
            string message = "";

            if (dataInput.Trim() == "")
            {
                check = "0";
                message = "Please input course name !";
            }
            else
            {
                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                var course = db.Courses.Where(c => c.TeacherID == teacherId && c.Name.ToLower().Trim().Equals(dataInput.ToLower().Trim())).FirstOrDefault();

                if (course != null)
                {
                    check = "0";
                    message = "This course name already existed!";
                }
                else
                {
                    check = "1";
                    message = "";
                }
            }

            return Json(new { mess = message, check = check }) ;

        }

        [HandleError]
        //Edit CourseName
        [HttpPost]
        public ActionResult EditCourse(string newCourseName, string courseIdUpdate/*, string chid*/) 
        {
            try { 
            int courseIdToUpdate = int.Parse(courseIdUpdate);
            var updateCourse = db.Courses.Find(courseIdToUpdate);
            updateCourse.Name = newCourseName.ToUpper().Trim();
            db.Entry(updateCourse).State = EntityState.Modified;
            db.SaveChanges();
           /* int chapID = int.Parse(chid);*/
            /* if (chapID == -1)
             {
                 return Redirect("~/Teacher/Question/QuestionBank");
             }
             else
             {
                 return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);
             }*/
            return Redirect(Request.UrlReferrer.ToString());

            }
            catch
            { return Redirect("~Error/NotFound"); }
        }


        //Check New Course Name when edit name
        [HttpPost]
        public JsonResult CheckDuplicateEditCourse(string text, string cid)
        {
            /* ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
             ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);*/
            string dataInput = text;
            int courseId = int.Parse(cid);
            string check = "";
            string message = "";


            if (dataInput.Trim() == "")
            {
                check = "0";
                message = "Please input course name!";
            }
            else
            {
                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                var course = db.Courses.Where(c => c.TeacherID == teacherId &&
                c.CID!= courseId&&
                c.Name.ToLower().Trim().Equals(dataInput.ToLower().Trim())).FirstOrDefault();
                var currentCourse = db.Courses.Find(courseId);

                if (course != null)
                {
                    check = "0";
                    message = "This course name already existed!";
                }
                else
                {
                    if (dataInput.Trim().ToLower().Equals(currentCourse.Name.Trim().ToLower()))
                    {
                        check = "0";
                        message = "Your course name is unchange!";
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
        //delete course
        [HttpPost]
        public ActionResult DeleteCourse(string courseIdDelete/*, string chid*/)
        {
            int courseIdToDelete = int.Parse(courseIdDelete);
            var deleteCourse = db.Courses.Find(courseIdToDelete);
            var chapterInsideCourse = db.Chapters.Where(ch => ch.CourseID == courseIdToDelete).ToList();
            var quizInsideCourse = db.Quizs.Where(qz => qz.CourseID == courseIdToDelete).ToList();
            var quizDoneInsideCourse = db.QuizDones.Where(qz => qz.CourseID == courseIdToDelete).ToList();

            //delete chapter inside course
            foreach (var chapter in chapterInsideCourse)
            {
                var questionInsideChapter = db.Questions.Where(q => q.ChapterID == chapter.ChID).ToList();
                //delete question inside chap
                foreach (var question in questionInsideChapter)
                {
                    var answers = db.QuestionAnswers.Where(q => q.QuestionID == question.QID).ToList();
                    //delete answer of a question
                    foreach (var answer in answers)
                    {
                        db.QuestionAnswers.Remove(answer);
                    }
                    db.Questions.Remove(question);
                }
                db.Chapters.Remove(chapter);
            }

            //delete quiz inside course
            foreach(var quiz in quizInsideCourse)
            {
                db.Quizs.Remove(quiz);
            }

            //delete report inside course
            foreach (var quizDone in quizDoneInsideCourse)
            {
                db.QuizDones.Remove(quizDone);
            }

            db.Courses.Remove(deleteCourse);
            db.SaveChanges();

            //delete quiz inside course

           /* int chapID = int.Parse(chid);*/

            return Redirect(Request.UrlReferrer.ToString());
        }

    }
}
