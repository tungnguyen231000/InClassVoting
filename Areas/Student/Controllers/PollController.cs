using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.Student.Controllers
{
    public class PollController : Controller
    {
        private DBModel db = new DBModel();
        public ActionResult DoPoll(string poid)
        {
            int pollID = int.Parse(poid);
            var poll = db.Polls.Find(pollID);
            if (poll.Time != null)
            {
                DateTime now = DateTime.Now;
                TimeSpan? totalTime = poll.EndTime - now;
                int? countDown = (int?)totalTime?.TotalSeconds;
                ViewBag.CountDown = countDown;

            }

            ViewBag.Poll = poll;
            return View();
        }

        public ActionResult SubmitPoll(string poid, FormCollection form)
        {
            int pollID = int.Parse(poid);
            var poll = db.Polls.Find(pollID);
            int stID = 1;
            List<string> options = new List<string>();

            if (form["option"] != null && !form["option"].Equals(""))
            {
                options = form["option"].Split(new char[] { ',' }).ToList();

                //get option student choose
                foreach(string opt in options)
                {
                    int optid = int.Parse(opt);
                    var pollAns = db.Poll_Answer.Find(optid);
                    pollAns.ChosenQuantity=pollAns.ChosenQuantity + 1;
                    Student_PollAnswer studenAnswer = new Student_PollAnswer();
                    studenAnswer.Poll_AnswerID = pollAns.PAID;
                    studenAnswer.StudentID = stID;
                    db.Student_PollAnswer.Add(studenAnswer);
                    db.Entry(pollAns).State=System.Data.Entity.EntityState.Modified;
                    poll.TotalParticipian = poll.TotalParticipian + 1;
                    db.Entry(poll).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }



            return Redirect("~/Student/Home/Home");
        }
    }
}