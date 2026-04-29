using System.Linq;
using System.Web.Mvc;
using DoAnQuanLyQuangCaoDiaPhimCaNhac.Models;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Controllers
{
    public class CategoryController : Controller
    {
        QuanLyDiaPhimCaNhac_EditedEntities db = new QuanLyDiaPhimCaNhac_EditedEntities();

        public ActionResult Index()
        {
            var list = db.LoaiDias.ToList();
            return View(list);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(LoaiDia model)
        {
            if (ModelState.IsValid)
            {
                db.LoaiDias.Add(model);
                db.SaveChanges();
                TempData["Success"] = "Thêm loại đĩa thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var loai = db.LoaiDias.Find(id);
            if (loai == null) return HttpNotFound();

            return View(loai);
        }

        [HttpPost]
        public ActionResult Edit(LoaiDia model)
        {
            if (ModelState.IsValid)
            {
                var loai = db.LoaiDias.Find(model.MaLoaiDia);
                loai.TenLoaiDia = model.TenLoaiDia;

                db.SaveChanges();
                TempData["Success"] = "Cập nhật loại đĩa thành công!";
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var loai = db.LoaiDias.Find(id);
            return View(loai);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var loai = db.LoaiDias.Find(id);

            bool used = db.DiaPhimCaNhacs.Any(p => p.MaLoaiDia == id);
            if (used)
            {
                TempData["Error"] = "Không thể xóa vì thể loại đang được dùng!";
                return RedirectToAction("Index");
            }

            db.LoaiDias.Remove(loai);
            db.SaveChanges();

            TempData["Success"] = "Xóa loại đĩa thành công!";
            return RedirectToAction("Index");
        }
    }
}
