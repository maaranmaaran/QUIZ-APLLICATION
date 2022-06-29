using quiz.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity.Validation;

namespace quiz.Controllers
{
    public class HomeController : Controller
    {
        DBQUIZSEntities db = new DBQUIZSEntities();
        public ActionResult Index()
        {
            if (Session["ad_id"]!=null)
                    {
                return RedirectToAction("Dashboard");
            }
            return View();
        }
      [HttpGet]
        public ActionResult sregister()
        {
            return View();
        }
        [HttpPost]
        public ActionResult sregister(TBL_STUDENT svw,HttpPostedFileBase imgfile )
        {
            TBL_STUDENT s = new TBL_STUDENT();
            try
            {
                s.S_NAME = svw.S_NAME;
                s.S_PASSWORD = svw.S_PASSWORD;
                s.S_IMAGE = uploadimage(imgfile);
                db.TBL_STUDENT.Add(s);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.msg = "Data could not be Inserted...";
            }
           
            return View();
        }
        public string uploadimage(HttpPostedFileBase imgfile)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();


            if (imgfile != null && imgfile.ContentLength > 0)
            {
                string extension = Path.GetExtension(imgfile.FileName);
                if (extension.ToLower().Equals("jpg") || extension.ToLower().Equals("jpeg") || extension.ToLower().Equals("jpg"))
                {
                    try
                    {
                        path = Path.Combine(Server.MapPath("~/Content/img"), r + Path.GetFileName(imgfile.FileName));
                        imgfile.SaveAs(path);
                        path = "~/Content/img" + r + Path.GetFileName(imgfile.FileName);
                    }



                    catch (Exception ex)
                    {
                        path = "-1";
                    }
                    return path;
                }

                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable... '); </script>");
                }
            }


            else
            {
                Response.Write("<script>alert('please select a file'); </script>");
                path = "-1";
            }
            return path;
        }

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult tLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult tLogin(TBL_ADMIN a)
        {
            TBL_ADMIN ad = db.TBL_ADMIN.Where(x => x.AD_NAME == a.AD_NAME && x.AD_PASSWORD == a.AD_PASSWORD).SingleOrDefault();

            if(ad!=null)
            {
                Session["ad_id"] = ad.AD_ID;
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.msg = "Invalid username or Password";
            }
            return View();
        }

        public ActionResult sLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult sLogin(TBL_STUDENT s)
        {
            TBL_STUDENT std = db.TBL_STUDENT.Where(x => x.S_NAME == s.S_NAME && x.S_PASSWORD == s.S_PASSWORD).SingleOrDefault();
            if(std==null)
            {
                ViewBag.msg = "Invalid email or Password";
            }
            else
            {
                Session["std_id"] = std.S_ID;
                return RedirectToAction("StudentExam");
            }
            return View();
        }

        public ActionResult Dashboard()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        public ActionResult StudentExam()
        {
            if (Session["std_id"] == null)
            {
                return RedirectToAction("sLogin");
            }
            return View();
        }
       
       
        [HttpGet]
        public ActionResult Addcategory()
        {
            if (Session["ad_id"]==null)
            {
                return RedirectToAction ("Index");
            }
            int adid = Convert.ToInt32(Session["ad_id"].ToString());
            List<tbl_categroy> li = db.tbl_categroy.Where(x => x.cat_fk_adid == adid).OrderByDescending(x => x.cat_id).ToList();
            ViewData["List"] = li;


            return View();
        }
        [HttpPost]
        public ActionResult Addcategory(tbl_categroy cat)
        {
           

            List<tbl_categroy> li = db.tbl_categroy.OrderByDescending(x => x.cat_id).ToList();
            ViewData["List"] = li;

            Random r = new Random();
            tbl_categroy c = new tbl_categroy();
            c.cat_name = cat.cat_name;
            c.cat_encyptedstring = cyptop.Encrypt(cat.cat_name.Trim() + r.Next().ToString(), true);
            c.cat_fk_adid = Convert.ToInt32(Session["ad_id"].ToString());

            db.tbl_categroy.Add(c);
            db.SaveChanges();


            return RedirectToAction("Addcategory");
           
        }
     [HttpGet]
     public ActionResult Addquestion()
        {
            int sid = Convert.ToInt32(Session["ad_id"]);
            List<tbl_categroy> li = db.tbl_categroy.Where(x => x.cat_fk_adid == sid).ToList();
            ViewBag.List = new SelectList(li, "cat_id", "cat_name");
            return View();
        }


        [HttpPost]
        public ActionResult Addquestion(TBL_QUESTIONS q)
        {
            int sid = Convert.ToInt32(Session["ad_id"]);
            List<tbl_categroy> li = db.tbl_categroy.Where(x => x.cat_fk_adid == sid).ToList();
            ViewBag.List = new SelectList(li, "cat_id", "cat_name");

            TBL_QUESTIONS QA = new TBL_QUESTIONS();
            QA.Q_TEXT = q.Q_TEXT;
            QA.OPA = q.OPA;
            QA.OPB = q.OPB;
            QA.OPC = q.OPC;
            QA.OPD = q.OPD;
            QA.COP = q.COP;
            QA.q_fk_catid = q.q_fk_catid;
            QA.tbl_categroy = li[0];

            db.TBL_QUESTIONS.Add(QA);
            try
            {
                db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                { 
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                        {


                        Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                    }
            }
        }

            TempData["msg"] = "Question added successfully...";
            TempData.Keep();
            return RedirectToAction("Addquestion");

            
        }
    }
}