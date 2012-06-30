using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using BizArk.Web.MVC;

namespace WebHelperTestSite.Controllers
{
    public class TestController : BizArkController
    {

        public ActionResult UploadValues(int intVal = int.MinValue, string strVal = null)
        {
            var file = Request.Files["file"];
            if (file == null)
                return Content(string.Format("{0} {1}", intVal, strVal));

            var sr = new StreamReader(file.InputStream);
            var contents = sr.ReadToEnd();
            return Content(string.Format("{0} {1} [{2}]", intVal, strVal, contents));
        }

        public ActionResult Puppy()
        {
            return File(Server.MapPath("/puppy.jpg"));
        }

    }
}
