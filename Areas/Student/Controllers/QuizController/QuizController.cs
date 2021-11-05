using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var getQuiz = db.Quizs.Find(quizId);
            if(getQuiz.Status.Equals("Not Done")|| getQuiz.Status.Equals("Done"))
            {
                return View("QuizNotStartYet");
            }
            else
            {
                var quiz_quizDone = db.Quiz_QuizDone.OrderByDescending(qz => qz.Q_qDoneID).Where(qz=>qz.QuizID==quizId).FirstOrDefault();
                
                var quiz = db.QuizDones.Find(quiz_quizDone.QuizDoneID);
                List<QuestionDone> mulChoiceList = new List<QuestionDone>();
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
                        if (keyValuePair.Value.Equals("Multiple Choice"))
                        {
                            var quest = db.QuestionDones.Find(keyValuePair.Key);
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
           
        }

        public ActionResult ShowAssignedQuiz()
        {
            return View(db.Quizs.ToList());
        }

        [HttpPost]
        public ActionResult SubmitQuiz(FormCollection form, string studentID,string qDoneID)
        {
            int sID = int.Parse(studentID);
            int qtID = 1;
            int qzDoneID = int.Parse(qDoneID);
            double? totalMark=0;
            string qListStr = "";
            string check1 = form["cbOption"];
            string check2 = form["qid"];
            string[] cbAnswer = form["cbOption"].Split(new char[] { ',' });
            string[] questionList = form["qid"].Split(new char[] { ','});
            Debug.WriteLine("////"+ check1 + "//" + studentID + "//=" + check2 + "//?");
            /* Dictionary<int, int> optionChoice = new Dictionary<int, int>();*/
            //==================================================
         /*   foreach (string st in cbAnswer)
            {
                Debug.WriteLine("cbbvlue:" + st);
            }
            foreach (string st in answer)
            {
                Debug.WriteLine("answer:" + st);
            }*/

            //=============================
            /*for (int i = 0; i < answer.Length; i++)
            {
                Debug.WriteLine(cbAnswer[i].ToString() + "-=-=-=-=" + cbAnswer.Length + "///" + answer.Length);
                optionChoice.Add(int.Parse(answer[i]), int.Parse(cbAnswer[i]));
            }*/
            foreach (var q in questionList)
            {
                var mulQuest = db.QuestionDones.Find(int.Parse(q));
                int numOfAns = mulQuest.QuestionAnswerDones.Count();
                QuestionAnswerDone choosenAns = new QuestionAnswerDone();
                Student_Answer studentChoice = new Student_Answer();
                studentChoice.QuizDoneID = qzDoneID;
                studentChoice.StudentID = sID;
                studentChoice.Question = q + "-MultipleChoice";
                foreach (var a in cbAnswer)
                {
                    int ansID = int.Parse(a);
                    choosenAns = db.QuestionAnswerDones.Where(qa => qa.QuestionID == mulQuest.Q_DoneID && qa.QA_DoneID == ansID).FirstOrDefault();
                    
                    
                    if (choosenAns == null)
                    {
                        Debug.WriteLine("non");
                    }
                    else
                    {
                        if (choosenAns.IsCorrect == true)
                        {
                            Debug.WriteLine("okela");
                            studentChoice.Answer = choosenAns.Text;
                            studentChoice.IsCorrect = true;
                            totalMark = totalMark + mulQuest.Mark;

                        }
                        else
                        {
                            Debug.WriteLine("sai");
                            studentChoice.Answer = choosenAns.Text;
                            studentChoice.IsCorrect = false;
                        }
                    }
                }
                db.Student_Answer.Add(studentChoice);
                qListStr = qListStr + q + "-MultipleChoice;";
                /*foreach(KeyValuePair<int,int> keyValue in optionChoice)
                {
                    Debug.WriteLine(keyValue.Value);
                    Student_Answer studentChoice = new Student_Answer();
                    if (keyValue.Value == 1)
                    {
                        var qAnswer = db.QuestionAnswerDones.Find(keyValue.Key);
                        studentChoice.QuizDoneID = qzDoneID;
                        studentChoice.StudentID = sID;
                        studentChoice.Question = q + "-MultipleChoice";
                        studentChoice.Answer = qAnswer.Text;
                        
                        if (qAnswer.IsCorrect == true)
                        {
                            studentChoice.IsCorrect = true;
                            totalMark = totalMark + mulQuest.Mark;
                        }
                        else
                        {
                            studentChoice.IsCorrect = false;
                        }

                        Debug.WriteLine(q + "-=-=-=-=" + keyValue.Value + "///" );
                        db.Student_Answer.Add(studentChoice);
                    }
                    
                    db.SaveChanges();
                     db.Student_Answer.Add(studentChoice);
                }*/
            }
            
            Student_QuizDone report = new Student_QuizDone();
            report.StudentID = sID;
            report.QuizDoneID = qzDoneID;
            report.TotalMark = totalMark;
            report.Status = "Done";
            report.ReceivedQuestions = qListStr.Substring(0, qListStr.Length - 1);
            db.Student_QuizDone.Add(report);
            db.SaveChanges();

            return RedirectToAction("ShowAssignedQuiz");
        }
        }
}