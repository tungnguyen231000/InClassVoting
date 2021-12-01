using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.Teacher.Controllers.PollController
{
    public class PollController : Controller
    {
        private DBModel db = new DBModel();

        public ActionResult CreatePoll()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePoll(FormCollection form)
        {
            string questionText = form["questionText"];
            string pollName = form["pollName"];
            string pollType = form["rdPollType"];
            List<string> optionList = new List<string>();
            int? polltime = null;
            int teacherID = 1;

            if (form["option"] != null && !form["option"].Equals(""))
            {
                optionList = form["option"].Split(new char[] { ',' }).ToList();
            }

            if (form["time"] != "" && !form["time"].Equals(""))
            {
                polltime = int.Parse(form["time"]);
            }
            int ptype = int.Parse(pollType);

            Poll newPoll = new Poll();

            newPoll.PollName = pollName;
            newPoll.Question = questionText;
            newPoll.Time = polltime;
            newPoll.TotalParticipian = 0;
            newPoll.IsDoing = false;
            newPoll.TeacherID = teacherID;
            newPoll.CreatedDate = DateTime.Today;
            if (ptype == 1)
            {
                newPoll.Polltype = "MultipleAnswer";
            }
            else
            {
                newPoll.Polltype = "OneAnswer";
            }

            db.Polls.Add(newPoll);

            db.SaveChanges();

            int pollID = db.Polls.Where(p => p.TeacherID == teacherID).OrderByDescending(p => p.PollID).Select(p => p.PollID).FirstOrDefault();

            foreach (string opt in optionList)
            {
                if (!opt.Equals("") && opt != null)
                {
                    Poll_Answer pollAns = new Poll_Answer();
                    pollAns.Answer_Text = opt;
                    pollAns.PollID = pollID;
                    pollAns.ChosenQuantity = 0;
                    db.Poll_Answer.Add(pollAns);
                }
            }

            db.SaveChanges();



            return Redirect("~/Teacher/Poll/ShowPollLink?poid=" + pollID);
        }

        public ActionResult ShowPollLink(string poid)
        {
            int pollID = int.Parse(poid);
            var poll = db.Polls.Find(pollID);
            ViewBag.PollLink = "https://inclassvoting.azurewebsites.net/Student/Poll/DoPoll?poid=" + pollID;


            ViewBag.Poll = poll;
            return View();
        }

        [HttpPost]
        public ActionResult StartPoll(string poid)
        {
            int pollID = int.Parse(poid);
            var poll = db.Polls.Find(pollID);

            if (poll.Time != null)
            {
                DateTime now = DateTime.Now;
                DateTime endTime = now.AddSeconds(double.Parse(poll.Time.ToString()));
                poll.EndTime = endTime;
                TimeSpan totalTime = (endTime - now);
                int countDown = (int)totalTime.TotalSeconds;
                ViewBag.CountDown = countDown + 15;

            }
            poll.IsDoing = true;
            db.Entry(poll).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.Poll = poll;


            return View();
        }

        [HttpPost]
        public ActionResult EndPoll(string poid)
        {
            int pollID = int.Parse(poid);
            var poll = db.Polls.Find(pollID);
            poll.IsDoing = false;
            db.Entry(poll).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect("~/Teacher/Poll/ShowPollResult?poid=" + pollID);
        }

        public ActionResult ShowPollResult(string poid)
        {
            int pollID = int.Parse(poid);
            var poll = db.Polls.Find(pollID);
            if (poll.Polltype.Equals("MultipleAnswer"))
            {
                return Redirect("~/Teacher/Poll/PollResultProgressBar?poid=" + pollID);
            }
            else
            {
                return Redirect("~/Teacher/Poll/PollResultPieChart?poid=" + pollID);
            }



        }

        public ActionResult PollResultProgressBar(string poid)
        {
            int pollID = int.Parse(poid);
            var poll = db.Polls.Find(pollID);
            ViewBag.Poll = poll;
            return View();

        }

        public ActionResult PollResultPieChart(string poid)
        {
            int pollID = int.Parse(poid);
            var poll = db.Polls.Find(pollID);
            ViewBag.Poll = poll;
            return View();

        }






    }
}