using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Diagnostics;
using System.Data.Entity;
using InClassVoting.Filter;

namespace InClassVoting.Areas.Teacher.Controllers.ReportController
{
    [AccessAuthenticationFilter]
    [UserAuthorizeFilter("Teacher")]
    public class ReportController : Controller
    {
        private DBModel db = new DBModel();

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

        private bool checkQuizDoneIdAvailbile(string qzid)
        {
            bool check = true;
            int quizDoneId;
            bool isInt = int.TryParse(qzid, out quizDoneId);
            //check if chapter id is int
            if (isInt == false)
            {
                check = false;
            }
            else
            {
                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                var quiz = db.QuizDones.Find(quizDoneId);
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

        private bool checkStudentIdAvailbile(string qzid, string stid)
        {
            bool check = true;
            int quizDoneId;
            int studentId;
            bool quizIsInt = int.TryParse(qzid, out quizDoneId);
            bool studentIsInt = int.TryParse(stid, out studentId);
            //check if quiz and student id is int
            if (quizIsInt == false || studentIsInt == false)
            {
                check = false;
            }
            else
            {
                int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);
                var student = db.Students.Find(studentId);
                var quiz = db.QuizDones.Find(quizDoneId);
                var student_quiz = db.Student_QuizDone.Where(sq => sq.QuizDoneID == quizDoneId && sq.StudentID == studentId).FirstOrDefault();
                //check if student quizdone exist in db
                if (student_quiz == null || student == null || quiz == null)
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

        [HandleError]
        public ActionResult ReportHome()
        {
            
            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
            return View();
            
        }

        [HandleError]
        public ActionResult ViewReportListByCourse(string cid, string searchText, int? i)
        {
            //check if course is availble
            if (checkCourserIdAvailbile(cid) == false)
            {
                return RedirectToAction("ReportHome");
            }
            else
            {
                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                int courseId = int.Parse(cid);
                var course = db.Courses.Find(courseId);
                var listQuizDone = db.QuizDones.Where(q => q.CourseID == courseId)/*.OrderByDescending(q => q.QuizDoneID)*/.ToList();

                List<QuizDone> quizzes = new List<QuizDone>();
                if (searchText != null && !searchText.Trim().Equals(""))
                {
                    quizzes = listQuizDone.Where(qz => qz.Quiz_Name.Trim().ToLower().Contains(searchText.Trim().ToLower())).ToList();
                }
                else
                {
                    quizzes = listQuizDone;
                }

                if (i == null || i == 0)
                {
                    i = 1;
                }
                else
                {
                    if (quizzes.Count % 10 == 0 && i > quizzes.Count / 10)
                    {
                        i = 1;
                    }
                    else if (quizzes.Count % 10 != 0 && i > ((quizzes.Count / 10) + 1))
                    {
                        i = 1;
                    }

                }

                /*quizzes = quizzes.Where(qz => qz.Student_QuizDone.Count != 0).ToList();*/
                ViewBag.QuizCount = (i - 1) * 10;
                ViewBag.Course = course;
                ViewBag.Search = searchText;
                ViewBag.CountReport = db.QuizDones.Where(q => q.CourseID == courseId).Count();
                return View(quizzes.ToPagedList(i ?? 1, 10));
            }
        }

        [HandleError]
        public ActionResult ReportByQuestion(string qzid, string searchText)
        {
            //check if quiz done is availble
            if (checkQuizDoneIdAvailbile(qzid) == false)
            {
                Debug.WriteLine("nope");
                return RedirectToAction("ReportHome");
            }
            else
            {

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                int quizDoneID = int.Parse(qzid);
                var qzDone = db.QuizDones.Find(quizDoneID);
                List<QuestionDone> questionsList = new List<QuestionDone>();

                //get question
                if (qzDone.Questions != null && !qzDone.Questions.Equals(""))
                {
                    List<string> questSet = qzDone.Questions.Split(new char[] { ';' }).ToList();

                    //get question in report
                    foreach (string questions in questSet)
                    {
                        string[] questAndType = questions.Split(new char[] { '-' });
                        int qType = int.Parse(questAndType[1]);
                        //if question is matching
                        if (qType == 5)
                        {
                            int mID = int.Parse(questAndType[0]);
                            var matchQuest = db.MatchQuestionDones.Find(mID);
                            QuestionDone matching = new QuestionDone();
                            matching.Q_DoneID = mID;
                            Debug.WriteLine("--2--" + matchQuest.Chapter.ChID);
                            matching.Chapter = matchQuest.Chapter;
                            matching.ChapterID = matchQuest.ChapterID;
                            matching.Text = matchQuest.ColumnA + "//" + matchQuest.ColumnB;
                            matching.Time = matchQuest.Time;
                            matching.Mark = matchQuest.Mark;
                            matching.StudentReceive = matchQuest.StudentReceive;
                            matching.CorrectNumber = matchQuest.CorrectNumber;
                            matching.Qtype = 5;
                            questionsList.Add(matching);
                        }
                        else
                        {
                            int qID = int.Parse(questAndType[0]);
                            QuestionDone question = db.QuestionDones.Find(qID);
                            Debug.WriteLine("--0--" + question.Chapter.ChID);
                            questionsList.Add(question);
                        }
                    }


                }
                List<Chapter> chapterList = new List<Chapter>();
                //get question chapter
                foreach (var q in questionsList)
                {
                    if (q.Chapter != null)
                    {
                        bool isExisted = false;
                        foreach (Chapter c in chapterList)
                        {
                            if (q.ChapterID == c.ChID)
                            {
                                isExisted = true;
                                /*Debug.WriteLine("--1--" + q.Chapter.ChID);*/
                            }
                        }
                        if (isExisted == false)
                        {
                            chapterList.Add(q.Chapter);
                        }

                    }

                }

                List<LearningOutcome> loList = new List<LearningOutcome>();

                //get questionLO
                foreach (var q in questionsList)
                {
                    var qdLOList = db.QuestionDoneLOes.Where(ql => ql.QuestionDoneID == q.Q_DoneID).ToList();
                    foreach (var qdLo in qdLOList)
                    {
                        var lo = qdLo.LearningOutcome;
                        if (!loList.Contains(lo))
                        {
                            loList.Add(lo);
                        }
                    }
                }

                if (searchText != null && !searchText.Trim().Equals(""))
                {
                    questionsList = questionsList.Where(ql => ql.Text.Trim().ToLower().Contains(searchText.ToLower())).ToList();
                }

               
                var studentDoneTest = db.Student_QuizDone.Where(stq => stq.QuizDoneID == quizDoneID).ToList();

                ViewBag.Quiz = qzDone;
                ViewBag.QuestionList = questionsList;
                ViewBag.QuestCount = questionsList.Count;
                ViewBag.StudentCount = studentDoneTest.Count;
                ViewBag.Search = searchText;
                ViewBag.ChapterList = chapterList.OrderBy(c => c.ChID);
                ViewBag.LOList = loList.OrderBy(lo => lo.LOID);
                ViewBag.QuestionLoList = db.QuestionDoneLOes.Where(ql => ql.LearningOutcome.CourseID == qzDone.CourseID).ToList();
                return View();
            }
        }

        [HandleError]
        public ActionResult ReportByStudent(string qzid, string searchText)
        {
            //check if quiz done is availble
            if (checkQuizDoneIdAvailbile(qzid) == false)
            {
                Debug.WriteLine("nope");
                return RedirectToAction("ReportHome");
            }
            else
            {

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                int quizDoneID = int.Parse(qzid);
                var qzDone = db.QuizDones.Find(quizDoneID);


                var studentDoneTest = db.Student_QuizDone.Where(stq => stq.QuizDoneID == quizDoneID).ToList();

                foreach (var stu in studentDoneTest)
                {
                    Debug.WriteLine(stu.SQID + "===" + stu.QuizDoneID + "==" + stu.Student.Name + "==" + stu.ReceivedQuestions);
                }

                if (searchText != null && !searchText.Trim().Equals(""))
                {
                    studentDoneTest = studentDoneTest.Where(st => st.Student.Name.Trim().ToLower().Contains(searchText.ToLower())).ToList();
                }
                else
                {
                    /*searchText*/
                }

                ViewBag.Quiz = qzDone;
                ViewBag.QuestCount = qzDone.NumOfQuestion;
                ViewBag.StudentCount = studentDoneTest.Count;
                ViewBag.StudenDone = studentDoneTest;
                ViewBag.Search = searchText;

                return View();
            }
        }

        [HandleError]
        public ActionResult Search(string qzid, string searchType, string searchText)
        {

            int type = int.Parse(searchType);
            if (type == 1)
            {
                return RedirectToAction("ReportByStudent", new { qzid = qzid, searchText = searchText });
            }
            else
            {
                return RedirectToAction("ReportByQuestion", new { qzid = qzid, searchText = searchText });
            }
        }

        [HandleError]
        public ActionResult ReportStudentQuiz(string qzid, string stid)
        {
            //check if quiz done is availble
            if (checkQuizDoneIdAvailbile(qzid) == false || checkStudentIdAvailbile(qzid, stid) == false)
            {
                Debug.WriteLine("nope");
                return RedirectToAction("ReportHome");
            }
            else
            {

                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                if (stid == null)
                {
                    stid = "1";
                }

                int quizId = int.Parse(qzid);
                int studentId = int.Parse(stid);
                var student_quiz = db.Student_QuizDone.Where(sq => sq.StudentID == studentId && sq.QuizDoneID == quizId).OrderByDescending(sq => sq.SQID).FirstOrDefault();
                

                string[] questionReceived = student_quiz.ReceivedQuestions.Split(new char[] { ';' });

                //get student answer
                var student_Answers = db.Student_Answer.Where(sa => sa.QuizDoneID == student_quiz.QuizDoneID && sa.StudentID == studentId).ToList();


                Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                List<QuestionDone> multipleQuestionsList = new List<QuestionDone>();
                List<QuestionDone> readingQuestionsList = new List<QuestionDone>();
                List<QuestionDone> fillBlankQuestionsList = new List<QuestionDone>();
                List<QuestionDone> shortAnswerQuestionsList = new List<QuestionDone>();
                List<QuestionDone> indicateMistakeQuestionsList = new List<QuestionDone>();
                List<MatchQuestionDone> matchQuestionsList = new List<MatchQuestionDone>();
                List<Passage_Done> passageList = new List<Passage_Done>();

                //get question that student received
                foreach (string questions in questionReceived)
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
                    var quest = db.QuestionDones.Find(keyValuePair.Key);

                    if (quest.Qtype == 1)
                    {

                        multipleQuestionsList.Add(quest);

                    }
                    else if (quest.Qtype == 2)
                    {
                        readingQuestionsList.Add(quest);
                        //add passage to a list
                        var passage = quest.Passage_Done;
                        bool existed = false;
                        foreach (var p in passageList)
                        {
                            if (passage.P_DoneID == p.P_DoneID)
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
                    var matchQuest = db.MatchQuestionDones.Find(keyValuePair.Key);
                    matchQuestionsList.Add(matchQuest);
                }

                double? markPercentage = (student_quiz.StudentMark / student_quiz.TotalMark) * 100;

                int percentage = Convert.ToInt32(markPercentage);

                ViewBag.MultipleQuestion = multipleQuestionsList;
                ViewBag.FillBlankQuestion = fillBlankQuestionsList;
                ViewBag.ShortAnswerQuestion = shortAnswerQuestionsList;
                ViewBag.IndicateMistakeQuestion = indicateMistakeQuestionsList;
                ViewBag.ReadingQuestion = readingQuestionsList;
                ViewBag.PassageList = passageList;
                ViewBag.MatchingQuestion = matchQuestionsList;
                ViewBag.Percentage = percentage;
                ViewBag.StudentAnswer = student_Answers;
                ViewBag.Quiz = db.QuizDones.Find(quizId);
                ViewBag.Student = db.Students.Find(studentId);

                return View();
            }
        }

        [HandleError]
        [HttpPost]
        public ActionResult SaveReportOption(string qzID, string currentPage, string searchText, string cbPublishMark, string cbPublishAnswer)
        {
            int quizDoneID = int.Parse(qzID);
            var qzDone = db.QuizDones.Find(quizDoneID);

            if (cbPublishMark != null)
            {
                qzDone.PublicResult = true;
            }
            else
            {
                qzDone.PublicResult = false;
            }

            if (cbPublishAnswer != null)
            {
                qzDone.PublicAnswer = true;
            }
            else
            {
                qzDone.PublicAnswer = false;
            }

            db.Entry(qzDone).State = EntityState.Modified;
            db.SaveChanges();

            int page = int.Parse(currentPage);
            if (page == 1)
            {
                return Redirect("~/Teacher/Report/ReportByQuestion?qzid=" + quizDoneID + "&searchText=" + searchText);
            }
            else
            {
                return Redirect("~/Teacher/Report/ReportByStudent?qzid=" + quizDoneID + "&searchText=" + searchText);
            }
        }

        [HandleError]
        public ActionResult ReportPollList(string searchText, int? i)
        {
            ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
            ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
            int teacherId = Convert.ToInt32(HttpContext.Session["TeacherId"]);

            var listPoll = db.Polls.Where(p => p.TeacherID == teacherId &&
            p.TotalParticipian != 0 || p.TeacherID == teacherId && p.IsDoing == true).OrderByDescending(p => p.PollID).ToList();

            List<Poll> polls = new List<Poll>();
            if (searchText != null && !searchText.Trim().Equals(""))
            {
                polls = listPoll.Where(p => p.Question.Trim().ToLower().Contains(searchText.Trim().ToLower())).ToList();
            }
            else
            {
                polls = listPoll;
            }

            if (i == null || i == 0)
            {
                i = 1;
            }
            else
            {
                if (polls.Count % 10 == 0 && i > polls.Count / 10)
                {
                    i = 1;
                }
                else if (polls.Count % 10 != 0 && i > ((polls.Count / 10) + 1))
                {
                    i = 1;
                }

            }

            ViewBag.PollCount = (i - 1) * 10;
            ViewBag.Search = searchText;
            ViewBag.CountPoll = listPoll.Count;
            return View(polls.ToPagedList(i ?? 1, 10));
        }

        [HandleError]
        [HttpPost]
        public ActionResult DeleteReport(string qzid)
        {
            int quizDoneID = int.Parse(qzid);
            var quizDone = db.QuizDones.Find(quizDoneID);
            int courseId = quizDone.CourseID;

            string [] questSet = quizDone.Questions.Split(new char[] { ';' });
      

            //delete question inside report
            foreach (string qIdAndType in questSet)
            {
                string[] questAndType = qIdAndType.Split(new char[] { '-' });
                int qtypeID = int.Parse(questAndType[1]);
                if (qtypeID == 5)
                {
                    int mID = int.Parse(questAndType[0]);
                    var matchQuest = db.MatchQuestionDones.Find(mID);
                    db.MatchQuestionDones.Remove(matchQuest);
                    var questionDoneLO = db.QuestionDoneLOes.Where(ql => ql.QuestionDoneID == mID && ql.Qtype == 5).ToList();
                    //deleteLO
                    foreach (var qlo in questionDoneLO)
                    {
                        db.QuestionDoneLOes.Remove(qlo);
                    }
                }
                else
                {
                    int qID = int.Parse(questAndType[0]);
                    var questionDone = db.QuestionDones.Find(qID);
                    var qaList = db.QuestionAnswerDones.Where(qa => qa.QuestionID == qID).ToList();
                    //remove answer of question
                    foreach (var qa in qaList)
                    {
                        db.QuestionAnswerDones.Remove(qa);
                    }

                    db.QuestionDones.Remove(questionDone);
                    var questionDoneLO = db.QuestionDoneLOes.Where(ql => ql.QuestionDoneID == qID && ql.Qtype == questionDone.Qtype).ToList();

                    //deleteLO
                    foreach (var qlo in questionDoneLO)
                    {
                        db.QuestionDoneLOes.Remove(qlo);
                    }
                }
            }



            var studentAnswers = db.Student_Answer.Where(sa => sa.QuizDoneID == quizDoneID).ToList();
            //delete student answer
            foreach (var studentAnswer in studentAnswers)
            {
                db.Student_Answer.Remove(studentAnswer);
            }

            var studenWorks = db.Student_QuizDone.Where(sq => sq.QuizDoneID == quizDoneID).ToList();
            //delete student report
            foreach(var studentWork in studenWorks)
            {
                db.Student_QuizDone.Remove(studentWork);
            }

            db.QuizDones.Remove(quizDone);
            db.SaveChanges();

            return Redirect("~/Teacher/Report/ViewReportListByCourse?cid=" + courseId);
        }
        
        [HandleError]
        [HttpPost]
        public ActionResult EditReportName(string qzid, string newReportName)
        {
            int quizDoneID = int.Parse(qzid);
            var quizDone = db.QuizDones.Find(quizDoneID);

            quizDone.Quiz_Name = newReportName;
            db.Entry(quizDone).State = EntityState.Modified;
            db.SaveChanges();
            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}