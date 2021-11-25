using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.teacher.Controllers
{
    public class CourseController : Controller
    {
        private DBModel db = new DBModel();
        
        public PartialViewResult ShowCourseListForQuestionBank()
        {
            ViewBag.CourseList = db.Courses.ToList();
            ViewBag.ChapterList = db.Chapters.ToList();
            ViewBag.CourseCount = db.Courses.Count();
            return PartialView("_ShowCourseListForQuestionBank");
        }

        public PartialViewResult ShowCourseListForQuizLibrary()
        {
            ViewBag.CourseList = db.Courses.ToList();
            ViewBag.ChapterList = db.Chapters.ToList();
            ViewBag.CourseCount = db.Courses.Count();
            return PartialView("_ShowCourseListForQuizLibrary");
       
        }

        public PartialViewResult ShowCourseListForReport()
        {
            ViewBag.CourseList = db.Courses.ToList();
            ViewBag.ChapterList = db.Chapters.ToList();
            ViewBag.CourseCount = db.Courses.Count();
            return PartialView("_ShowCourseListForReport");

        }

        //Create New Course
        [HttpPost]
        public ActionResult CreateCourse(string newcourseName, string chid)
        {
            Course course = new Course();
            course.Name = newcourseName.ToUpper().Trim();
            db.Courses.Add(course);
            db.SaveChanges();
            int chapID = int.Parse(chid);
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

        //Edit CourseName
        [HttpPost]
        public ActionResult EditCourse(string newCourseName, string courseIdUpdate, string chid)
        {
            int courseIdToUpdate = int.Parse(courseIdUpdate);
            var updateCourse = db.Courses.Find(courseIdToUpdate);
            updateCourse.Name = newCourseName.ToUpper().Trim();
            db.Entry(updateCourse).State = EntityState.Modified;
            db.SaveChanges();
            int chapID = int.Parse(chid);
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

        //delete course
        [HttpPost]
        public ActionResult DeleteCourse(string courseIdDelete, string chid)
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

            int chapID = int.Parse(chid);

            return Redirect(Request.UrlReferrer.ToString());
        }

    }
}
