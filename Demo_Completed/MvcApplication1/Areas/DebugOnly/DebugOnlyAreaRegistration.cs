using System.Web.Mvc;

namespace MvcApplication1.Areas.Admin 
{
    public class DebugOnlyAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "DebugOnly";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "DebugOnly_default",
                "DebugOnly/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional } );
        }
    }
}
