using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
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
            ViewBag.CourseName = chapter.Course.Name;
            ViewBag.Chapter = chapter;
            ViewBag.QuestionType = db.QuestionTypes.ToList();
            ViewBag.CountQuest = db.Questions.Where(q => q.ChapterID == chapID).Count() + db.MatchQuestions.Where(m => m.ChapterId == chapID).Count();
            return View();
        }

        //show question list
        public ActionResult ShowQuestionsList(string chid, string qtype, string searchText)
        {
            int chapID = int.Parse(chid);

            var questions = db.Questions.Where(q => q.ChapterID == chapID).ToList();
            var matchings = db.MatchQuestions.Where(m => m.ChapterId == chapID).ToList();
            List<Question> qList = new List<Question>();
            List<MatchQuestion> mList = new List<MatchQuestion>();

            if (qtype != null)
            {
                int qTypeID = int.Parse(qtype);
                if (qTypeID == -1)
                {
                    mList = matchings;
                    qList = questions;
                }
                else
                {
                    if (qTypeID == 5)
                    {
                        mList = matchings;
                    }
                    else
                    {
                        qList = questions.Where(q => q.Qtype == qTypeID).ToList();
                    }

                }

            }

            if (searchText != null && !searchText.Equals(""))
            {
                if (mList != null)
                {
                    mList = mList.Where(m => m.ColumnA.ToLower().Contains(searchText.ToLower().Trim()) ||
                                    m.ColumnB.ToLower().Contains(searchText.ToLower().Trim())).ToList();
                }
                if (qList != null)
                {
                    qList = qList.Where(q => q.Text.ToLower().Contains(searchText.ToLower().Trim())).ToList();
                }

            }

            dynamic dyQuestions = new ExpandoObject();
            dyQuestions.Questions = qList;
            dyQuestions.Matchings = mList;
            ViewBag.CountQuest = qList.Count() + mList.Count();

            return PartialView("_ShowQuestionsList", dyQuestions);
        }

        //delete question
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

                        //delete question in a quiz
                        string qIdAndType = qid + "-" + question.QuestionType.Name;
                        var quizContainQuests = db.Quizs.Where(qz => qz.Questions.Contains(qIdAndType));
                        if (quizContainQuests != null)
                        {
                            foreach (Quiz qz in quizContainQuests)
                            {
                                string newSet = "";
                                string[] questSet = qz.Questions.Split(new char[] { ';' });
                                foreach (string set in questSet)
                                {
                                    if (!set.Equals(qIdAndType))
                                    {
                                        newSet = newSet + set + ";";
                                    }
                                }

                                qz.Time = qz.Time - question.Time;
                                qz.Mark = qz.Mark - question.Mark;
                                qz.NumOfQuestion = qz.NumOfQuestion - 1;
                                if (newSet != "")
                                {
                                    qz.Questions = newSet.Substring(0, newSet.Length - 1);
                                }
                                else
                                {
                                    qz.Questions = null;
                                }
                                db.Entry(qz).State = EntityState.Modified;
                            }
                        }
                        //delete question
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

                        //delete match question inside quiz
                        string qIdAndType = mid + "-" + "Matching";
                        var quizContainQuests = db.Quizs.Where(qz => qz.Questions.Contains(qIdAndType));
                        if (quizContainQuests != null)
                        {
                            foreach (Quiz qz in quizContainQuests)
                            {
                                string newSet = "";
                                string[] questSet = qz.Questions.Split(new char[] { ';' });
                                foreach (string set in questSet)
                                {
                                    if (!set.Equals(qIdAndType))
                                    {
                                        newSet = newSet + set + ";";
                                    }
                                }
                                qz.Time = qz.Time - matchQuest.Time;
                                qz.Mark = qz.Mark - matchQuest.Mark;
                                qz.NumOfQuestion = qz.NumOfQuestion - 1;
                                if (newSet != "")
                                {
                                    qz.Questions = newSet.Substring(0, newSet.Length - 1);
                                }
                                else
                                {
                                    qz.Questions = null;
                                }
                                db.Entry(qz).State = EntityState.Modified;
                            }
                        }
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


        //edit question
        public ActionResult EditQuestion(string qid)
        {
            int questID = int.Parse(qid);
            var question = db.Questions.Find(questID);
            if (question.Qtype == 1)
            {
                return Redirect("~/Teacher/Question/EditMultipleChoiceQuestion?qid=" + questID);
            }
            else if (question.Qtype == 2)
            {
                return Redirect("~/Teacher/Question/EditReadingQuestion?qid=" + questID);
            }
            else if (question.Qtype == 3)
            {
                return Redirect("~/Teacher/Question/EditFillBlankQuestion?qid=" + questID);
            }
            else if (question.Qtype == 4)
            {
                return Redirect("~/Teacher/Question/EditShortAnswerQuestion?qid=" + questID);
            }
            else
            {
                return Redirect("~/Teacher/Question/EditIndicateMistakeQuestion?qid=" + questID);
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

            if (questionImage != null && questionImage.ContentLength > 0)
            {
                var fileName = Path.GetFileName(questionImage.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                questionImage.SaveAs(path);


                question.ImageData = new byte[questionImage.ContentLength];
                questionImage.InputStream.Read(question.ImageData, 0, questionImage.ContentLength);
                Debug.WriteLine("======" + question.ImageData);
            }
            else
            {
                Debug.WriteLine("none");
            }


            string mixChoice = collection["mixChoice"];
            //get mix choice check box value if 1 is true, 0 is false
            if (mixChoice == null)
            {
                question.MixChoice = false;

            }
            else
            {
                question.MixChoice = true;

            }
            db.Questions.Add(question);
            db.SaveChanges();

            int qid = int.Parse(db.Questions.OrderByDescending(q => q.QID).Select(q => q.QID).First().ToString());

            string[] options = collection["option"].Split(new char[] { ',' });
            string cbOption = collection["cbOption"];
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] != null && !options[i].Trim().Equals(""))
                {
                    QuestionAnswer qa = new QuestionAnswer();
                    qa.QuestionID = qid;
                    qa.Text = options[i];
                    int answerIndex = i + 1;
                    if (cbOption.Contains(answerIndex.ToString()))
                    {
                        Debug.WriteLine(options[i] + "-1--");
                        qa.IsCorrect = true;
                    }
                    else
                    {
                        qa.IsCorrect = false;
                        Debug.WriteLine(options[i] + "-2--");
                    }
                    db.QuestionAnswers.Add(qa);
                }
            }
            db.SaveChanges();

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);


        }


        public ActionResult EditMultipleChoiceQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            return View();
        }

        [HttpPost]
        public ActionResult EditMultipleChoiceQuestion(string qid, string chid, string questionText, FormCollection collection, string mark, string time,
            HttpPostedFileBase questionImage)
        {
            int questionID = int.Parse(qid);
            int chapID = int.Parse(chid);
            Question question = db.Questions.Find(questionID);
            question.Text = questionText;
            question.Mark = float.Parse(mark);
            question.Time = int.Parse(time);
            string mixChoice = collection["mixChoice"];
            //get mix choice check box value if 1 is true, 0 is false
            if (mixChoice == null)
            {
                question.MixChoice = false;
            }
            else
            {
                question.MixChoice = true;

            }
            db.Entry(question).State = EntityState.Modified;

            var answerList = db.QuestionAnswers.Where(qa => qa.QuestionID == questionID);
            //delete the old answer
            foreach (var a in answerList)
            {
                db.QuestionAnswers.Remove(a);
            }

            db.SaveChanges();

            string[] options = collection["option"].Split(new char[] { ',' });
            string[] cbOption = collection["cbOption"].Split(new char[] { ',' });

            //new answer
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] != null && !options[i].Trim().Equals(""))
                {
                    QuestionAnswer qa = new QuestionAnswer();
                    qa.QuestionID = questionID;
                    /*Debug.WriteLine(i + "=-=-=" + options[i]);*/
                    qa.Text = options[i];
                    int answerIndex = i + 1;
                    if (cbOption.Contains(answerIndex.ToString()))
                    {
                        /* Debug.WriteLine(options[i] + "-1--");*/
                        qa.IsCorrect = true;
                    }
                    else
                    {
                        qa.IsCorrect = false;
                        /* Debug.WriteLine(options[i] + "-2--");*/
                    }
                    db.QuestionAnswers.Add(qa);
                }

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

        [HttpPost]
        public ActionResult CreateReadingQuestion(string chid, string questionText, FormCollection collection, string mark, string time, string paragraph,
            HttpPostedFileBase questionImage)
        {

            int chapID = int.Parse(chid);
            Passage passage = new Passage();
            passage.Text = paragraph;
            passage.ChapterID = chapID;
            db.Passages.Add(passage);
            db.SaveChanges();
            int pid = int.Parse(db.Passages.OrderByDescending(p => p.PID).Where(p => p.ChapterID == chapID).Select(p => p.PID).First().ToString());

            Question question = new Question();
            Chapter chapter = db.Chapters.Find(chapID);
            question.ChapterID = chapter.ChID;
            question.PassageID = pid;
            question.Text = questionText;
            question.Qtype = 2;
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

            if (questionImage != null && questionImage.ContentLength > 0)
            {
                var fileName = Path.GetFileName(questionImage.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                questionImage.SaveAs(path);


                question.ImageData = new byte[questionImage.ContentLength];
                questionImage.InputStream.Read(question.ImageData, 0, questionImage.ContentLength);
                Debug.WriteLine("======" + question.ImageData);
            }
            else
            {
                Debug.WriteLine("none");
            }


            string mixChoice = collection["mixChoice"];
            //get mix choice check box value if 1 is true, 0 is false
            if (mixChoice == null)
            {
                question.MixChoice = false;

            }
            else
            {
                question.MixChoice = true;

            }
            db.Questions.Add(question);
            db.SaveChanges();

            int qid = int.Parse(db.Questions.OrderByDescending(q => q.QID).Where(q => q.ChapterID == chapID).Select(q => q.QID).First().ToString());

            string[] options = collection["option"].Split(new char[] { ',' });
            string cbOption = collection["cbOption"];
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] != null && !options[i].Trim().Equals(""))
                {
                    QuestionAnswer qa = new QuestionAnswer();
                    qa.QuestionID = qid;
                    qa.Text = options[i];
                    int answerIndex = i + 1;
                    if (cbOption.Contains(answerIndex.ToString()))
                    {
                        Debug.WriteLine(options[i] + "-1--");
                        qa.IsCorrect = true;
                    }
                    else
                    {
                        qa.IsCorrect = false;
                        Debug.WriteLine(options[i] + "-2--");
                    }
                    db.QuestionAnswers.Add(qa);
                }
            }
            db.SaveChanges();

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);


        }

        public ActionResult EditReadingQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            return View();
        }

        [HttpPost]
        public ActionResult EditReadingQuestion(string qid, string chid, string questionText, FormCollection collection, string mark, string time,
            string paragraph, HttpPostedFileBase questionImage)
        {
            int questionID = int.Parse(qid);
            int chapID = int.Parse(chid);
            Question question = db.Questions.Find(questionID);
            //edit passage
            var passage = question.Passage;
            passage.Text = paragraph;
            db.Entry(passage).State = EntityState.Modified;

            question.Text = questionText;
            question.Mark = float.Parse(mark);
            question.Time = int.Parse(time);
            string mixChoice = collection["mixChoice"];
            //get mix choice check box value if 1 is true, 0 is false
            if (mixChoice == null)
            {
                question.MixChoice = false;
            }
            else
            {
                question.MixChoice = true;

            }
            db.Entry(question).State = EntityState.Modified;

            var answerList = db.QuestionAnswers.Where(qa => qa.QuestionID == questionID);
            //delete the old answer
            foreach (var a in answerList)
            {
                db.QuestionAnswers.Remove(a);
            }

            db.SaveChanges();

            string[] options = collection["option"].Split(new char[] { ',' });
            string[] cbOption = collection["cbOption"].Split(new char[] { ',' });

            //new answer
            for (int i = 0; i < options.Length; i++)
            {
                if (options[i] != null && !options[i].Trim().Equals(""))
                {
                    QuestionAnswer qa = new QuestionAnswer();
                    qa.QuestionID = questionID;
                    Debug.WriteLine(i + "=-=-=" + options[i]);
                    qa.Text = options[i];
                    int answerIndex = i + 1;
                    if (cbOption.Contains(answerIndex.ToString()))
                    {
                        Debug.WriteLine(options[i] + "-1--");
                        qa.IsCorrect = true;
                    }
                    else
                    {
                        qa.IsCorrect = false;
                        Debug.WriteLine(options[i] + "-2--");
                    }
                    db.QuestionAnswers.Add(qa);
                }

            }
            db.SaveChanges();


            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);
        }

        public ActionResult CreateShortAnswerQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            return View();
        }

        [HttpPost]
        public ActionResult CreateShortAnswerQuestion(string chid, string questionText, FormCollection collection, string mark, string time,
            HttpPostedFileBase questionImage)
        {
            int chapID = int.Parse(chid);
            Question question = new Question();
            Chapter chapter = db.Chapters.Find(chapID);
            question.ChapterID = chapter.ChID;
            question.Text = questionText;
            question.Qtype = 4;
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

            if (questionImage != null && questionImage.ContentLength > 0)
            {
                var fileName = Path.GetFileName(questionImage.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                questionImage.SaveAs(path);


                question.ImageData = new byte[questionImage.ContentLength];
                questionImage.InputStream.Read(question.ImageData, 0, questionImage.ContentLength);
                Debug.WriteLine("======" + question.ImageData);
            }
            else
            {
                Debug.WriteLine("none");
            }
            db.Questions.Add(question);
            db.SaveChanges();

            int qid = int.Parse(db.Questions.OrderByDescending(q => q.QID).Where(q => q.ChapterID == chapID).Select(q => q.QID).First().ToString());

            string answer = collection["answer"];
           
            if (answer != null && !answer.Trim().Equals(""))
            {
                QuestionAnswer qa = new QuestionAnswer();
                qa.QuestionID = qid;
                qa.Text = answer;
                qa.IsCorrect = true;
                db.QuestionAnswers.Add(qa);
            }
           
            
            string altAnswer = collection["altAnswer"];
            
            if (altAnswer != null && !altAnswer.Trim().Equals(""))
            {
                QuestionAnswer altQa = new QuestionAnswer();
                altQa.QuestionID = qid;
                altQa.Text = answer;
                altQa.IsCorrect = true;
                db.QuestionAnswers.Add(altQa);
            }
            

            db.SaveChanges();

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);

        }

        
        public ActionResult EditShortAnswerQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            return View();
        }

        [HttpPost]
        public ActionResult EditShortAnswerQuestion(string qid, string chid, string questionText, FormCollection collection, string mark, string time,
            HttpPostedFileBase questionImage)
        {
            int questionID = int.Parse(qid);
            int chapID = int.Parse(chid);
            Question question = db.Questions.Find(questionID);
            question.Text = questionText;
            question.Mark = float.Parse(mark);
            question.Time = int.Parse(time);
            db.Entry(question).State = EntityState.Modified;

            var answerList = db.QuestionAnswers.Where(a => a.QuestionID == questionID);
            //delete the old answer
            foreach (var a in answerList)
            {
                db.QuestionAnswers.Remove(a);
            }

            db.SaveChanges();

            //add new answer
            string answer = collection["answer"];
            
            if (answer != null && !answer.Trim().Equals(""))
            {
                QuestionAnswer qa = new QuestionAnswer();
                qa.QuestionID = questionID;
                qa.Text = answer;
                qa.IsCorrect = true;
                db.QuestionAnswers.Add(qa);
            }
            

            string altAnswer = collection["altAnswer"];
           
            if (altAnswer != null && !altAnswer.Trim().Equals(""))
            {
                QuestionAnswer altQa = new QuestionAnswer();
                altQa.QuestionID = questionID;
                altQa.Text = altAnswer;
                altQa.IsCorrect = true;
                db.QuestionAnswers.Add(altQa);
            }
            
            db.SaveChanges();

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);
        }
    }
}