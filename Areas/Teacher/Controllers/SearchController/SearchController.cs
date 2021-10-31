using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.teacher.Controllers.SearchController
{
    public class SearchController : Controller
    {
        // GET: teacher/Search
        private DBModel db = new DBModel();



        public ActionResult QuestionBankSearch(string Qtype, string searchText, string chapterId)
        {
            List<Question> questionList = null;
            int qTypeID = int.Parse(Qtype);
            int chapID = int.Parse(chapterId);
            if (searchText == null)
            {
                if (qTypeID == -1)
                {
                    var qList = from q in db.Questions
                                where q.ChapterID == chapID
                                select q;
                    questionList = qList.ToList();
                }
                else
                {
                    var qList = from q in db.Questions
                                where q.Qtype == qTypeID
                                where q.ChapterID == chapID
                                select q;
                    questionList = qList.ToList();
                }
            }
            else
            {
                if (qTypeID == -1)
                {
                    var qList = from q in db.Questions
                                where q.ChapterID == chapID
                                select q;
                    questionList = qList.Where(q => q.Text.Contains(searchText)).ToList();
                }
                else
                {
                    var qList = from q in db.Questions
                                where q.ChapterID == chapID
                                where q.Qtype == qTypeID
                                select q;
                    questionList = qList.Where(q => q.Text.Contains(searchText)).ToList();
                }
            }

            var chapter = db.Chapters.Find(chapID);
            ViewBag.CourseName = chapter.Course.Name;
            ViewBag.Chapter = chapter;
            ViewBag.QuestionType = db.QuestionTypes.ToList();
            ViewBag.SelectedType = qTypeID;
            ViewBag.SearchText = searchText;
            ViewBag.SearchUrl = "~/Search/QuestionBankSearch?chapterId=" + chapID + "&searchText=" + searchText + "&QType=" + qTypeID;
            return View(questionList);
        }
    }
}