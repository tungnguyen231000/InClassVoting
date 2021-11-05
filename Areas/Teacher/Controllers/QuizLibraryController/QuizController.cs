﻿using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public ActionResult ViewQuizByCourse(string cid)
        {
            int courseID = int.Parse(cid);
            ViewBag.Course = db.Courses.Find(courseID);
            var quizList = db.Quizs.Where(q => q.CourseID == courseID);
            return View(quizList);
        }

        //View Quiz Detail
        public ActionResult QuizDetail(string qzID)
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
                if (quiz.Questions != null && !quiz.Questions.Equals(""))
                {
                    string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
                    Dictionary<int, string> questionSet = new Dictionary<int, string>();
                    Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                    foreach (string questions in quizQuestions)
                    {
                        bool isMatching = questions.Contains("Matching");
                        if (isMatching)
                        {
                            string[] questAndType = questions.Split(new char[] { '-' });
                            int mID = int.Parse(questAndType[0]);
                            matchingSet.Add(mID, "Matching");
                        }
                        else
                        {
                            string[] questAndType = questions.Split(new char[] { '-' });
                            int qID = int.Parse(questAndType[0]);
                            questionSet.Add(qID, questAndType[1]);
                        }

                    }

                    foreach (KeyValuePair<int, string> keyValuePair in questionSet)
                    {

                        var quest = db.Questions.Find(keyValuePair.Key);
                        qList.Add(quest);

                    }

                    foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                    {
                        var mQuest = db.MatchQuestions.Find(keyValuePair.Key);
                        mList.Add(mQuest);

                    }
                }
                dynamic dyQuestions = new ExpandoObject();
                dyQuestions.Questions = qList;
                dyQuestions.Matchings = mList;
                ViewBag.Quiz = quiz;
                ViewBag.Course = course;
                ViewBag.QuestionType = db.QuestionTypes.ToList();
                ViewBag.ChapterList = db.Chapters.Where(ch => ch.CourseID == course.CID);
                ViewBag.CountQuest = qList.Count() + mList.Count();
                return View(dyQuestions);
            }

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
            return Redirect("~/Teacher/Quiz/QuizDetail?qzID=" + quiz.QuizID);
        }

        //Delete Quiz
        [HttpPost]
        public ActionResult DeleteQuiz(string qzID)
        {
            int quizID = int.Parse(qzID);
            var quiz_quizDoneList = db.Quiz_QuizDone.Where(qq => qq.QuizID == quizID).ToList();
            foreach(var qqd in quiz_quizDoneList)
            {
                db.Quiz_QuizDone.Remove(qqd);
                db.SaveChanges();
            }
            var quiz = db.Quizs.Find(quizID);
            db.Quizs.Remove(quiz);
            db.SaveChanges();
            return Redirect("~/Teacher/Quiz/ViewQuizByCourse?cid=" + quiz.CourseID);
        }

        //Show question to add to quiz
        public PartialViewResult ShowQuestionForEditQuiz(string chid, string cid, string qzid, string qtype, string searchText)
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
            if (quiz.Questions != null)
            {
                string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
                foreach (string questions in quizQuestions)
                {
                    bool isMatching = questions.Contains("Matching");
                    if (isMatching)
                    {
                        string[] questAndType = questions.Split(new char[] { '-' });
                        int mID = int.Parse(questAndType[0]);
                        matchingSet.Add(mID, "Matching");
                    }
                    else
                    {
                        string[] questAndType = questions.Split(new char[] { '-' });
                        int qID = int.Parse(questAndType[0]);
                        questionSet.Add(qID, questAndType[1]);
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
                            matchList.Add(mDiff);
                        }
                    }
                }
            }

            ViewBag.Questions = questList;
            ViewBag.Matchings = matchList;
            return PartialView("_ShowQuestionForEditQuiz");
        }

        [HttpPost]
        //Add QuestionTo Quiz
        public ActionResult AddQuestionToQuiz(FormCollection collection, string qzID)
        {
            int quizID = int.Parse(qzID);
            var quiz = db.Quizs.Find(quizID);
            string questSet = "";
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
                        questSet = questSet + ";" + q.QID.ToString() + "-" + q.QuestionType.Name;

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
                        questSet = questSet + ";" + m.MID.ToString() + "-" + "Matching";
                        /*Debug.WriteLine("====8=====" + questSet);*/
                        quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                        if (m.Mark != null)
                        {
                            quiz.Mark = quiz.Mark + m.Mark;
                        }
                        /*if (m.Time != null)
                        {
                            quiz.Time = quiz.Time + m.Time;
                        }*/

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
                        questSet = questSet + q.QID.ToString() + "-" + q.QuestionType.Name + ";";

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
                        questSet = questSet + m.MID.ToString() + "-" + "Matching" + ";";
                        /* Debug.WriteLine("====11=====" + questSet);*/
                        quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                        if (m.Mark != null)
                        {
                            quiz.Mark = quiz.Mark + m.Mark;
                        }
                        /*if (m.Time != null)
                        {
                            quiz.Time = quiz.Time + m.Time;
                        }*/
                        db.Entry(quiz).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                }
                if (!questSet.Equals(""))
                {
                    questSet = questSet.Substring(0, questSet.Length - 1);
                }
                else
                {
                    questSet = null;
                }
                quiz.Questions = questSet;

                /*  Debug.WriteLine("=======12=" + quiz.Questions);*/
                db.Entry(quiz).State = EntityState.Modified;
            }
            db.SaveChanges();

            return Redirect("~/Teacher/Quiz/QuizDetail?qzID=" + quiz.QuizID);
        }

        //Delete Question Inside Quiz
        public ActionResult DeleteQuestionsInsideQuiz(string qzID, string qid, string qtype)
        {
            int quizID = int.Parse(qzID);
            var quiz = db.Quizs.Find(quizID);
            int typeID = int.Parse(qtype);
            var questionType = db.QuestionTypes.Find(typeID);
            string questSet = qid + "-" + questionType.Name;
            string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
            string newQuestionSet = null;
            foreach (string set in quizQuestions)
            {

                if (!set.Equals(questSet))
                {
                    newQuestionSet = newQuestionSet + set + ";";
                }
            }
            if (typeID == 5)
            {
                var m = db.MatchQuestions.Find(int.Parse(qid));
                if (m.Mark != null)
                {
                    quiz.Mark = quiz.Mark - m.Mark;
                }
                /*if (m.Time != null)
                {
                    quiz.Time = quiz.Time - m.Time;
                }*/
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

            return Redirect("~/Teacher/Quiz/QuizDetail?qzID=" + quiz.QuizID);
        }

        //Create Quiz View
        public ActionResult CreateNewQuiz(string cid, string questions, string tempName)
        {

            int courseID = int.Parse(cid);
            var course = db.Courses.Find(courseID);
            List<Question> qList = new List<Question>();
            List<MatchQuestion> mList = new List<MatchQuestion>();
            dynamic dyQuestions = new ExpandoObject();

            if (!questions.Equals(""))
            {
                string[] quizQuestions = questions.Split(new char[] { ';' });
                Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                foreach (string quest in quizQuestions)
                {
                    bool isMatching = quest.Contains("Matching");
                    if (isMatching)
                    {
                        string[] questAndType = quest.Split(new char[] { '-' });
                        int mID = int.Parse(questAndType[0]);
                        matchingSet.Add(mID, "Matching");
                    }
                    else
                    {
                        string[] questAndType = quest.Split(new char[] { '-' });
                        int qID = int.Parse(questAndType[0]);
                        if(questAndType[1].Contains("+"))
                        {
                            string[] qTypeSplit= questAndType[1].Split(new char[] { '+' });
                            string qtypeFixed = qTypeSplit[0] + " " + qTypeSplit[1];
                            questionSet.Add(qID, questAndType[1]);
                        }
                        else
                        {
                            questionSet.Add(qID, questAndType[1]);
                        }
                        
                    }

                }

                foreach (KeyValuePair<int, string> keyValuePair in questionSet)
                {
                    var quest = db.Questions.Find(keyValuePair.Key);
                    qList.Add(quest);

                }

                foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                {
                    var mQuest = db.MatchQuestions.Find(keyValuePair.Key);
                    mList.Add(mQuest);
                }

            }
            ViewBag.TempName = tempName;
            Debug.WriteLine(tempName + "=======-----------");
            ViewBag.Course = course;
            ViewBag.QuestionType = db.QuestionTypes.ToList();
            ViewBag.ChapterList = db.Chapters.Where(ch => ch.CourseID == course.CID);
            ViewBag.Questions = questions;
            dyQuestions.Questions = qList;
            dyQuestions.Matchings = mList;
            ViewBag.CountQuest = qList.Count() + mList.Count();
            return View(dyQuestions);
        }

        //Create new quiz
        [HttpPost]
        public ActionResult CreateNewQuiz(string cid, string quizName, string questions, string cbMixQuestions, string rdQuestionNum)
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
            Debug.WriteLine(cid + "==-=-=-=" + quiz.CourseID);
            int mixChoice = int.Parse(cbMixQuestions);

            if (mixChoice == 1)
            {
                quiz.MixQuestion = true;
            }
            else
            {
                quiz.MixQuestion = false;
            }

            Dictionary<int, string> questionSet = new Dictionary<int, string>();
            Dictionary<int, string> matchingSet = new Dictionary<int, string>();

            if (!questions.Equals(""))
            {
                string[] quizQuestions = questions.Split(new char[] { ';' });
                foreach (string q in quizQuestions)
                {
                    quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                    bool isMatching = q.Contains("Matching");
                    if (isMatching)
                    {

                        string[] questAndType = q.Split(new char[] { '-' });
                        int mID = int.Parse(questAndType[0]);
                        Debug.WriteLine("=====1===" + q);
                        var match = db.MatchQuestions.Find(mID);
                        Debug.WriteLine("=====32===" + match.Mark);
                        if (match.Mark != null)
                        {
                            quiz.Mark = quiz.Mark + match.Mark;
                        }
                    }
                    else
                    {
                        string[] questAndType = q.Split(new char[] { '-' });
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
                        Debug.WriteLine("=======24=" + question.Mark + question.Time);
                    }

                }
            }
            if (!rdQuestionNum.Equals(""))
            {
                int numOfRandom = int.Parse(rdQuestionNum);
                quiz.MixQuestionNumber = quiz.NumOfQuestion;
                quiz.MixQuestionNumber = numOfRandom;

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
                    bool isMatching = q.Contains("Matching");
                    if (isMatching)
                    {
                        string[] questAndType = q.Split(new char[] { '-' });
                        int mID = int.Parse(questAndType[0]);
                        matchingSet.Add(mID, "Matching");
                        /* Debug.WriteLine("mmmmmmmmmm" + mID + questAndType[1]);*/

                    }
                    else
                    {
                        string[] questAndType = q.Split(new char[] { '-' });
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

            var questions = collection["qID"];
            var matchings = collection["mID"];
            /*Debug.WriteLine("ok========" + questSet);*/
            
            //if temparory question list have question inside
            if (!questSet.Equals(""))
            {
                if (questions != null)
                {
                    string[] ids = collection["qID"].Split(new char[] { ',' });
                    //get question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int questID = int.Parse(id);
                        var q = db.Questions.Find(questID);
                        if (q.QuestionType.Name.Contains(" "))
                        {
                            string[] qtypeSplit = q.QuestionType.Name.Split(new char[] { ' ' });
                            questSet = questSet + ";" + q.QID.ToString() + "-" + qtypeSplit[0]+ "+" + qtypeSplit[1];
                        }
                        else
                        {
                            questSet = questSet + ";" + q.QID.ToString() + "-" + q.QuestionType.Name;
                        }

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
                        questSet = questSet + ";" + m.MID.ToString() + "-" + "Matching";
                        /*Debug.WriteLine("====3======");
                        Debug.WriteLine(questSet);
                        Debug.WriteLine(questSet);
*/
                    }
                }
            }
            else
            {
                Debug.WriteLine("nahhhh");
                if (questions != null)
                {
                    string[] ids = collection["qID"].Split(new char[] { ',' });
                    //get question id that user checked on checkbox
                    foreach (string id in ids)
                    {
                        int questID = int.Parse(id);
                        var q = db.Questions.Find(questID);
                        if(q.QuestionType.Name.Contains(" "))
                        {
                            string [] qtypeSplit = q.QuestionType.Name.Split(new char[] { ' ' });
                            questSet = questSet + q.QID.ToString() + "-" + qtypeSplit[0]+"+"+ qtypeSplit[1] + ";";
                        }
                        else
                        {
                            questSet = questSet + q.QID.ToString() + "-" + q.QuestionType.Name + ";";
                        }

                        /*Debug.WriteLine("==========4");
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
                        questSet = questSet + m.MID.ToString() + "-" + "Matching" + ";";
                        /*Debug.WriteLine("==========5");
                        Debug.WriteLine(questSet);*/

                    }
                }
                questSet = questSet.Substring(0, questSet.Length - 1);
                /*Debug.WriteLine(questSet);*/
            }


            int courseId = int.Parse(cid);
            if (questSet == null)
            {
                /*Debug.WriteLine("oklaaa");*/
                return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + courseId + "&questions=" + "&tempName=" + tempName);
            }
            else
            {
                /*Debug.WriteLine("okj");*/
                return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + courseId + "&questions=" + questSet + "&tempName=" + tempName);

            }
        }

        //Delete question inside temporary list
        public ActionResult DeleteQuestionsInsideTemporaryQuiz(string qid, string qtype, string questSet, string cid, string tempName)
        {
            /* Debug.WriteLine(questSet+"000000000000");*/
            int typeID = int.Parse(qtype);
            var questionType = db.QuestionTypes.Find(typeID);
            string quest = qid + "-" + questionType.Name;
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
                return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + cid + "&questions=" + newQuestionSet.Substring(0, newQuestionSet.Length - 1) + "&tempName=" + tempName);

            }
        }

        // teacher click on start quiz button
        public ActionResult QuizStarted(string qzid)
        {
            int quizID = int.Parse(qzid);
            var quiz = db.Quizs.Find(quizID);

            if (!quiz.Status.Equals("Doing"))
            {
                string qStringForQuizSave = "";
                double? qSaveMark = 0;
                int? qSaveTime = 0;
                int qSaveNumOfQuest = 0;
                List<QuestionDone> saveQuest = new List<QuestionDone>();
                List<MatchQuestionDone> saveMatch = new List<MatchQuestionDone>();
                Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                //check if quiz contain any question
                if (quiz.Questions != null)
                {
                    string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });

                    foreach (string questions in quizQuestions)
                    {
                        bool isMatching = questions.Contains("Matching");
                        if (isMatching)
                        {
                            string[] questAndType = questions.Split(new char[] { '-' });
                            int mID = int.Parse(questAndType[0]);
                            matchingSet.Add(mID, "Matching");
                        }
                        else
                        {
                            string[] questAndType = questions.Split(new char[] { '-' });
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
                            Passage_Done pSave = new Passage_Done();
                            //if question is reading
                            if (quest.Qtype == 2)
                            {
                                pSave.Text = quest.Passage.Text;
                                pSave.ChapterName = quest.Passage.Chapter.Name;
                                db.Passage_Done.Add(pSave);
                                db.SaveChanges();
                                qSave.PassageID = int.Parse(db.Passage_Done.OrderByDescending(p => p.P_DoneID).Select(p => p.P_DoneID).First().ToString());
                            }
                            qSave.Text = quest.Text;
                            qSave.Mark = quest.Mark;
                            qSave.Qtype = quest.Qtype;
                            qSave.ImageData = quest.ImageData;
                            qSave.ChapterName = quest.Chapter.Name;
                            qSave.Time = quest.Time;
                            qSave.MixChoice = quest.MixChoice;
                            db.QuestionDones.Add(qSave);
                            db.SaveChanges();

                            int qSaveID = int.Parse(db.QuestionDones.OrderByDescending(q => q.Q_DoneID).Select(q => q.Q_DoneID).First().ToString());
                            Debug.WriteLine("==-=-=" + qSaveID.ToString());
                            qStringForQuizSave = qStringForQuizSave + qSaveID + "-" + quest.QuestionType.Name + ";";
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
                            mSave.ChapterName = mQuest.Chapter.Name;
                            mSave.Mark = mQuest.Mark;
                            mSave.Time = mQuest.Time;
                            mSave.ColumnA = mQuest.ColumnA;
                            mSave.ColumnB = mQuest.ColumnB;
                            db.MatchQuestionDones.Add(mSave);
                            db.SaveChanges();
                            int mSaveID = int.Parse(db.MatchQuestionDones.OrderByDescending(m => m.M_DoneID).Select(m => m.M_DoneID).First().ToString());
                            qStringForQuizSave = qStringForQuizSave + mSaveID + "-Matching;";
                            qSaveMark = qSaveMark + mSave.Mark;
                            qSaveTime = qSaveTime + mSave.Time;
                            qSaveNumOfQuest = qSaveNumOfQuest + 1;
                        }
                    }


                }
                quiz.Status = "Doing";
                db.Entry(quiz).State = EntityState.Modified;
                QuizDone saveQuiz = new QuizDone();
                saveQuiz.NumOfQuestion = qSaveNumOfQuest;
                saveQuiz.TotalMark = qSaveMark;
                saveQuiz.Time = qSaveTime;
                saveQuiz.Questions = qStringForQuizSave.Substring(0,qStringForQuizSave.Length-1) ;
                saveQuiz.MixQuestion = quiz.MixQuestion;
                saveQuiz.MixQuestionNumber = quiz.MixQuestionNumber;
                saveQuiz.CourseID = quiz.Course.CID;
                saveQuiz.Quiz_Name = quiz.QuizName;
                saveQuiz.CreatedDate = DateTime.Today;
                db.QuizDones.Add(saveQuiz);
                db.SaveChanges();
                int quizSaveID = int.Parse(db.QuizDones.OrderByDescending(q => q.QuizDoneID).Select(q => q.QuizDoneID).First().ToString());
                Quiz_QuizDone quiz_QuizDone = new Quiz_QuizDone();
                quiz_QuizDone.QuizID = quizID;
                quiz_QuizDone.QuizDoneID = quizSaveID;
                db.Quiz_QuizDone.Add(quiz_QuizDone);
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

    }


}