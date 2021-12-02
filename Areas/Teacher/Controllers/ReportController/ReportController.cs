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
            ViewBag.QuizCount = (i - 1) * 10;
            ViewBag.Course = course;
            ViewBag.Search = searchText;
            return View(quizzes.ToPagedList(i ?? 1, 10));
        }

        public ActionResult ReportByQuestion(string qzid, string searchText)
        {
            int quizDoneID = int.Parse(qzid);
            var qzDone = db.QuizDones.Find(quizDoneID);
            List<QuestionDone> questionsList = new List<QuestionDone>();
            List<Chapter> chapterList = new List<Chapter>();
            //get question
            if (qzDone.Questions != null && !qzDone.Questions.Equals(""))
            {
                List<string> questSet = qzDone.Questions.Split(new char[] { ';' }).ToList();
                /*Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();*/

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
                        if (matchQuest.Chapter != null)
                        {
                            bool isExisted = false;
                            foreach (Chapter c in chapterList)
                            {
                                if (matchQuest.ChapterID == c.ChID)
                                {
                                    isExisted = true;
                                    Debug.WriteLine("--3--" + matchQuest.Chapter.ChID);

                                }
                            }
                            if (isExisted == false)
                            {
                                chapterList.Add(matchQuest.Chapter);
                            }

                        }

                        matching.Text = matchQuest.ColumnA + "//" + matchQuest.ColumnB;
                        matching.Time = matchQuest.Time;
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
                        if (question.Chapter != null)
                        {
                            bool isExisted = false;
                            foreach (Chapter c in chapterList)
                            {
                                if (question.ChapterID == c.ChID)
                                {
                                    isExisted = true;
                                    Debug.WriteLine("--1--" + question.Chapter.ChID);
                                }
                            }
                            if (isExisted == false)
                            {
                                chapterList.Add(question.Chapter);
                            }


                        }

                        questionsList.Add(question);
                    }
                }


            }

            foreach (Chapter c in chapterList)
            {
                Debug.WriteLine("jijiji" + c.ChID);
            }

            if (searchText != null && !searchText.Trim().Equals(""))
            {
                questionsList = questionsList.Where(ql => ql.Text.Trim().ToLower().Contains(searchText.ToLower())).ToList();
                /*matchQuestionsList = matchQuestionsList.Where(ml => ml.ColumnA.Trim().ToLower().Contains(searchText.ToLower()) ||
                 ml.ColumnB.Trim().ToLower().Contains(searchText.ToLower())).ToList();*/
            }

            var studentDoneTest = db.Student_QuizDone.Where(stq => stq.QuizDoneID == quizDoneID).ToList();

            ViewBag.Quiz = qzDone;
            ViewBag.QuestionList = questionsList;
            ViewBag.QuestCount = questionsList.Count;
            ViewBag.StudentCount = studentDoneTest.Count;
            ViewBag.Search = searchText;
            ViewBag.ChapterList = chapterList.OrderBy(c=>c.ChID);

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
                return RedirectToAction("ReportByStudent", new { qzid = qzid, searchText = searchText });
            }
            else
            {
                return RedirectToAction("ReportByQuestion", new { qzid = qzid, searchText = searchText });
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
    }
}