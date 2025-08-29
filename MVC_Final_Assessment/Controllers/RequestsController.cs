using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVC_Final_Assessment.Models;

namespace MVC_Final_Assessment.Controllers
{
    public class RequestsController : Controller
    {
        private MVC_Final_AssessmentEntities db = new MVC_Final_AssessmentEntities();

        // GET: Requests
        public ActionResult Index()
        {
            var requests = db.Requests.Include(r => r.BloodGroup);
            return View(requests.ToList());
        }

        // GET: Requests/Details/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // GET: Requests/Create
        public ActionResult Create()
        {
            ViewBag.BloodGroupID = new SelectList(db.BloodGroups, "BloodGroupID", "BloodGroupName");
            return View();
        }

        // POST: Requests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RequestID,BloodGroupID,MessageTitle,MessageBody,RequestDate,ContactNo,Email,Address,Status")] Request request)
        {
            if (ModelState.IsValid)
            {
                db.Requests.Add(request);
                db.SaveChanges();
                if (User.IsInRole("Admin"))
                {
                    ViewBag.BloodGroupID = new SelectList(db.BloodGroups, "BloodGroupID", "BloodGroupName", request.BloodGroupID);
                    //return View(request);
                    TempData["SuccessMessage"] = "Request created successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["SuccessMessage"] = "Request created successfully. Please wait for the approval mail.";
                    return RedirectToAction("Index","Home");
                }               
            }            
            return RedirectToAction("Index");
        }

        // GET: Requests/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            ViewBag.BloodGroupID = new SelectList(db.BloodGroups, "BloodGroupID", "BloodGroupName", request.BloodGroupID);
            return View(request);
        }

        // POST: Requests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RequestID,BloodGroupID,MessageTitle,MessageBody,RequestDate,ContactNo,Email,Address,Status")] Request request, string submitButton)
        {
            if (ModelState.IsValid)
            {
                var originalRequest = db.Requests.AsNoTracking().FirstOrDefault(r => r.RequestID == request.RequestID);

                db.Entry(request).State = EntityState.Modified;
                db.SaveChanges();

                // If the button clicked is "Save & Send Mail", and status making "true" then send mail
                if (submitButton == "Save & Send Mail" && request.Status == true)
                {
                    SendApprovalEmail(request.Email, request.MessageTitle);
                    TempData["SuccessMessage"] = "Request updated and email sent successfully.";
                }
                else if (submitButton == "Save & Send Mail" && request.Status != true)
                {
                    TempData["ErrorMessage"] = "Request updated successfully but you cannot send mail unless Request Status is true.";
                }
                else if (submitButton == "Save")
                {
                    TempData["SuccessMessage"] = "Request updated successfully.";
                }

                return RedirectToAction("Index", new { id = request.RequestID });
            }

            ViewBag.BloodGroupID = new SelectList(db.BloodGroups, "BloodGroupID", "BloodGroupName", request.BloodGroupID);
            return View(request);
        }

        // GET: Requests/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Request request = db.Requests.Find(id);
            if (request == null)
            {
                return HttpNotFound();
            }
            return View(request);
        }

        // POST: Requests/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Request request = db.Requests.Find(id);
            db.Requests.Remove(request);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Request deleted successfully.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void SendApprovalEmail(string toEmail, string subjectTitle)
        {
            try
            {
                var fromAddress = new System.Net.Mail.MailAddress("jalal.gtrbd@gmail.com", "Blood Bank");
                var toAddress = new System.Net.Mail.MailAddress(toEmail);
                const string subject = "Blood Request Approved";
                string body = $"Dear User,\n\nYour blood request titled '{subjectTitle}' has been approved.\n\nIf you want to modify or delete your request, please reply to this mail or call +8801720945217.\n\nThank you,\nBlood Bank Team";

                var smtp = new System.Net.Mail.SmtpClient
                {
                    Host = "smtp.gmail.com",  // same as in web.config
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential("jalal.gtrbd@gmail.com", "ocjb znxa noqd eszy")
                };

                using (var message = new System.Net.Mail.MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine("Email sending failed: " + ex.Message);
            }
        }
    }
}
