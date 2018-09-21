using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GatePass1.Models;

namespace GatePass1.Controllers
{
    public class HomeController : Controller
    {
        public static int gatePassId;
        DataAccessLayer dataAccessLayer = new DataAccessLayer();
        public static string  msg ;
        ViewModel viewModel = new ViewModel();
        public IActionResult Index()
        {           
            return View();
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Security(string Message)
        {
            
            ViewBag.Message = Message;
            return View();
        }


        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost]
        public IActionResult FetchDetails()
        {
            int.TryParse(Request.Form["gatePassId"], out gatePassId);
            string status = dataAccessLayer.SelectStatus(gatePassId);
            if (status == "notfound")
            {
                ViewBag.Message = "GatePass Id not found";
                return RedirectToAction("Security", "Home", new { ViewBag.Message });
            }
            else
            {
                DateTime d = dataAccessLayer.SelectDate(gatePassId);
                if (DateTime.Now > d)
                {
                    ViewBag.Message = "GatePass Id Expired";
                    return RedirectToAction("Security", "Home", new { ViewBag.Message });

                }
                if (status == "accepted")
                {
                    viewModel.Gates = dataAccessLayer.Display(gatePassId);
                    viewModel.Assets = dataAccessLayer.DisplayAsset(gatePassId);
                    int gatePassExistsId = dataAccessLayer.SelectExistsId(gatePassId);
                    int logOutDate = dataAccessLayer.SelectLogOut(gatePassExistsId);
                    if(logOutDate==0)
                        {
                        msg = "loggedout";
                    }
                    else
                    {
                        msg = "loggedin";
                    }           
                    ViewData["Status"] = msg;
                    return View(viewModel);
                }
                ViewData["Status"] = msg;
                ViewBag.Message = "Gate Pass Id invalid or does not have permission ";
                return RedirectToAction("Security", "Home", new { ViewBag.Message });
            }
        }
        public IActionResult Entry()
        {
            string status = dataAccessLayer.SelectStatus(gatePassId);
            int gatePassExistsId = dataAccessLayer.SelectExistsId(gatePassId);
            if (gatePassExistsId != 0)
            {
                int logOutDate = dataAccessLayer.SelectLogOut(gatePassExistsId);
                if (logOutDate == 0)
                {
                    InsertLogTime();
                    msg = "loggedin";
                    return RedirectToAction("Security");
                }
                else
                {
                    DateTime tout = DateTime.Now;
                    dataAccessLayer.UpdateLogOut(logOutDate, tout);
                    msg = "loggedout";
                    return RedirectToAction("Security");
                }

            }

            else
            {
                InsertLogTime();
                msg = "loggedin";
                return RedirectToAction("Security");

            }
        }
        public void InsertLogTime()
        {
            DateTime currentTime;
            currentTime = DateTime.Now;
            dataAccessLayer.Insert(gatePassId, currentTime);

        }
        public IActionResult MoveBack()
        {            
            return RedirectToAction("Security");
        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        public IActionResult Back()
        {
            return RedirectToAction("Security");
        }
        public IActionResult Privacy()
        {
            return View();
        }
     
     
        
    }
}
