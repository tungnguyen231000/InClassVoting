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
           
            return Redirect(Request.UrlReferrer.ToString());
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
            return Redirect(Request.UrlReferrer.ToString());
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
            
            var questionDoneContain = db.QuestionDones.Where(q => q.ChapterID == chapterId).ToList();
            //remove chapter of question done
            foreach (var questionDone in questionDoneContain)
            {
                questionDone.ChapterID= null;
                db.Entry(questionDone).State = EntityState.Modified;
            }
            db.Chapters.Remove(chapter);
            
            var passageContain = db.Passages.Where(p => p.ChapterID == chapterId).ToList();

            //remove passage inside chapter
            foreach (var passage in passageContain)
            {
                db.Passages.Remove(passage);
            }
            
            var passageDoneContain = db.Passages.Where(p => p.ChapterID == chapterId).ToList();
            //remove chapter of question done
            foreach (var passageD in passageDoneContain)
            {
                passageD.ChapterID= null;
                db.Entry(passageD).State = EntityState.Modified;
            }

            db.Chapters.Remove(chapter);

            db.SaveChanges();
            int chapID = int.Parse(chid);

            //return questionbank page
            return Redirect("~/Teacher/Question/QuestionBank");

        }

    }

}
