﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PsiBB.Controllers
{
    public class OMGController : Controller
    {
        // GET: OMG
        public ActionResult Index()
        {
            return View();
        }
        // public string Index() 
        // {
        //     return "<b>OMG.</b>";
        // }
 
        // 
        // GET: /OMG/Louder/
        public ActionResult Louder(int volume=1)
        {
            string exclamationPoints = "";

            for (int i = 0; i < volume; i++)
            {
                exclamationPoints += "!";
            }

            ViewBag.exclamationPoints = exclamationPoints;

            return View();
        }
        // public string Louder() 
        // { 
        //     return "<h1>OMG!</h1"; 
        // } 
    }
}
