using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.Teacher.Controllers.ReportController
{
    public class ReportController : Controller
    {
        private DBModel db = new DBModel();
        // GET: Teacher/Report
        public ActionResult ReportHome()
        {
            return View();
        } 
        
        public ActionResult ViewReportListByCourse(string cid)
        {
            int courseId = int.Parse(cid);
            var course = db.Courses.Find(courseId);
            var listQuizDone = db.QuizDones.Where(q => q.CourseID == courseId);
            ViewBag.Course = course;
            return View(listQuizDone);
        }
        public ActionResult ReportDetail(string quizprId)
        {
            
            return View();
        }
    }
}