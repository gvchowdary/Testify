using AppNameTestify.DBModel;
using AppNameTestify.Helpers;
using AppNameTestify.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AppNameTestify.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class TenantController : Controller
    {
        /// <summary>
        /// The database context
        /// </summary>
        TestifyEntities dbContext = new TestifyEntities();

        /// <summary>
        /// Tickets the details.
        /// </summary>
        /// <returns></returns>
        public ActionResult TicketDetails()
        {
            return View();
        }
        /// <summary>
        /// Gets the ticket lists.
        /// </summary>
        /// <param name="sidx">The sidx.</param>
        /// <param name="sord">The sord.</param>
        /// <param name="page">The page.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="search">The search.</param>
        /// <param name="nd">The nd.</param>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        public JsonResult GetTicketLists(string sidx, string sord, int page, int rows, string search, string nd, string status)
        {
            int tenantId = Convert.ToInt32(FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name);

            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var ticketList = dbContext.TenantTickets.Where(c => c.TenantId == tenantId && c.IsActive == true && c.Status == status).Select(
                    a => new
                    {
                        a.ID,
                        a.Title,
                        a.Description,
                        a.TenantId,
                        a.IsActive,
                        a.CreatedDate,
                        a.ModifiedDate,
                        a.ApprovedDate,
                        a.Status

                    }).ToList();

            int totalRecords = ticketList.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sord.ToUpper() == "DESC")
            {
                ticketList = ticketList.OrderByDescending(s => s.CreatedDate).ToList();
                ticketList = ticketList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else
            {
                ticketList = ticketList.OrderBy(s => s.CreatedDate).ToList();
                ticketList = ticketList.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = ticketList
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Creates the specified object ticket.
        /// </summary>
        /// <param name="objTicket">The object ticket.</param>
        /// <returns></returns>
        [HttpPost]
        public string Create([Bind(Exclude = "ID")] TenantTicket objTicket)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    int tenantId = Convert.ToInt32(FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name);
                    //  var tenantObj = dbContext.TenantRegistrations.FirstOrDefault(c => c.TenantId == tenantId && c.IsActive == true);
                    objTicket.TenantId = tenantId;
                    objTicket.IsActive = true;
                    objTicket.Status = "Pending";
                    objTicket.CreatedDate = DateTime.Now.ToString();
                    dbContext.TenantTickets.Add(objTicket);
                    dbContext.SaveChanges();
                    //if (tenantObj != null)
                    //{
                    //    var houseObj = dbContext.LandlordHouseDetails.FirstOrDefault(c => c.Id == tenantObj.LandlordPRTBNO);
                    //    var landlordObj = dbContext.LandlordRegistrations.FirstOrDefault(c => c.LandlordId == houseObj.LandlordId);
                    SendReportDetailsToLanlord(3,tenantId, "Tenant Reported New Problem");

                    //}
                    msg = "Saved Successfully";


                }
                else
                {
                    msg = "Validation data not successfull";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }

            return msg;
        }
        private void SendReportDetailsToLanlord(int emailtempid,int tenantId, string mailSubject)
        {
            try
            {
                var model = dbContext.TenantRegistrations.FirstOrDefault(c => c.TenantId == tenantId && c.IsActive == true);

                if (model != null)
                {
                    var houseObj = dbContext.LandlordHouseDetails.FirstOrDefault(c => c.Id == model.LandlordPRTBNO);
                    var landlordObj = dbContext.LandlordRegistrations.FirstOrDefault(c => c.LandlordId == houseObj.LandlordId);
                    var emailTemplateObj = dbContext.EmailTemplates.FirstOrDefault(c => c.EmailTemplateId == emailtempid);
                    if (emailTemplateObj != null)
                    {
                        var emailBody = emailTemplateObj.EmailBodyTemplate;
                        emailBody = emailBody.Replace("#FirstName", model.FirstName);
                        emailBody = emailBody.Replace("#LastName", model.LastName);
                        emailBody = emailBody.Replace("#PhoneNo", model.PhoneNo);
                        emailBody = emailBody.Replace("#EmailId", model.Email);
                        EmailComponent.SendEmail(landlordObj.Email, mailSubject, emailBody);
                    }
                }
            }
            catch (Exception ex)
            {

            }
            


        }
        /// <summary>
        /// Edits the specified object ticket.
        /// </summary>
        /// <param name="objTicket">The object ticket.</param>
        /// <returns></returns>
        public string Edit(TenantTicket objTicket)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                   var ticketObj= dbContext.TenantTickets.FirstOrDefault(c => c.ID == objTicket.ID);
                   if (ticketObj!=null)
                   {
                       
                       objTicket.CreatedDate = ticketObj.CreatedDate;
                       objTicket.ModifiedDate = DateTime.Now.ToString();
                       objTicket.Status = "Pending";
                     //  dbContext.Entry(objTicket).State = EntityState.Modified;
                       dbContext.Entry(ticketObj).CurrentValues.SetValues(objTicket);
                       dbContext.SaveChanges();
                       SendReportDetailsToLanlord(4, Convert.ToInt32(objTicket.TenantId), "Tenant Updated Report Details");

                       msg = "Updated Successfully";
                   }
                   else
                   {
                       msg = "Error occured while updating the details";
                   }
                    



                }
                else
                {
                    msg = "Validation data not successfull";
                }
            }
            catch (Exception ex)
            {
                msg = "Error occured:" + ex.Message;
            }
            return msg;
        }
        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <returns></returns>
        public string Delete(int ID)
        {
            var objTicket = dbContext.TenantTickets.Find(ID);
            objTicket.IsActive = false;
            objTicket.ModifiedDate = DateTime.Now.ToString();
            dbContext.Entry(objTicket).State = EntityState.Modified;
            dbContext.SaveChanges();
            SendReportDetailsToLanlord(5,Convert.ToInt32(objTicket.TenantId), "Tenant Deleted Report Details");

            return "Deleted successfully";
        }



    }
}
