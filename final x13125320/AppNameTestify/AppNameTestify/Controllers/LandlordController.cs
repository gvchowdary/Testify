using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppNameTestify.Models;
using System.Web.Security;
using AppNameTestify.DBModel;
using System.Data;
using AppNameTestify.Helpers;

namespace AppNameTestify.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Authorize]
    public class LandlordController : Controller
    {

        /// <summary>
        /// The database context
        /// </summary>
        TestifyEntities dbContext = new TestifyEntities();

        /// <summary>
        /// Gets the current landlord details.
        /// </summary>
        /// <returns></returns>
        private LandlordRegister GetCurrentLandlordDetails()
        {
            LandlordRegister LandlordModel = new LandlordRegister();
            int userid = Convert.ToInt32(FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name);
            var landlordEntity = dbContext.LandlordRegistrations.FirstOrDefault(r => r.LandlordId == userid && r.IsActive == true);
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
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return null;
            //return View();
        }

        /// <summary>
        /// Manages the houses.
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageHouses()
        {

            return View();
        }


        /// <summary>
        /// Gets the manage house lists.
        /// </summary>
        /// <param name="sidx">The sidx.</param>
        /// <param name="sord">The sord.</param>
        /// <param name="page">The page.</param>
        /// <param name="rows">The rows.</param>
        /// <param name="search">The search.</param>
        /// <param name="nd">The nd.</param>
        /// <returns></returns>
        public JsonResult GetManageHouseLists(string sidx, string sord, int page, int rows, string search, string nd)
        {
            var landlordModel = GetCurrentLandlordDetails();
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            var houseListsResults = dbContext.LandlordHouseDetails.Where(c => c.IsActive == true && c.LandlordId == landlordModel.ID).Select(
                    a => new
                    {
                        a.Id,
                        a.LandlordId,
                        a.PRTBNO,
                        a.Address,
                        a.IsActive

                    }).ToList();

            int totalRecords = houseListsResults.Count();
            var totalPages = (int)Math.Ceiling((float)totalRecords / (float)rows);
            if (sord.ToUpper() == "DESC")
            {
                houseListsResults = houseListsResults.OrderByDescending(s => s.PRTBNO).ToList();
                houseListsResults = houseListsResults.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }
            else
            {
                houseListsResults = houseListsResults.OrderBy(s => s.PRTBNO).ToList();
                houseListsResults = houseListsResults.Skip(pageIndex * pageSize).Take(pageSize).ToList();
            }

            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = houseListsResults
            };
            return Json(jsonData, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Creates the specified object lanlord house.
        /// </summary>
        /// <param name="objLanlordHouse">The object lanlord house.</param>
        /// <returns></returns>
        [HttpPost]
        public string Create([Bind(Exclude = "Id")] LandlordHouseDetail objLanlordHouse)
        {
            string msg;
            try
            {

                if (ModelState.IsValid)
                {
                    var landLordModel = GetCurrentLandlordDetails();
                    var result = IsPRTBNOExit(landLordModel.ID, objLanlordHouse.PRTBNO.Trim(), 0);
                    if (result == false)
                    {
                        objLanlordHouse.LandlordId = landLordModel.ID;
                        objLanlordHouse.IsActive = true;
                        dbContext.LandlordHouseDetails.Add(objLanlordHouse);
                        dbContext.SaveChanges();
                        msg = "Saved Successfully";
                    }
                    else
                    {
                        msg = "Already PRTBNO is Exists";

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
        /// Edits the specified object lanlord house.
        /// </summary>
        /// <param name="objLanlordHouse">The object lanlord house.</param>
        /// <returns></returns>
        public string Edit(LandlordHouseDetail objLanlordHouse)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    var landLordModel = GetCurrentLandlordDetails();
                    var result = IsPRTBNOExit(landLordModel.ID, objLanlordHouse.PRTBNO.Trim(), objLanlordHouse.Id);
                    if (result == false)
                    {
                        dbContext.Entry(objLanlordHouse).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        msg = "Updated Successfully";
                    }
                    else
                    {
                        msg = "Already PRTBNO is Exists";

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
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public string Delete(int Id)
        {
            LandlordHouseDetail objLanlordHouse = dbContext.LandlordHouseDetails.Find(Id);
            objLanlordHouse.IsActive = false;
            dbContext.Entry(objLanlordHouse).State = EntityState.Modified;
            dbContext.SaveChanges();
            return "Deleted successfully";
        }
        /// <summary>
        /// Determines whether [is prtbno exit] [the specified landlordid].
        /// </summary>
        /// <param name="landlordid">The landlordid.</param>
        /// <param name="PRTBNO">The prtbno.</param>
        /// <param name="houseId">The house identifier.</param>
        /// <returns></returns>
        private bool IsPRTBNOExit(int landlordid, string PRTBNO, int houseId)
        {


            var result = dbContext.LandlordHouseDetails.Where(c => c.LandlordId == landlordid && c.PRTBNO == PRTBNO && c.Id != houseId && c.IsActive == true).ToList();

            if (result != null && result.Count > 0)
            {
                return true;
            }
            return false;

        }
        /// <summary>
        /// Manages the ticket details.
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageTicketDetails()
        {
            int landlordId = Convert.ToInt32(FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name);
            var landlordHouseList = dbContext.LandlordHouseDetails.Where(c => c.LandlordId == landlordId && c.IsActive == true).ToList();
            var houseList = landlordHouseList.Select(
                    a => new HouseModel
                    {
                        Id = a.Id,
                        PRTBNO = a.PRTBNO
                    }).ToList();
            houseList.Insert(0, new HouseModel() { Id = 0, PRTBNO = "--Select PRTBNO--" });
            ViewBag.houseList = new SelectList(houseList, "Id", "PRTBNO");
            return View("ManageTicketDetails");
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
        /// <param name="prtbid">The prtbid.</param>
        /// <returns></returns>
        public JsonResult GetTicketLists(string sidx, string sord, int page, int rows, string search, string nd, string status, int prtbid)
        {
            var tenant = dbContext.TenantRegistrations.FirstOrDefault(c => c.LandlordPRTBNO == prtbid);
            if (tenant != null)
            {
                int pageIndex = Convert.ToInt32(page) - 1;
                int pageSize = rows;
                var ticketList = dbContext.TenantTickets.Where(c => c.TenantId == tenant.TenantId && c.IsActive == true && c.Status == status).Select(
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
                            a.Status,
                            a.TenantRegistration.FirstName,
                            a.TenantRegistration.LastName,
                            a.TenantRegistration.Email,
                            a.TenantRegistration.PhoneNo

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
            else
            {
                var ticketList = dbContext.TenantTickets.Where(c => c.TenantId == -1).Select(
                        a => new
                        {
                            a.ID,
                            a.Title,
                            a.Description,
                            a.TenantId,
                            a.IsActive,
                            a.CreatedDate,
                            a.ModifiedDate,
                            a.Status

                        }).ToList();
                var jsonData = new
                {
                    total = 0,
                    page,
                    records = 0,
                    rows = ticketList
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Edits the ticket.
        /// </summary>
        /// <param name="objTicket">The object ticket.</param>
        /// <returns></returns>
        public string EditTicket(TenantTicket objTicket)
        {
            string msg;
            try
            {
                if (ModelState.IsValid)
                {
                    var ticketObj = dbContext.TenantTickets.FirstOrDefault(c => c.ID == objTicket.ID);
                    if (ticketObj != null)
                    {
                        objTicket.CreatedDate = ticketObj.CreatedDate;
                        objTicket.ModifiedDate = ticketObj.ModifiedDate;
                        objTicket.ApprovedDate = objTicket.Status == "Approved" ? DateTime.Now.ToString() : string.Empty;
                        dbContext.Entry(ticketObj).CurrentValues.SetValues(objTicket);
                        dbContext.SaveChanges();
                        msg = "Updated Successfully";
                        if (objTicket.Status == "Approved")
                        {
                            msg = "Approved Successfully";
                            SendResolvedNotificationToTenant(objTicket);
                        }


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
        private void SendResolvedNotificationToTenant(TenantTicket objTicket)
        {
            try
            {
                var model = dbContext.TenantRegistrations.FirstOrDefault(c => c.TenantId == objTicket.TenantId && c.IsActive == true);

                if (model != null)
                {

                    var emailTemplateObj = dbContext.EmailTemplates.FirstOrDefault(c => c.EmailTemplateId == 6);
                    if (emailTemplateObj != null)
                    {
                        var emailBody = emailTemplateObj.EmailBodyTemplate;
                        emailBody = emailBody.Replace("#Title", objTicket.Title);
                        emailBody = emailBody.Replace("#Description", objTicket.Description);

                        EmailComponent.SendEmail(model.Email, "Problem Sloved", emailBody);
                    }
                }
            }
            catch (Exception ex)
            {

            }



        }

    }
}
