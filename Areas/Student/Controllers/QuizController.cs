using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                var quiz = db.QuizDones.Where(qz => qz.QuizID == quizId).OrderByDescending(qz => qz.QuizDoneID).FirstOrDefault();


                List<QuestionDone> multipleQuestionsList = new List<QuestionDone>();
                List<QuestionDone> readingQuestionsList = new List<QuestionDone>();
                List<QuestionDone> fillBlankQuestionsList = new List<QuestionDone>();
                List<QuestionDone> shortAnswerQuestionsList = new List<QuestionDone>();
                List<QuestionDone> indicateMistakeQuestionsList = new List<QuestionDone>();
                List<MatchQuestionDone> matchQuestionsList = new List<MatchQuestionDone>();
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

                        List<QuestionAnswerDone> qAnswer = quest.QuestionAnswerDones.ToList();
                        /*foreach (var ans in quest.QuestionAnswerDones)
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
                                quest.QuestionAnswerDones = qAnswer;
                            }
                            multipleQuestionsList.Add(quest);
                            quizTime = quizTime + quest.Time;
                            Debug.WriteLine("///" + quizTime);
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
                                quest.QuestionAnswerDones = qAnswer;
                            }
                            readingQuestionsList.Add(quest);
                            quizTime = quizTime + quest.Time;
                            Debug.WriteLine("///" + quizTime);
                        }
                        else if (quest.Qtype == 3)
                        {
                            fillBlankQuestionsList.Add(quest);
                            quizTime = quizTime + quest.Time;
                        }
                        else if (quest.Qtype == 4)
                        {
                            shortAnswerQuestionsList.Add(quest);
                            quizTime = quizTime + quest.Time;
                        }
                        else if (quest.Qtype == 6)
                        {
                            indicateMistakeQuestionsList.Add(quest);
                            quizTime = quizTime + quest.Time;
                        }
                    }

                    foreach (KeyValuePair<int, string> keyValuePair in matchingSet)
                    {
                        var matchQuest = db.MatchQuestionDones.Find(keyValuePair.Key);
                        matchQuestionsList.Add(matchQuest);
                        quizTime = quizTime + matchQuest.Time;

                    }
                }

                ViewBag.Quiz = quiz;
                ViewBag.MultipleQuestion = multipleQuestionsList;
                ViewBag.FillBlankQuestion = fillBlankQuestionsList;
                ViewBag.ShortAnswerQuestion = shortAnswerQuestionsList;
                ViewBag.IndicateMistakeQuestion = indicateMistakeQuestionsList;
                ViewBag.ReadingQuestion = readingQuestionsList;
                ViewBag.MatchingQuestion = matchQuestionsList;
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
            /*int qtID = 1;*/
            int qzDoneID = int.Parse(qDoneID);
            double? totalMark = 0;
            double? studentMark = 0;
            string qListStr = "";
            ////////////////////////////////////////////////
            string check1 = form["cbMultipleOption"];
            string check2 = form["qid"];
            Debug.WriteLine("////" + check1 + "//" + studentID + "//=" + check2 + "//?");

            ////////////////////////////////////////////////

            string[] questionList = null;
            string[] matchingList = null;
            //get question id
            if (form["qid"] != null && !form["qid"].Trim().Equals(""))
            {
                questionList = form["qid"].Split(new char[] { ',' });
            }


            List<QuestionDone> multipleQuestionsList = new List<QuestionDone>();
            List<QuestionDone> readingQuestionsList = new List<QuestionDone>();
            List<QuestionDone> fillBlankQuestionsList = new List<QuestionDone>();
            List<QuestionDone> shortAnswerQuestionsList = new List<QuestionDone>();
            List<QuestionDone> indicateMistakeQuestionsList = new List<QuestionDone>();
            List<MatchQuestionDone> matchQuestionsList = new List<MatchQuestionDone>();

            if (questionList != null)
            {
                //get student question 
                foreach (var q in questionList)
                {
                    var question = db.QuestionDones.Find(int.Parse(q));
                    totalMark = totalMark + question.Mark;
                    //if question is multiple choice type
                    if (question.Qtype == 1)
                    {
                        multipleQuestionsList.Add(question);

                    }
                    else if (question.Qtype == 2)
                    {
                        readingQuestionsList.Add(question);

                    }
                    else if (question.Qtype == 3)
                    {
                        fillBlankQuestionsList.Add(question);

                    }
                    else if (question.Qtype == 4)
                    {
                        shortAnswerQuestionsList.Add(question);
                    }
                    else if (question.Qtype == 6)
                    {
                        indicateMistakeQuestionsList.Add(question);
                    }
                }
            }

            if (matchingList != null)
            {
                //get student matching question 
                foreach (var m in matchingList)
                {
                    var match = db.MatchQuestionDones.Find(int.Parse(m));
                    totalMark = totalMark + match.Mark;
                }
            }

            //add student answer of multiple question to db
            if (multipleQuestionsList != null)
            {
                foreach (var question in multipleQuestionsList)
                {
                    question.StudentReceive = question.StudentReceive + 1;
                    string[] cbAnswer = null;
                    //get mutiple choice answer
                    if (form["cbMultipleOption"] != null && !form["cbMultipleOption"].Trim().Equals(""))
                    {
                        cbAnswer = form["cbMultipleOption"].Split(new char[] { ',' });
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
                            studentChoice.QuestionDoneID = question.Q_DoneID;
                            studentChoice.Qtype = 1;
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
                            studentMark = studentMark + question.Mark;
                            question.CorrectNumber = question.CorrectNumber + 1;
                        }
                    }
                    db.Entry(question).State = EntityState.Modified;
                    db.SaveChanges();
                    qListStr = qListStr + question.Q_DoneID + "-1;";
                }
            }

            //add student answer of reading question to db
            if (readingQuestionsList != null)
            {
                foreach (var question in readingQuestionsList)
                {
                    question.StudentReceive = question.StudentReceive + 1;
                    string[] cbAnswer = null;
                    //get mutiple choice answer
                    if (form["cbReadingOption"] != null && !form["cbReadingOption"].Trim().Equals(""))
                    {
                        cbAnswer = form["cbReadingOption"].Split(new char[] { ',' });
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
                                /*Debug.WriteLine(answer.Text + "--0-" + question.Q_DoneID);*/
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
                            studentChoice.QuestionDoneID = question.Q_DoneID;
                            studentChoice.Qtype = 2;
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
                            studentMark = studentMark + question.Mark;
                            question.CorrectNumber = question.CorrectNumber + 1;
                        }
                    }
                    db.Entry(question).State = EntityState.Modified;
                    db.SaveChanges();
                    qListStr = qListStr + question.Q_DoneID + "-2;";
                }

            }

            //add student answer of fill blank question to db
            if (fillBlankQuestionsList != null)
            {
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
                foreach (var question in fillBlankQuestionsList)
                {
                    question.StudentReceive = question.StudentReceive + 1;
                    //if this quesion doesn't have given word
                    if (question.GivenWord == false)
                    {
                        //check if student have answer
                        if (studentAnswerFillBlankNotGiven != null)
                        {
                            List<QuestionAnswerDone> correctAnswers = db.QuestionAnswerDones.Where(qa => qa.QuestionID == question.Q_DoneID).ToList();
                            int numOfCorrectAns = correctAnswers.Count;
                            int countCorrectAns = 0;
                            for (int i = 0; i < correctAnswers.Count; i++)
                            {
                                Student_Answer studentChoice = new Student_Answer();
                                studentChoice.QuizDoneID = qzDoneID;
                                studentChoice.StudentID = sID;
                                studentChoice.QuestionDoneID = question.Q_DoneID;
                                studentChoice.Qtype = 3;
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
                                studentMark = studentMark + question.Mark;
                                question.CorrectNumber = question.CorrectNumber + 1;
                            }
                        }

                    }
                    else
                    {
                        //if student have answer
                        if (studentAnswerFillBlankGiven != null)
                        {
                            List<QuestionAnswerDone> correctAnswers = db.QuestionAnswerDones.Where(qa => qa.QuestionID == question.Q_DoneID).ToList();
                            int numOfCorrectAns = correctAnswers.Count;
                            int countCorrectAns = 0;
                            for (int i = 0; i < correctAnswers.Count; i++)
                            {
                                Student_Answer studentChoice = new Student_Answer();
                                studentChoice.QuizDoneID = qzDoneID;
                                studentChoice.StudentID = sID;
                                studentChoice.QuestionDoneID = question.Q_DoneID;
                                studentChoice.Qtype = 3;
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
                                studentMark = studentMark + question.Mark;
                                question.CorrectNumber = question.CorrectNumber + 1;
                            }
                        }

                    }
                    db.Entry(question).State = EntityState.Modified;
                    db.SaveChanges();
                    qListStr = qListStr + question.Q_DoneID + "-3;";
                }
            }

            //add student answer of short answer question to db
            if (shortAnswerQuestionsList != null)
            {
                int countShortAnswerQuest = 0;
                foreach (var question in shortAnswerQuestionsList)
                {
                    question.StudentReceive = question.StudentReceive + 1;
                    countShortAnswerQuest++;
                    string[] answerList = null;
                    //get short answer question answer
                    if (form["txtshortAnswer"] != null && !form["txtshortAnswer"].Trim().Equals(""))
                    {
                        answerList = form["txtshortAnswer"].Split(new char[] { '\\' });
                    }
                    var correctAnswer = db.QuestionAnswerDones.Where(qd => qd.QuestionID == question.Q_DoneID && qd.IsCorrect == true).FirstOrDefault();

                    string[] correctAnswerList = correctAnswer.Text.Split(new char[] { '\\' });

                    if (answerList != null)
                    {
                        for (int i = 0; i < answerList.Count(); i++)
                        {
                            Debug.WriteLine(answerList[i] + "---");
                        }
                        var answer = answerList[countShortAnswerQuest - 1];
                        //if student answer not null
                        if (answer != null)
                        {
                            Student_Answer studentChoice = new Student_Answer();
                            studentChoice.QuizDoneID = qzDoneID;
                            studentChoice.StudentID = sID;
                            studentChoice.QuestionDoneID = question.Q_DoneID;
                            studentChoice.Qtype = 4;
                            studentChoice.Answer = answer;
                            studentChoice.IsCorrect = false;
                            foreach (var ans in correctAnswerList)
                            {
                                Debug.WriteLine(answer.Trim().ToLower() + "=Answer:" + ans.Trim().ToLower());
                                //check if the answer is correct
                                if (ans.Trim().ToLower().Equals(answer.Trim().ToLower()))
                                {
                                    studentChoice.IsCorrect = true;
                                    studentMark = studentMark + question.Mark;
                                    question.CorrectNumber = question.CorrectNumber + 1;
                                }
                            }
                            db.Student_Answer.Add(studentChoice);
                        }
                    }

                    db.Entry(question).State = EntityState.Modified;
                    db.SaveChanges();
                    qListStr = qListStr + question.Q_DoneID + "-4;";
                }
            }

            //add student answer of indicate mistake question to db
            if (indicateMistakeQuestionsList != null)
            {
                foreach (var question in indicateMistakeQuestionsList)
                {
                    question.StudentReceive = question.StudentReceive + 1;
                    string[] cbAnswer = null;
                    //get mutiple choice answer
                    if (form["cbIndicateOption"] != null && !form["cbIndicateOption"].Trim().Equals(""))
                    {
                        cbAnswer = form["cbIndicateOption"].Split(new char[] { ',' });
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

                        /*int countCorrectAns = 0;*/
                        //if number of chosen option is different from correct answers
                        foreach (var choosenAns in choosenAnsList)
                        {
                            /*Debug.WriteLine(choosenAns.Text);*/
                            Student_Answer studentChoice = new Student_Answer();
                            studentChoice.QuizDoneID = qzDoneID;
                            studentChoice.StudentID = sID;
                            studentChoice.QuestionDoneID = question.Q_DoneID;
                            studentChoice.Qtype = 6;
                            studentChoice.Answer = choosenAns.Text;

                            //if answer is correct
                            if (choosenAns.IsCorrect == true)
                            {
                                studentChoice.IsCorrect = true;
                                studentMark = studentMark + question.Mark;
                                question.CorrectNumber = question.CorrectNumber + 1;
                                /*Debug.WriteLine("chooo:" + choosenAns.QA_DoneID + "//" + countCorrectAns);*/
                            }
                            else
                            {
                                studentChoice.IsCorrect = false;
                            }
                            db.Student_Answer.Add(studentChoice);
                        }
                    }
                    db.Entry(question).State = EntityState.Modified;
                    db.SaveChanges();
                    qListStr = qListStr + question.Q_DoneID + "-6;";
                }
            }

            //add student answer of indicate mistake question to db
            if (matchQuestionsList != null)
            {
                foreach (var question in matchQuestionsList)
                {

                }
            }

            Student_QuizDone report = new Student_QuizDone();
            report.StudentID = sID;
            report.QuizDoneID = qzDoneID;
            report.TotalMark = totalMark;
            report.StudentMark = studentMark;
            report.Status = "Done";
            report.ReceivedQuestions = qListStr.Substring(0, qListStr.Length - 1);
            db.Student_QuizDone.Add(report);
            db.SaveChanges();

            return Redirect("~/Student/Report/QuizReport?qzid=" + qzDoneID + "&stid=" + studentID);
        }
    }
}