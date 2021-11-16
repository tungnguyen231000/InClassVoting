using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

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
        public ActionResult ViewQuestionByChapter(string chid, string qtype, string searchText, int? i)
        {
            int chapID = int.Parse(chid);
            var chapter = db.Chapters.Find(chapID); 
            var questions = db.Questions.Where(q => q.ChapterID == chapID).ToList();
            var matchings = db.MatchQuestions.Where(m => m.ChapterId == chapID).ToList();
            List<Question> qList = new List<Question>();
            List<MatchQuestion> mList = new List<MatchQuestion>();

            if (qtype == null || qtype.Trim().Equals(""))
            {
                qtype = "-1";
            }
            //get question type by dropdown list
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


            // check if user search by question text
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

            foreach (var m in mList)
            {
                Question q = new Question();
                q.Text = m.ColumnA + "//" + m.ColumnB;
                q.Qtype = 5;
                q.Mark = m.Mark;
                q.QID = m.MID;
                qList.Add(q);
            }

            if (i == null)
            {
                i = 1;
            }

            ViewBag.Page = i;
            //fix question number (question no)
            ViewBag.QuestCount = (i - 1) * 3;
            /*else
            {
                ViewBag.questCount = questCount;
            }*/

            //get search text
            if (searchText == null)
            {
                ViewBag.Search = "";
            }
            else
            {
                ViewBag.Search = searchText;
            }

            //get question type

            if (qtype == null)
            {
                ViewBag.QType = "";
            }
            else
            {
                ViewBag.QType = qtype;
            }
            /*Debug.WriteLine("///" + qtype);*/

            ViewBag.CountQuest = db.Questions.Count(q=>q.ChapterID==chapID) + db.MatchQuestions.Count(m => m.ChapterId == chapID);
            ViewBag.CourseName = chapter.Course.Name;
            ViewBag.Chapter = chapter;
            ViewBag.QuestionType = db.QuestionTypes.ToList();
           
            return View(qList.ToPagedList(i ?? 1, 3));
        }

        //show question list
       /* public ActionResult ShowQuestionsList(string chid, string qtype, string searchText, int? i, int? questCount)
        {
            int chapID = int.Parse(chid);
            var chapter = db.Chapters.Find(chapID);
            var questions = db.Questions.Where(q => q.ChapterID == chapID).ToList();
            var matchings = db.MatchQuestions.Where(m => m.ChapterId == chapID).ToList();
            List<Question> qList = new List<Question>();
            List<MatchQuestion> mList = new List<MatchQuestion>();

            if (qtype == null || qtype.Trim().Equals(""))
            {
                qtype = "-1";
            }
            //get question type by dropdown list
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


            // check if user search by question text
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

            foreach (var m in mList)
            {
                Question q = new Question();
                q.Text = m.ColumnA + "//" + m.ColumnB;
                q.Qtype = 5;
                q.Mark = m.Mark;
                q.QID = m.MID;
                qList.Add(q);
            }
            ViewBag.CountQuest = qList.Count();

            ViewBag.QuestCount = questCount;
            ViewBag.Chapter = chapter;
            ViewBag.QType = qtype;
            *//*Debug.WriteLine("///" + qtype);*//*
            return PartialView("_ShowQuestionsList", qList.ToPagedList(i ?? 1, 3));
        }*/

        //delete question
        public ActionResult DeleteQuestion(string chapterId, FormCollection collection)
        {
            int chapID = int.Parse(chapterId);
            var questions = collection["questionIdAndType"];

            //check if question id and matching question id list is null
            if (questions == null)
            {
                return Redirect(Request.UrlReferrer.ToString());

            }
            else
            {
                //get question set List
                if (questions != null)
                {
                    string[] qSet = questions.Split(new char[] { ',' });

                    foreach (string set in qSet)
                    {
                        int? minusTime = 0;
                        double? minusMark = 0;

                        string qtypeCheck = set.Substring(set.Length - 1, 1);
                        //if it is matching question
                        if (qtypeCheck.Equals("5"))
                        {
                            int mid = int.Parse(set.Substring(0, set.Length - 2));
                            var matchQuest = db.MatchQuestions.Find(mid);
                            minusMark = minusMark + matchQuest.Mark;
                            minusTime = minusTime + matchQuest.Time;
                            db.MatchQuestions.Remove(matchQuest);

                        }
                        else
                        {
                            int qid = int.Parse(set.Substring(0, set.Length - 2));
                            var question = db.Questions.Find(qid);
                            var qaList = db.QuestionAnswers.Where(qa => qa.QuestionID == qid).ToList();
                            //remove answer of question
                            foreach (QuestionAnswer qa in qaList)
                            {
                                db.QuestionAnswers.Remove(qa);
                            }
                            minusMark = minusMark + question.Mark;
                            minusTime = minusTime + question.Time;
                            db.Questions.Remove(question);
                        }
                        var quizContainQuests = db.Quizs.Where(qz => qz.Questions.Contains(set));
                        //delete question inside quest
                        foreach (var quiz in quizContainQuests)
                        {
                            string newSet = "";
                            string[] questSet = quiz.Questions.Split(new char[] { ';' });
                            foreach (string qIdAndType in questSet)
                            {
                                if (!qIdAndType.Equals(set))
                                {
                                    newSet = newSet + qIdAndType + ";";
                                }
                            }
                            quiz.Time = quiz.Time - minusTime;
                            quiz.Mark = quiz.Mark - minusMark;
                            quiz.NumOfQuestion = quiz.NumOfQuestion - 1;
                            if (newSet != "")
                            {
                                quiz.Questions = newSet.Substring(0, newSet.Length - 1);
                            }
                            else
                            {
                                quiz.Questions = null;
                            }
                            db.Entry(quiz).State = EntityState.Modified;
                        }
                    }

                    db.SaveChanges();
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }


        //edit question
        public ActionResult EditQuestion(string qid, string qtype)
        {
            int questID = int.Parse(qid);
            int qTypeID = int.Parse(qtype);
            /*var question = db.Questions.Find(questID);*/
            if (qTypeID == 1)
            {
                return Redirect("~/Teacher/Question/EditMultipleChoiceQuestion?qid=" + questID);
            }
            else if (qTypeID == 2)
            {
                return Redirect("~/Teacher/Question/EditReadingQuestion?qid=" + questID);
            }
            else if (qTypeID == 3)
            {
                return Redirect("~/Teacher/Question/EditFillBlankQuestion?qid=" + questID);
            }
            else if (qTypeID == 4)
            {
                return Redirect("~/Teacher/Question/EditShortAnswerQuestion?qid=" + questID);
            }
            else if (qTypeID == 5)
            {
                return Redirect("~/Teacher/Question/EditMatchingQuestion?qid=" + questID);
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

        //add new multiple choice question
        [HttpPost]
        public ActionResult CreateMultipleChoiceQuestion(string chid, string questionText, FormCollection collection, string mark, string time,
            HttpPostedFileBase imgfile)
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

            //add image
            if (imgfile != null && imgfile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(imgfile.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                imgfile.SaveAs(path);


                question.ImageData = new byte[imgfile.ContentLength];
                imgfile.InputStream.Read(question.ImageData, 0, imgfile.ContentLength);
                /*Debug.WriteLine("======" + question.ImageData);*/
            }
            /*else
            {
                Debug.WriteLine("none");
            }*/


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
            int lastPage = 0;
            if (db.Questions.Count(q => q.ChapterID == chapID) % 3 == 0)
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3);
            }
            else
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3 + 1);
            }

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID + "&i=" + lastPage);
        }

        //show page to edit multiple choice question
        public ActionResult EditMultipleChoiceQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //save multiple choice question after edit
        [HttpPost]
        public ActionResult EditMultipleChoiceQuestion(string previousUrl, string qid, string chid, string questionText, FormCollection collection, string mark, string time,
            HttpPostedFileBase imgfile)
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

            //change image
            if (imgfile != null && imgfile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(imgfile.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                imgfile.SaveAs(path);
                question.ImageData = new byte[imgfile.ContentLength];
                imgfile.InputStream.Read(question.ImageData, 0, imgfile.ContentLength);
                /*Debug.WriteLine("======" + question.ImageData);*/
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


            return Redirect(previousUrl);
        }

        //show page to create reading question
        public ActionResult CreateReadingQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //add new reading question
        [HttpPost]
        public ActionResult CreateReadingQuestion(string chid, string questionText, FormCollection collection, string mark, string time, string paragraph,
            HttpPostedFileBase imgfile)
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

            //add image
            if (imgfile != null && imgfile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(imgfile.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                imgfile.SaveAs(path);


                question.ImageData = new byte[imgfile.ContentLength];
                imgfile.InputStream.Read(question.ImageData, 0, imgfile.ContentLength);
                /*Debug.WriteLine("======" + question.ImageData);*/
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
            int lastPage = 0;
            if (db.Questions.Count(q => q.ChapterID == chapID) % 3 == 0)
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3);
            }
            else
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3 + 1);
            }

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID + "&i=" + lastPage);

        }

        //show page to edit reading question
        public ActionResult EditReadingQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //savereading question after edit
        [HttpPost]
        public ActionResult EditReadingQuestion(string previousUrl, string qid, string chid, string questionText, FormCollection collection, string mark, string time,
            string paragraph, HttpPostedFileBase imgfile)
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
            //change image
            if (imgfile != null && imgfile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(imgfile.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                imgfile.SaveAs(path);
                question.ImageData = new byte[imgfile.ContentLength];
                imgfile.InputStream.Read(question.ImageData, 0, imgfile.ContentLength);
                /*Debug.WriteLine("======" + question.ImageData);*/
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


            return Redirect(previousUrl);
        }

        //show page to create short answer question
        public ActionResult CreateShortAnswerQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //add new short answer question
        [HttpPost]
        public ActionResult CreateShortAnswerQuestion(string chid, string questionText, FormCollection collection, string mark, string time,
            HttpPostedFileBase imgfile)
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

            //add image
            if (imgfile != null && imgfile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(imgfile.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                imgfile.SaveAs(path);


                question.ImageData = new byte[imgfile.ContentLength];
                imgfile.InputStream.Read(question.ImageData, 0, imgfile.ContentLength);
                /*Debug.WriteLine("======" + question.ImageData);*/
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
            int lastPage = 0;
            if (db.Questions.Count(q => q.ChapterID == chapID) % 3 == 0)
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3);
            }
            else
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3 + 1);
            }

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID + "&i=" + lastPage);

        }

        //show page to edit short answer question
        public ActionResult EditShortAnswerQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //saveedit short answer question after edit
        [HttpPost]
        public ActionResult EditShortAnswerQuestion(string previousUrl, string qid, string chid, string questionText, FormCollection collection, string mark, string time,
            HttpPostedFileBase imgfile)
        {
            int questionID = int.Parse(qid);
            int chapID = int.Parse(chid);
            Question question = db.Questions.Find(questionID);
            question.Text = questionText;
            question.Mark = float.Parse(mark);
            question.Time = int.Parse(time);
            //change image
            if (imgfile != null && imgfile.ContentLength > 0)
            {
                var fileName = Path.GetFileName(imgfile.FileName);
                var path = Path.Combine(Server.MapPath("~/App_Data/images"), fileName);
                imgfile.SaveAs(path);
                question.ImageData = new byte[imgfile.ContentLength];
                imgfile.InputStream.Read(question.ImageData, 0, imgfile.ContentLength);
                /*Debug.WriteLine("======" + question.ImageData);*/
            }
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

            return Redirect(previousUrl);
        }

        //show page to create matching question
        public ActionResult CreateMatchingQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //add new matching question
        [HttpPost]
        public ActionResult CreateMatchingQuestion(string chid, FormCollection collection, string mark, string time)
        {
            int chapID = int.Parse(chid);
            MatchQuestion matching = new MatchQuestion();
            Chapter chapter = db.Chapters.Find(chapID);
            matching.ChapterId = chapter.ChID;

            //check if mark is null
            if (!mark.Trim().Equals(""))
            {
                matching.Mark = float.Parse(mark);
            }
            //check if time is null
            if (!time.Trim().Equals(""))
            {
                matching.Time = int.Parse(time);
            }

            string columnA = "";
            string columnB = "";
            string solution = "";
            string[] rightAnswer = collection["answerLeft"].Split(new char[] { ',' });
            string[] leftAnswer = collection["answerRight"].Split(new char[] { ',' });

            //add right column to string
            foreach (string right in rightAnswer)
            {
                if (right != null && !right.Trim().Equals(""))
                {
                    columnA = columnA + right + ";";
                }

            }

            //add left column to string
            foreach (string left in leftAnswer)
            {
                if (left != null && !left.Trim().Equals(""))
                {
                    columnB = columnB + left + ";";
                }

            }

            //add solution
            for (int i = 0; i < rightAnswer.Length; i++)
            {
                if (rightAnswer[i] != null && !rightAnswer[i].Trim().Equals(""))
                {
                    solution = solution + rightAnswer[i] + "-" + leftAnswer[i] + ";";
                }
            }

            matching.ColumnA = columnA.Substring(0, columnA.Length - 1);
            matching.ColumnB = columnB.Substring(0, columnB.Length - 1);
            matching.Solution = solution.Substring(0, solution.Length - 1);

            db.MatchQuestions.Add(matching);
            db.SaveChanges();

            int lastPage = ((db.Questions.Count(q => q.ChapterID == chapID) + db.MatchQuestions.Count(m => m.ChapterId == chapID)) / 3 + 1);
            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID + "&i=" + lastPage);

        }

        //show page to edit matching question
        public ActionResult EditMatchingQuestion(string qid)
        {
            int matchingID = int.Parse(qid);
            MatchQuestion matching = db.MatchQuestions.Find(matchingID);
            ViewBag.ChapterID = matching.ChapterId;
            ViewBag.Question = matching;
            ViewBag.ColumnA = matching.ColumnA.Split(new char[] { ';' });
            ViewBag.ColumnB = matching.ColumnB.Split(new char[] { ';' });
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //save matching question after edit
        [HttpPost]
        public ActionResult EditMatchingQuestion(string previousUrl, string chid, string qid, FormCollection collection, string mark, string time)
        {
            int matchingID = int.Parse(qid);
            int chapID = int.Parse(chid);
            MatchQuestion matching = db.MatchQuestions.Find(matchingID);
            matching.Mark = float.Parse(mark);
            matching.Time = int.Parse(time);

            string columnA = "";
            string columnB = "";
            string solution = "";
            string[] rightAnswer = collection["answerLeft"].Split(new char[] { ',' });
            string[] leftAnswer = collection["answerRight"].Split(new char[] { ',' });

            //edit right column
            foreach (string right in rightAnswer)
            {
                if (right != null && !right.Trim().Equals(""))
                {
                    columnA = columnA + right + ";";
                }

            }

            //edit left column
            foreach (string left in leftAnswer)
            {
                if (left != null && !left.Trim().Equals(""))
                {
                    columnB = columnB + left + ";";
                }

            }

            //edit solution
            for (int i = 0; i < rightAnswer.Length; i++)
            {
                if (rightAnswer[i] != null && !rightAnswer[i].Trim().Equals(""))
                {
                    solution = solution + rightAnswer[i] + "-" + leftAnswer[i] + ";";
                }
            }

            matching.ColumnA = columnA.Substring(0, columnA.Length - 1);
            matching.ColumnB = columnB.Substring(0, columnB.Length - 1);
            matching.Solution = solution.Substring(0, solution.Length - 1);
            db.Entry(matching).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect(previousUrl);
        }

        //show page to create indicate mistake question
        public ActionResult CreateIndicateMistakeQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //add new indicate mistake question
        [HttpPost]
        public ActionResult CreateIndicateMistakeQuestion(string chid, string questionText, FormCollection collection, string mark, string time)
        {
            int chapID = int.Parse(chid);
            Question question = new Question();
            Chapter chapter = db.Chapters.Find(chapID);
            question.ChapterID = chapter.ChID;
            question.Text = questionText;
            question.Qtype = 6;
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

            db.Questions.Add(question);
            db.SaveChanges();

            int qid = int.Parse(db.Questions.OrderByDescending(q => q.QID).Where(q => q.ChapterID == chapID).Select(q => q.QID).First().ToString());

            //add correct answer
            string correctAnswer = collection["answer"];
            QuestionAnswer answer = new QuestionAnswer();
            answer.QuestionID = qid;
            answer.Text = correctAnswer.Trim();
            answer.IsCorrect = true;
            db.QuestionAnswers.Add(answer);

            //get incorrect inside round bracket
            List<string> answerList = new List<string>();
            Regex regex = new Regex(@"\(([^()]+)\)*");
            foreach (Match match in regex.Matches(questionText))
            {
                string ans = match.Value;
                answerList.Add(ans);
            }

            //add incorrect answer to db
            if (answerList != null)
            {
                foreach (string ans in answerList)
                {
                    string trimBracketAns = ans.Trim().Substring(1, ans.Length - 2);
                    QuestionAnswer qa = new QuestionAnswer();
                    qa.QuestionID = qid;
                    qa.Text = trimBracketAns;
                    /*Debug.WriteLine(trimBracketAns + "===" + correctAnswer);*/
                    if (!trimBracketAns.Trim().ToLower().Equals(correctAnswer.Trim().ToLower()))
                    {
                        qa.IsCorrect = false;
                        db.QuestionAnswers.Add(qa);
                        /*Debug.WriteLine(trimBracketAns + "=11==" + correctAnswer);*/
                    }

                }


            }
            db.SaveChanges();
            ////////////////////////////////////////////

            /* string[] splitResults = Regex.Split(questionText, @"\(.*?\)");

             foreach (var str in splitResults)
             {
                 Debug.WriteLine(str);
             }*/

            int lastPage = 0;
            if (db.Questions.Count(q => q.ChapterID == chapID) % 3 == 0)
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3);
            }
            else
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3 + 1);
            }

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID + "&i=" + lastPage);

        }

        //show page to edit indicate mistake question
        public ActionResult EditIndicateMistakeQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //save indicate mistake question after edit
        [HttpPost]
        public ActionResult EditIndicateMistakeQuestion(string previousUrl, string qid, string chid, string questionText, FormCollection collection, string mark, string time)
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

            //add new correct answers
            string correctAnswer = collection["answer"];
            QuestionAnswer answer = new QuestionAnswer();
            answer.QuestionID = questionID;
            answer.Text = correctAnswer.Trim();
            answer.IsCorrect = true;
            db.QuestionAnswers.Add(answer);

            List<string> answers = new List<string>();
            //get list of answer inside round bracket "()"
            Regex regex = new Regex(@"\(([^()]+)\)*");
            foreach (Match match in regex.Matches(questionText))
            {
                string ans = match.Value;
                answers.Add(ans);
            }

            //add new incorrect answers
            if (answers != null)
            {
                foreach (string ans in answers)
                {
                    string trimBracketAns = ans.Trim().Substring(1, ans.Length - 2);
                    QuestionAnswer qa = new QuestionAnswer();
                    qa.QuestionID = questionID;
                    qa.Text = trimBracketAns;
                    Debug.WriteLine(trimBracketAns + "===" + correctAnswer);
                    if (!trimBracketAns.Trim().ToLower().Equals(correctAnswer.Trim().ToLower()))
                    {
                        qa.IsCorrect = false;
                        db.QuestionAnswers.Add(qa);
                        Debug.WriteLine(trimBracketAns + "=11==" + correctAnswer);
                    }

                }
                db.SaveChanges();

            }

            return Redirect(previousUrl);
        }


        //show page to create fill blank question
        public ActionResult CreateFillBlankQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //add new fill blank question
        [HttpPost]
        public ActionResult CreateFillBlankQuestion(string chid, string questionText, FormCollection collection, string mark, string time)
        {
            int chapID = int.Parse(chid);
            Question question = new Question();
            Chapter chapter = db.Chapters.Find(chapID);
            question.ChapterID = chapter.ChID;
            question.Text = questionText;
            question.Qtype = 3;
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



            //check if question have given word or not
            string givenWord = collection["givenWord"];
            if (givenWord != null && !givenWord.Trim().Equals(""))
            {
                question.GivenWord = true;

            }
            else
            {
                question.GivenWord = false;
            }

            db.Questions.Add(question);
            db.SaveChanges();

            int qid = int.Parse(db.Questions.OrderByDescending(q => q.QID).Where(q => q.ChapterID == chapID).Select(q => q.QID).First().ToString());

            //check if the question is given word or not
            if (question.GivenWord == true)
            {
                List<string> answerList = new List<string>();
                Regex regex = new Regex(@"\(([^()]+)\)*");
                //get text inside round bracket
                foreach (Match match in regex.Matches(questionText))
                {
                    string ansList = match.Value;
                    answerList.Add(ansList);
                }

                //add correct answer to db
                if (answerList != null)
                {
                    foreach (string ans in answerList)
                    {
                        string trimBracketAns = ans.Trim().Substring(2, ans.Length - 3);
                        string[] choices = trimBracketAns.Split(new char[] { '~' });
                        foreach (string choice in choices)
                        {
                            if (choice.Trim().ToLower().Contains("="))
                            {
                                QuestionAnswer qa = new QuestionAnswer();
                                qa.QuestionID = qid;
                                qa.Text = choice.Substring(1, choice.Length - 1);
                                qa.IsCorrect = true;
                                db.QuestionAnswers.Add(qa);
                            }
                        }
                    }

                }
            }
            else
            {
                Debug.WriteLine("hihi");
                List<string> answerList = new List<string>();
                Regex regex = new Regex(@"\(([^()]+)\)*");
                //get text inside round bracket
                foreach (Match match in regex.Matches(questionText))
                {
                    string ansList = match.Value;
                    answerList.Add(ansList);
                }
                //add correct answer to db
                if (answerList != null)
                {
                    foreach (string ans in answerList)
                    {
                        string correctAns = ans.Trim().Substring(1, ans.Length - 2);
                        QuestionAnswer qa = new QuestionAnswer();
                        qa.QuestionID = qid;
                        qa.Text = correctAns.Trim();
                        qa.IsCorrect = true;
                        db.QuestionAnswers.Add(qa);
                    }
                }
            }


            db.SaveChanges();

            int lastPage = 0;
            if (db.Questions.Count(q => q.ChapterID == chapID) % 3 == 0)
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3);
            }
            else
            {
                lastPage = (db.Questions.Count(q => q.ChapterID == chapID) / 3 + 1);
            }

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID + "&i=" + lastPage);

        }

        //show page to edit fill blank question
        public ActionResult EditFillBlankQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            ViewBag.Previous = Request.UrlReferrer.ToString();
            return View();
        }

        //save fill blank question after edit
        [HttpPost]
        public ActionResult EditFillBlankQuestion(string previousUrl, string qid, string chid, string questionText, FormCollection collection, string mark, string time)
        {
            int questionID = int.Parse(qid);
            int chapID = int.Parse(chid);
            Question question = db.Questions.Find(questionID);
            question.Text = questionText;
            question.Mark = float.Parse(mark);
            question.Time = int.Parse(time);
            //check if question have given word or not
            string givenWord = collection["givenWord"];
            if (givenWord != null && !givenWord.Trim().Equals(""))
            {
                question.GivenWord = true;

            }
            else
            {
                question.GivenWord = false;
            }

            db.Entry(question).State = EntityState.Modified;

            db.SaveChanges();

            return Redirect(previousUrl);
        }


    }
}