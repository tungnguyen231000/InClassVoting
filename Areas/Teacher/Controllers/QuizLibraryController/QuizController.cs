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
using InClassVoting.Filter;

namespace InClassVoting.Areas.teacher.Controllers.QuizLibraryController
{
    [AccessAuthenticationFilter]
    [UserAuthorizeFilter("Teacher")]
    public class QuizController : Controller
    {
        private DBModel db = new DBModel();
        
        [HandleError]
        public ActionResult QuizLibrary()
        {

            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
            return View();
        }

        private bool checkCourserIdAvailbile(string cid)
        {
            bool check = true;
            int courseId;
            bool isInt = int.TryParse(cid, out courseId);
            //check if chapter id is int
            if (isInt == false)
            {
                check = false;
            }
            else
            {
                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                var course = db.Courses.Find(courseId);
                //check if chapter exist in db
                if (course == null)
                {
                    check = false;
                }
                else
                {
                    //check if chapter belong to teacher
                    if (course.TeacherID != teacherId)
                    {
                        check = false;
                    }
                }
            }
            return (check);
        }

        private bool checkQuizIdAvailbile(string qzid)
        {
            bool check = true;
            int quizId;
            bool isInt = int.TryParse(qzid, out quizId);
            //check if chapter id is int
            if (isInt == false)
            {
                check = false;
            }
            else
            {
                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                var quiz = db.Quizs.Find(quizId);
                //check if chapter exist in db
                if (quiz == null)
                {
                    check = false;
                }
                else
                {
                    //check if chapter belong to teacher
                    if (quiz.Course.TeacherID != teacherId)
                    {
                        check = false;
                    }
                }
            }
            return (check);
        }

        private bool checkTempModeAvailble(string tempMode)
        {
            bool check = true;
            //check if tempMode is null
            if (tempMode == null)
            {
                check = false;
            }
            else
            {
                //check if temp mode is correct
                if (!tempMode.Equals("1") && !tempMode.Equals("0"))
                {
                    check = false;
                }
            }


            return (check);
        }

        //view quiz inside course page
        [HandleError]
        public ActionResult ViewQuizByCourse(string cid, string searchText, int? i)
        {
            //check if course is availble
            if (checkCourserIdAvailbile(cid) == false)
            {
                Debug.WriteLine("nope");
                return RedirectToAction("QuizLibrary");
            }
            else
            {

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                int courseID = int.Parse(cid);
                Course course = db.Courses.Find(courseID);

                //get quizlist
                var qzList = db.Quizs.Where(qz => qz.CourseID == courseID)/*.OrderByDescending(qz => qz.QuizID)*/.ToList();
                List<Quiz> quizzes = new List<Quiz>();

                if (searchText != null && !searchText.Trim().Equals(""))
                {
                    quizzes = qzList.Where(qz => qz.QuizName.Trim().ToLower().Contains(searchText.Trim().ToLower())).ToList();

                }
                else
                {
                    quizzes = qzList;
                }

                ViewBag.QuizCount = (i - 1) * 10;
                ViewBag.Course = course;
                ViewBag.CoutnQuiz = db.Quizs.Where(q => q.CourseID == course.CID).Count();
                if (i == null || i == 0)
                {
                    i = 1;
                }
                else
                {
                    if (qzList.Count % 10 == 0 && i > qzList.Count / 10)
                    {
                        i = 1;
                    }
                    else if (qzList.Count % 10 != 0 && i > ((qzList.Count / 10) + 1))
                    {
                        i = 1;
                    }

                }


                //fix quiz number (quiz no)
                ViewBag.QuizCount = (i - 1) * 10;
                ViewBag.Search = searchText;
                return View(quizzes.ToPagedList(i ?? 1, 10));
            }
        }

