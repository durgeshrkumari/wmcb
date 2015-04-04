﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using wmcb.repo;

namespace wmcb.web.Controllers
{
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            var news = new NewsFeedRepo().getLast3NewsFeed();
            return View();
        }
        public ActionResult Fixtures()
        {
            return View();
        }
        public ActionResult Teams()
        {
            return View();
        }
        public ActionResult Stats()
        {
            return View();
        }
        public ActionResult Grounds()
        {
            return View();
        }
        public ActionResult Gallery()
        {
            return View();
        }
        public ActionResult Downloads()
        {
            return View();
        }
        public ActionResult Newsletter()
        {
            //ViewBag.Message = "Your newsletter description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Washington Metropolitan Cricket Board";

            return View();
        }
    }
}