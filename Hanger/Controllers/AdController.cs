﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hanger.Models;
using System.Data.Entity.Infrastructure;
using System.Net.Mail;
using System.Net;
using System.Data.Entity;

namespace Hanger.Controllers
{
    public class AdController : Controller
    {
        private HangerDatabase db = new HangerDatabase();
        // GET: Ad
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Details(int Id)
        {
            Ad advertisement = db.Ad.Find(Id);
            return View(advertisement);
        }


        public ActionResult Product(int Id)
        {
            //MergeModel model = new MergeModel();
            // Ad advertisement = db.Ad.Find(Id);
            var advertisement = from a in db.Ad
                                where (a.Id == Id)
                                select a;

            const int MaxLength = 10;
            var name = advertisement.FirstOrDefault().Date_start.ToString();



            if (name.Length > MaxLength)
                name = name.Substring(0, MaxLength);
            //DateTime dateAndTime = advertisement.Date_start;
            ViewBag.date = name;




            var ad = from s in db.Ad
                     select s;
            //ViewBag.AddCount = ad.Count();
            ViewBag.Id = Id;

            ViewBag.index = ad.ToList().FindIndex(a => a.Id == Id);
            ViewBag.model = ad.Count();

            if (Session["LogedUserID"] != null)
            {
                int user = (Session["LogedUserID"] as User).Id;
                var fav = from s in db.Favourite
                          where (s.UserId == user) && (s.AdId == Id)
                          select s;
                if (fav.Count() == 0)
                {
                    ViewBag.isInFavourites = false;
                }
            }

            var counter = advertisement.FirstOrDefault().Counter;
            if (counter == null)
            {
                advertisement.FirstOrDefault().Counter = 1;
            }
            else
            {
                int counter1 = advertisement.FirstOrDefault().Counter.GetValueOrDefault();
                counter1++;
                advertisement.FirstOrDefault().Counter = counter1;
            }
            db.Entry(advertisement.FirstOrDefault()).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
            }

            var adds = (from s in db.Ad
                        orderby s.Id
                        select s.Id);

            List<int> list = new List<int>();
            list = adds.Take(3).ToList();

            //ViewData["Recommendation"] = Recommendation(Id);
            ViewData["Recommendation"] = list;

            return View(ad.ToList());
        }
        
        public ActionResult Photo1(int Id)
        {
            ViewBag.ad = Id;
            Ad advertisement = db.Ad.Find(Id);

            return View(advertisement);
        }

        public ActionResult Photo(int Id)
        {
            ViewBag.ad = Id;
            Ad advertisement = db.Ad.Find(Id);

            return View(advertisement);
        }


        [HttpPost]
        public ActionResult SendMail()
        {
            // MailMessage msg = new MailMessage();
            // msg.To.Add("hanger.natalia@gmail.com");
            // msg.From = new MailAddress("hanger.natalia@gmail.com");
            MailMessage msg = new MailMessage("hanger.natalia@gmail.com", "hanger.natalia@gmail.com");
            msg.Subject = "Welcome To REBAR Mobile Showcase";
            msg.Body = "Hi," + Environment.NewLine + @"Welcome to REBAR Mobile Showcase. Please click on the below link : https://ciouishowcase.accenture.com/mobile/m"
                + Environment.NewLine + "Regards," + Environment.NewLine + "CIO Design Agency";
            msg.Priority = MailPriority.Normal;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);

