using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Text;

namespace InClassVoting.Areas.teacher.Controllers.QuizLibraryController
{
    public class QuizController : Controller
    {
        private DBModel db = new DBModel();
        // GET: teacher/Quiz
        public ActionResult QuizLibrary()
        {
            return View();
        }

        //view quiz inside course page
        public ActionResult ViewQuizByCourse(string cid, string searchText, int? i)
        {
            int courseID = int.Parse(cid);
            ViewBag.Course = db.Courses.Find(courseID);
            ViewBag.CoutnQuiz = db.Quizs.Where(q => q.CourseID == courseID).OrderByDescending(qz => qz.QuizID).Count();
            if (i == null)
            {
                i = 1;
            }

            ViewBag.Page = i;
            //fix quiz number (quiz no)
            ViewBag.QuizCount = (i - 1) * 10;

            //get search text
            if (searchText == null)
            {
                ViewBag.Search = "";
            }
            else
            {
                ViewBag.Search = searchText;
            }


            return View();
        }

        public ActionResult ShowQuizList(string cid, string searchText, int? i, int? quizCount)
        {
            /*Debug.WriteLine("=====" + searchText);*/
            int courseID = int.Parse(cid);
            var qzList = db.Quizs.Where(qz => qz.CourseID == courseID).OrderByDescending(qz => qz.QuizID).ToList();
            List<Quiz> quizzes = new List<Quiz>();
            if (searchText != null && !searchText.Trim().Equals(""))
            {
                quizzes = qzList.Where(qz => qz.QuizName.Trim().ToLower().Contains(searchText.Trim().ToLower())).ToList();
                /*Debug.WriteLine("hjhihi" + searchText);*/

            }
            else
            {
                quizzes = qzList;
                /*Debug.WriteLine("hjhihi" + searchText);*/
            }
            ViewBag.QuizCount = quizCount;
            /*Debug.WriteLine("hjhihi" + quizCount); */
            return PartialView("_ShowQuizList", quizzes.ToPagedList(i ?? 1, 10));
        }

