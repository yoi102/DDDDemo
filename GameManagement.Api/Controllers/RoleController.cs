using GameManagement.Api.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Authorize(Roles = "Administrator")]
    public class RoleController : ControllerBase
    {
        private readonly UserManager<ApiIdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public RoleController(
            UserManager<ApiIdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpPost]
        public async Task<IActionResult> AddRole()
        {


            return Ok();
        }
        public async Task<IActionResult> EditRole(Guid id)
        {


            return NoContent();
        }
        public async Task<IActionResult> DeleteRole(string id)
        {

            return NoContent();
        }













    }
}
