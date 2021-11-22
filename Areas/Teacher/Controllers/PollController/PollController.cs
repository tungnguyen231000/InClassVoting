using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Areas.Teacher.Controllers.PollController
{
    public class PollController : Controller
    {
        // GET: Teacher/Poll
        public ActionResult CreateOneAnswerPoll()
        {
            return View();
        }
    }
}