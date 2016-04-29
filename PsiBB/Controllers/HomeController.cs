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
            // temporarily create Users from GET parameters
            var name = Request.QueryString["name"];
            var email = Request.QueryString["email"];

            if (name != null && email != null)
            {
                await (new Models.User { DisplayName = name, Email = email }).CreateAsync();
                ViewBag.CreatedUserMessage = "Created user with Name " + name.ToString() + " and Email " + email.ToString();
            }

            ViewBag.Message = "Congratulations!";
            // ViewBag.Message = CultureInfo.CurrentCulture.Name;
            
            ViewBag.UsersJSON = (await Models.User.GetAllAsync()).ToJson(new JsonWriterSettings { Indent = true });
            return View();
        }
    }
}