        //View Quiz Detail
        public ActionResult QuizDetail(string qzID, int? i)
        {
            int quizID = int.Parse(qzID);
            var quiz = db.Quizs.Find(quizID);
            if (quiz.Status.Equals("Doing"))
            {
                return Redirect("~/Teacher/Quiz/QuizStarted?qzid=" + qzID);
            }
            else
            {
                var course = db.Courses.Find(quiz.CourseID);
                List<Question> qList = new List<Question>();
                List<MatchQuestion> mList = new List<MatchQuestion>();
                //check if question List is null
                if (quiz.Questions != null && !quiz.Questions.Equals(""))
                {
                    string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
                    Dictionary<int, string> questionSet = new Dictionary<int, string>();
                    Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                    //get question List from test
                    foreach (string questions in quizQuestions)
                    {
                        string[] questAndType = questions.Split(new char[] { '-' });
                        int qtypeID = int.Parse(questAndType[1]);
                        if (qtypeID == 5)
                        {
                            int mID = int.Parse(questAndType[0]);
                            matchingSet.Add(mID, "5");
                        }
                        else
                        {
                            int qID = int.Parse(questAndType[0]);
                            questionSet.Add(qID, questAndType[1]);
                        }

                    }

                    //get question from DB
                    foreach (KeyValuePair<int, string> keyValuePair in questionSet)
                    {
                        var quest = db.Questions.Find(keyValuePair.Key);
                        qList.Add(quest);

                    }
                    //get matching question from DB
                    foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                    {
                        var mQuest = db.MatchQuestions.Find(keyValuePair.Key);
                        /*mList.Add(mQuest);*/
                        Question matchQuest = new Question();
                        matchQuest.QID = mQuest.MID;
                        matchQuest.Text = mQuest.ColumnA + "//" + mQuest.ColumnB;
                        matchQuest.Qtype = 5;
                        matchQuest.Mark = mQuest.Mark;
                        qList.Add(matchQuest);
                    }
                }
                ViewBag.Quiz = quiz;
                ViewBag.Course = course;
                ViewBag.QuestionType = db.QuestionTypes.ToList();
                ViewBag.ChapterList = db.Chapters.Where(ch => ch.CourseID == course.CID);
                int totalQuestion = qList.Count();
                ViewBag.CountQuest = totalQuestion;

                //if quiz have random question
                if (quiz.MixQuestionNumber != null)
                {
                    ViewBag.RandomQuestionNum = quiz.MixQuestionNumber;
                }

                //if quiz shuffle the question
                if (quiz.MixQuestion == true)
                {
                    ViewBag.Shuffle = 1;
                }
                else
                {
                    ViewBag.Shuffle = 0;
                }

                //if pulish mark is true
                if (quiz.PublicResult == true)
                {
                    ViewBag.PublishMark = 1;
                }
                else
                {
                    ViewBag.PublishMark = 0;
                }

                //if pulish answer is true
                if (quiz.PublicAnswer == true)
                {
                    ViewBag.PublicAnswer = 1;
                }
                else
                {
                    ViewBag.PublicAnswer = 0;
                }


                string domain = "https://inclassvoting.azurewebsites.net/Student/Quiz/DoQuizPaperTest?qzID=";
                string parameterPart = ViewBag.Quiz.QuizID.ToString();
                string encodePart = Base64Encode(parameterPart);
                ViewBag.QuizLink = domain + encodePart;

                if (i == null)
                {
                    i = 1;
                }
                else
                {
                    if (totalQuestion % 10 == 0)
                    {
                        if (i > (totalQuestion / 10))
                        {
                            i = (totalQuestion / 10);
                        }
                    }
                }
                if (i == 0)
                {
                    i = null;
                }
                ViewBag.QuestionNo = (i - 1) * 10;
                ViewBag.Page = i;
                return View(qList.ToPagedList(i ?? 1, 10));
            }

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        //Edit Quiz Option
        [HttpPost]
        public ActionResult SaveQuizOption(string qzID, string cbMixQuestions, string rdQuestionNum,
            string cbPublishMark, string cbPublishAnswer, string cbRandomQuestion)
        {
            int quizID = int.Parse(qzID);
            var quiz = db.Quizs.Find(quizID);
            int mixChoice = int.Parse(cbMixQuestions);
            //check if shuffle answer check box is checked
            if (mixChoice == 1)
            {
                quiz.MixQuestion = true;
            }
            else
            {
                quiz.MixQuestion = false;
            }

            int publishMark = int.Parse(cbPublishMark);
            //check if the quiz mark publish option is check
            if (publishMark == 1)
            {
                quiz.PublicResult = true;
            }
            else
            {
                quiz.PublicResult = false;
            }

            int publishAnswer = int.Parse(cbPublishAnswer);
            //check if the quiz answer publish option is check
            if (publishAnswer == 1)
            {
                quiz.PublicAnswer = true;
            }
            else
            {
                quiz.PublicAnswer = false;
            }

            int rdQuestion = int.Parse(cbRandomQuestion);
            if (rdQuestion == 0)
            {
                quiz.MixQuestionNumber = null;
            }
            else
            {
                //get quiz random question number
                if (!rdQuestionNum.Equals("") && rdQuestionNum != null)
                {
                    int numOfRandom = int.Parse(rdQuestionNum);
                    quiz.MixQuestionNumber = numOfRandom;
                }
            }


            db.Entry(quiz).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        //Edit Quiz Name
        [HttpPost]
        public ActionResult EditQuizName(string qzID, string newQuizName)
        {
            int quizID = int.Parse(qzID);
            var quiz = db.Quizs.Find(quizID);
            quiz.QuizName = newQuizName;
            db.Entry(quiz).State = EntityState.Modified;
            db.SaveChanges();
            /*return Redirect("~/Teacher/Quiz/QuizDetail?qzID=" + quiz.QuizID);*/
            return Redirect(Request.UrlReferrer.ToString());
        }

        //Delete Quiz
        [HttpPost]
        public ActionResult DeleteQuiz(string qzID)
        {
            int quizID = int.Parse(qzID);
            var quizDoneList = db.QuizDones.Where(qq => qq.QuizID == quizID).ToList();
            foreach (var qqd in quizDoneList)
            {
                qqd.QuizID = null;
                db.Entry(qqd).State = EntityState.Modified;
                db.SaveChanges();
            }
            var quiz = db.Quizs.Find(quizID);
            db.Quizs.Remove(quiz);
            db.SaveChanges();
            return Redirect("~/Teacher/Quiz/ViewQuizByCourse?cid=" + quiz.CourseID);
        }

        //Show question to add to quiz
        public PartialViewResult ShowQuestionForEditQuiz(string chid, string cid, string qzid, string qtype, string searchText, int? i)
        {
            int chapID = int.Parse(chid);
            int courseID = int.Parse(cid);
            int questionType = int.Parse(qtype);

            List<Question> qListFromDB = new List<Question>();
            List<MatchQuestion> mListFromDB = new List<MatchQuestion>();

            Dictionary<int, string> questionSet = new Dictionary<int, string>();
            Dictionary<int, string> matchingSet = new Dictionary<int, string>();

            int quizID = int.Parse(qzid);
            var quiz = db.Quizs.Find(quizID);
            //check if there are question inside quiz
            if (quiz.Questions != null)
            {
                string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
                foreach (string q in quizQuestions)
                {

                    string[] questAndType = q.Split(new char[] { '-' });
                    int qTypeID = int.Parse(questAndType[1]);
                    if (qTypeID == 5)
                    {
                        int mID = int.Parse(questAndType[0]);
                        matchingSet.Add(mID, "5");
                        /* Debug.WriteLine("mmmmmmmmmm" + mID + questAndType[1]);*/
                    }
                    else
                    {
                        int qID = int.Parse(questAndType[0]);
                        questionSet.Add(qID, questAndType[1]);
                        /*  Debug.WriteLine("oooooooo" + qID + questAndType[1]);*/
                    }
                }
            }

            //on dropdownlist chapter change
            if (chapID == -1)
            {
                qListFromDB = db.Questions.Where(q => q.Chapter.CourseID == courseID).ToList();
                mListFromDB = db.MatchQuestions.Where(m => m.Chapter.CourseID == courseID).ToList();
            }
            else
            {
                qListFromDB = db.Questions.Where(q => q.ChapterID == chapID).ToList();
                mListFromDB = db.MatchQuestions.Where(m => m.ChapterId == chapID).ToList();
            }

            //on dropdownlist question type change
            if (questionType == -1)
            {
                if (!searchText.Trim().Equals(""))
                {
                    qListFromDB = qListFromDB.Where(q => q.Text.ToLower().Contains(searchText.ToLower().Trim())).ToList();
                    mListFromDB = mListFromDB.Where(m => m.ColumnA.ToLower().Contains(searchText.ToLower().Trim()) ||
                   m.ColumnB.ToLower().Contains(searchText.Trim())).ToList();
                }

            }
            else if (questionType == 5)
            {
                qListFromDB = null;
                if (!searchText.Trim().Equals(""))
                {
                    mListFromDB = mListFromDB.Where(m => m.ColumnA.ToLower().Contains(searchText.ToLower().Trim()) ||
                   m.ColumnB.ToLower().Contains(searchText.Trim())).ToList();
                }

            }
            else
            {
                mListFromDB = null;
                if (!searchText.Trim().Equals(""))
                {
                    qListFromDB = qListFromDB.Where(q => q.Text.ToLower().Contains(searchText.ToLower().Trim()) && q.Qtype == questionType).ToList();
                }
                else
                {
                    qListFromDB = qListFromDB.Where(q => q.Qtype == questionType).ToList();
                }
            }

            List<Question> questList = new List<Question>();
            List<MatchQuestion> matchList = new List<MatchQuestion>();

            //check if question already in quiz
            if (questionSet.Count() == 0)
            {
                questList = qListFromDB;
            }
            else
            {
                if (qListFromDB != null)
                {
                    foreach (var question in qListFromDB)
                    {
                        Question qDiff = new Question();
                        foreach (KeyValuePair<int, string> keyValuePair in questionSet)
                        {
                            if (question.QID == keyValuePair.Key)
                            {
                                qDiff = null;
                                break;
                            }
                            else
                            {
                                qDiff = question;
                            }


                        }
                        if (qDiff != null)
                        {
                            questList.Add(qDiff);
                        }
                    }
                }
            }

            //check if match question already in quiz
            if (matchingSet.Count() == 0)
            {
                matchList = mListFromDB;
            }
            else
            {
                if (mListFromDB != null)
                {
                    foreach (var match in mListFromDB)
                    {
                        MatchQuestion mDiff = new MatchQuestion();
                        foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                        {
                            if (match.MID == keyValuePair.Key)
                            {
                                mDiff = null;
                                break;
                            }
                            else
                            {
                                mDiff = match;
                            }


                        }
                        if (mDiff != null)
                        {
                            Question q = new Question();
                            q.ChapterID = mDiff.ChapterId;
                            q.Qtype = 5;
                            q.Text = mDiff.ColumnA + "//" + mDiff.ColumnB;
                            q.Mark = mDiff.Mark;
                            matchList.Add(mDiff);
                            /*questList.Add(q);*/
                        }
                    }
                }
            }

            ViewBag.Questions = questList;
            ViewBag.Matchings = matchList;
            return PartialView("_ShowQuestionForEditQuiz"/*, questList.ToPagedList(i ?? 1, 10)*/);
        }

        [HttpPost]
        //Add QuestionTo Quiz
        public ActionResult AddQuestionToQuiz(FormCollection collection, string qzID)
        {
            int quizID = int.Parse(qzID);
            var quiz = db.Quizs.Find(quizID);
            string questSet = "";
            int? page = null;
            var questions = collection["qID"];
            var matchings = collection["mID"];
            if (quiz.Questions != null)
            {
                if (questions != null)
                {
                    string[] ids = collection["qID"].Split(new char[] { ',' });
                    //get question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int questID = int.Parse(id);
                        var q = db.Questions.Find(questID);
                        questSet = questSet + ";" + q.QID.ToString() + "-" + q.Qtype.ToString();

                        /*Debug.WriteLine("========7=" + questSet);*/
                        if (q.Mark != null)
                        {
                            quiz.Mark = quiz.Mark + q.Mark;
                        }
                        if (q.Time != null)
                        {
                            quiz.Time = quiz.Time + q.Time;
                        }
                        quiz.NumOfQuestion = quiz.NumOfQuestion + 1;

                    }
                    /*Debug.WriteLine("========6=" + quiz.Questions);*/
                    db.SaveChanges();
                }

                if (matchings != null)
                {
                    string[] ids = collection["mID"].Split(new char[] { ',' });
                    //get match question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int matchID = int.Parse(id);
                        var m = db.MatchQuestions.Find(matchID);
                        questSet = questSet + ";" + m.MID.ToString() + "-" + "5";
                        /*Debug.WriteLine("====8=====" + questSet);*/
                        quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                        if (m.Mark != null)
                        {
                            quiz.Mark = quiz.Mark + m.Mark;
                        }
                        if (m.Time != null)
                        {
                            quiz.Time = quiz.Time + m.Time;
                        }

                    }
                    db.SaveChanges();
                }

                quiz.Questions = quiz.Questions + questSet;
                /* Debug.WriteLine("=======9=" + quiz.Questions);*/
                db.Entry(quiz).State = EntityState.Modified;
            }
            else
            {
                if (questions != null)
                {
                    string[] ids = collection["qID"].Split(new char[] { ',' });
                    //get question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int questID = int.Parse(id);
                        var q = db.Questions.Find(questID);
                        questSet = questSet + q.QID.ToString() + "-" + q.Qtype.ToString() + ";";

                        /*Debug.WriteLine("====10=====" + questSet);*/
                        if (q.Mark != null)
                        {
                            quiz.Mark = quiz.Mark + q.Mark;
                        }
                        if (q.Time != null)
                        {
                            quiz.Time = quiz.Time + q.Time;
                        }
                        quiz.NumOfQuestion = quiz.NumOfQuestion + 1;

                    }

                }

                if (matchings != null)
                {
                    string[] ids = collection["mID"].Split(new char[] { ',' });
                    //get match question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int matchID = int.Parse(id);
                        var m = db.MatchQuestions.Find(matchID);
                        questSet = questSet + m.MID.ToString() + "-" + "5" + ";";
                        /* Debug.WriteLine("====11=====" + questSet);*/
                        quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                        if (m.Mark != null)
                        {
                            quiz.Mark = quiz.Mark + m.Mark;
                        }
                        if (m.Time != null)
                        {
                            quiz.Time = quiz.Time + m.Time;
                        }
                        db.Entry(quiz).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }


                if (!questSet.Equals("") && questSet != null)
                {

                    questSet = questSet.Substring(0, questSet.Length - 1);

                }
                else
                {
                    questSet = null;
                }
                quiz.Questions = questSet;

                db.Entry(quiz).State = EntityState.Modified;
            }
            db.SaveChanges();

