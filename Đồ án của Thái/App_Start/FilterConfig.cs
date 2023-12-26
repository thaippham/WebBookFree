using System.Web;
using System.Web.Mvc;

namespace Đồ_án_của_Thái
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
