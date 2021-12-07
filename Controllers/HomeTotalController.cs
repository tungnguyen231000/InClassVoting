using InClassVoting.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace InClassVoting.Controllers
{
    public class HomeTotalController : Controller
    {
        private DBModel db = new DBModel();
        // GET: Home
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Login()
        {
            Session.Clear();
            return View();
        }

        [HttpPost]
        public ActionResult LoginAdmin(string inputUserName, string inputPassword)
        {
            //Debug.WriteLine("userName" + inputUserName);
            //Debug.WriteLine("pass" + inputPassword);
            return Redirect("~/Teacher/Home/Home");
        }
        [HttpPost]
        public ActionResult getInfoUser(string email, string name, string image_URL)
        {
            var redirectUrl = "";
            try
            {
                if ("".Equals(email) || "".Equals(name) || "".Equals(image_URL))
                {
                    redirectUrl = new UrlHelper(Request.RequestContext).Action("Login", "HomeTotal");
                    return Json(new { Url = redirectUrl });
                }
                else
                {
                    string[] words = email.Split('@');
                    /*if (words[words.Length - 1].Equals("fe.edu.vn"))*/
                    if (words[words.Length - 1].Equals("gmail.com"))
                    {
                        Session["User"] = "Teacher";
                        Session["Name"] = name;
                        Session["Email"] = email;
                        Session["ImageURL"] = image_URL;

                        var teacherInDb = db.Teachers.Where(t => t.Email.ToLower().Trim().Equals(email.ToLower().Trim())).FirstOrDefault();

                        if (teacherInDb != null)
                        {
                            Session["TeacherId"] = teacherInDb.TID;
                        }
                        else
                        {
                            Teacher newTeacher = new Teacher();
                            newTeacher.Email = email;
                            newTeacher.Name = name;
                            db.Teachers.Add(newTeacher);
                            db.SaveChanges();
                            var getTeacher = db.Teachers.Where(t => t.Email.ToLower().Trim().Equals(newTeacher.Email.ToLower().Trim())).FirstOrDefault();
                            Session["TeacherId"] = getTeacher.TID;
                        }

                        redirectUrl = new UrlHelper(Request.RequestContext).Action("Home", "Home", new { area = "teacher" });
                       /* redirectUrl = Common.PreUrl;
                        if (redirectUrl == null || redirectUrl.Equals(""))
                        {
                            redirectUrl = new UrlHelper(Request.RequestContext).Action("Home", "Home", new { area = "teacher" });
                        }*/
                        return Json(new { Url = redirectUrl });
                    }
                    else if (words[words.Length - 1].Equals("fpt.edu.vn"))
                    /* else if (words[words.Length - 1].Equals("gmail.com"))*/
                    {

                        Session["User"] = "Student";
                        Session["Name"] = name;
                        Session["Email"] = email;
                        Session["ImageURL"] = image_URL;

                        var studentInDb = db.Students.Where(s => s.Email.ToLower().Trim().Equals(email.ToLower().Trim())).FirstOrDefault();

                        if (studentInDb != null)
                        {
                            Debug.WriteLine("hihih1");
                            Session["StudentId"] = studentInDb.SID;
                        }
                        else
                        {
                            Student newStudent = new Student();
                            newStudent.Email = email;
                            newStudent.Name = name;
                            db.Students.Add(newStudent);
                            db.SaveChanges();
                            Debug.WriteLine("123123" + newStudent.Email);
                            var getStudent = db.Students.Where(s => s.Email.ToLower().Trim().Equals(newStudent.Email.ToLower().Trim())).FirstOrDefault();
                            Session["StudentId"] = getStudent.SID;
                            Debug.WriteLine(getStudent.Email + "=====" + getStudent.SID);

                        }
                        redirectUrl = new UrlHelper(Request.RequestContext).Action("Home", "Home", new { area = "Student" });
                        /*redirectUrl = Common.PreUrl;
                        if (redirectUrl == null || redirectUrl.Equals(""))
                        {
                            redirectUrl = new UrlHelper(Request.RequestContext).Action("Home", "Home", new { area = "Student" });
                        }*/
                        return Json(new { Url = redirectUrl });

                        /*string resultString = Regex.Match(email, @"\d+").Value;

                        if (resultString.Length >= 5)
                        {
                            Session["User"] = "Student";
                            Session["Name"] = name;
                            Session["Email"] = email;
                            Session["ImageURL"] = image_URL;
                            var studentInDb = db.Students.Where(s => s.Email.ToLower().Trim().Equals(email.ToLower().Trim())).FirstOrDefault();

                            if (studentInDb != null)
                            {
                                Session["StudentId"] = studentInDb.SID;
                            }
                            else
                            {
                                Student newStudent = new Student();
                                newStudent.Email = email;
                                newStudent.Name = name;
                                db.Students.Add(newStudent);
                                db.SaveChanges();
                                var getStudent = db.Students.Where(s => s.Email.ToLower().Trim().Equals(newStudent.Email.ToLower().Trim())).FirstOrDefault();
                                Session["StudentId"] = getStudent.SID;
                            }
                            redirectUrl = new UrlHelper(Request.RequestContext).Action("Home", "Home", new { area = "Student" });
                            return Json(new { Url = redirectUrl });
                        }
                        else
                        {
                            Session["User"] = "Teacher";
                            Session["Name"] = name;
                            Session["Email"] = email;
                            Session["ImageURL"] = image_URL;
                            var teacherInDb = db.Teachers.Where(t => t.Email.ToLower().Trim().Equals(email.ToLower().Trim())).FirstOrDefault();

                            if (teacherInDb != null)
                            {
                                Session["TeacherId"] = teacherInDb.TID;
                            }
                            else
                            {
                                Teacher newTeacher = new Teacher();
                                newTeacher.Email = email;
                                newTeacher.Name = name;
                                db.Teachers.Add(newTeacher);
                                db.SaveChanges();
                                var getTeacher = db.Teachers.Where(t => t.Email.ToLower().Trim().Equals(newTeacher.Email.ToLower().Trim())).FirstOrDefault();
                                Session["TeacherId"] = getTeacher.TID;
                            }

                            redirectUrl = new UrlHelper(Request.RequestContext).Action("Home", "Home", new { area = "teacher" });
                            return Json(new { Url = redirectUrl });
                        }*/

                    }
                    else
                    {
                        redirectUrl = new UrlHelper(Request.RequestContext).Action("Login", "HomeTotal");
                        return Json(new { Url = redirectUrl });
                    }
                }

            }
            catch
            {
                redirectUrl = new UrlHelper(Request.RequestContext).Action("Login", "HomeTotal");
                return Json(new { Url = redirectUrl });
            }
        }
    }
}