            client.Credentials = new NetworkCredential("hanger.natalia@gmail.com", "hangertest");
            // client.Host = "email.abc.com";
            //client.Port = 587;
            //  client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            // client.UseDefaultCredentials = true;
            client.Send(msg);
            return RedirectToAction("Index", "Mail");
        }





        public ActionResult New()
        {
            SwapDropDownList();
            SizeDropDownList();
            BrandDropDownList();
            ColorDropDownList();
            ConditionDropDownList();
            SubcategoryDropDownList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(Ad A)
        {
            //int adId=32;
            try
            {
                if (ModelState.IsValid)

                {
                    A.Date_start = DateTime.Now;
                    A.UserId = (Session["LogedUserID"] as User).Id;
                    //A.Id = 23;
                    
                    db.Ad.Add(A);
                    
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
                    SwapDropDownList(A.Swap);
                    SizeDropDownList(A.SizeId);
                    BrandDropDownList(A.BrandId);
                    ColorDropDownList(A.ColorId);
                    ConditionDropDownList(A.ConditionId);
                    SubcategoryDropDownList(A.SubcategoryId);



                    return View(A);
                }
            }
            catch (RetryLimitExceededException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.)
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            SwapDropDownList(A.Swap);
            SizeDropDownList(A.SizeId);
            BrandDropDownList(A.BrandId);
            ColorDropDownList(A.ColorId);
            ConditionDropDownList(A.ConditionId);
            SubcategoryDropDownList(A.SubcategoryId);
            int adId = (from ad in db.Ad
                        select ad.Id).Max();

            //return View(A);
            return RedirectToAction("Photo1", "Ad", new { id = adId });
            
        }
        public ActionResult Edit(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ad ad = db.Ad.Find(id);
            if (ad == null)
            {
                return HttpNotFound();
            }
            SwapDropDownList(ad.Swap);
            SizeDropDownList(ad.SizeId);
            BrandDropDownList(ad.BrandId);
            ColorDropDownList(ad.ColorId);
            ConditionDropDownList(ad.ConditionId);
            SubcategoryDropDownList(ad.SubcategoryId);
            return View(ad);
        }

        [HttpPost]
        public ActionResult Edit(Ad A)
        {
           // ModelState.Remove("Date_start");
            if (ModelState.IsValid)
            {
                A.UserId = (Session["LogedUserID"] as User).Id;
                A.Date_start = DateTime.Now;
                db.Entry(A).State = EntityState.Modified;
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
                return RedirectToAction("Photo1", "Ad", new { id = A.Id });

            }
            SwapDropDownList(A.Swap);
            SizeDropDownList(A.SizeId);
            BrandDropDownList(A.BrandId);
            ColorDropDownList(A.ColorId);
            ConditionDropDownList(A.ConditionId);
            SubcategoryDropDownList(A.SubcategoryId);
            return View(A);
        }

        private void SwapDropDownList(object selectedSwap = null)
        {
            List<SelectListItem> Swap = new List<SelectListItem>();
            Swap.Add(new SelectListItem() { Text = "Tak", Value = "True" });
            Swap.Add(new SelectListItem() { Text = "Nie", Value = "False" });

            ViewBag.Swap = new SelectList(Swap, "Value", "Text", selectedSwap);
           
         
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
            ViewBag.COlorId = new SelectList(sizeQuery, "Id", "Name", selectedColor);
        }
        private void BrandDropDownList(object selectedBrand = null)
        {
            var sizeQuery = from d in db.Brand
                            orderby d.Id
                            select d;
            ViewBag.BrandId = new SelectList(sizeQuery, "Id", "Name", selectedBrand);
        }

        private void ConditionDropDownList(object selectedCondition = null)
        {
            var sizeQuery = from d in db.Condition
                            orderby d.Name
                            select d;
            ViewBag.ConditionId = new SelectList(sizeQuery, "Id", "Name", selectedCondition);
        }

        private void SubcategoryDropDownList(object selectedSubcategory = null)
        {
            var sizeQuery = from d in db.Subcategory
                            orderby d.Id
                            select d;
            ViewBag.SubcategoryId = new SelectList(sizeQuery, "Id", "Name", selectedSubcategory);
        }

        //private void CategoryDropDownList(object selectedSubcategory = null)
        //{
        //    var sizeQuery = from d in db.Category
        //                    orderby d.Name
        //                    select d;
        //    ViewBag.SelectedtegoryId = new SelectList(sizeQuery, "Id", "Name", selectedCategory);
        //}




        public ActionResult Tiles()
            {
                ViewBag.Title = "Hanger";
                return View();
            }

        public ActionResult Details2()
        {
            var ad = from s in db.Ad
                        select s;

            return View(ad.ToList());
        }

        public ActionResult MainPhoto(int adId)
        {
            HttpPostedFileBase file = Request.Files[0];
            byte[] imageSize = new byte[file.ContentLength];
            file.InputStream.Read(imageSize, 0, (int)file.ContentLength);


            using (HangerDatabase db = new HangerDatabase())
            {
                Photos p = new Photos();
                p.Photo = imageSize;
                p.FIle_name = file.FileName;

                if (db.Photos != null && db.Photos.Count() != 0)
                {
                    p.Id = (from ph in db.Photos
                            select ph.Id).Max() + 1;
                }
                else
                    p.Id = 0;

                // p.OwnerId = (Session["CurrentUserEmail"] as User).UserId;
                p.AdId = adId;
                p.Type = file.ContentType;
                p.Main_photo = true;
                p.PhotoSiteId = 1;
                db.Photos.Add(p);
                db.SaveChanges();
            }

            //return RedirectToAction("New", "Home");
            return RedirectToAction("Photo1", "Ad", new { id = adId });
        }

        public ActionResult FrontPhoto()
        {
            HttpPostedFileBase file = Request.Files[0];
            byte[] imageSize = new byte[file.ContentLength];
            file.InputStream.Read(imageSize, 0, (int)file.ContentLength);


            using (HangerDatabase db = new HangerDatabase())
            {
                Photos p = new Photos();
                p.Photo = imageSize;
                p.FIle_name = file.FileName;

                if (db.Photos != null && db.Photos.Count() != 0)
                {
                    p.Id = (from ph in db.Photos
                                 select ph.Id).Max() + 1;
                }
                else
                    p.Id = 0;

               // p.OwnerId = (Session["CurrentUserEmail"] as User).UserId;
                p.AdId= (from ad in db.Ad
                         select ad.Id).Max();
                p.Type = file.ContentType;
                db.Photos.Add(p);
                db.SaveChanges();
            }

            //return RedirectToAction("New", "Home");
            return RedirectToAction("Photo", "Ad");
        }
        public ActionResult MPhoto(int adId)
        {
            HttpPostedFileBase file = Request.Files[0];
            byte[] imageSize = new byte[file.ContentLength];
            file.InputStream.Read(imageSize, 0, (int)file.ContentLength);


            using (HangerDatabase db = new HangerDatabase())
            {
                Photos p = new Photos();
                p.Photo = imageSize;
                p.FIle_name = file.FileName;

                if (db.Photos != null && db.Photos.Count() != 0)
                {
                    p.Id = (from ph in db.Photos
                            select ph.Id).Max() + 1;
                }
                else
                    p.Id = 0;

                // p.OwnerId = (Session["CurrentUserEmail"] as User).UserId;
                //  p.AdId = (from ad in db.Ad
                //            select ad.Id).Max();
                p.AdId = adId;
                p.Type = file.ContentType;
                p.Main_photo = true;
                p.PhotoSiteId = 1;
                db.Photos.Add(p);
                db.SaveChanges();
            }
            //return RedirectToAction("New", "Home");
            return RedirectToAction("Photo", "Ad", new { id = adId });
        }
        public ActionResult ModelPhoto(int adId)
        {
            HttpPostedFileBase file = Request.Files[0];
            byte[] imageSize = new byte[file.ContentLength];
            file.InputStream.Read(imageSize, 0, (int)file.ContentLength);


            using (HangerDatabase db = new HangerDatabase())
            {
                Photos p = new Photos();
                p.Photo = imageSize;
                p.FIle_name = file.FileName;

                if (db.Photos != null && db.Photos.Count() != 0)
                {
                    p.Id = (from ph in db.Photos
                            select ph.Id).Max() + 1;
                }
                else
                    p.Id = 0;

                // p.OwnerId = (Session["CurrentUserEmail"] as User).UserId;
                //  p.AdId = (from ad in db.Ad
                //            select ad.Id).Max();
                p.AdId = adId;
                p.Type = file.ContentType;
                p.Main_photo = false;
                p.PhotoSiteId = 2;
                db.Photos.Add(p);
                db.SaveChanges();
            }
            //return RedirectToAction("New", "Home");
            return RedirectToAction("Photo", "Ad",new { id = adId });
        }
        public ActionResult ZoomPhoto(int adId)
        {
            HttpPostedFileBase file = Request.Files[0];
            byte[] imageSize = new byte[file.ContentLength];
            file.InputStream.Read(imageSize, 0, (int)file.ContentLength);


            using (HangerDatabase db = new HangerDatabase())
            {
                Photos p = new Photos();
                p.Photo = imageSize;
                p.FIle_name = file.FileName;

                if (db.Photos != null && db.Photos.Count() != 0)
                {
                    p.Id = (from ph in db.Photos
                            select ph.Id).Max() + 1;
                }
                else
                    p.Id = 0;

                // p.OwnerId = (Session["CurrentUserEmail"] as User).UserId;
                p.AdId = adId;
                p.Type = file.ContentType;
                p.Main_photo = false;
                p.PhotoSiteId = 3;
                db.Photos.Add(p);
                db.SaveChanges();
            }

            //return RedirectToAction("New", "Home");
            return RedirectToAction("Photo", "Ad", new { id = adId });
        }
        public ActionResult BackPhoto(int adId)
        {
            HttpPostedFileBase file = Request.Files[0];
            byte[] imageSize = new byte[file.ContentLength];
            file.InputStream.Read(imageSize, 0, (int)file.ContentLength);


            using (HangerDatabase db = new HangerDatabase())
            {
                Photos p = new Photos();
                p.Photo = imageSize;
                p.FIle_name = file.FileName;

                if (db.Photos != null && db.Photos.Count() != 0)
                {
                    p.Id = (from ph in db.Photos
                            select ph.Id).Max() + 1;
                }
                else
                    p.Id = 0;

                // p.OwnerId = (Session["CurrentUserEmail"] as User).UserId;
                p.AdId = adId;
                p.Type = file.ContentType;
                p.Main_photo = false;
                p.PhotoSiteId = 4;
                db.Photos.Add(p);
                db.SaveChanges();
            }

            //return RedirectToAction("New", "Home");
            return RedirectToAction("Photo", "Ad", new { id = adId });
        }

        public ActionResult AddPhoto(int adId)
        {
            HttpPostedFileBase file = Request.Files[0];
            byte[] imageSize = new byte[file.ContentLength];
            file.InputStream.Read(imageSize, 0, (int)file.ContentLength);


            using (HangerDatabase db = new HangerDatabase())
            {
                Photos p = new Photos();
                p.Photo = imageSize;
                p.FIle_name = file.FileName;

                if (db.Photos != null && db.Photos.Count() != 0)
                {
                    p.Id = (from ph in db.Photos
                            select ph.Id).Max() + 1;
                }
                else
                    p.Id = 0;

                // p.OwnerId = (Session["CurrentUserEmail"] as User).UserId;
                p.AdId = adId;
                p.Type = file.ContentType;
                p.Main_photo = false;
                db.Photos.Add(p);
                db.SaveChanges();
            }

            //return RedirectToAction("New", "Home");
            return RedirectToAction("Photo1", "Ad", new { id = adId });
        }
        [HttpPost]
        public ActionResult Delete(int adId)
        {
            using (HangerDatabase DataContext = new HangerDatabase())
            {
                var photoToDelete = (from p in DataContext.Photos
                                     where p.AdId == adId
                                     select p);
                while (photoToDelete.Count() > 0)
                {   
                    DataContext.Photos.Remove(photoToDelete.FirstOrDefault());
                    DataContext.SaveChanges();
                }
                var ad = (from p in DataContext.Ad
                                     where p.Id == adId
                                     select p).FirstOrDefault();

                DataContext.Ad.Remove(ad);
                DataContext.SaveChanges();
            }
            
            return RedirectToAction("UserProfil", "UserProfil", new { id = (Session["LogedUserID"] as Hanger.Models.User).Id });
        }

        [HttpPost]
        public ActionResult DeletePhoto(int id, int adId)
        {
            using (HangerDatabase DataContext = new HangerDatabase())
            {
                var photoToDelete = (from p in DataContext.Photos
                                     where p.Id == id
                                     select p).FirstOrDefault();

                DataContext.Photos.Remove(photoToDelete);
                DataContext.SaveChanges();
            }

            return RedirectToAction("Photo1", "Ad", new { id = adId });
        }

        [HttpPost]
        public ActionResult SendMail2(string email, string body, string subject, string to, int id )
        {
            if (email != "" && subject != "" && body != "")
            {
                var user = from p in db.User
                           where p.Profil_name == to
                           select p;

                string adId = id.ToString();
                MailMessage msg = new MailMessage();

                String emailTo = user.First().Mail;

                msg.To.Add(emailTo);
                //msg.To.Add("hanger.natalia@gmail.com");de
                //msg.From = new MailAddress(email);
                msg.From = new MailAddress("hanger.natalia@gmail.com",email);
                //msg.Sender = new MailAddress(email);

                msg.Subject = subject;

                msg.Body = "Dzień dobry," + Environment.NewLine 
                    + Environment.NewLine + @"Użytkownik "+@email+" jest zainteresowany Twoim ogłoszeniem: http://localhost:15054/Ad/Product/" + adId + Environment.NewLine
                    + Environment.NewLine + "Treść wiadomości od użytkownika: " + Environment.NewLine
                    + Environment.NewLine  + @body+ Environment.NewLine
                    + Environment.NewLine + "Pozdrawiamy serdecznie," + Environment.NewLine + "Zespół Hanger" ;
                msg.Priority = MailPriority.Normal;

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
               

                //client.Credentials = new NetworkCredential("hanger.natalia@gmail.com", "hangertest");
                client.Credentials = new NetworkCredential("tester.apo@gmail.com", "selenium");

                client.EnableSsl = true;

                client.Send(msg);
            }
            return RedirectToAction("Product", "Ad", new { Id = id });
        }

        [HttpPost]
        public ActionResult AddToFavourites(int adId)
        {

            using (HangerDatabase db = new HangerDatabase())
            {
                Favourite f = new Favourite();
                f.Date_start = DateTime.Now;
                f.UserId = (Session["LogedUserID"] as User).Id;


                if (db.Favourite != null && db.Favourite.Count() != 0)
                {
                    f.Id = (from ph in db.Favourite
                            select ph.Id).Max() + 1;
                }
                else
                    f.Id = 0;

                // p.OwnerId = (Session["CurrentUserEmail"] as User).UserId;
                f.AdId = adId;
                db.Favourite.Add(f);
                db.SaveChanges();
            }

            //return RedirectToAction("New", "Home");
            return RedirectToAction("Product", "Ad", new { id = adId });
        }

        [HttpPost]
        public ActionResult RemoveFromFavourites(int adId)
        {
            using (HangerDatabase DataContext = new HangerDatabase())
            {
                int user = (Session["LogedUserID"] as User).Id;
                var fav = (from s in DataContext.Favourite
                          where (s.UserId == user) && (s.AdId == adId)
                          select s).FirstOrDefault();

                DataContext.Favourite.Remove(fav);

                DataContext.SaveChanges();
            }

            return RedirectToAction("Product", "Ad", new { id = adId });
        }

        [HttpPost]
        public ActionResult IsSold(int adId)
        {

            using (HangerDatabase db = new HangerDatabase())
            {
                Ad A = db.Ad.Find(adId);
                if (ModelState.IsValid)
                {
                    A.Is_sold = true;
                    A.Date_end = DateTime.Now;
                    db.Entry(A).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                    }
                }
                
          
            }

            //return RedirectToAction("New", "Home");
            return RedirectToAction("Product", "Ad", new { id = adId });
        }

        public List<int> Recommendation(int Id) {


            var adds = (from s in db.Ad
                     
                      select s.Id).Take(3);



            // sprawdzam czy ktos dodal ogloszenie do ulubionych
            var fav = from s in db.Favourite
                        where (s.AdId == Id)
                        select s;

            if (fav.Count() == 0)
            {
                Random rnd = new Random();
            
                List<int> randomList = new List<int>();
                var ad = (from s in db.Ad                                  
                                    select s).ToList();

                int a = rnd.Next(0, ad.Count());
                randomList.Add(a);
                for (int i = 0; i < 3; i++)
                {
                    while (randomList.Contains(a))
                    {
                        a = rnd.Next(0, ad.Count());

                    }
                }


            }
            return adds.ToList();
               

        }

        
   /*         

            var counter = advertisement.FirstOrDefault().Counter;
            if (counter == null)
            {
                advertisement.FirstOrDefault().Counter = 1;
            }
            else
            {
                int counter1 = advertisement.FirstOrDefault().Counter.GetValueOrDefault();
                counter1++;
                advertisement.FirstOrDefault().Counter = counter1;
            }
            db.Entry(advertisement.FirstOrDefault()).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
            }


         
        }
*/

    }

    }

