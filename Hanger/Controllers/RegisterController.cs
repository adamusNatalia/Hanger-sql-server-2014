using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hanger.Models;
using System.Data.Entity.Infrastructure;
using System.Net;
using System.Data.Entity;

namespace Hanger.Controllers
{
    public class RegisterController : Controller
    {
        private HangerDatabase db = new HangerDatabase();
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult IsProfilNameAvailable(string Profil_name)
        {
            return Json(!db.User.Any(user => user.Profil_name == Profil_name),JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult IsMailAvailable(string Mail)
        {
            return Json(!db.User.Any(user => user.Mail == Mail), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User U)
        {
            int userId=1;
            if (ModelState.IsValid)
            {
              
                {
                    U.Date_access = DateTime.Now;
                    db.User.Add(U);
                    db.SaveChanges();
                    ModelState.Clear();
                    String profil_name = U.Profil_name;
                    userId = U.Id;
                   
                    ViewBag.Message = "Successfully Registration Done";

                    U = null;



                }
            }

            //return RedirectToAction("Favorites", "UserProfil", new { id = userId } );

            //return RedirectToAction("New", "Catalog");
            return RedirectToAction("AfterRegister", "Register",new { id = userId } );
        }

        public ActionResult AfterRegister(int id)
        {
            if (ModelState.IsValid) // this is check validity
            {

                var user = from p in db.User
                           where p.Id == id
                           select p;

                if (user.Count() != 0)
                {
                    Session["LogedUserID"] = user.First();
                    // return RedirectToAction("AfterLogin");
                }


            }
            return RedirectToAction("New", "Catalog");
        }

        public ActionResult NewProfil(int id)
        {
            if (ModelState.IsValid) // this is check validity
            {

                    var user = from p in db.User
                               where p.Id == id 
                               select p;

                    if (user.Count() != 0)
                    {
                        Session["LogedUserID"] = user.First();
                       // return RedirectToAction("AfterLogin");
                    }


              }
            SizeDropDownList();
            BrandDropDownList();
            ColorDropDownList();
            ColorDropDownList2();
            // ColorDropDownList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NewProfil(UserProfil up)
        {
            //int adId=32;
            try
            {
                if (ModelState.IsValid)

                {
                    up.UserId = (Session["LogedUserID"] as User).Id;

                    db.UserProfil.Add(up);

                    //db.SaveChanges();
                    try
                    {
                        db.SaveChanges();

                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting
                                // the current instance as InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                    ModelState.Clear();

                }
                else
                {

                    SizeDropDownList(up.SizeId);
                    BrandDropDownList(up.BrandId);
                    ColorDropDownList(up.Color1Id);
                    ColorDropDownList2(up.Color2Id);



                    return View(up);
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }


            SizeDropDownList(up.SizeId);
            BrandDropDownList(up.BrandId);
            ColorDropDownList(up.Color1Id);
            ColorDropDownList2(up.Color2Id);



            //return View(A);
            return RedirectToAction("AfterLogin", "Login");

        }

        public ActionResult EditProfil(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var u = from p in db.UserProfil
                       where p.UserId == id
                       select p;
            if (u.Count() == 0)
            {
                return RedirectToAction("NewProfil", "Register", new { id = id });
            }
            int index = u.FirstOrDefault().Id;
            UserProfil up = db.UserProfil.Find(index);

            SizeDropDownList(up.SizeId);
            BrandDropDownList(up.BrandId);
            ColorDropDownList(up.Color1Id);
            ColorDropDownList2(up.Color2Id);

            return View(up);
        }




        [HttpPost]
        public ActionResult EditProfil(UserProfil UP)
        {
            // ModelState.Remove("Date_start");
            if (ModelState.IsValid)
            {
                UP.UserId = (Session["LogedUserID"] as User).Id;
                int usId= (Session["LogedUserID"] as User).Id; 

                var u = (from p in db.UserProfil
                        where p.UserId == usId
                        select p.Id).FirstOrDefault();
                UP.Id = u;


                db.Entry(UP).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting
                            // the current instance as InnerException
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
                return RedirectToAction("AfterLogin", "Login");

            }
            
            SizeDropDownList(UP.SizeId);
            BrandDropDownList(UP.BrandId);
            ColorDropDownList(UP.Color1Id);
            ColorDropDownList2(UP.Color2Id);

            return View(UP);
        }


        private void SizeDropDownList(object selectedSize = null)
        {
            var sizeQuery = from d in db.Size
                            orderby d.Id
                            select d;
            ViewBag.SizeId = new SelectList(sizeQuery, "Id", "Name", selectedSize);
        }
       
        private void ColorDropDownList(object selectedColor = null)
        {
            var sizeQuery = from d in db.Color
                            orderby d.Name
                            select d;
            ViewBag.Color1Id = new SelectList(sizeQuery, "Id", "Name", selectedColor);
        }

        private void ColorDropDownList2(object selectedColor = null)
        {
            var sizeQuery = from d in db.Color
                            orderby d.Name
                            select d;
            ViewBag.Color2Id = new SelectList(sizeQuery, "Id", "Name", selectedColor);
        }

        private void BrandDropDownList(object selectedBrand = null)
        {
            var sizeQuery = from d in db.Brand
                            orderby d.Id
                            select d;
            ViewBag.BrandId = new SelectList(sizeQuery, "Id", "Name", selectedBrand);
        }


    }
}