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
            if (getQuiz.Status.Equals("Not Done") || getQuiz.Status.Equals("Done"))
            {
                return View("QuizNotStartYet");
            }
            else
            {
                //get quiz saved in database
                var quiz_quizDone = db.Quiz_QuizDone.OrderByDescending(qz => qz.Q_qDoneID).Where(qz => qz.QuizID == quizId).FirstOrDefault();
                var quiz = db.QuizDones.Find(quiz_quizDone.QuizDoneID);


                /*List<MatchQuestion> mList = new List<MatchQuestion>();*/
                List<QuestionDone> questionsList = new List<QuestionDone>();
                int? quizTime = 0;
                //check if questions list is null
                if (quiz.Questions != null && !quiz.Questions.Equals(""))
                {
                    //////////////////////////////////////
                    string[] quizQuestions = quiz.Questions.Split(new char[] { ';' });
                    List<string> questionList = new List<string>();
                    //get random question in quiz
                    if (quiz.MixQuestionNumber != null)
                    {
                        List<int> addedQuestion = new List<int>();
                        Random rd = new Random();
                        for (int i = 0; i < quiz.MixQuestionNumber; i++)
                        {
                            int q = rd.Next(quizQuestions.Length);
                            /*Debug.WriteLine("q1:=" + q);*/
                            while (addedQuestion.Contains(q))
                            {
                                q = rd.Next(quizQuestions.Length);
                                /*Debug.WriteLine("q2:=" + q);*/
                            }
                            /*Debug.WriteLine("q3:=" + q);*/
                            questionList.Add(quizQuestions[q]);
                            addedQuestion.Add(q);

                        }
                        /*Debug.WriteLine("===1=1==");*/
                        foreach (string qq in questionList)
                        {

                            Debug.WriteLine(qq);
                        }

                    }
                    else
                    {
                        questionList = quizQuestions.ToList();
                    }

                    //////////////////////////////////////

                    Dictionary<int, string> questionSet = new Dictionary<int, string>();
                    Dictionary<int, string> matchingSet = new Dictionary<int, string>();
                    foreach (string questions in questionList)
                    {
                        string[] questAndType = questions.Split(new char[] { '-' });
                        int qType = int.Parse(questAndType[1]);
                        if (qType == 5)
                        {
                            /*
                            string[] questAndType = questions.Split(new char[] { '-' });
                            int mID = int.Parse(questAndType[0]);
                            matchingSet.Add(mID, "Matching");*/
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

                        List<QuestionAnswerDone> qAnswer = quest.QuestionAnswerDones.ToList();
                        foreach (var ans in quest.QuestionAnswerDones)
                        {
                            Debug.WriteLine("i=-" + ans.Text);
                        }
                        if (quest.Qtype == 1 || quest.Qtype == 2)
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
                                quest.QuestionAnswerDones = qAnswer;
                                foreach (var ans in quest.QuestionAnswerDones)
                                {
                                    Debug.WriteLine("i=-" + ans.Text);
                                }
                            }

                        }
                        /*else
                        {

                        }*/
                        questionsList.Add(quest);
                        quizTime = quizTime + quest.Time;
                        Debug.WriteLine("///" + quizTime);
                    }

                    /*  foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                      {
                          var mQuest = db.MatchQuestions.Find(keyValuePair.Key);
                          mList.Add(mQuest);

                      }*/
                }

                ViewBag.Quiz = quiz;
                /*ViewBag.QuizMultipleChoiceQuestions = mulChoiceList;*/
                ViewBag.Questions = questionsList;
                ViewBag.Time = quizTime;
                return View();
            }

        }

        public ActionResult ShowAssignedQuiz()
        {
            return View(db.Quizs.ToList());
        }

        //student press submit quiz
        [HttpPost]
        public ActionResult SubmitQuiz(FormCollection form, string studentID, string qDoneID)
        {
            int sID = int.Parse(studentID);
            int qtID = 1;
            int qzDoneID = int.Parse(qDoneID);
            double? totalMark = 0;
            string qListStr = "";
            ////////////////////////////////////////////////
            string check1 = form["cbOption"];
            string check2 = form["qid"];
            Debug.WriteLine("////" + check1 + "//" + studentID + "//=" + check2 + "//?");

            ////////////////////////////////////////////////

            string[] questionList = null;
            //get question id
            if (form["qid"] != null && !form["qid"].Trim().Equals(""))
            {
                questionList = form["qid"].Split(new char[] { ',' });
            }

            List<string> studentAnswerFillBlankNotGiven = null;
            //get fillblank not given word answer
            if (form["fillBankNotGivenAnswer"] != null && !form["fillBankNotGivenAnswer"].Trim().Equals(""))
            {
                studentAnswerFillBlankNotGiven = form["fillBankNotGivenAnswer"].Split(new char[] { ',' }).ToList();

            }

            List<string> studentAnswerFillBlankGiven = null;
            //get fillblank given word answer
            if (form["fillBankGivenAnswer"] != null && !form["fillBankGivenAnswer"].Trim().Equals(""))
            {
                studentAnswerFillBlankGiven = form["fillBankGivenAnswer"].Split(new char[] { ',' }).ToList();

            }

            //get student question 
            foreach (var q in questionList)
            {
                var question = db.QuestionDones.Find(int.Parse(q));
                //if question is multiple choice type
                if (question.Qtype == 1)
                {
                    string[] cbAnswer = null;
                    //get mutiple choice answer
                    if (form["cbOption"] != null && !form["cbOption"].Trim().Equals(""))
                    {
                        cbAnswer = form["cbOption"].Split(new char[] { ',' });
                    }

                    List<QuestionAnswerDone> correctAnswerList = db.QuestionAnswerDones.Where(qd => qd.QuestionID == question.Q_DoneID && qd.IsCorrect == true).ToList();
                    List<QuestionAnswerDone> choosenAnsList = new List<QuestionAnswerDone>();

                    //if student answer not null
                    if (cbAnswer != null)
                    {
                        //get student answer
                        foreach (var a in cbAnswer)
                        {
                            int ansID = int.Parse(a);
                            /*Debug.WriteLine(question.Q_DoneID + "cbbvlue2:" + ansID);*/
                            var answer = db.QuestionAnswerDones.Where(qa => qa.QuestionID == question.Q_DoneID && qa.QA_DoneID == ansID).FirstOrDefault();
                            if (answer != null)
                            {
                                choosenAnsList.Add(answer);
                                Debug.WriteLine(answer.Text + "--0-" + question.Q_DoneID);

                            }

                        }

                        int countCorrectAns = 0;
                        //if number of chosen option is different from correct answers
                        foreach (var choosenAns in choosenAnsList)
                        {
                            /*Debug.WriteLine(choosenAns.Text);*/
                            Student_Answer studentChoice = new Student_Answer();
                            studentChoice.QuizDoneID = qzDoneID;
                            studentChoice.StudentID = sID;
                            studentChoice.Question = q + "-1";
                            studentChoice.Answer = choosenAns.Text;

                            //if answer is correct
                            if (choosenAns.IsCorrect == true)
                            {
                                studentChoice.IsCorrect = true;
                                countCorrectAns++;
                                Debug.WriteLine("chooo:" + choosenAns.QA_DoneID + "//" + countCorrectAns);
                            }
                            else
                            {
                                studentChoice.IsCorrect = false;
                            }
                            db.Student_Answer.Add(studentChoice);
                        }
                        //check if number of correct answer is equal as correct choosen answer
                        if (countCorrectAns == correctAnswerList.Count && correctAnswerList.Count == choosenAnsList.Count)
                        {
                            totalMark = totalMark + question.Mark;
                        }
                    }
                    qListStr = qListStr + q + "-1;";
                    db.SaveChanges();
                }
                else if (question.Qtype == 3)
                {
                    //if this quesion doesn't have given word
                    if (question.GivenWord == false)
                    {
                        List<QuestionAnswerDone> correctAnswers = db.QuestionAnswerDones.Where(qa => qa.QuestionID == question.Q_DoneID).ToList();
                        int numOfCorrectAns = correctAnswers.Count;
                        int countCorrectAns = 0;
                        for (int i = 0; i < correctAnswers.Count; i++)
                        {
                            Student_Answer studentChoice = new Student_Answer();
                            studentChoice.QuizDoneID = qzDoneID;
                            studentChoice.StudentID = sID;
                            studentChoice.Question = q + "-3";
                            studentChoice.Answer = studentAnswerFillBlankNotGiven[i];
                            //check if answer is correct
                            if (studentAnswerFillBlankNotGiven[i].Trim().ToLower().Equals(correctAnswers[i].Text.ToLower().Trim()))
                            {
                                studentChoice.IsCorrect = true;
                                countCorrectAns++;
                            }
                            else
                            {
                                studentChoice.IsCorrect = false;
                            }
                            
                            db.Student_Answer.Add(studentChoice);
                        }
                        for (int i = 0; i < correctAnswers.Count; i++)
                        {
                            studentAnswerFillBlankNotGiven.RemoveAt(0);
                        }
                            //if the number of correct answer that student chose equal to number of correct answer of question
                            if (countCorrectAns == numOfCorrectAns)
                        {
                            totalMark = totalMark + question.Mark;
                        }
                    }
                    else
                    {
                        List<QuestionAnswerDone> correctAnswers = db.QuestionAnswerDones.Where(qa => qa.QuestionID == question.Q_DoneID).ToList();
                        int numOfCorrectAns = correctAnswers.Count;
                        int countCorrectAns = 0;
                        for (int i = 0; i < correctAnswers.Count; i++)
                        {
                            Student_Answer studentChoice = new Student_Answer();
                            studentChoice.QuizDoneID = qzDoneID;
                            studentChoice.StudentID = sID;
                            studentChoice.Question = q + "-3";
                            studentChoice.Answer = studentAnswerFillBlankGiven[i];
                            //check if answer is correct
                            if (studentAnswerFillBlankGiven[i].Trim().ToLower().Equals(correctAnswers[i].Text.ToLower().Trim()))
                            {
                                studentChoice.IsCorrect = true;
                                countCorrectAns++;
                            }
                            else
                            {
                                studentChoice.IsCorrect = false;
                            }

                            db.Student_Answer.Add(studentChoice);
                        }
                        for (int i = 0; i < correctAnswers.Count; i++)
                        {
                            studentAnswerFillBlankGiven.RemoveAt(0);
                        }
                        //if the number of correct answer that student chose equal to number of correct answer of question
                        if (countCorrectAns == numOfCorrectAns)
                        {
                            totalMark = totalMark + question.Mark;
                        }
                    }
                    qListStr = qListStr + q + "-3;";
                    db.SaveChanges();
                }

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