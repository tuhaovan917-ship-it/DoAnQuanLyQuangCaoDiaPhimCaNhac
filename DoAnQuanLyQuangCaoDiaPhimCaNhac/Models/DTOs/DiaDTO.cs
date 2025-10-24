using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Models.DTOs
{
    public class DiaDTO
    {
        public int MaDia { get; set; }
        public string TenDia { get; set; }
        public decimal GiaBan { get; set; }
        public string AnhBia { get; set; }
        public int SoLuongTon { get; set; }
        public string TenLoaiDia { get; set; }
        public bool LaSanPhamNoiBat { get; set; }
    }
}