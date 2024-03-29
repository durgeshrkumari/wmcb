﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using wmcb.model;
using wmcb.model.Security;
using wmcb.model.View;
using wmcb.repo;
using wmcb.web.Controllers;

namespace wmcb.adminportal.Controllers
{
    public class AccountController : Controller
    {
        [AllowAnonymous]
        public ActionResult Index(LoginViewModel model, string returnUrl = "")
        {
            if (ModelState.IsValid)
            {
                var user = new UsersRepo().getLoggedInUser(model.UserName, model.Password);
                if (user != null)
                {
                    var roles = user.Roles.Select(m => m.Role);
                    CustomPrincipalSerializeModel serializeModel = new CustomPrincipalSerializeModel();
                    serializeModel.ID = user.ID;
                    serializeModel.FirstName = user.FirstName;
                    serializeModel.LastName = user.LastName;
                    serializeModel.roles = roles.Select(r => r.Name).ToArray();
                    if (user.Team != null)
                    {
                        serializeModel.TeamId = user.TeamId;
                        serializeModel.TeamName = user.Team.Name;
                    }

                    string userData = JsonConvert.SerializeObject(serializeModel);
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                    1,
                    user.Email,
                    DateTime.Now,
                    DateTime.Now.AddMinutes(15),
                    false, //pass here true, if you want to implement remember me functionality
                    userData);

                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpCookie faCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                    Response.Cookies.Add(faCookie);

                    if (roles.Any(r => r.Name.Contains("Admin")))
                    {
                        return RedirectToAction("Fixtures", "Home");
                    }
                    else if (roles.Any(r => r.Name.Contains("User")))
                    {
                        return RedirectToAction("Index", "User");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError("", "Incorrect username and/or password");
            }

            return View(model);
        }
        [AllowAnonymous]
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Account", null);
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        //[AllowAnonymous]
        //public JsonResult ResetPassword(string email)
        //{
        //  Result res =  new UsersRepo().ResetPassword(email);
        //  return Json(res, JsonRequestBehavior.AllowGet);
        //}

        [WMCBAdminAuthorize("Admin")]
        public JsonResult Users()
        {
            var users = new UsersRepo().GetUsers();
            return Json(users, JsonRequestBehavior.AllowGet);
        }     
     
        public ActionResult MyAccount()
        {
            var user = new UsersRepo().GetUserDetails(HttpContext.User.Identity.Name);           
            return View(user);
        }
        public JsonResult UpdateMyProfile(UpdateProfile user)
        {
            user.Email = HttpContext.User.Identity.Name;
            var rst = new UsersRepo().UpdateUserProfile(user);
            return Json(rst,JsonRequestBehavior.AllowGet);
        }
    }

}