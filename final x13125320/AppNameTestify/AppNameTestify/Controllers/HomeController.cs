using AppNameTestify.DBModel;
using AppNameTestify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppNameTestify.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    
    public class HomeController : Controller
    {
        

        
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
         
            return View();
        }
        /// <summary>
        /// Abouts this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult About()
        {
            return View();
        }
        public ActionResult Contact()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Contact(ContactModel model)
        {
            TestifyEntities dbContext = new TestifyEntities();
            try
            {
                Contact contactEntity = new Contact();
                contactEntity.ContactName = model.Name;
                contactEntity.EmailAddress = model.Email;
                contactEntity.PhoneNumber = model.PhoneNo;
                contactEntity.Message = model.Message;
                contactEntity.CreatedDate = DateTime.Now.ToString();
                dbContext.Contacts.Add(contactEntity);
                int success = dbContext.SaveChanges();
                return RedirectToAction("ContactSuccess");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("", "Error while submitting the contact details");

            }


            return View(model);
        }
        public ActionResult ContactSuccess()
        {
           
            return View();
        }
    }
}
