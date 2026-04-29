using System.Web.Http;

namespace DoAnQuanLyQuangCaoDiaPhimCaNhac
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Cấu hình route chuẩn cho Web API
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Cho phép JSON format
            config.Formatters.JsonFormatter.SerializerSettings.Formatting =
                Newtonsoft.Json.Formatting.Indented;
        }
    }
}