        //View Quiz Detail
        [HandleError]
        public ActionResult QuizDetail(string qzID, int? i)
        {
            //check if quiz is availble
            if (checkQuizIdAvailbile(qzID) == false)
            {
                Debug.WriteLine("nope");
                return RedirectToAction("QuizLibrary");
            }
            else
            {

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
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
                                var mQuest = db.MatchQuestions.Find(mID);
                                Question matchQuest = new Question();
                                matchQuest.QID = mQuest.MID;
                                matchQuest.Text = mQuest.ColumnA + "//" + mQuest.ColumnB;
                                matchQuest.Qtype = 5;
                                matchQuest.Mark = mQuest.Mark;
                                qList.Add(matchQuest);
                                /*matchingSet.Add(mID, "5");*/
                            }
                            else
                            {
                                int qID = int.Parse(questAndType[0]);
                                var quest = db.Questions.Find(qID);
                                qList.Add(quest);
                                /*questionSet.Add(qID, questAndType[1]);*/
                            }

                        }

                    }
                    ViewBag.Quiz = quiz;
                    ViewBag.Course = course;
                    ViewBag.LoList = db.QuestionLOes.Where(ql => ql.LearningOutcome.CourseID == course.CID).ToList();
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


                    /*string domain = "https://inclassvoting.azurewebsites.net/Student/Quiz/DoQuiz?qzID=";*/
                    string domain = "https://localhost:44350/Student/Quiz/DoQuiz?qzID=";
                    string parameterPart = ViewBag.Quiz.QuizID.ToString();
                    string encodePart = Base64Encode(parameterPart);
                    ViewBag.QuizLink = domain + encodePart;

