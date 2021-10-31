using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.teacher.Controllers
{
    public class ChapterController : Controller
    {
        private DBModel db = new DBModel();

        //Create Chapter
        [HttpPost]
        public ActionResult CreateChapter(string newChapCID, string chapterName, string chid)
        {

            Chapter chapter = new Chapter();
            chapter.CourseID = int.Parse(newChapCID);
            chapter.Name = chapterName.Trim();
            db.Chapters.Add(chapter);
            db.SaveChanges();
            int chapID = int.Parse(chid);
            if (chapID == -1)
            {
                return Redirect("~/Teacher/Question/QuestionBank");
            }
            else
            {
                return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);
            }
        }

        //Edit Chapter Name
        [HttpPost]
        public ActionResult EditChapter(string newChapterName, string chid)
        {
            int chapterID = int.Parse(chid);
            //check for dupilcate name
            var updateChapter = db.Chapters.Find(chapterID);
            updateChapter.Name = newChapterName.Trim();
            db.Entry(updateChapter).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapterID);
        }


        //Delete Chapter
        [HttpPost]
        public ActionResult DeleteChapter(string chid)
        {

            int chapterId = int.Parse(chid);
            var chapter = db.Chapters.Find(chapterId);

            var questionInsideChapter = db.Questions.Where(q => q.ChapterID == chapterId).ToList();
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

            db.SaveChanges();
            int chapID = int.Parse(chid);

            //return questionbank page
            return Redirect("~/Teacher/Question/QuestionBank");

        }

    }

}