            int question = quiz.Questions.Split(new char[] { ';' }).Count();
            if (question == 0)
            {
                page = null;
            }
            else
            {
                if (question % 10 == 0)
                {
                    page = question / 10;
                }
                else
                {
                    page = (question / 10) + 1;

                }
            }


            return Redirect("~/Teacher/Quiz/QuizDetail?qzID=" + quiz.QuizID + "&i=" + page);
        }

        //Delete Question Inside Quiz
        public ActionResult DeleteQuestionsInsideQuiz(string qzID, string qid, string qtype, int? i)
        {
            int quizID = int.Parse(qzID);
            var quiz = db.Quizs.Find(quizID);
            int typeID = int.Parse(qtype);
            string questSet = qid + "-" + typeID;
            string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
            string newQuestionSet = null;
            //get question list
            foreach (string set in quizQuestions)
            {

                if (!set.Equals(questSet))
                {
                    newQuestionSet = newQuestionSet + set + ";";
                }
            }

            //if question is matching
            if (typeID == 5)
            {
                var m = db.MatchQuestions.Find(int.Parse(qid));
                if (m.Mark != null)
                {
                    quiz.Mark = quiz.Mark - m.Mark;
                }
                if (m.Time != null)
                {
                    quiz.Time = quiz.Time - m.Time;
                }
            }
            else
            {
                var q = db.Questions.Find(int.Parse(qid));
                if (q.Mark != null)
                {
                    quiz.Mark = quiz.Mark - q.Mark;
                }
                if (q.Time != null)
                {
                    quiz.Time = quiz.Time - q.Time;
                }
            }
            //if all question in quest is deleted
            if (newQuestionSet == null)
            {
                quiz.Questions = null;
                quiz.NumOfQuestion = 0;
                quiz.Mark = 0;
                quiz.Time = 0;
                quiz.MixQuestionNumber = 0;
            }
            else
            {
                quiz.Questions = newQuestionSet.Substring(0, newQuestionSet.Length - 1);
                quiz.NumOfQuestion = quiz.NumOfQuestion - 1;
                if (quiz.MixQuestionNumber > quiz.NumOfQuestion)
                {
                    quiz.MixQuestionNumber = quiz.NumOfQuestion;
                }
            }

            db.Entry(quiz).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect("~/Teacher/Quiz/QuizDetail?qzID=" + quiz.QuizID + "&i=" + i);
        }

