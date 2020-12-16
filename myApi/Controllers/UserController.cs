using DataAccessLayer;
using DataAccessLayer.Custom;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace myApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {

        [HttpGet]
        [Route("GetUserData")]
        [Authorize(Policy = Policies.User)]
        public IActionResult GetUserData()
        {
            var data = _Data.BaseSQL.getDataTable(QUERIES.CATALOG_BRANDS);

            return Ok("This is a response from user method");
        }

        [HttpGet]
        [Route("GetAdminData")]
        [Authorize(Policy = Policies.Admin)]
        public IActionResult GetAdminData()
        {
            var data = _Data.BaseSQL.getDataTable("select * from CatalogBrands");

            return Ok("This is a response from Admin method");
        }



    }
}
