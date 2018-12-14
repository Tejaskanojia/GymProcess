using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using GymProcess.Models;
using ClosedXML;
using GymProcess.Areas.Member.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Web.Security;
using ClosedXML.Excel;

namespace GymProcess.Areas.Member.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private GymProcessEntities1 db = new GymProcessEntities1();

       
        // GET: Member/Member
        [HttpPost]
        public ActionResult exportExel()
        {
            GymProcessEntities1 db = new GymProcessEntities1();
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[11] { new DataColumn("MemberId"),
                                            new DataColumn("MemberName"),
                                            new DataColumn("mobile"),
                                            new DataColumn("Email"),
                                            new DataColumn("Address"),
                                             new DataColumn("Height"),
                                              new DataColumn("Weight"),
                                                new DataColumn("BloodGroup"),
                                            new DataColumn("MemberType"),
                                            new DataColumn("Scheme"),
                                             new DataColumn("PayementDetail")});

            var member = from Tbl_Member in db.Tbl_Member.Take(10)
                            select Tbl_Member;
            

            foreach (var Tbl_Member in member)
            {
                dt.Rows.Add(Tbl_Member.MemberId,Tbl_Member.MemberName,Tbl_Member.Mobile,Tbl_Member.Email,Tbl_Member.Addess,Tbl_Member.Height,Tbl_Member.Weight,Tbl_Member.BloodGroup,Tbl_Member.MemberName,Tbl_Member.Scheme,Tbl_Member.PaymentDetail);
            }


            using (XLWorkbook wb = new XLWorkbook())
            {
               
                wb.Worksheets.Add(dt);
                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename=SqlExport.xlsx");
                wb.Worksheet(1).Cells("A1:k1").Style.Fill.BackgroundColor = XLColor.White;
                wb.ShowGridLines = false;
               
                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    //A resembles First Column while C resembles Third column.
                    //Header row is at Position 1 and hence First row starts from Index 2.
                    string cellRange = string.Format("A{0}:k{0}", i + 1);
                    if (i % 2 != 0)
                    {
                        wb.Worksheet(1).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.WhiteSmoke;
                    }
                    else
                    {
                        wb.Worksheet(1).Cells(cellRange).Style.Fill.BackgroundColor = XLColor.DimGray;
                    }

                }
                // font Size 
                wb.Worksheet(1).Cells("A1:K1").Style.Font.Bold = true;
                //Adjust widths of Columns.
                wb.Worksheet(1).Columns().AdjustToContents();

                using (MemoryStream stream = new MemoryStream())
                {


                    wb.SaveAs(stream);
                    stream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
              
            }
          
        }
       



        [Authorize(Roles = "Admin")]
        public ActionResult Index(string sortOrder, string CurrentSort, string Filter_Value, int? page, string searchString)
        {
            GymProcessEntities1 db = new GymProcessEntities1();
            int pageSize = 10;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            ViewBag.CurrentSort = sortOrder;
            sortOrder = String.IsNullOrEmpty(sortOrder) ? "Member" : sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = Filter_Value;
            }

            ViewBag.FilterValue = searchString;

            var members = from mem in db.Tbl_Member select mem;

            if (!String.IsNullOrEmpty(searchString))
            {
                members = members.Where(mem => mem.MemberName.ToUpper().Contains(searchString.ToUpper())
                    || mem.MemberType.ToUpper().Contains(searchString.ToUpper()));
            }
            IPagedList<Tbl_Member> employees = null;

            switch (sortOrder)
            {
                case "Member":
                    if (sortOrder.Equals(CurrentSort))
                        employees = db.Tbl_Member.OrderByDescending
                                (m => m.MemberName).ToPagedList(pageIndex, pageSize);
                    else
                        employees = db.Tbl_Member.OrderBy
                                (m => m.MemberName).ToPagedList(pageIndex, pageSize);
                    break;
                case "Email":
                    if (sortOrder.Equals(CurrentSort))
                        employees = db.Tbl_Member.OrderByDescending
                                (m => m.Email).ToPagedList(pageIndex, pageSize);
                    else
                        employees = db.Tbl_Member.OrderBy
                                (m => m.Email).ToPagedList(pageIndex, pageSize);
                    break;
                case "Phone":
                    if (sortOrder.Equals(CurrentSort))
                        employees = db.Tbl_Member.OrderByDescending
                                (m => m.Mobile).ToPagedList(pageIndex, pageSize);
                    else
                        employees = db.Tbl_Member.OrderBy
                                (m => m.Mobile).ToPagedList(pageIndex, pageSize);
                    break;


                case "Default":
                    employees = db.Tbl_Member.OrderBy
                        (m => m.MemberName).ToPagedList(pageIndex, pageSize);
                    break;
            }
            return View(employees);

        }

        public ActionResult GetList()
        {
            
            using (GymProcessEntities1 db = new GymProcessEntities1())
            {
                var memberList = db.Tbl_Member.ToList<Tbl_Member>();
              
                        db.Configuration.LazyLoadingEnabled = false;
                return Json(new { data = memberList }, JsonRequestBehavior.AllowGet);
            }
            
        }


        // GET: Member/Member/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Member tbl_Member = db.Tbl_Member.Find(id);
            if (tbl_Member == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Member);
        }

        // GET: Member/Member/Create
        public ActionResult Create()
        {
            List<SelectListItem> sex_items = new List<SelectListItem>();
            sex_items.Add(new SelectListItem() { Text = "FEMALE", Value = "FEMALE" });
            sex_items.Add(new SelectListItem() { Text = "MALE", Value = "MALE" });

            ViewBag.Sex = sex_items;
            //Session["userId"] = Session["userId"];

            ViewBag.Member = new SelectList(db.Schemes, "SchemeId", "SchemeName");
            Tbl_Member tbl_Member = new Tbl_Member();
            tbl_Member.AspUserId = Session["userId"].ToString();
            return View(tbl_Member);



        }


        // POST: Member/Member/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tbl_Member tbl_Member, HttpPostedFileBase Photo, AspNetRole role,AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                /// Phone No 

                if (Photo.ContentLength > 0)
                {
                    string _FileName = Path.GetFileName(Photo.FileName);
                    string _path = Path.Combine(Server.MapPath("~/Content/Image"), _FileName);
                    tbl_Member.ImageURL = _FileName;
                    Photo.SaveAs(_path);
                }
               
                db.Tbl_Member.Add(tbl_Member);
                db.SaveChanges();
                 FormsAuthentication.SignOut();
                Session.Abandon();
                return RedirectToAction("Index");
            }

            //var result = (from a in tbl_Member//.schemeid
            //              join b in Scheme on  //.schemeid on 
            //              a.schemeid equals b.)

            ViewBag.Member = new SelectList(db.Tbl_Member, "SchemeId", "SchemeName");
            //ViewBag.Member = new SelectList(db.role, "Id", "Name");



            return View(tbl_Member);
        }




        // GET: Member/Member/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Member tbl_Member = db.Tbl_Member.Find(id);
            ViewBag.Member = new SelectList(db.Schemes, "SchemeId", "SchemeName");

            if (tbl_Member == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Member);
        }

        // POST: Member/Member/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MemberId,MemberName,Mobile,Email,ImageURL,DateOfBirth,SchemeId,Addess,Sex,Weight,Height,BloodGroup,MemberType,PaymentDetail")] Tbl_Member tbl_Member)
        {
            List<SelectListItem> sex_items = new List<SelectListItem>();
            sex_items.Add(new SelectListItem() { Text = "FEMALE", Value = "FEMALE" });
            sex_items.Add(new SelectListItem() { Text = "MALE", Value = "MALE" });

            ViewBag.Sex = sex_items;
            ViewBag.Member = new SelectList(db.Tbl_Member, "SchemeId", "SchemeName");
            if (ModelState.IsValid)
            {
                db.Entry(tbl_Member).State = EntityState.Modified;
                db.SaveChanges();
               
                return RedirectToAction("Index");
            }
            return View(tbl_Member);
        }

        // GET: Member/Member/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tbl_Member tbl_Member = db.Tbl_Member.Find(id);
            if (tbl_Member == null)
            {
                return HttpNotFound();
            }
            return View(tbl_Member);
        }

        // POST: Member/Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int MemberId)
        {
            Tbl_Member tbl_Member = db.Tbl_Member.Find(MemberId);
            db.Tbl_Member.Remove(tbl_Member);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                Session.Abandon();
            }
            base.Dispose(disposing);
        }



    }
}
