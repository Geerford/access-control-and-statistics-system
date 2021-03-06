﻿using Domain.Core;
using Service.Interfaces;
using sstu_nevdev.Models;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;

namespace sstu_nevdev.Controllers
{
    [Authorize(Roles = "SSTU_Administrator")]
    public class AdmissionController : Controller
    {
        IAdmissionService service;

        public AdmissionController(IAdmissionService service)
        {
            this.service = service;
        }

        public ActionResult Index()
        {
            return View(service.GetAll());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = service.Get(id);
            if (item != null)
            {
                return PartialView(item);
            }
            return HttpNotFound();
        }

        public ActionResult Create()
        {
            AdmissionViewModel model = new AdmissionViewModel
            {
                RoleList = new SelectList(new List<StatusForList> {
                    new StatusForList {
                        Key = "Сотрудник",
                        Display = "Сотрудник" },
                    new StatusForList {
                        Key = "Студент",
                        Display = "Студент" } },
                    "Key", "Display")
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AdmissionViewModel model)
        {
            if (string.IsNullOrEmpty(model.Description))
            {
                ModelState.AddModelError("Description", "Описание должно быть заполнено");
            }
            if (string.IsNullOrEmpty(model.Role))
            {
                ModelState.AddModelError("Role", "Выберите роль");
            }
            if (ModelState.IsValid)
            {
                service.Create(new Admission
                {
                    Description = model.Description,
                    Role = model.Role
                });
                return RedirectToAction("Index", "Admission");
            }
            else
            {
                model.RoleList = new SelectList(new List<StatusForList> {
                    new StatusForList {
                        Key = "Сотрудник",
                        Display = "Сотрудник" },
                    new StatusForList {
                        Key = "Студент",
                        Display = "Студент" } },
                    "Key", "Display");
                return View(model);
            }
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdmissionViewModel model = (AdmissionViewModel)service.Get(id);
            if (model != null)
            {
                model.RoleList = new SelectList(new List<StatusForList> {
                    new StatusForList {
                        Key = "Сотрудник",
                        Display = "Сотрудник" },
                    new StatusForList {
                        Key = "Студент",
                        Display = "Студент" } },
                    "Key", "Display");
                return View(model);
            }
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdmissionViewModel model)
        {
            if (string.IsNullOrEmpty(model.Description))
            {
                ModelState.AddModelError("Description", "Описание должно быть заполнено");
            }
            if (string.IsNullOrEmpty(model.Role))
            {
                ModelState.AddModelError("Role", "Выберите роль");
            }
            if (ModelState.IsValid)
            {
                service.Edit(new Admission
                {
                    ID = model.ID,
                    Description = model.Description,
                    Role = model.Role
                });
                return RedirectToAction("Index", "Admission");
            }
            else
            {
                model.RoleList = new SelectList(new List<StatusForList> {
                    new StatusForList {
                        Key = "Сотрудник",
                        Display = "Сотрудник" },
                    new StatusForList {
                        Key = "Студент",
                        Display = "Студент" } },
                    "Key", "Display");
                return View(model);
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Admission model = service.Get(id);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(Admission model)
        {
            try
            {
                service.Delete(model);
                return RedirectToAction("Index", "Admission");
            }
            catch
            {
                return View(model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            service.Dispose();
            base.Dispose(disposing);
        }
    }
}