                    if (i == null || i == 0)
                    {
                        i = 1;
                    }
                    else
                    {
                        if (qList.Count % 10 == 0 && i > qList.Count / 10)
                        {
                            i = 1;
                        }
                        else if (qList.Count % 10 != 0 && i > ((qList.Count / 10) + 1))
                        {
                            i = 1;
                        }

                    }
                    ViewBag.QuestionNo = (i - 1) * 10;
                    ViewBag.Page = i;
                    return View(qList.ToPagedList(i ?? 1, 10));
                }
            }

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        //Edit Quiz Option
        [HandleError]
        [HttpPost]
        public ActionResult SaveQuizOption(string qzID, string cbMixQuestions, string rdQuestionNum,
            string cbPublishMark, string cbPublishAnswer, string cbRandomQuestion, string qtypeChange)
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

            quiz.QuizType = qtypeChange;

            db.Entry(quiz).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }

        //Edit Quiz Name
        [HandleError]
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
        [HandleError]
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
        [HandleError]
        public PartialViewResult ShowQuestionForEditQuiz(string chid, string cid, string qzid, string qtype, string searchText/*, int? i*/)
        {

            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
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
            if (questionType != -1)
            {
                if (questionType == 5)
                {
                    qListFromDB = null;
                    

                }
                else
                {
                    mListFromDB = null;
                    qListFromDB = qListFromDB.Where(q => q.Qtype == questionType).ToList();
                }
            }


            List<Question> questList = new List<Question>();
            List<MatchQuestion> matchList = new List<MatchQuestion>();

            //check if question already in quiz
            if (questionSet.Count() == 0)
            {
                if (qListFromDB != null)
                {
                    questList = qListFromDB;
                }
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
                if (mListFromDB != null)
                {
                    foreach (var match in mListFromDB)
                    {
                        Question q = new Question();
                        q.QID = match.MID;
                        q.ChapterID = match.ChapterId;
                        q.Qtype = 5;
                        q.Text = match.ColumnA + "//" + match.ColumnB;
                        q.Mark = match.Mark;
                        q.CreatedDate = match.CreatedDate;
                        /*matchList.Add(mDiff);*/
                        questList.Add(q);
                    }
                }
            }
            else
            {
                /*Debug.WriteLine("hi132323hi");*/
                if (mListFromDB != null)
                {
                    /*Debug.WriteLine("hi1hi");*/
                    foreach (var match in mListFromDB)
                    {
                        /*Debug.WriteLine("hihi" + match.MID);*/
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
                            q.QID = mDiff.MID;
                            q.ChapterID = mDiff.ChapterId;
                            q.Qtype = 5;
                            q.Text = mDiff.ColumnA + "//" + mDiff.ColumnB;
                            q.Mark = mDiff.Mark;
                            q.CreatedDate = mDiff.CreatedDate;
                            /*matchList.Add(mDiff);*/
                            questList.Add(q);
                        }
                    }
                }
            }

            if (searchText != null)
            {
                searchText = searchText.Replace('%', ' ');
                questList = questList.Where(q => q.Text.ToLower().Contains(searchText.ToLower())).ToList();
            }
            ViewBag.Questions = questList.OrderBy(q => q.CreatedDate).ToList();
            
            ViewBag.LoList = db.QuestionLOes.Where(ql => ql.LearningOutcome.CourseID == courseID).ToList();
            /*ViewBag.Matchings = matchList;*/
            return PartialView("_ShowQuestionForEditQuiz"/*, questList.ToPagedList(i ?? 1, 10)*/);
        }

        //Add QuestionTo Quiz
        [HandleError]
        [HttpPost]
        public ActionResult AddQuestionToQuiz(FormCollection collection, string qzID)
        {

            int quizID = int.Parse(qzID);
            var quiz = db.Quizs.Find(quizID);
            string questSet = "";
            int? page = null;
            var questions = collection["qIDAndType"];
            if (quiz.Questions != null)
            {
                if (questions != null)
                {
                    string[] idAndTypes = collection["qIDAndType"].Split(new char[] { ',' });
                    foreach (string id in idAndTypes)
                    {
                        string[] idAndTypeSplit = id.Split(new char[] { '-' });
                        int qtypeID = int.Parse(idAndTypeSplit[1]);
                        if (qtypeID != 5)
                        {
                            int questID = int.Parse(idAndTypeSplit[0]);
                            var q = db.Questions.Find(questID);
                            questSet = questSet + ";" + q.QID.ToString() + "-" + q.Qtype.ToString();
                            quiz.Mark = quiz.Mark + q.Mark;
                            quiz.Time = quiz.Time + q.Time;
                            quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                        }
                        else
                        {
                            int matchID = int.Parse(idAndTypeSplit[0]);
                            var m = db.MatchQuestions.Find(matchID);
                            questSet = questSet + ";" + m.MID.ToString() + "-5";
                            quiz.Mark = quiz.Mark + m.Mark;
                            quiz.Time = quiz.Time + m.Time;
                            quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                        }

                    }
                    /*Debug.WriteLine("========6=" + quiz.Questions);*/
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
                    string[] idAndTypes = collection["qIDAndType"].Split(new char[] { ',' });
                    foreach (string id in idAndTypes)
                    {
                        string[] idAndTypeSplit = id.Split(new char[] { '-' });
                        int qtypeID = int.Parse(idAndTypeSplit[1]);
                        if (qtypeID != 5)
                        {

                            int questID = int.Parse(idAndTypeSplit[0]);
                            var q = db.Questions.Find(questID);
                            questSet = questSet + q.QID.ToString() + "-" + q.Qtype.ToString() + ";";
                            quiz.Mark = quiz.Mark + q.Mark;
                            quiz.Time = quiz.Time + q.Time;
                            quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                        }
                        else
                        {
                            int matchID = int.Parse(idAndTypeSplit[0]);
                            var m = db.MatchQuestions.Find(matchID);
                            questSet = questSet + m.MID.ToString() + "-5;";
                            quiz.Mark = quiz.Mark + m.Mark;
                            quiz.Time = quiz.Time + m.Time;
                            quiz.NumOfQuestion = quiz.NumOfQuestion + 1;
                        }

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
        [HandleError]
        public ActionResult DeleteQuestionsInsideQuiz(string qzID, string qid, string qtype, int? i)
        {
            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
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
                quiz.Mark = quiz.Mark - m.Mark;
                quiz.Time = quiz.Time - m.Time;
            }
            else
            {
                var q = db.Questions.Find(int.Parse(qid));
                quiz.Mark = quiz.Mark - q.Mark;
                quiz.Time = quiz.Time - q.Time;
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
        [HandleError]
        public ActionResult CreateNewQuiz(string cid, string tempName, int? i, string tempMode)
        {
            //check if course is availble
            if (checkCourserIdAvailbile(cid) == false)
            {
                /* Debug.WriteLine("nope");*/
                return RedirectToAction("QuizLibrary");
            }
            else
            {
                string questions = Convert.ToString(HttpContext.Session["TemporaryQuestions"]);

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                int courseID = int.Parse(cid);
                var course = db.Courses.Find(courseID);

                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                List<Question> qList = new List<Question>();
                List<MatchQuestion> mList = new List<MatchQuestion>();

                //check if question list is empty
                if (!questions.Equals("") && questions != null)
                {
                    string[] quizQuestions = questions.Split(new char[] { ';' });
                    /*Dictionary<int, string> questionSet = new Dictionary<int, string>();
                    Dictionary<int, string> matchingSet = new Dictionary<int, string>();*/

                    //get question list and add to dictionary
                    foreach (string quest in quizQuestions)
                    {
                        Debug.WriteLine("====" + quest);
                        string[] questAndType = quest.Split(new char[] { '-' });
                        int qtypeID = int.Parse(questAndType[1]);
                        if (qtypeID == 5)
                        {
                            int mID = int.Parse(questAndType[0]);
                            var mQuest = db.MatchQuestions.Find(mID);
                            Question matchQuest = new Question();
                            matchQuest.QID = mQuest.MID;
                            matchQuest.Text = mQuest.ColumnA + "//" + mQuest.ColumnB;
                            matchQuest.Qtype = 5;
                            QuestionType qt = db.QuestionTypes.Find(5);
                            matchQuest.QuestionType = qt;
                            matchQuest.Mark = mQuest.Mark;
                            qList.Add(matchQuest);
                        }
                        else
                        {
                            int qID = int.Parse(questAndType[0]);
                            var question = db.Questions.Find(qID);
                            qList.Add(question);
                        }


                    }


                }

                ViewBag.TempName = tempName;

                Debug.WriteLine("hihih3+=== " + tempMode);
                if (checkTempModeAvailble(tempMode) == false)
                {
                    tempMode = "1";
                    Debug.WriteLine("hihih+=== " + tempMode);
                }

                Debug.WriteLine("hihih2+=== " + tempMode);
                ViewBag.TempMode = tempMode;
                ViewBag.Course = course;
                ViewBag.QuestionType = db.QuestionTypes.ToList();
                ViewBag.ChapterList = db.Chapters.Where(ch => ch.CourseID == course.CID);
                ViewBag.LoList = db.QuestionLOes.Where(ql => ql.LearningOutcome.CourseID == course.CID).ToList();
                ViewBag.Questions = questions;
                int totalQuestion = qList.Count();
                ViewBag.CountQuest = totalQuestion;

                if (i == null || i == 0)
                {
                    i = 1;
                }
                else
                {
                    if (qList.Count % 10 == 0 && i > qList.Count / 10)
                    {
                        i = 1;
                    }
                    else if (qList.Count % 10 != 0 && i > ((qList.Count / 10) + 1))
                    {
                        i = 1;
                    }

                }

                ViewBag.QuestionNo = (i - 1) * 10;
                ViewBag.Page = i;
                return View(qList.ToPagedList(i ?? 1, 10));
            }
        }

        //Create new quiz
        [HandleError]
        [HttpPost]
        public ActionResult CreateNewQuiz(string cid, string quizName, string quizMode, string cbMixQuestions, string rdQuestionNum,
            string cbPublishMark, string cbPublishAnswer)
        {
            string questions = Convert.ToString(HttpContext.Session["TemporaryQuestions"]);
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
                        quiz.Mark = quiz.Mark + match.Mark;
                        quiz.Time = quiz.Time + match.Time;
                    }
                    else
                    {
                        int qID = int.Parse(questAndType[0]);
                        var question = db.Questions.Find(qID);
                        quiz.Mark = quiz.Mark + question.Mark;
                        quiz.Time = quiz.Time + question.Time;
                    }

                }
            }
            if (quizMode.Equals("0"))
            {
                quiz.QuizType = "ShowQuestionByQuestion";
            }
            else
            {
                quiz.QuizType = "ShowAllQuestion";
            }

            db.Quizs.Add(quiz);
            db.SaveChanges();
            Session["TemporaryQuestions"] = null;

            int latestQuizID = int.Parse(db.Quizs.OrderByDescending(q => q.QuizID).Select(q => q.QuizID).First().ToString());
            return Redirect("~/Teacher/Quiz/QuizDetail?qzid=" + latestQuizID);
        }

        //Show question list for new quiz
        [HandleError]
        public PartialViewResult ShowQuestionForNewQuiz(string chid, string cid, string qtype, string searchText)
        {
            string questions = Convert.ToString(HttpContext.Session["TemporaryQuestions"]);
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
            if (questionType != -1)
            {
                if (questionType == 5)
                {
                    qListFromDB = null;


                }
                else
                {
                    mListFromDB = null;
                    qListFromDB = qListFromDB.Where(q => q.Qtype == questionType).ToList();
                }
            }

            List<Question> questList = new List<Question>();
            //check if question already in quiz

            if (questionSet.Count() == 0)
            {
                if (qListFromDB != null)
                {
                    questList = qListFromDB;
                }
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
                if (mListFromDB != null)
                {
                    foreach (var match in mListFromDB)
                    {
                        Question q = new Question();
                        q.QID = match.MID;
                        q.ChapterID = match.ChapterId;
                        q.Qtype = 5;
                        q.Text = match.ColumnA + "//" + match.ColumnB;
                        q.Mark = match.Mark;
                        q.CreatedDate = match.CreatedDate;
                        questList.Add(q);
                    }
                }
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
                            q.QID = mDiff.MID;
                            q.ChapterID = mDiff.ChapterId;
                            q.Qtype = 5;
                            q.Text = mDiff.ColumnA + "//" + mDiff.ColumnB;
                            q.Mark = mDiff.Mark;
                            q.CreatedDate = mDiff.CreatedDate;
                            questList.Add(q);
                        }
                    }
                }
            }

            if (searchText != null)
            {
                searchText = searchText.Replace('%', ' ');
                questList = questList.Where(q => q.Text.ToLower().Contains(searchText.ToLower())).ToList();
            }
            ViewBag.LoList = db.QuestionLOes.Where(ql => ql.LearningOutcome.CourseID == courseID).ToList();
            ViewBag.Questions = questList.OrderBy(q => q.CreatedDate).ToList();
            return PartialView("_ShowQuestionForNewQuiz");
        }

        //Add question to a temporary list
        [HandleError]
        [HttpPost]
        public ActionResult AddQuestionToTemporaryQuiz(FormCollection collection, string cid, string tempName, string tempMode)
        {
            string questSet = Convert.ToString(HttpContext.Session["TemporaryQuestions"]);
            var questions = collection["qIDAndType"];


            //if temparory question list have question inside
            if (!questSet.Equals("") && questSet != null)
            {
                if (questions != null)
                {
                    string[] idAndTypes = collection["qIDAndType"].Split(new char[] { ',' });
                    //get question id that user checked on checkbox
                    foreach (string id in idAndTypes)
                    {
                        string[] idAndTypeSplit = id.Split(new char[] { '-' });
                        int qtypeID = int.Parse(idAndTypeSplit[1]);
                        if (qtypeID != 5)
                        {
                            int questID = int.Parse(idAndTypeSplit[0]);
                            var q = db.Questions.Find(questID);
                            questSet = questSet + ";" + q.QID.ToString() + "-" + q.Qtype.ToString();
                        }
                        else
                        {
                            int matchID = int.Parse(idAndTypeSplit[0]);
                            var m = db.MatchQuestions.Find(matchID);
                            questSet = questSet + ";" + m.MID.ToString() + "-5";
                        }

                    }
                }

            }
            else
            {
                //check if questions list is null
                if (questions != null)
                {
                    string[] idAndTypes = collection["qIDAndType"].Split(new char[] { ',' });
                    foreach (string id in idAndTypes)
                    {
                        string[] idAndTypeSplit = id.Split(new char[] { '-' });
                        int qtypeID = int.Parse(idAndTypeSplit[1]);
                        if (qtypeID != 5)
                        {
                            int questID = int.Parse(idAndTypeSplit[0]);
                            var q = db.Questions.Find(questID);
                            questSet = questSet + q.QID.ToString() + "-" + q.Qtype.ToString() + ";";
                        }
                        else
                        {
                            int matchID = int.Parse(idAndTypeSplit[0]);
                            var m = db.MatchQuestions.Find(matchID);
                            questSet = questSet + m.MID.ToString() + "-5;";
                        }

                    }

                }

                if (questSet != null && !questSet.Trim().Equals(""))
                {
                    questSet = questSet.Substring(0, questSet.Length - 1);
                }
            }


            int courseId = int.Parse(cid);

            if (questSet == null || questSet.Trim().Equals(""))
            {
                return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + courseId + "&tempName=" + tempName + "&tempMode=" + tempMode);
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
                Session["TemporaryQuestions"] = questSet;
                /*Debug.WriteLine("okla2222aa-" + questSet + tempName);*/

                return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + courseId + "&tempName=" + tempName + "&tempMode=" + tempMode + "&i=" + page);

            }
        }

        //Delete question inside temporary list
        [HandleError]
        public ActionResult DeleteQuestionsInsideTemporaryQuiz(string qid, string qtype, /*string questSet,*/ string cid, string tempName, int? i)
        {
            string questSet = Convert.ToString(HttpContext.Session["TemporaryQuestions"]);
            Debug.WriteLine(questSet);

            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
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
            if (newQuestionSet != null && !newQuestionSet.Equals(""))
            {
                newQuestionSet = newQuestionSet.Substring(0, newQuestionSet.Length - 1);

            }
            Session["TemporaryQuestions"] = newQuestionSet;

            return Redirect("~/Teacher/Quiz/CreateNewQuiz?cid=" + cid + "&tempName=" + tempName);
        }

        // teacher click on start quiz button
        [HandleError]
        public ActionResult QuizStarted(string qzid, int? i, FormCollection form)
        {
            //check if quiz is availble
            if (checkQuizIdAvailbile(qzid) == false)
            {
                Debug.WriteLine("nope");
                return RedirectToAction("QuizLibrary");
            }
            else
            {
                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                string quizType = form["qtype"];
                /*Debug.WriteLine(quizType+"-----67-67-6-7-6"+ form["qtype"]);*/
                int quizID = int.Parse(qzid);
                var quiz = db.Quizs.Find(quizID);
                /*var course = db.Courses.Find(quiz.CourseID);*/
                List<Question> qList = new List<Question>();
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
                            var mQuest = db.MatchQuestions.Find(mID);
                            Question matchQuest = new Question();
                            matchQuest.QID = mQuest.MID;
                            matchQuest.Text = mQuest.ColumnA + "//" + mQuest.ColumnB;
                            matchQuest.Qtype = 5;
                            matchQuest.Mark = mQuest.Mark;
                            qList.Add(matchQuest);
                            /*matchingSet.Add(mID, "5");*/
                        }
                        else
                        {
                            int qID = int.Parse(questAndType[0]);
                            var quest = db.Questions.Find(qID);
                            qList.Add(quest);
                            /*questionSet.Add(qID, questAndType[1]);*/
                        }

                    }

                }
                ViewBag.Quiz = quiz;
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

                if (i == null)
                {
                    i = 1;
                }
                if (i == 0)
                {
                    i = null;
                }
                ViewBag.QuestionNo = (i - 1) * 10;
                ViewBag.Quiz = quiz;

                //if quiz status not "doing"
                if (!quiz.Status.Equals("Doing"))
                {
                    string qStringForQuizSave = "";
                    double qSaveMark = 0;
                    int qSaveTime = 0;
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

                                var loList = db.QuestionLOes.Where(ql => ql.QuestionID == quest.QID && ql.Qtype == quest.Qtype).ToList();
                                foreach (var ql in loList)
                                {
                                    QuestionDoneLO qdLO = new QuestionDoneLO();
                                    qdLO.QuestionDoneID = qSaveID;
                                    qdLO.Qtype = qSave.Qtype;
                                    qdLO.LearningOutcomeID = ql.LearningOutcomeID;
                                    db.QuestionDoneLOes.Add(qdLO);
                                }
                                /*db.SaveChanges();*/
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

                                var loList = db.QuestionLOes.Where(ql => ql.QuestionID == mQuest.MID && ql.Qtype == 5).ToList();
                                foreach (var ql in loList)
                                {
                                    QuestionDoneLO qdLO = new QuestionDoneLO();
                                    qdLO.QuestionDoneID = mSaveID;
                                    qdLO.Qtype = 5;
                                    qdLO.LearningOutcomeID = ql.LearningOutcomeID;
                                    db.QuestionDoneLOes.Add(qdLO);
                                }
                            }
                            /*db.SaveChanges();*/
                        }


                    }

                    //save quiz to database
                    quiz.Status = "Doing";
                    quiz.QuizType = quizType;
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
                    saveQuiz.QuizType = quiz.QuizType;
                    DateTime now = DateTime.Now;
                    DateTime endTime = now.AddSeconds(double.Parse(saveQuiz.Time.ToString()));
                    saveQuiz.EndTime = endTime;
                    TimeSpan totalTime = (endTime - now);
                    int countDown = (int)totalTime.TotalSeconds;
                    ViewBag.CountDown = countDown + 10;
                    db.QuizDones.Add(saveQuiz);
                    db.SaveChanges();

                    /* int quizSaveID = int.Parse(db.QuizDones.OrderByDescending(q => q.QuizDoneID).Where(q => q.CourseID == saveQuiz.CourseID).Select(q => q.QuizDoneID).First().ToString());

                     db.SaveChanges();*/
                    return View(qList.ToPagedList(i ?? 1, 10));
                }
                else
                {
                    var doingQuiz = db.QuizDones.Where(qz => qz.QuizID == quizID).OrderByDescending(qz => qz.QuizDoneID).FirstOrDefault();
                    DateTime now = DateTime.Now;

                    //if time is expired
                    if (now > doingQuiz.EndTime)
                    {
                        quiz.Status = "Done";
                        db.Entry(quiz).State = EntityState.Modified;
                        db.SaveChanges();
                        return Redirect("~/Teacher/Quiz/QuizDetail?qzID=" + quiz.QuizID);
                    }
                    else
                    {

                        TimeSpan? totalTime = doingQuiz.EndTime - now;
                        int? countDown = (int?)totalTime?.TotalSeconds;
                        ViewBag.CountDown = countDown + 10;

                        return View(qList.ToPagedList(i ?? 1, 10));
                    }

                }
            }


        }

        // teacher click on finish quiz button
        [HandleError]
        [HttpPost]
        public ActionResult FinishQuiz(string qzid)
        {
            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
            int quizID = int.Parse(qzid);
            var quiz = db.Quizs.Find(quizID);
            quiz.Status = "Done";
            db.Entry(quiz).State = EntityState.Modified;

            /* var quizDoneID = db.QuizDones.OrderByDescending(qd => qd.QuizDoneID).Where(qd => qd.QuizID == quizID).Select(qd => qd.QuizDoneID).FirstOrDefault();
             var quizDoingByStudent = db.Student_QuizDone.Where(sq => sq.QuizDoneID == quizDoneID).ToList();

             foreach(var studentQuiz in quizDoingByStudent)
             {
                 studentQuiz.Status = "Done";
                 db.Entry(studentQuiz).State = EntityState.Modified;
             }
 */
            db.SaveChanges();
            var quizDoneID = db.QuizDones.OrderByDescending(qd => qd.QuizDoneID).Where(qd => qd.QuizID == quizID).Select(qd => qd.QuizDoneID).FirstOrDefault();
            return Redirect("~/Teacher/Report/ReportByStudent?qzid=" + quizDoneID);

        }

        [HandleError]
        public ActionResult PreviewQuiz(string qzid, int rdPreview)
        {
            //check if quiz is availble
            if (checkQuizIdAvailbile(qzid) == false)
            {
                Debug.WriteLine("nope");
                return RedirectToAction("QuizLibrary");
            }
            else
            {

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                if (rdPreview == 1)
                {
                    return Redirect("~/Teacher/Quiz/PreviewQuizPaperTest?qzid=" + qzid);
                }
                else
                {
                    return Redirect("~/Teacher/Quiz/PreviewQuizQuestionByQuestion?qzid=" + qzid);
                }
            }
        }

        [HandleError]
        public ActionResult PreviewQuizPaperTest(string qzid)
        {
            //check if quiz is availble
            if (checkQuizIdAvailbile(qzid) == false)
            {
                Debug.WriteLine("nope");
                return RedirectToAction("QuizLibrary");
            }
            else
            {

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
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


                int time = quiz.Time;
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

        }

        [HandleError]
        public ActionResult PreviewQuizQuestionByQuestion(string qzid)
        {
            //check if quiz is availble
            if (checkQuizIdAvailbile(qzid) == false)
            {
                Debug.WriteLine("nope");
                return RedirectToAction("QuizLibrary");
            }
            else
            {

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                int quizId = int.Parse(qzid);
                var quiz = db.Quizs.Find(quizId);
                List<Question> questionsList = new List<Question>();
                /*List<MatchQuestion> matchQuestionsList = new List<MatchQuestion>();
                List<Passage> passageList = new List<Passage>();*/
                //check if questions list is null
                if (quiz.Questions != null && !quiz.Questions.Equals(""))
                {
                    //////////////////////////////////////
                    string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
                    List<string> questionIdList = quizQuestions.ToList();

                    //////////////////////////////////////

                    /*Dictionary<int, string> questionSet = new Dictionary<int, string>();
                    Dictionary<int, string> matchingSet = new Dictionary<int, string>();*/
                    foreach (string questions in questionIdList)
                    {
                        string[] questAndType = questions.Split(new char[] { '-' });
                        int qType = int.Parse(questAndType[1]);
                        if (qType == 5)
                        {
                            int mID = int.Parse(questAndType[0]);
                            var matchQuest = db.MatchQuestions.Find(mID);
                            Question matching = new Question();
                            matching.QID = mID;
                            matching.Text = matchQuest.ColumnA + "//" + matchQuest.ColumnB;
                            matching.Time = matchQuest.Time;
                            matching.Qtype = 5;
                            questionsList.Add(matching);
                            /*matchingSet.Add(mID, questAndType[1]);*/
                        }
                        else
                        {
                            int qID = int.Parse(questAndType[0]);
                            var question = db.Questions.Find(qID);
                            if (qType == 1 || qType == 2)
                            {
                                List<QuestionAnswer> qAnswer = question.QuestionAnswers.ToList();
                                //if question mix choice
                                if (question.MixChoice == true)
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
                                    question.QuestionAnswers = qAnswer;
                                }

                            }
                            questionsList.Add(question);
                            /*questionSet.Add(qID, questAndType[1]);*/
                        }

                    }


                }

                ViewBag.Quiz = quiz;
                ViewBag.QuestionList = questionsList;
                /*ViewBag.PassageList = passageList;*/
                /*ViewBag.MatchingQuestion = matchQuestionsList;*/



                return View();
            }

        }
    }


}