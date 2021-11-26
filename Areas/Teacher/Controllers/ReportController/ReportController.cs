using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;
using System.Diagnostics;

namespace InClassVoting.Areas.Teacher.Controllers.ReportController
{
    public class ReportController : Controller
    {
        private DBModel db = new DBModel();
        // GET: Teacher/Report
        public ActionResult ReportHome()
        {
            return View();
        }

        public ActionResult ViewReportListByCourse(string cid, string searchText, int? i)
        {
            int courseId = int.Parse(cid);
            var course = db.Courses.Find(courseId);
            var listQuizDone = db.QuizDones.Where(q => q.CourseID == courseId).OrderByDescending(q => q.QuizDoneID).ToList();
            if (i == null)
            {
                i = 1;
            }

            List<QuizDone> quizzes = new List<QuizDone>();
            if (searchText != null && !searchText.Trim().Equals(""))
            {
                quizzes = listQuizDone.Where(qz => qz.Quiz_Name.Trim().ToLower().Contains(searchText.Trim().ToLower())).ToList();
                Debug.WriteLine("hjhihi12" + searchText);

            }
            else
            {
                quizzes = listQuizDone;
                Debug.WriteLine("hjhihi" + searchText);
            }
            ViewBag.QuizCount = (i - 1) * 3;
            ViewBag.Course = course;
            ViewBag.Search = searchText;
            return View(quizzes.ToPagedList(i ?? 1, 3));
        }

        public ActionResult ReportByQuestion(string qzid, string searchText)
        {
            int quizDoneID = int.Parse(qzid);
            var qzDone = db.QuizDones.Find(quizDoneID);

            List<QuestionDone> questionsList = new List<QuestionDone>();
            List<MatchQuestionDone> matchQuestionsList = new List<MatchQuestionDone>();
            //get question
            if (qzDone.Questions != null && !qzDone.Questions.Equals(""))
            {
                List<string> questSet = qzDone.Questions.Split(new char[] { ';' }).ToList();

                Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                foreach (string questions in questSet)
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
                    questionsList.Add(quest);
                }

                foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                {
                    var matchQuest = db.MatchQuestionDones.Find(keyValuePair.Key);
                    matchQuestionsList.Add(matchQuest);

                }
            }

            if (searchText != null && !searchText.Trim().Equals(""))
            {
                questionsList = questionsList.Where(ql => ql.Text.Trim().ToLower().Contains(searchText.ToLower())).ToList();
                matchQuestionsList = matchQuestionsList.Where(ml => ml.ColumnA.Trim().ToLower().Contains(searchText.ToLower()) ||
                 ml.ColumnB.Trim().ToLower().Contains(searchText.ToLower())).ToList();
            }

            var studentDoneTest = db.Student_QuizDone.Where(stq => stq.QuizDoneID == quizDoneID).ToList();

            ViewBag.Quiz = qzDone;
            ViewBag.QuestionList = questionsList;
            ViewBag.MatchingList = matchQuestionsList;
            ViewBag.QuestCount = questionsList.Count + matchQuestionsList.Count;
            ViewBag.StudentCount = studentDoneTest.Count;
            ViewBag.Search = searchText;

            return View();
        }


        public ActionResult ReportByStudent(string qzid, string searchText)
        {
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

        public ActionResult Search(string qzid, string searchType, string searchText)
        {
            int type = int.Parse(searchType);
            if (type == 1)
            {
                return RedirectToAction("ReportByStudent", new { @qzid = qzid, @searchText = searchText });
            }
            else
            {
                return RedirectToAction("ReportByQuestion", new { @qzid = qzid, @searchText = searchText });
            }
        }



        public ActionResult ReportStudentQuiz(string qzid, string stid)
        {
            if (stid == null)
            {
                stid = "1";
            }

            int quizId = int.Parse(qzid);
            int studentId = int.Parse(stid);
            var student_quiz = db.Student_QuizDone.Where(sq => sq.StudentID == studentId && sq.QuizDoneID == quizId).OrderByDescending(sq => sq.SQID).FirstOrDefault();
            Debug.WriteLine("----" + student_quiz.SQID);

            string[] questionReceived = student_quiz.ReceivedQuestions.Split(new char[] { ';' });

            //get student answer
            var student_Answers = db.Student_Answer.Where(sa => sa.QuizDoneID == student_quiz.QuizDoneID).ToList();
            foreach (var st in student_Answers)
            {
                Debug.WriteLine(st.StudentID + "==" + st.QuestionDoneID + "===" + st.Answer + "==" + st.IsCorrect);
            }

            Dictionary<int, string> questionSet = new Dictionary<int, string>();
            Dictionary<int, string> matchingSet = new Dictionary<int, string>();
            List<QuestionDone> multipleQuestionsList = new List<QuestionDone>();
            List<QuestionDone> readingQuestionsList = new List<QuestionDone>();
            List<QuestionDone> fillBlankQuestionsList = new List<QuestionDone>();
            List<QuestionDone> shortAnswerQuestionsList = new List<QuestionDone>();
            List<QuestionDone> indicateMistakeQuestionsList = new List<QuestionDone>();
            List<MatchQuestionDone> matchQuestionsList = new List<MatchQuestionDone>();

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
            ViewBag.MatchingQuestion = matchQuestionsList;
            ViewBag.Percentage = percentage;
            ViewBag.StudentAnswer = student_Answers;
            ViewBag.Quiz = db.QuizDones.Find(quizId);
            ViewBag.Student = db.Students.Find(studentId);

            return View();
        }
    }
}