        //Create Quiz View
        public ActionResult CreateNewQuiz(string cid, string questions, string tempName, int? i)
        {

            int courseID = int.Parse(cid);
            var course = db.Courses.Find(courseID);
            List<Question> qList = new List<Question>();
            List<MatchQuestion> mList = new List<MatchQuestion>();

            //check if question list is empty
            if (!questions.Equals(""))
            {
                string[] quizQuestions = questions.Split(new char[] { ';' });
                Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();

                //get question list and add to dictionary
                foreach (string quest in quizQuestions)
                {
                    Debug.WriteLine("====" + quest);
                    string[] questAndType = quest.Split(new char[] { '-' });
                    int qtypeID = int.Parse(questAndType[1]);
                    if (qtypeID == 5)
                    {
                        int mID = int.Parse(questAndType[0]);
                        matchingSet.Add(mID, questAndType[1]);
                    }
                    else
                    {
                        int qID = int.Parse(questAndType[0]);
                        questionSet.Add(qID, questAndType[1]);
                    }


                }

                //add question to list to show it at view
                foreach (KeyValuePair<int, string> keyValuePair in questionSet)
                {
                    var quest = db.Questions.Find(keyValuePair.Key);
                    qList.Add(quest);

                }

                //add matching question to list to show it at view
                foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                {
                    var mQuest = db.MatchQuestions.Find(keyValuePair.Key);
                    Question matchQuest = new Question();
                    matchQuest.QID = mQuest.MID;
                    matchQuest.Text = mQuest.ColumnA + "//" + mQuest.ColumnB;
                    matchQuest.Qtype = 5;
                    QuestionType qt = db.QuestionTypes.Find(5);
                    matchQuest.QuestionType = qt;
                    matchQuest.Mark = mQuest.Mark;
                    qList.Add(matchQuest);
                }

            }
            ViewBag.TempName = tempName;
            Debug.WriteLine(tempName + "=======-----------");
            ViewBag.Course = course;
            ViewBag.QuestionType = db.QuestionTypes.ToList();
            ViewBag.ChapterList = db.Chapters.Where(ch => ch.CourseID == course.CID);
            ViewBag.Questions = questions;
            int totalQuestion = qList.Count();
            ViewBag.CountQuest = totalQuestion;

            if (i == null)
            {
                i = 1;
            }
            else
            {
                if (totalQuestion % 10 == 0)
                {
                    if (i > (totalQuestion / 10))
                    {
                        i = totalQuestion / 10;
                    }
                }

            }
            ViewBag.QuestionNo = (i - 1) * 10;
            ViewBag.Page = i;
            return View(qList.ToPagedList(i ?? 1, 10));
        }

        //Create new quiz
        [HttpPost]
        public ActionResult CreateNewQuiz(string cid, string quizName, string questions, string cbMixQuestions, string rdQuestionNum,
            string cbPublishMark, string cbPublishAnswer)
        {

            Quiz quiz = new Quiz();
            quiz.QuizName = quizName;
            quiz.Questions = questions;
            quiz.Mark = 0;
            quiz.Time = 0;
            quiz.NumOfQuestion = 0;
            quiz.Status = "Not Done";
            int courseID = int.Parse(cid);
            quiz.CourseID = courseID;

            int mixChoice = int.Parse(cbMixQuestions);
            //check if shuffle answer check box is checked
            if (mixChoice == 1)
            {
                quiz.MixQuestion = true;
            }
            else
            {
                quiz.MixQuestion = false;
            }

            int publishMark = int.Parse(cbPublishMark);
            //check if the quiz mark publish option is check
            if (publishMark == 1)
            {
                quiz.PublicResult = true;
            }
            else
            {
                quiz.PublicResult = false;
            }

            int publishAnswer = int.Parse(cbPublishAnswer);
            //check if the quiz answer publish option is check
            if (publishAnswer == 1)
            {
                quiz.PublicAnswer = true;
            }
            else
            {
                quiz.PublicAnswer = false;
            }

            if (!rdQuestionNum.Equals("") && rdQuestionNum != null)
            {
                int numOfRandom = int.Parse(rdQuestionNum);
                quiz.MixQuestionNumber = numOfRandom;

            }


            Dictionary<int, string> questionSet = new Dictionary<int, string>();
            Dictionary<int, string> matchingSet = new Dictionary<int, string>();

            if (!questions.Equals(""))
            {
                string[] quizQuestions = questions.Split(new char[] { ';' });
                foreach (string q in quizQuestions)
                {
                    quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                    string[] questAndType = q.Split(new char[] { '-' });
                    int qtypeID = int.Parse(questAndType[1]);
                    if (qtypeID == 5)
                    {
                        int mID = int.Parse(questAndType[0]);
                        var match = db.MatchQuestions.Find(mID);
                        if (match.Mark != null)
                        {
                            quiz.Mark = quiz.Mark + match.Mark;
                        }
                        if (match.Time != null)
                        {
                            quiz.Time = quiz.Time + match.Time;
                        }
                    }
                    else
                    {
                        int qID = int.Parse(questAndType[0]);
                        var question = db.Questions.Find(qID);
                        if (question.Mark != null)
                        {
                            quiz.Mark = quiz.Mark + question.Mark;
                        }
                        if (question.Time != null)
                        {
                            quiz.Time = quiz.Time + question.Time;
                        }
                    }

                }
            }
            db.Quizs.Add(quiz);
            db.SaveChanges();

            int latestQuizID = int.Parse(db.Quizs.OrderByDescending(q => q.QuizID).Select(q => q.QuizID).First().ToString());
            return Redirect("~/Teacher/Quiz/QuizDetail?qzid=" + latestQuizID);
        }

