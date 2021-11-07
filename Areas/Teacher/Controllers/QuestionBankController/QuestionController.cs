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
            else if (question.Qtype == 5)
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

        public ActionResult CreateMatchingQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            return View();
        }

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
            foreach(string right in rightAnswer)
            {
                if (right != null && !right.Trim().Equals(""))
                {
                    columnA = columnA + right +";";
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
            for(int i = 0; i < rightAnswer.Length; i++)
            {
                if(rightAnswer[i]!=null&& !rightAnswer[i].Trim().Equals(""))
                {
                    solution = solution + rightAnswer[i] + "-" + leftAnswer[i] + ";";
                }
            }

            matching.ColumnA = columnA.Substring(0, columnA.Length - 1);
            matching.ColumnB = columnB.Substring(0, columnB.Length - 1);
            matching.Solution = solution.Substring(0, solution.Length - 1);

            db.MatchQuestions.Add(matching);
            db.SaveChanges();

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);

        }

        public ActionResult EditMatchingQuestion(string qid)
        {
            int matchingID = int.Parse(qid);
            MatchQuestion matching = db.MatchQuestions.Find(matchingID);
            ViewBag.ChapterID = matching.ChapterId;
            ViewBag.Question = matching;
            ViewBag.ColumnA = matching.ColumnA.Split(new char[] { ';' });
            ViewBag.ColumnB = matching.ColumnB.Split(new char[] { ';' });
            return View();
        }

        [HttpPost]
        public ActionResult EditMatchingQuestion(string chid, string qid, FormCollection collection, string mark, string time) {
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
            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);
        }

        public ActionResult CreateIndicateMistakeQuestion(string chid)
        {
            int chapID = int.Parse(chid);
            ViewBag.ChapterID = chapID;
            return View();
        }
        [HttpPost]
        public ActionResult CreateIndicateMistakeQuestion(string chid, string questionText, FormCollection collection, string mark, string time,
           HttpPostedFileBase questionImage)
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
                db.SaveChanges();

            }
            ////////////////////////////////////////////

           /* string[] splitResults = Regex.Split(questionText, @"\(.*?\)");

            foreach (var str in splitResults)
            {
                Debug.WriteLine(str);
            }*/

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);

        }

        public ActionResult EditIndicateMistakeQuestion(string qid)
        {
            int questionID = int.Parse(qid);
            Question question = db.Questions.Find(questionID);
            ViewBag.ChapterID = question.ChapterID;
            ViewBag.Question = question;
            return View();
        }

        [HttpPost]
        public ActionResult EditIndicateMistakeQuestion(string qid, string chid, string questionText, FormCollection collection, string mark, string time)
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

            return Redirect("~/Teacher/Question/ViewQuestionByChapter?chid=" + chapID);
        }


    }
}