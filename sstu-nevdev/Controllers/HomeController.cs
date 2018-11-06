﻿using Service.Interfaces;
using System.Web.Mvc;

namespace sstu_nevdev.Controllers
{
    public class HomeController : Controller
    {
        IRoleService service;

        public HomeController(IRoleService service)
        {
            this.service = service;
        }

        public ActionResult Index()
        {
            return View(service.GetAll());
        }

        protected override void Dispose(bool disposing)
        {
            service.Dispose();
            base.Dispose(disposing);
        }
    }
}