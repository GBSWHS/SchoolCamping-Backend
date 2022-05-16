using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using SchoolCamping.Attributes;
using SchoolCamping.Database;
using SchoolCamping.Models.Requests;
using SchoolCamping.Models.Responses;

namespace SchoolCamping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost]
        [Route("admin")]
        public async Task<IActionResult> LoginAdminAsync([FromBody] LoginRequestModel key)
        {
            var response = new GeneralResponseModel();
            if (key.Password == Vault.AuthKey)
            {
                HttpContext.Session.SetString("admin", "true");
            }
            else
            {
                response.Success = false;
                response.Message = "Invalid Password.";
            }

            return new JsonResult(response);
        }

        [HttpDelete]
        [RequireAuth]
        [Route("reserve")]
        public async Task<IActionResult> DeleteReserveAsync([FromQuery] int id)
        {
            var db = new LocalDbContext();

            var u = await db.Reserves.SingleOrDefaultAsync(x => x.Id == id);
            var response = new GeneralResponseModel();
            if (u == null)
            {
                response.Success = false;
                response.Message = "Failed.";
                return new JsonResult(response);
            }

            db.Reserves.Remove(u);
            await db.SaveChangesAsync();

            response.Message = "Successfully deleted reserve.";

            return new JsonResult(response);
        }
    }
}