using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace PsiBB.Controllers
{
    // public class HomeController : Controller
    public class HomeController : AsyncController
    {
        // GET: Home
        // public ActionResult Index()
        public async Task<ActionResult> IndexAsync()
        {
            ViewBag.Message = "Congratulations!";
            // ViewBag.Message = CultureInfo.CurrentCulture.Name;

            var userRepo = DataAccess.Layer.GetRepository<Models.User>();
            // bool success = await repo.Add("56cd35159d2feb7ef4100e59", e => e.Prescriptions, "cowbell");

            ViewBag.UsersJSON = (await userRepo.GetAll()).ToJson();

            return View();
        }
    }
}
