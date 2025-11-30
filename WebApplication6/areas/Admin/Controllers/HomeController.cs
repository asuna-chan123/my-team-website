using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebApplication6.Models;

namespace WebApplication6.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        private DBSportStoreEntities db = new DBSportStoreEntities();

        public ActionResult Index()
        {
            DateTime fromDate = DateTime.Today.AddDays(-6);

            var orders = db.OrderProes
                .Where(o => o.DateOrder >= fromDate && o.DateOrder != null)
                .ToList();

            var groupByDate = orders
                .GroupBy(o => o.DateOrder.Value.Date)
                .OrderBy(g => g.Key)
                .ToList();

            var orderLabels = groupByDate.Select(g => g.Key.ToString("dd/MM")).ToList();
            var orderCounts = groupByDate.Select(g => g.Count()).ToList();

            ViewBag.OrderLabels = orderLabels;
            ViewBag.OrderCounts = orderCounts;

            var statusText = new Dictionary<int, string>
    {
        { 0, "Chờ duyệt" },
        { 1, "Đang chuẩn bị" },
        { 2, "Đang giao" },
        { 3, "Hoàn thành" },
        { 4, "Đã hủy" }
    };

            var groupByStatus = db.OrderProes.GroupBy(o => o.Status).ToList();
            var statusLabels = new List<string>();
            var statusCounts = new List<int>();

            foreach (var g in groupByStatus)
            {
                int s = g.Key;
                string label = statusText.ContainsKey(s) ? statusText[s] : "Khác";
                statusLabels.Add(label);
                statusCounts.Add(g.Count());
            }

            ViewBag.StatusLabels = statusLabels;
            ViewBag.StatusCounts = statusCounts;

            return View();
        }

    }
}
