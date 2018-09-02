using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WPFHostMvc.Controller
{
    [AllowAnonymous]
    [Route("api")]
    public class LoginController : Microsoft.AspNetCore.Mvc.Controller
    {
        [HttpGet]
        [Route("/myview")]
        public IActionResult MyView()
        {
            var v = View("/View/Index.cshtml");
            return v;
        }
    }
}
