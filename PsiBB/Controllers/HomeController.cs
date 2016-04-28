using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
// using System.Reflection;

namespace PsiBB.Controllers
{
    // public class HomeController : Controller
    public class HomeController : AsyncController
    {
        // GET: Home
        public async Task<ActionResult> IndexAsync()
        {
            ViewBag.Message = "Congratulations!";
            // ViewBag.Message = CultureInfo.CurrentCulture.Name;
            
            var userRepo = DataAccess.Layer.GetRepository<Models.User>();
            
            ViewBag.UsersJSON = (await userRepo.GetAll()).ToJson(new JsonWriterSettings { Indent = true });
            // ViewBag.UsersJSON = anonyTest(new { oneFish = "red", twoFish = "blue" });
            
            return View();
        }
    }
}
