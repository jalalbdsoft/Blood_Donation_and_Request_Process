using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVC_Final_Assessment.Models;

namespace MVC_Final_Assessment.Controllers
{
    public class HomeController : Controller
    {
        MVC_Final_AssessmentEntities db = new MVC_Final_AssessmentEntities();
        public ActionResult Index()
        {
            var request = db.Requests.Where(r => r.Status == true).OrderByDescending(r => r.RequestID).ToList().Take(10);
            ViewBag.requests = request;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}