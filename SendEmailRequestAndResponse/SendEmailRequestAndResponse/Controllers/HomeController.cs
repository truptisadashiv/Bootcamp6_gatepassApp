using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SendEmailRequestAndResponse.Models;

namespace SendEmailRequestAndResponse.Controllers
{
    public class HomeController : Controller
    {
        Emailconnectivity ec = new Emailconnectivity();
        public  static  string RequestedEmployeeId;
        public static string empid;
        public static DateTime fromdate;
        public static DateTime todate;
        public static string fromlocation;
        public static string tolocation;
        public static string status;
        public int r;

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RequestforMovement()
        {

            fromlocation = Request.Form["floca"];
            tolocation = Request.Form["toloca"];
            DateTime.TryParse(Request.Form["fromdate"], out fromdate);
            DateTime.TryParse(Request.Form["todate"], out todate);
            empid = Request.Form["empid"];
            RequestedEmployeeId = empid;
            string empemailid = ec.select(empid);
            string managermailid = ec.selectmanagerid(empid);
            string body =" Request for a laptop movement  Click here to reply to the request " + " " + "https://localhost:44368/Home/AnswerToTheRequest" + "\n";
            string subject ="Hello i am  "+empid+"requesting for a laptop movement  ";
            SendEmail(body, subject, managermailid);   ///for manager mail sent 
            return RedirectToAction("ThankYou");
        }
        public IActionResult AnswerToTheRequest()
        {/* 
       <a href="~/Home/Accept" class="btn btn-primary btn-block">Accept</a>

       <a href="~/Home/Reject" class="btn btn-primary btn-block">Reject</a>*/

            /* if(r==1)
            {
                return RedirectToAction("CheckForValidity");
            }
            else */
            return View();
        }

        public IActionResult Accept()
        {
            string body = "your request for laptop movement is accepted";
            string Subject = "New Gate Pass request is accepted";
            string To = ec.select(RequestedEmployeeId);
           int a=CheckForValidity(RequestedEmployeeId);
            if (a==1)
            {

                return View();
            }
            else
            {
                a = 0;
                r = 0;
                ec.select(RequestedEmployeeId);
                SendEmail(body, Subject, To);
                // fromlocation = Request.Form["floca"];
                // tolocation = Request.Form["toloca"];
                // DateTime.TryParse(Request.Form["fromdate"], out fromdate);
                //  DateTime.TryParse(Request.Form["todate"], out todate);
                status = "accepted";
                ec.InsertToGatepassTable(RequestedEmployeeId, fromlocation, tolocation, fromdate, todate, status);
                //insert to gatepass detail should occur and details should be fetched from the existing table nd a gatepassid nfd detils to be added
                return RedirectToAction("ThankYou");
            }


        }
        public IActionResult Reject()
        {
            string body = "your request for laptop movement is  not accepted";
            string Subject = "New Gate Pass request is rejected";
           int b= CheckForValidity(RequestedEmployeeId);
            if (b==1)
            {
                return View();
            }
            else
            {
                b = 0;
                r = 0;
                string To = ec.select(RequestedEmployeeId);
                SendEmail(body, Subject, To);
                // return RedirectToAction("ThankYou");
                status = "rejected";
                ec.InsertToGatepassTable(RequestedEmployeeId, fromlocation, tolocation, fromdate, todate, status);
                return RedirectToAction("ThankYou");
               
            }
   
        }
      
        public int CheckForValidity(string Empid)
        {
            ///from to from to 


            r = ec.selectall(Empid, fromlocation, tolocation, fromdate, todate);

            return r;
          
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        public IActionResult ThankYou()
        {
          

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public void SendEmail(string Body, string Subject, string ToEmail)
        {

               SmtpClient smtpClient = new SmtpClient();
            NetworkCredential smtpCredentials = new NetworkCredential("gatepass.advanced@sagarbayar.info", "gatepass#1");

            MailMessage message = new MailMessage();
            MailAddress fromAddress = new MailAddress("gatepass.advanced@sagarbayar.info");
            MailAddress toAddress = new MailAddress(ToEmail);

            smtpClient.Host = "mail.sagarbayar.info";
            smtpClient.Port = 587;
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = smtpCredentials;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.Timeout = 20000;
            //https://localhost:44368/Home/RequestforMovement
            message.From = fromAddress;
            message.To.Add(toAddress);
            message.IsBodyHtml = false;
            message.Subject = Subject;
            message.Body = Body;

            smtpClient.Send(message);
        }
      
    }
}
