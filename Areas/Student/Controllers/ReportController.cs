using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.Student.Controllers
{
    public class ReportController : Controller
    {
        private DBModel db = new DBModel();
        // GET: Student/Report

        public ActionResult ReportHome()
        {
            return View();
        }
        public ActionResult QuizReport(string qzid, string stid)
        {
            int quizId = int.Parse(qzid);
            int studentId = int.Parse(stid);
            

            var student_quiz = db.Student_QuizDone.Where(sq => sq.StudentID == studentId && sq.QuizDoneID == quizId).OrderByDescending(sq => sq.SQID).FirstOrDefault();
            if (student_quiz.QuizDone.PublicResult==true)
            {
                double? markPercentage = (student_quiz.StudentMark / student_quiz.TotalMark) * 100;
                int percentage = Convert.ToInt32(markPercentage);
                ViewBag.Percentage = percentage;
            }


            List<QuestionDone> multipleQuestionsList = new List<QuestionDone>();
            List<QuestionDone> readingQuestionsList = new List<QuestionDone>();
            List<QuestionDone> fillBlankQuestionsList = new List<QuestionDone>();
            List<QuestionDone> shortAnswerQuestionsList = new List<QuestionDone>();
            List<QuestionDone> indicateMistakeQuestionsList = new List<QuestionDone>();
            List<MatchQuestionDone> matchQuestionsList = new List<MatchQuestionDone>();

            
            List<Student_Answer> student_Answers = new List<Student_Answer>();
            if (student_quiz.QuizDone.PublicAnswer == true)
            {
                //get student answer
                student_Answers= db.Student_Answer.Where(sa => sa.QuizDoneID == student_quiz.QuizDoneID).ToList();
                string[] questionReceived = student_quiz.ReceivedQuestions.Split(new char[] { ';' });

                Dictionary<int, string> questionSet = new Dictionary<int, string>();
                Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                

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

               
                
            }
            ViewBag.Quiz = db.QuizDones.Find(quizId);
            ViewBag.Student = db.Students.Find(studentId);
            ViewBag.MultipleQuestion = multipleQuestionsList;
            ViewBag.FillBlankQuestion = fillBlankQuestionsList;
            ViewBag.ShortAnswerQuestion = shortAnswerQuestionsList;
            ViewBag.IndicateMistakeQuestion = indicateMistakeQuestionsList;
            ViewBag.ReadingQuestion = readingQuestionsList;
            ViewBag.MatchingQuestion = matchQuestionsList;
            ViewBag.StudentAnswer = student_Answers;
            return View();
        }
    }
}