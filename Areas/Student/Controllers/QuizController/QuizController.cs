using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.Student.Controllers.QuizController
{
    public class QuizController : Controller
    {
        private DBModel db = new DBModel();
        // GET: Student/Quiz
        public ActionResult DoQuizPaperTest(string qzid)
        {
            int quizId = int.Parse(qzid);
            var quiz = db.Quizs.Find(quizId);
            List<Question> mulChoiceList = new List<Question>();
            /*List<MatchQuestion> mList = new List<MatchQuestion>();*/
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
                        /*
                        string[] questAndType = questions.Split(new char[] { '-' });
                        int mID = int.Parse(questAndType[0]);
                        matchingSet.Add(mID, "Matching");*/
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
                    if (keyValuePair.Value.Equals("MultipleChoice"))
                    {
                        var quest = db.Questions.Find(keyValuePair.Key);
                        mulChoiceList.Add(quest);
                    }

                }

              /*  foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                {
                    var mQuest = db.MatchQuestions.Find(keyValuePair.Key);
                    mList.Add(mQuest);

                }*/
            }



            ViewBag.Quiz = quiz;
            ViewBag.QuizMultipleChoiceQuestions = mulChoiceList;
            return View();
        }



        public ActionResult ShowAssignedQuiz()
        {
            
            return View(db.Quizs.ToList());
        }
    }
}