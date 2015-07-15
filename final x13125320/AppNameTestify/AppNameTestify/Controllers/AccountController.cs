using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppNameTestify.Models;
using AppNameTestify.DBModel;
using System.Web.Security;
using AppNameTestify.Helpers;
using System.Data;

namespace AppNameTestify.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [HandleError]
    public class AccountController : Controller
    {

        /// <summary>
        /// The database context
        /// </summary>
        TestifyEntities dbContext = new TestifyEntities();
        /// <summary>
        /// Logins the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult Login(int? id)
        {

            ViewBag.id = id;
            if (id == 1)//id=1 means Landlord
                return View();

            if (id == 2)//id=2 means Tenant
                return View();

            return View("Error");
        }
        /// <summary>
        /// Logins the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Login(LoginModel model, int? Id)
        {

            if (model != null)
            {
                if (Id == 1)
                {
                    var reg = dbContext.LandlordRegistrations.FirstOrDefault(r => r.Email == model.Email && r.Password == model.Password && r.IsActive == true);
                    if (reg != null)
                    {

                        FormsAuthentication.SetAuthCookie(Convert.ToString(reg.LandlordId), false);
                        return RedirectToAction("ManageHouses", "Landlord");

                    }


                }
                else if (Id == 2)
                {
                    var isPRTBNOExit = dbContext.LandlordHouseDetails.FirstOrDefault(r => r.PRTBNO == model.PRTBNO && r.IsActive == true);
                    if (isPRTBNOExit != null)
                    {
                        var isLandlordExit = dbContext.LandlordRegistrations.FirstOrDefault(r => r.LandlordId == isPRTBNOExit.LandlordId && r.IsActive == true);
                        if (isLandlordExit != null)
                        {
                            var reg = dbContext.TenantRegistrations.FirstOrDefault(r => r.LandlordPRTBNO == isPRTBNOExit.Id && r.Password == model.Password);
                            if (reg != null)
                            {

                                FormsAuthentication.SetAuthCookie(Convert.ToString(reg.TenantId), false);
                                return RedirectToAction("TicketDetails", "Tenant");

                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {

                    }


                }

            }
            ViewBag.id = Id;
            ModelState.AddModelError("", "Invalid user name or password.");
            return View(model);
        }
        /// <summary>
        /// Registers the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult Register(int? id)
        {
            if (id == 1)
                return RedirectToAction("LandlordRegister");

            if (id == 2)
                return RedirectToAction("TenantRegister");

            return View("Error");


        }
        /// <summary>
        /// Landlords the register.
        /// </summary>
        /// <returns></returns>
        public ActionResult LandlordRegister()
        {
            return View();
        }

        /// <summary>
        /// Landlords the register.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LandlordRegister(LandlordRegister model)
        {
            if (ModelState.IsValid)
            {
                var isLandlordExit = dbContext.LandlordRegistrations.FirstOrDefault(r => r.PPSNNO == model.PPSNNO && r.IsActive == true);
                if (isLandlordExit == null)
                {
                    LandlordRegistration reg = new LandlordRegistration();
                    reg.FirstName = model.FirstName;
                    reg.LastName = model.LastName;
                    reg.Email = model.Email;
                    reg.Password = model.Password;
                    reg.PPSNNO = model.PPSNNO;
                    reg.PhoneNo = model.PhoneNo;
                    reg.Address = model.Address;
                    reg.IsActive = true;
                    reg.CreatedDate = DateTime.Now.ToString();
                    dbContext.LandlordRegistrations.Add(reg);
                    int success = dbContext.SaveChanges();
                    if (success > 0)
                    {
                        return RedirectToAction("RegistrationSuccess", new { id = 1 });

                    }
                }
                else
                {
                    ModelState.AddModelError("", "Already registration completed with the same PPSNNO");

                }

            }

            return View(model);
        }
        /// <summary>
        /// Tenants the register.
        /// </summary>
        /// <returns></returns>
        public ActionResult TenantRegister()
        {
            return View();
        }
        /// <summary>
        /// Forgots the password.
        /// </summary>
        /// <returns></returns>
        public ActionResult ForgotPassword(int? id)
        {

            ViewBag.id = id;
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model, int? Id)
        {
            if (model != null)
            {
                if (Id == 1)
                {
                    var objLandlore = dbContext.LandlordRegistrations.FirstOrDefault(r => r.Email == model.EmailId && r.IsActive == true);
                    if (objLandlore != null)
                    {
                        var guid = Convert.ToString(Guid.NewGuid());
                        objLandlore.TokenId = guid;
                        dbContext.Entry(objLandlore).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        SendResetPasswordLink(objLandlore.Email, guid, Convert.ToString(Id));
                        return RedirectToAction("ForgotPasswordMessage");
                    }

                }
                else if (Id == 2)
                {
                    var reg = dbContext.TenantRegistrations.FirstOrDefault(r => r.Email == model.EmailId && r.IsActive == true);
                    if (reg != null)
                    {
                        var guid = Convert.ToString(Guid.NewGuid());
                        reg.TokenId = guid;
                        dbContext.Entry(reg).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        SendResetPasswordLink(reg.Email, guid, Convert.ToString(Id));
                        return RedirectToAction("ForgotPasswordMessage");
                    }
                }

            }
            ViewBag.id = Id;
            ModelState.AddModelError("", "Invalid Email Id.");
            return View();
        }
        public ActionResult ForgotPasswordMessage()
        {
            return View();
        }
        public ActionResult ResetPassword(string token, string id)
        {
            ViewBag.Token = token;
            ViewBag.id = id;

            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(ForgotPasswordModel model, string token, string id)
        {
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(id))
            {
                if (id == "1")
                {
                    var reg = dbContext.LandlordRegistrations.FirstOrDefault(r => r.TokenId == token && r.IsActive == true);
                    if (reg != null)
                    {
                        reg.Password = model.NewPassword;
                        reg.TokenId = null;
                        dbContext.Entry(reg).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        return RedirectToAction("ResetPasswordConfirmation");
                    }
                }
                if (id == "2")
                {
                    var reg = dbContext.TenantRegistrations.FirstOrDefault(r => r.TokenId == token && r.IsActive == true);
                    if (reg != null)
                    {
                        reg.Password = model.NewPassword;
                        reg.TokenId = null;
                        dbContext.Entry(reg).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        return RedirectToAction("ResetPasswordConfirmation");
                    }
                }
            }
            ModelState.AddModelError("", "Invalid token id");
            return View();
        }
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }
        /// <summary>
        /// Tenants the register.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult TenantRegister(TenantRegister model)
        {
            if (ModelState.IsValid)
            {
                var isPRTBNOExit = dbContext.LandlordHouseDetails.FirstOrDefault(r => r.PRTBNO == model.PRTBNO && r.IsActive == true);
                if (isPRTBNOExit != null)
                {
                    var isLandlordExit = dbContext.LandlordRegistrations.FirstOrDefault(r => r.LandlordId == isPRTBNOExit.LandlordId && r.IsActive == true);
                    if (isLandlordExit != null)
                    {


                        var isTenantdExit = dbContext.TenantRegistrations.FirstOrDefault(r => r.PPSNNO == model.PPSNNO && r.IsActive == true && r.LandlordPRTBNO == isPRTBNOExit.Id);
                        if (isTenantdExit == null)
                        {
                            TenantRegistration reg = new TenantRegistration();
                            reg.FirstName = model.FirstName;
                            reg.LastName = model.LastName;
                            reg.Email = model.Email;
                            reg.Password = model.Password;
                            reg.PPSNNO = model.PPSNNO;
                            reg.PhoneNo = model.PhoneNo;
                            reg.Address = model.Address;
                            reg.IsActive = true;
                            reg.LandlordPRTBNO = isPRTBNOExit.Id;
                            reg.CreatedDate = DateTime.Now.ToString();
                            dbContext.TenantRegistrations.Add(reg);
                            int success = dbContext.SaveChanges();
                            if (success > 0)
                            {
                                SendEmailToLanlord(model, isLandlordExit.Email);
                                return RedirectToAction("RegistrationSuccess", new { id = 2 });

                            }
                        }
                        else
                        {
                            ModelState.AddModelError("", "Invalid PRTBNO");

                        }

                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid PRTBNO");

                    }
                }

                else
                {
                    ModelState.AddModelError("", "Invalid PRTBNO");

                }



            }

            return View(model);
        }
        /// <summary>
        /// Registrations the success.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public ActionResult RegistrationSuccess(int id)
        {
            ViewBag.id = id;
            return View();
        }
        /// <summary>
        /// Gets the current landlord details.
        /// </summary>
        /// <returns></returns>
        private LandlordRegister GetCurrentLandlordDetails()
        {
            LandlordRegister LandlordModel = new LandlordRegister();
            int userid = Convert.ToInt32(FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name);
            var landlordEntity = dbContext.LandlordRegistrations.FirstOrDefault(r => r.LandlordId == userid);
            if (landlordEntity != null)
            {
                LandlordModel.ID = landlordEntity.LandlordId;
                LandlordModel.FirstName = landlordEntity.FirstName;
                LandlordModel.LastName = landlordEntity.LastName;
                LandlordModel.PPSNNO = landlordEntity.PPSNNO;
            }
            return LandlordModel;


        }
        /// <summary>
        /// Logs the in information.
        /// </summary>
        /// <param name="userType">Type of the user.</param>
        /// <returns></returns>
        public ActionResult LogInInfo(int userType)
        {
            string userName = string.Empty;
            if (userType == 1)
            {
                LandlordRegister LandlordModel = new LandlordRegister();
                int userid = Convert.ToInt32(FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name);
                var landlordModel = dbContext.LandlordRegistrations.FirstOrDefault(r => r.LandlordId == userid && r.IsActive == true);
                userName = landlordModel != null ? landlordModel.FirstName + " " + landlordModel.LastName : string.Empty;
            }
            else if (userType == 2)
            {
                int tenantId = Convert.ToInt32(FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name);
                var tenantModel = dbContext.TenantRegistrations.FirstOrDefault(c => c.TenantId == tenantId && c.IsActive == true);
                userName = tenantModel != null ? tenantModel.FirstName + " " + tenantModel.LastName : string.Empty;

            }
            ViewBag.UserName = userName;
            return PartialView("_LogInInfo");
        }
        /// <summary>
        /// Logs the out.
        /// </summary>
        /// <returns></returns>
        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// Sends the email to lanlord.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="ToAddress">To address.</param>
        private void SendEmailToLanlord(TenantRegister model, string ToAddress)
        {
            var emailTemplateObj = dbContext.EmailTemplates.FirstOrDefault(c => c.EmailTemplateId == 1);
            if (emailTemplateObj != null)
            {
                var emailBody = emailTemplateObj.EmailBodyTemplate;
                emailBody = emailBody.Replace("#PRTBNO", model.PRTBNO);
                emailBody = emailBody.Replace("#FirstName", model.FirstName);
                emailBody = emailBody.Replace("#LastName", model.LastName);
                emailBody = emailBody.Replace("#PhoneNo", Convert.ToString(model.PhoneNo));
                EmailComponent.SendEmail(ToAddress, "Tenant Registered", emailBody);
            }

        }

        
        //private void SendResetPasswordLink(string ToAddress, string token, string id)
        //{
        //    var emailTemplateObj = dbContext.EmailTemplates.FirstOrDefault(c => c.EmailTemplateId == 2);
        //    if (emailTemplateObj != null)
        //    {
        //        var emailBody = emailTemplateObj.EmailBodyTemplate;
        //        emailBody = emailBody.Replace("#tokenid", token);
        //        emailBody = emailBody.Replace("#id", id);
        //        EmailComponent.SendEmail(ToAddress, "Reset Password", emailBody);
        //    }
        //}

        private void SendResetPasswordLink(string ToAddress, string token, string id)
        {
            var emailTemplateObj = dbContext.EmailTemplates.FirstOrDefault(c => c.EmailTemplateId == 2);
            if (emailTemplateObj != null)
            {
                var emailBody = emailTemplateObj.EmailBodyTemplate;
                emailBody = emailBody.Replace("#tokenid", token);
                emailBody = emailBody.Replace("#id", id);
                EmailComponent.SendEmail(ToAddress, "Reset Password", emailBody);
            }
        }

    }
}
