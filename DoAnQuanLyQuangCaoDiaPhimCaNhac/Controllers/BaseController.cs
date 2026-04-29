using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        public ActionResult Index()
        {
            return View();
        }
        protected JsonResult JsonNet(object data)
        {
            return new DoAnQuanLyQuangCaoDiaPhimCaNhac.Helpers.NewtonsoftJsonResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
    }
}