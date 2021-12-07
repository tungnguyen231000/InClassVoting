using InClassVoting.Filter;
using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.Student.Controllers
{
    [AccessAuthenticationFilter]
    [UserAuthorizeFilter("Student")]
    public class PollController : Controller
    {
        private DBModel db = new DBModel();



        private bool checkPollIdEncodeAvailbile(string poid)
        {
            bool check = true;
            string checkPollId = Base64Decode(poid);
            int pollId;
            bool isInt = int.TryParse(checkPollId, out pollId);
            //check if poll id is int
            if (isInt == false)
            {
                check = false;
            }
            else
            {
                var poll = db.Polls.Find(pollId);
                //check if poll exist in db
                if (poll == null)
                {
                    check = false;
                }
            }
            return (check);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public ActionResult DoPoll(string poid)
        {
            //check if poll is availble
            if (checkPollIdEncodeAvailbile(poid) == false)
            {
                return Redirect("~/Student/Home/Home");
            }
            else
            {
                ViewBag.UserName = Convert.ToString(HttpContext.Session["Name"]);
                ViewBag.ImageURL = Convert.ToString(HttpContext.Session["ImageURL"]);
                poid = Base64Decode(poid);
                int pollID = int.Parse(poid);
                int studentId = Convert.ToInt32(HttpContext.Session["StudentId"]);
                Student_PollAnswer poll_Answer = db.Student_PollAnswer.Where(pa => pa.StudentID == studentId && pa.Poll_Answer.PollID == pollID).FirstOrDefault();

                //if student already do poll
                if (poll_Answer != null)
                {
                    ////////////////////////////////////-- trả về màn đã làm poll~
                    return Redirect("~/Student/Home/Home");
                    ////////////////////////////////////////
                }
                else
                {
                    var poll = db.Polls.Find(pollID);
                    //check if poll availble
                    if (!poll.IsDoing)
                    {
                        //trả màn thông báo poll not availble
                        return Redirect("~/Student/Home/Home");
                    }
                    else
                    {
                        //if poll have time
                        if (poll.Time != null)
                        {
                            DateTime now = DateTime.Now;
                            //check if poll run out of time
                            if (now < poll.EndTime)
                            {
                                TimeSpan? totalTime = poll.EndTime - now;
                                int? countDown = (int?)totalTime?.TotalSeconds;
                                ViewBag.CountDown = countDown;
                            }
                            else
                            {
                                return Redirect("~/Student/Home/Home");
                            }
                        }
                    }

                    ViewBag.Poll = poll;
                    return View();
                }


            }
        }

        [HttpPost]
        public ActionResult SubmitPoll(string poid, FormCollection form)
        {
            int pollID = int.Parse(poid);
            var poll = db.Polls.Find(pollID);
            int stID = Convert.ToInt32(HttpContext.Session["StudentId"]);

            var studentPollAnswer = db.Student_PollAnswer.Where(pa => pa.StudentID == stID && pa.Poll_Answer.PollID == pollID).FirstOrDefault();

            if (studentPollAnswer != null)
            {

                //trả về màn bạn đã làm poll này
                return Redirect("~/Student/Home/Home");
            }
            else
            {

                List<string> options = new List<string>();

                if (form["option"] != null && !form["option"].Equals(""))
                {
                    options = form["option"].Split(new char[] { ',' }).ToList();

                    //get option student choose
                    foreach (string opt in options)
                    {
                        int optid = int.Parse(opt);
                        var pollAns = db.Poll_Answer.Find(optid);
                        pollAns.ChosenQuantity = pollAns.ChosenQuantity + 1;
                        Student_PollAnswer studenAnswer = new Student_PollAnswer();
                        studenAnswer.Poll_AnswerID = pollAns.PAID;
                        studenAnswer.StudentID = stID;
                        db.Student_PollAnswer.Add(studenAnswer);
                        db.Entry(pollAns).State = System.Data.Entity.EntityState.Modified;

                    }
                    poll.TotalParticipian = poll.TotalParticipian + 1;
                    db.Entry(poll).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }


                return Redirect("~/Student/Home/Home");
            }

        }

    }
}