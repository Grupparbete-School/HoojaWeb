using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace HoojaWeb.Controllers
{
    public class SendEmailController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SendEmail(string subject, string body, string fromEmail = "customer@hooja.se")
        {
            try
            {
                if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(body))
                {
                    ViewBag.Error = "Subject and Body are required fields.";
                    return View();
                }

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail);
                    mail.ReplyToList.Add(new MailAddress(fromEmail));
                    mail.To.Add("hoojaabgroup@gmail.com");
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential("hoojaabgroup@gmail.com", "uiqofmuswsmgmyub");
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }

                ViewBag.Message = "Email sent successfully";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred while sending the email: " + ex.Message;
            }

            return View();
        }
    }
}
