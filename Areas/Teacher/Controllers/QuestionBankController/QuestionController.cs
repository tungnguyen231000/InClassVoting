using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.teacher.Controllers.QuestionBankController
{
    public class QuestionController : Controller
    {
        private DBModel db = new DBModel();
        // GET: teacher/Question
        public ActionResult QuestionBank()
        {
            return View();
        }
        //Get question list by chapter (Question Bank)
        public ActionResult ViewQuestionByChapter(string chid)
        {
            int chapID = int.Parse(chid);
            var chapter = db.Chapters.Find(chapID);
            var questionList = db.Questions.Where(q => q.ChapterID == chapID).ToList();
            var matchingQuest = db.MatchQuestions.Where(m => m.ChapterId == chapID).ToList();

            List<Question> questions = questionList.OrderByDescending(q => q.QID).ToList();
            ViewBag.CourseName = chapter.Course.Name;
            ViewBag.Chapter = chapter;
            ViewBag.QuestionType = db.QuestionTypes.ToList();
            ViewBag.MatchQuestion = null;

            if (matchingQuest.Count() != 0)
            {
                ViewBag.MatchQuestion = matchingQuest;
            }

            return View(questions);
        }

        public ActionResult DeleteQuestion(string chapterId, FormCollection collection, string searchUrl)
        {
            int chapID = int.Parse(chapterId);
            var questions = collection["questionId"];
            var matchQuestions = collection["matchQuestionId"];

            //check if question id and matching question id list is null
            if (questions == null && matchQuestions == null)
            {
                return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);

            }
            else
            {
                if (questions != null)
                {
                    string[] ids = collection["questionId"].Split(new char[] { ',' });
                    //get question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int qid = int.Parse(id);

                        var qaList = db.QuestionAnswers.Where(qa => qa.QuestionID == qid).ToList();
                        foreach (QuestionAnswer qa in qaList)
                        {
                            db.QuestionAnswers.Remove(qa);
                        }
                        var question = db.Questions.Find(qid);
                        db.Questions.Remove(question);
                        db.SaveChanges();

                    }
                }

                if (matchQuestions != null)
                {
                    string[] ids = collection["matchQuestionId"].Split(new char[] { ',' });
                    //get question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int mid = int.Parse(id);

                        var matchQuest = db.MatchQuestions.Find(mid);
                        db.MatchQuestions.Remove(matchQuest);
                        db.SaveChanges();

                    }
                }


                if (searchUrl == null)
                {
                    return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);
                }
                //return page search if user was on that page
                else
                {
                    return Redirect(searchUrl);
                }

            }

        }

        //show page to create multiple choice question
        public ActionResult CreateMultipleChoiceQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            return View();
        }

        [HttpPost]
        public ActionResult CreateMultipleChoiceQuestion(string chid, string questionText, FormCollection collection, string mark, string time,
            HttpPostedFileBase questionImage)
        {

            int chapID = int.Parse(chid);
            Question question = new Question();
            Chapter chapter = db.Chapters.Find(chapID);
            question.ChapterID = chapter.ChID;
            question.Text = questionText;
            question.Qtype = 1;
            //check if mark is null
            if (!mark.Trim().Equals(""))
            {
                question.Mark = float.Parse(mark);
            }
            //check if time is null
            if (!time.Trim().Equals(""))
            {
                question.Time = int.Parse(time);
            }

            /*if (questionImage != null && questionImage.ContentLength>0)
            {  
                    var fileName = Path.GetFileName(questionImage.FileName);
                    var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                questionImage.SaveAs(path);
                

                question.ImageData = new byte[questionImage.ContentLength];
                questionImage.InputStream.Read(question.ImageData, 0, questionImage.ContentLength);
            }
            else
            {
                Debug.WriteLine("none");
            }*/


            string mixChoice = collection["mixChoice"].Trim().ToString();
            //get mix choice check box value if 1 is true, 0 is false
            if (mixChoice.Equals("1"))
            {
                question.MixChoice = true;

            }
            else
            {
                question.MixChoice = false;

            }
            db.Questions.Add(question);
            db.SaveChanges();

            int qid = int.Parse(db.Questions.OrderByDescending(q => q.QID).Select(q => q.QID).First().ToString());



            //check if there are text in option 1
            if (collection["option1"] != null && !collection["option1"].Equals(""))
            {
                QuestionAnswer qa1 = new QuestionAnswer();
                qa1.QuestionID = qid;
                qa1.Text = collection["option1"].ToString();
                //get check box value if 1 is correct answer, 0 is wrong answer
                if (collection["cb-option1"].ToString().Trim().Equals("0"))
                {
                    qa1.IsCorrect = false;
                }
                else
                {
                    qa1.IsCorrect = true;
                }
                db.QuestionAnswers.Add(qa1);
            }


            if (collection["option2"] != null && !collection["option2"].Equals(""))
            {
                QuestionAnswer qa2 = new QuestionAnswer();
                qa2.QuestionID = qid;
                qa2.Text = collection["option2"].ToString();
                if (collection["cb-option2"].ToString().Trim().Equals("0"))
                {
                    qa2.IsCorrect = false;
                }
                else
                {
                    qa2.IsCorrect = true;
                }
                db.QuestionAnswers.Add(qa2);
            }


            if (collection["option3"] != null && !collection["option3"].Equals(""))
            {
                QuestionAnswer qa3 = new QuestionAnswer();
                qa3.QuestionID = qid;
                qa3.Text = collection["option3"].ToString();
                if (collection["cb-option3"].ToString().Trim().Equals("0"))
                {
                    qa3.IsCorrect = false;
                }
                else
                {
                    qa3.IsCorrect = true;
                }
                db.QuestionAnswers.Add(qa3);
            }


            if (collection["option4"] != null && !collection["option4"].Equals(""))
            {
                QuestionAnswer qa4 = new QuestionAnswer();
                qa4.QuestionID = qid;
                qa4.Text = collection["option4"].ToString();
                if (collection["cb-option4"].ToString().Trim().Equals("0"))
                {
                    qa4.IsCorrect = false;
                }
                else
                {
                    qa4.IsCorrect = true;
                }
                db.QuestionAnswers.Add(qa4);
            }


            if (collection["option5"] != null && !collection["option5"].Equals(""))
            {
                Debug.WriteLine("okokokoko" + collection["option5"]);
                QuestionAnswer qa5 = new QuestionAnswer();
                qa5.QuestionID = qid;
                qa5.Text = collection["option5"].ToString();
                if (collection["cb-option5"].ToString().Trim().Equals("0"))
                {
                    qa5.IsCorrect = false;
                }
                else
                {
                    qa5.IsCorrect = true;
                }
                db.QuestionAnswers.Add(qa5);
            }


            if (collection["option6"] != null && !collection["option6"].Equals(""))
            {
                QuestionAnswer qa6 = new QuestionAnswer();
                qa6.QuestionID = qid;
                qa6.Text = collection["option6"].ToString();
                if (collection["cb-option6"].ToString().Trim().Equals("0"))
                {
                    qa6.IsCorrect = false;
                }
                else
                {
                    qa6.IsCorrect = true;
                }
                db.QuestionAnswers.Add(qa6);
            }

            db.SaveChanges();

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);


        }

        public ActionResult CreateReadingQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            return View();
        }


        public ActionResult EditMultipleChoiceQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            return View();
        }
    }
}