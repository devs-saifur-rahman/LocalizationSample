using LocalizationProcess.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LocalizationProcess.Controllers
{
    public class TestController : Controller
    {
        private readonly IStringLocalizer<SharedResource> _sharedResourceLocalizer;

        public TestController(IStringLocalizer<SharedResource> stringLocalizer)
        {

            _sharedResourceLocalizer = stringLocalizer; 
        }

     //   [HttpGet("GetValue")]
        public IActionResult Index()
        {
            var hello = _sharedResourceLocalizer.GetString("Hello").Value ?? "";
            var world = _sharedResourceLocalizer["World"];

            return Ok(world);
        }
    }
}
