using GymProcess.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GymProcess.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
       

        // GET: Users
        public Boolean isAdminUser()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ApplicationDbContext context = new ApplicationDbContext();
                var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
                var s = UserManager.GetRoles(user.GetUserId());
                if (s[0].ToString() == "Admin")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public ActionResult Index(string result)
       {
            GymProcessEntities1 db = new GymProcessEntities1();
            result = (Convert.ToString(Session["userId"]));
            ViewBag.resultquery = db.sp_getUserDetail(result).FirstOrDefault();
          
            if (User.Identity.IsAuthenticated)
            {
                var user = User.Identity;
                ViewBag.Name = user.Name;
               
                //	ApplicationDbContext context = new ApplicationDbContext();
                //	var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

                //var s=	UserManager.GetRoles(user.GetUserId());
                ViewBag.displayMenu = "No";

                if (isAdminUser())
                {
                    ViewBag.displayMenu = "Yes";
                   
                   // ViewBag.resultquery = (from c in db.Tbl_Member where c.MemberId == Convert.ToInt32(Session["userId"]) select c);
                }

                return View();
            }
            else
            {
                ViewBag.Name = "Not Logged IN";
            }
            Session.Abandon();
          
            return View();

          
        }
       
    }
}