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

        [HttpGet]
        [RequireAuth]
        [Route("reserves")]
        public async Task<IActionResult> GetReservesAsync()
        {
            var db = new LocalDbContext();
            var reserves = db.Reserves.OrderBy(x => x.ReservedAt)
                .Take(50);

            var data = from m in reserves
                select new { m.Id, Mates = m.Mates.Mask(), m.ReservedAt, m.Teacher };
            var response = new GeneralResponseModel
            {
                Data = data
            };
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
        [HttpPut]
        [RequireAuth]
        [Route("reserve")]
        public async Task<IActionResult> PutReserveAsync([FromQuery] int id, [FromBody]AdminReserveModel m)
        {
            var db = new LocalDbContext();

            var u = await db.Reserves.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            var response = new GeneralResponseModel();
            if (u == null)
            {
                response.Success = false;
                response.Message = "Unknown Reserve.";
                return new JsonResult(response);
            }

            if (u.ReservedAt == m.Date)
            {
                db.Reserves.Update(new Reserves() { Id = id, Mates = m.Mates, Passcode = u.Passcode, ReservedAt = m.Date, Teacher = m.Teacher });
                await db.SaveChangesAsync();

                response.Message = "Successfully modified reserve.";
                return new JsonResult(response);
            }

            bool exists = await db.Reserves.AnyAsync(x => x.ReservedAt == m.Date);

            if (exists)
            {
                response.Success = false;
                response.Message = "already reserved.";
                return new JsonResult(response);
            }

            db.Reserves.Update(new Reserves() { Id = id, Mates = m.Mates, Passcode = u.Passcode, ReservedAt = m.Date, Teacher = m.Teacher });
            await db.SaveChangesAsync();

            response.Message = "Successfully modified reserve.";

            return new JsonResult(response);
        }
    }
}