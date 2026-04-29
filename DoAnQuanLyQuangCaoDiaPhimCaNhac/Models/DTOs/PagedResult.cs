using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac.Models.DTOs
{
    public class PagedResult<T>
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public List<T> Items { get; set; }
    }
}