using System.Linq;
using System.Web.Http;
using DoAnQuanLyQuangCaoDiaPhimCaNhac.Models;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Controllers.API
{
    [RoutePrefix("api/category")]
    public class CategoryApiController : ApiController
    {
        QuanLyDiaPhimCaNhac_EditedEntities db = new QuanLyDiaPhimCaNhac_EditedEntities();

        // GET api/category
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            var list = db.LoaiDias.Select(x => new
            {
                x.MaLoaiDia,
                x.TenLoaiDia
            });

            return Ok(list);
        }

        // GET api/category/5
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var loai = db.LoaiDias.Find(id);
            if (loai == null) return NotFound();

            return Ok(loai);
        }

        // POST api/category
        [HttpPost]
        [Route("")]
        public IHttpActionResult Add(LoaiDia model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            db.LoaiDias.Add(model);
            db.SaveChanges();

            return Ok(new { message = "Thêm thể loại thành công!" });
        }

        // PUT api/category/5
        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Update(int id, LoaiDia model)
        {
            var loai = db.LoaiDias.Find(id);
            if (loai == null) return NotFound();

            loai.TenLoaiDia = model.TenLoaiDia;
            db.SaveChanges();

            return Ok(new { message = "Cập nhật thể loại thành công!" });
        }

        // DELETE api/category/5
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            var loai = db.LoaiDias.Find(id);
            if (loai == null) return NotFound();

            bool hasProduct = db.DiaPhimCaNhacs.Any(p => p.MaLoaiDia == id);
            if (hasProduct)
                return BadRequest("Không thể xóa: Thể loại đang được sử dụng trong sản phẩm!");

            db.LoaiDias.Remove(loai);
            db.SaveChanges();

            return Ok(new { message = "Xóa thể loại thành công!" });
        }
    }
}