        //Show question list for new quiz
        public PartialViewResult ShowQuestionForNewQuiz(string chid, string cid, string qtype, string searchText, string questions)
        {
            /*Debug.WriteLine("0000099999" + questions);*/
            int chapID = int.Parse(chid);
            int courseID = int.Parse(cid);
            int questionType = int.Parse(qtype);
            List<Question> qListFromDB = new List<Question>();
            List<MatchQuestion> mListFromDB = new List<MatchQuestion>();

            Dictionary<int, string> questionSet = new Dictionary<int, string>();
            Dictionary<int, string> matchingSet = new Dictionary<int, string>();
            if (!questions.Equals(""))
            {
                /*Debug.WriteLine("hihi");*/
                string[] quizQuestions = questions.Split(new char[] { ';' });
                foreach (string q in quizQuestions)
                {

                    string[] questAndType = q.Split(new char[] { '-' });
                    int qTypeID = int.Parse(questAndType[1]);
                    if (qTypeID == 5)
                    {
                        int mID = int.Parse(questAndType[0]);
                        matchingSet.Add(mID, "5");
                        /* Debug.WriteLine("mmmmmmmmmm" + mID + questAndType[1]);*/
                    }
                    else
                    {
                        int qID = int.Parse(questAndType[0]);
                        questionSet.Add(qID, questAndType[1]);
                        /*  Debug.WriteLine("oooooooo" + qID + questAndType[1]);*/
                    }
                }
            }

            //on dropdownlist chapter change
            if (chapID == -1)
            {
                qListFromDB = db.Questions.Where(q => q.Chapter.CourseID == courseID).ToList();
                mListFromDB = db.MatchQuestions.Where(m => m.Chapter.CourseID == courseID).ToList();
            }
            else
            {
                qListFromDB = db.Questions.Where(q => q.ChapterID == chapID).ToList();
                mListFromDB = db.MatchQuestions.Where(m => m.ChapterId == chapID).ToList();
            }

            //on dropdownlist question type change
            if (questionType == -1)
            {
                if (!searchText.Trim().Equals(""))
                {
                    qListFromDB = qListFromDB.Where(q => q.Text.ToLower().Contains(searchText.ToLower().Trim())).ToList();
                    mListFromDB = mListFromDB.Where(m => m.ColumnA.ToLower().Contains(searchText.ToLower().Trim()) ||
                   m.ColumnB.ToLower().Contains(searchText.Trim())).ToList();
                }

            }
            else if (questionType == 5)
            {
                qListFromDB = null;
                if (!searchText.Trim().Equals(""))
                {
                    mListFromDB = mListFromDB.Where(m => m.ColumnA.ToLower().Contains(searchText.ToLower().Trim()) ||
                   m.ColumnB.ToLower().Contains(searchText.Trim())).ToList();
                }

            }
            else
            {
                mListFromDB = null;
                if (!searchText.Trim().Equals(""))
                {
                    qListFromDB = qListFromDB.Where(q => q.Text.ToLower().Contains(searchText.ToLower().Trim()) && q.Qtype == questionType).ToList();
                }
                else
                {
                    qListFromDB = qListFromDB.Where(q => q.Qtype == questionType).ToList();
                }
            }

            List<Question> questList = new List<Question>();
            List<MatchQuestion> matchList = new List<MatchQuestion>();
            //check if question already in quiz

            if (questionSet.Count() == 0)
            {
                questList = qListFromDB;
            }
            else
            {
                if (qListFromDB != null)
                {
                    foreach (var question in qListFromDB)
                    {
                        /*Debug.WriteLine(question.QID + "===========");*/
                        Question qDiff = new Question();
                        foreach (KeyValuePair<int, string> keyValuePair in questionSet)
                        {
                            if (question.QID == keyValuePair.Key)
                            {
                                /* Debug.WriteLine(question.QID + "======yyyyyyyy====="+keyValuePair.Key);*/
                                qDiff = null;
                                break;
                            }
                            else
                            {
                                /* Debug.WriteLine(question.QID + "======tttttttttttt=====" + keyValuePair.Key);*/
                                qDiff = question;
                            }


                        }
                        if (qDiff != null)
                        {
                            questList.Add(qDiff);
                        }
                    }
                }
            }

            //check if match question already in quiz
            if (matchingSet.Count() == 0)
            {
                matchList = mListFromDB;
            }
            else
            {
                if (mListFromDB != null)
                {
                    foreach (var match in mListFromDB)
                    {
                        MatchQuestion mDiff = new MatchQuestion();
                        foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                        {
                            if (match.MID == keyValuePair.Key)
                            {
                                mDiff = null;
                                break;
                            }
                            else
                            {
                                mDiff = match;
                            }


                        }
                        if (mDiff != null)
                        {
                            matchList.Add(mDiff);
                        }
                    }
                }
            }

            ViewBag.Questions = questList;
            ViewBag.Matchings = matchList;
            return PartialView("_ShowQuestionForNewQuiz");
        }

