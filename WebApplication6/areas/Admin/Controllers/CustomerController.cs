//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web.Mvc;
//using WebApplication6.Models;

//namespace WebApplication6.Areas.Admin.Controllers
//{
//    public class CustomerController : Controller
//    {
//        private DBSportStoreEntities db = new DBSportStoreEntities();

//        // GET: Admin/Customer
//        public ActionResult Index()
//        {
//            var list = db.Customers.ToList();
//            return View(list); // Areas/Admin/Views/Customer/Index.cshtml
//        }

//        // GET: Admin/Customer/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//            var cus = db.Customers.Find(id);
//            if (cus == null || cus.IsDeleted)
//                return HttpNotFound();

//            return View(cus);
//        }

//        // GET: Admin/Customer/Create
//        public ActionResult Create()
//        {
//            return View();
//        }

//        // POST: Admin/Customer/Create
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create(Customer customer)
//        {
//            if (!ModelState.IsValid)
//                return View(customer);

//            db.Customers.Add(customer);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        // GET: Admin/Customer/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//            var cus = db.Customers.Find(id);
//            if (cus == null || cus.IsDeleted)
//                return HttpNotFound();

//            return View(cus);
//        }

//        // POST: Admin/Customer/Edit/5
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit(Customer customer)
//        {
//            if (!ModelState.IsValid)
//                return View(customer);

//            db.Entry(customer).State = EntityState.Modified;
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        // GET: Admin/Customer/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

//            var cus = db.Customers.Find(id);
//            if (cus == null)
//                return HttpNotFound();

//            return View(cus);
//        }

//        // POST: Admin/Customer/Delete/5 (soft delete)
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            var cus = db.Customers.Find(id);
//            if (cus == null)
//                return HttpNotFound();

//            cus.IsDeleted = true;
//            db.Entry(cus).State = EntityState.Modified;
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing) db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}