        //Add question to a temporary list
        [HttpPost]
        public ActionResult AddQuestionToTemporaryQuiz(FormCollection collection, string cid, string questSet, string tempName)
        {
            /*Debug.WriteLine("oklaaa-" + questSet);*/
            var questions = collection["qID"];
            var matchings = collection["mID"];


            //if temparory question list have question inside
            if (!questSet.Equals("") && questSet != null)
            {
                if (questions != null)
                {
                    string[] ids = collection["qID"].Split(new char[] { ',' });
                    //get question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int questID = int.Parse(id);
                        var q = db.Questions.Find(questID);
                        questSet = questSet + ";" + q.QID.ToString() + "-" + q.Qtype.ToString();


                        /*Debug.WriteLine("========2==");
                        Debug.WriteLine(questSet);*/

                    }
                }

                if (matchings != null)
                {
                    string[] ids = collection["mID"].Split(new char[] { ',' });
                    //get match question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int matchID = int.Parse(id);
                        var m = db.MatchQuestions.Find(matchID);
                        questSet = questSet + ";" + m.MID.ToString() + "-" + "5";
                        /*Debug.WriteLine("====3======");
                        Debug.WriteLine(questSet);
                        Debug.WriteLine(questSet);
*/
                    }
                }
            }
            else
            {
                /*Debug.WriteLine("nahhhh");*/
                //check if questions list is null
                if (questions != null)
                {
                    string[] ids = collection["qID"].Split(new char[] { ',' });
                    //get question id that user checked on checkbox to add to question list
                    foreach (string id in ids)
                    {
                        int questID = int.Parse(id);
                        var q = db.Questions.Find(questID);
                        questSet = questSet + q.QID.ToString() + "-" + q.Qtype.ToString() + ";";

                        Debug.WriteLine("==========4");
                        Debug.WriteLine(questSet);

                    }

                }

                if (matchings != null)
                {
                    string[] ids = collection["mID"].Split(new char[] { ',' });
                    //get match question id that user checked on checkbox to add to question list
                    foreach (string id in ids)
                    {
                        int matchID = int.Parse(id);
                        var m = db.MatchQuestions.Find(matchID);
                        questSet = questSet + m.MID.ToString() + "-5;";
                        /*Debug.WriteLine("==========5");
                        Debug.WriteLine(questSet);*/

                    }
                }
                if (questSet != null && !questSet.Trim().Equals(""))
                {
                    questSet = questSet.Substring(0, questSet.Length - 1);
                }

                /*Debug.WriteLine(questSet);*/
            }

            int courseId = int.Parse(cid);

            if (questSet == null || questSet.Trim().Equals(""))
            {
                Debug.WriteLine("oklaaa-" + questSet);
                return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + courseId + "&questions=" + "&tempName=" + tempName);
            }
            else
            {
                int question = questSet.Split(new char[] { ';' }).Count();
                int page;
                if (question % 10 == 0)
                {
                    page = question / 10;
                }
                else
                {
                    page = (question / 10) + 1;
                }
                Debug.WriteLine("okla2222aa-" + questSet + tempName);
                /*Debug.WriteLine("okj");*/
                return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + courseId + "&questions=" + questSet + "&tempName=" + tempName + "&i=" + page);

            }
        }

        //Delete question inside temporary list
        public ActionResult DeleteQuestionsInsideTemporaryQuiz(string qid, string qtype, string questSet, string cid, string tempName, int? i)
        {
            /* Debug.WriteLine(questSet+"000000000000");*/
            int typeID = int.Parse(qtype);
            string quest = qid + "-" + typeID.ToString();
            string[] quizQuestions = questSet.Split(new char[] { ';' });
            string newQuestionSet = null;
            foreach (string set in quizQuestions)
            {
                /*Debug.WriteLine(set+"1111111111"+quest);*/
                if (!set.Equals(quest))
                {
                    newQuestionSet = newQuestionSet + set + ";";
                    /* Debug.WriteLine(set + "22222222"+newQuestionSet);*/
                }
            }
            /* Debug.WriteLine( "3333333333333" + newQuestionSet);*/
            if (newQuestionSet == null)
            {
                return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + cid + "&questions=" + "&tempName=" + tempName);
            }
            else
            {
                return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + cid + "&questions=" + newQuestionSet.Substring(0, newQuestionSet.Length - 1) + "&tempName=" + tempName + "&i=" + i);

            }
        }

        // teacher click on start quiz button
        public ActionResult QuizStarted(string qzid)
        {
            int quizID = int.Parse(qzid);
            var quiz = db.Quizs.Find(quizID);

            //if quiz status not "doing"
            if (!quiz.Status.Equals("Doing"))
            {
                string qStringForQuizSave = "";
                double? qSaveMark = 0;
                int? qSaveTime = 0;
                int qSaveNumOfQuest = 0;
                List<QuestionDone> saveQuest = new List<QuestionDone>();
                List<MatchQuestionDone> saveMatch = new List<MatchQuestionDone>();
                List<Passage> passageAdded = new List<Passage>();
                Passage_Done pSave = new Passage_Done();
                Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();

                //check if quiz contain any question
                if (quiz.Questions != null)
                {
                    string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });

                    foreach (string questions in quizQuestions)
                    {
                        string[] questAndType = questions.Split(new char[] { '-' });
                        int qtypeID = int.Parse(questAndType[1]);
                        if (qtypeID == 5)
                        {
                            int mID = int.Parse(questAndType[0]);
                            matchingSet.Add(mID, questAndType[1]);
                        }
                        else
                        {
                            int qID = int.Parse(questAndType[0]);
                            questionSet.Add(qID, questAndType[1]);
                        }

                    }

                    //check if there is any question
                    if (questionSet != null)
                    {
                        foreach (KeyValuePair<int, string> keyValuePair in questionSet)
                        {
                            var quest = db.Questions.Find(keyValuePair.Key);
                            var qAnswer = db.QuestionAnswers.Where(qa => qa.QuestionID == quest.QID).ToList();
                            QuestionDone qSave = new QuestionDone();
                            //if question is reading
                            if (quest.Qtype == 2)
                            {

                                bool added = false;
                                foreach (var p in passageAdded)
                                {
                                    Debug.WriteLine("123---" + p.PID);
                                    if (quest.PassageID == p.PID)
                                    {
                                        added = true;
                                    }
                                }
                                if (!added)
                                {
                                    passageAdded.Add(quest.Passage);
                                    Passage_Done newPassage = new Passage_Done();
                                    newPassage.Text = quest.Passage.Text;
                                    newPassage.ChapterID = quest.Passage.ChapterID;
                                    db.Passage_Done.Add(newPassage);
                                    db.SaveChanges();
                                    pSave.P_DoneID = int.Parse(db.Passage_Done.OrderByDescending(p => p.P_DoneID).Where(p => p.ChapterID == newPassage.ChapterID).Select(p => p.P_DoneID).First().ToString());

                                }

                                qSave.PassageID = pSave.P_DoneID;
                            }
                            //save question to db
                            qSave.Text = quest.Text;
                            qSave.Mark = quest.Mark;
                            qSave.Qtype = quest.Qtype;
                            qSave.ImageData = quest.ImageData;
                            qSave.ChapterID = quest.ChapterID;
                            qSave.Time = quest.Time;
                            qSave.MixChoice = quest.MixChoice;
                            qSave.GivenWord = quest.GivenWord;
                            qSave.StudentReceive = 0;
                            qSave.CorrectNumber = 0;

                            db.QuestionDones.Add(qSave);
                            db.SaveChanges();

                            //save question answer to db
                            int qSaveID = int.Parse(db.QuestionDones.OrderByDescending(q => q.Q_DoneID).Where(q => q.ChapterID == qSave.ChapterID).Select(q => q.Q_DoneID).First().ToString());
                            /*Debug.WriteLine("==-=-=" + qSaveID.ToString());*/
                            qStringForQuizSave = qStringForQuizSave + qSaveID + "-" + quest.Qtype.ToString() + ";";
                            qSaveMark = qSaveMark + qSave.Mark;
                            qSaveTime = qSaveTime + qSave.Time;
                            qSaveNumOfQuest = qSaveNumOfQuest + 1;
                            //save answer
                            foreach (var answer in quest.QuestionAnswers)
                            {
                                QuestionAnswerDone qaSave = new QuestionAnswerDone();
                                qaSave.QuestionID = qSaveID;
                                qaSave.Text = answer.Text;
                                qaSave.IsCorrect = answer.IsCorrect;
                                db.QuestionAnswerDones.Add(qaSave);
                            }
                            db.SaveChanges();
                        }


                    }
                    //check if there is any matching question
                    if (matchingSet != null)
                    {
                        foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                        {
                            var mQuest = db.MatchQuestions.Find(keyValuePair.Key);
                            MatchQuestionDone mSave = new MatchQuestionDone();
                            mSave.ChapterID = mQuest.ChapterId;
                            mSave.Mark = mQuest.Mark;
                            mSave.Time = mQuest.Time;
                            mSave.ColumnA = mQuest.ColumnA;
                            mSave.ColumnB = mQuest.ColumnB;
                            mSave.Solution = mQuest.Solution;
                            mSave.StudentReceive = 0;
                            mSave.CorrectNumber = 0;
                            db.MatchQuestionDones.Add(mSave);
                            db.SaveChanges();
                            int mSaveID = int.Parse(db.MatchQuestionDones.OrderByDescending(m => m.M_DoneID).Where(m => m.ChapterID == mSave.ChapterID).Select(m => m.M_DoneID).First().ToString());
                            qStringForQuizSave = qStringForQuizSave + mSaveID + "-5;";
                            qSaveMark = qSaveMark + mSave.Mark;
                            qSaveTime = qSaveTime + mSave.Time;
                            qSaveNumOfQuest = qSaveNumOfQuest + 1;
                        }
                    }


                }

                //save quiz to database
                quiz.Status = "Doing";
                db.Entry(quiz).State = EntityState.Modified;
                QuizDone saveQuiz = new QuizDone();
                saveQuiz.QuizID = quizID;
                saveQuiz.NumOfQuestion = qSaveNumOfQuest;
                saveQuiz.TotalMark = qSaveMark;
                saveQuiz.Time = qSaveTime;
                saveQuiz.Questions = qStringForQuizSave.Substring(0, qStringForQuizSave.Length - 1);
                saveQuiz.MixQuestion = quiz.MixQuestion;
                saveQuiz.MixQuestionNumber = quiz.MixQuestionNumber;
                saveQuiz.CourseID = quiz.Course.CID;
                saveQuiz.Quiz_Name = quiz.QuizName;
                saveQuiz.CreatedDate = DateTime.Today;
                saveQuiz.PublicResult = quiz.PublicResult;
                saveQuiz.PublicAnswer = quiz.PublicAnswer;
                db.QuizDones.Add(saveQuiz);
                db.SaveChanges();

                int quizSaveID = int.Parse(db.QuizDones.OrderByDescending(q => q.QuizDoneID).Where(q => q.CourseID == saveQuiz.CourseID).Select(q => q.QuizDoneID).First().ToString());

                db.SaveChanges();
                ViewBag.Quiz = quiz;
                return View();
            }
            else
            {
                ViewBag.Quiz = quiz;
                return View();
            }


        }

        // teacher click on finish quiz button
        public ActionResult FinishQuiz(string qzid)
        {

            int quizID = int.Parse(qzid);
            var quiz = db.Quizs.Find(quizID);
            quiz.Status = "Done";

            db.Entry(quiz).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect("~/Teacher/Quiz/QuizDetail?qzID=" + quiz.QuizID);

        }

        public ActionResult PreviewQuiz(string qzid, int rdPreview)
        {
            if (rdPreview == 1)
            {
                return Redirect("~/Teacher/Quiz/PreviewQuizPaperTest?qzid=" + qzid);
            }
            else
            {
                return Redirect("~/Teacher/Quiz/PreviewQuizQuestionByQuestion?qzid=" + qzid);
            }
        }


        public ActionResult PreviewQuizPaperTest(string qzid)
        {
            int quizId = int.Parse(qzid);
            var quiz = db.Quizs.Find(quizId);

            List<Question> multipleQuestionsList = new List<Question>();
            List<Question> readingQuestionsList = new List<Question>();
            List<Question> fillBlankQuestionsList = new List<Question>();
            List<Question> shortAnswerQuestionsList = new List<Question>();
            List<Question> indicateMistakeQuestionsList = new List<Question>();
            List<MatchQuestion> matchQuestionsList = new List<MatchQuestion>();
            List<Passage> passageList = new List<Passage>();
            //check if questions list is null
            if (quiz.Questions != null && !quiz.Questions.Equals(""))
            {
                //////////////////////////////////////
                string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
                List<string> questionList = quizQuestions.ToList();

                //////////////////////////////////////

                Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                foreach (string questions in questionList)
                {
                    string[] questAndType = questions.Split(new char[] { '-' });
                    int qType = int.Parse(questAndType[1]);
                    if (qType == 5)
                    {
                        int mID = int.Parse(questAndType[0]);
                        matchingSet.Add(mID, questAndType[1]);
                    }
                    else
                    {
                        int qID = int.Parse(questAndType[0]);
                        questionSet.Add(qID, questAndType[1]);
                    }

                }


                foreach (KeyValuePair<int, string> keyValuePair in questionSet)
                {
                    var quest = db.Questions.Find(keyValuePair.Key);

                    List<QuestionAnswer> qAnswer = quest.QuestionAnswers.ToList();
                    /*foreach (var ans in quest.QuestionAnswers)
                    {
                        Debug.WriteLine("i=-" + ans.Text);
                    }*/
                    if (quest.Qtype == 1)
                    {
                        //if question mix choice
                        if (quest.MixChoice == true)
                        {
                            Random rd = new Random();
                            int numOfAnswer = qAnswer.Count;
                            while (numOfAnswer > 1)
                            {
                                numOfAnswer--;
                                int k = rd.Next(numOfAnswer + 1);
                                var qaTemp = qAnswer[k];
                                qAnswer[k] = qAnswer[numOfAnswer];
                                qAnswer[numOfAnswer] = qaTemp;
                            }
                            quest.QuestionAnswers = qAnswer;
                        }
                        multipleQuestionsList.Add(quest);
                    }
                    else if (quest.Qtype == 2)
                    {
                        //if question mix choice
                        if (quest.MixChoice == true)
                        {
                            Random rd = new Random();
                            int numOfAnswer = qAnswer.Count;
                            while (numOfAnswer > 1)
                            {
                                numOfAnswer--;
                                int k = rd.Next(numOfAnswer + 1);
                                var qaTemp = qAnswer[k];
                                qAnswer[k] = qAnswer[numOfAnswer];
                                qAnswer[numOfAnswer] = qaTemp;
                            }
                            quest.QuestionAnswers = qAnswer;
                        }
                        readingQuestionsList.Add(quest);

                        //add passage to a list
                        var passage = quest.Passage;
                        bool existed = false;
                        foreach (var p in passageList)
                        {
                            if (passage.PID == p.PID)
                            {
                                existed = true;
                            }

                        }
                        if (!existed)
                        {
                            passageList.Add(passage);
                        }

                    }
                    else if (quest.Qtype == 3)
                    {
                        fillBlankQuestionsList.Add(quest);
                    }
                    else if (quest.Qtype == 4)
                    {
                        shortAnswerQuestionsList.Add(quest);
                    }
                    else if (quest.Qtype == 6)
                    {
                        indicateMistakeQuestionsList.Add(quest);
                    }
                }

                foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                {
                    var matchQuest = db.MatchQuestions.Find(keyValuePair.Key);
                    matchQuestionsList.Add(matchQuest);

                }
            }

            ViewBag.Quiz = quiz;
            ViewBag.MultipleQuestion = multipleQuestionsList;
            ViewBag.FillBlankQuestion = fillBlankQuestionsList;
            ViewBag.ShortAnswerQuestion = shortAnswerQuestionsList;
            ViewBag.IndicateMistakeQuestion = indicateMistakeQuestionsList;
            ViewBag.ReadingQuestion = readingQuestionsList;
            ViewBag.PassageList = passageList;
            ViewBag.MatchingQuestion = matchQuestionsList;


            int? time = quiz.Time;
            string second = "";
            string minute = "";
            if ((time % 60) != 0)
            {
                if (time % 60 < 10)
                {
                    second = "0" + (time % 60);
                }
                else
                {
                    second = (time % 60).ToString();
                }
            }
            if ((time / 60) != 0)
            {
                if (time / 60 < 10)
                {
                    minute = "0" + (time / 60);
                }
                else
                {
                    minute = (time / 60).ToString();
                }
            }
            string timeDisplay = minute + ":" + second;

            ViewBag.Time = timeDisplay;
            return View();


        }

        public ActionResult PreviewQuizQuestionByQuestion(string qzid)
        {
            int quizId = int.Parse(qzid);
            var quiz = db.Quizs.Find(quizId);

            List<Question> questionsList = new List<Question>();
            List<MatchQuestion> matchQuestionsList = new List<MatchQuestion>();
            List<Passage> passageList = new List<Passage>();
            //check if questions list is null
            if (quiz.Questions != null && !quiz.Questions.Equals(""))
            {
                //////////////////////////////////////
                string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
                List<string> questionIdList = quizQuestions.ToList();

                //////////////////////////////////////

                Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                foreach (string questions in questionIdList)
                {
                    string[] questAndType = questions.Split(new char[] { '-' });
                    int qType = int.Parse(questAndType[1]);
                    if (qType == 5)
                    {
                        int mID = int.Parse(questAndType[0]);
                        matchingSet.Add(mID, questAndType[1]);
                    }
                    else
                    {
                        int qID = int.Parse(questAndType[0]);
                        questionSet.Add(qID, questAndType[1]);
                    }

                }


                foreach (KeyValuePair<int, string> keyValuePair in questionSet)
                {
                    var quest = db.Questions.Find(keyValuePair.Key);

                    List<QuestionAnswer> qAnswer = quest.QuestionAnswers.ToList();
                    /*foreach (var ans in quest.QuestionAnswers)
                    {
                        Debug.WriteLine("i=-" + ans.Text);
                    }*/
                    if (quest.Qtype == 1)
                    {
                        //if question mix choice
                        if (quest.MixChoice == true)
                        {
                            Random rd = new Random();
                            int numOfAnswer = qAnswer.Count;
                            while (numOfAnswer > 1)
                            {
                                numOfAnswer--;
                                int k = rd.Next(numOfAnswer + 1);
                                var qaTemp = qAnswer[k];
                                qAnswer[k] = qAnswer[numOfAnswer];
                                qAnswer[numOfAnswer] = qaTemp;
                            }
                            quest.QuestionAnswers = qAnswer;
                        }
                        questionsList.Add(quest);
                    }
                    else if (quest.Qtype == 2)
                    {
                        //if question mix choice
                        if (quest.MixChoice == true)
                        {
                            Random rd = new Random();
                            int numOfAnswer = qAnswer.Count;
                            while (numOfAnswer > 1)
                            {
                                numOfAnswer--;
                                int k = rd.Next(numOfAnswer + 1);
                                var qaTemp = qAnswer[k];
                                qAnswer[k] = qAnswer[numOfAnswer];
                                qAnswer[numOfAnswer] = qaTemp;
                            }
                            quest.QuestionAnswers = qAnswer;
                        }
                        questionsList.Add(quest);

                        //add passage to a list
                        var passage = quest.Passage;
                        bool existed = false;
                        foreach (var p in passageList)
                        {
                            if (passage.PID == p.PID)
                            {
                                existed = true;
                            }

                        }
                        if (!existed)
                        {
                            passageList.Add(passage);
                        }

                    }
                    else if (quest.Qtype == 3)
                    {
                        questionsList.Add(quest);
                    }
                    else if (quest.Qtype == 4)
                    {
                        questionsList.Add(quest);
                    }
                    else if (quest.Qtype == 6)
                    {
                        questionsList.Add(quest);
                    }
                }

                foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                {
                    var matchQuest = db.MatchQuestions.Find(keyValuePair.Key);
                    matchQuestionsList.Add(matchQuest);

                }
            }

            ViewBag.Quiz = quiz;
            ViewBag.QuestionList = questionsList;
            ViewBag.PassageList = passageList;
            ViewBag.MatchingQuestion = matchQuestionsList;


            
            return View();


        }
    }


}