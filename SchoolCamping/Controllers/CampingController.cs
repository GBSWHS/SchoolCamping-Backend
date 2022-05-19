using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolCamping.Attributes;
using SchoolCamping.Database;
using SchoolCamping.Models.Requests;
using SchoolCamping.Models.Responses;

namespace SchoolCamping.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampingController : ControllerBase
    {
        [HttpGet]
        [Route("reserves")]
        public async Task<IActionResult> GetReservesAsync()
        {
            var db = new LocalDbContext();
            var reserves = db.Reserves.Where(x=> x.ReservedAt >= DateOnly.FromDateTime(DateTime.Now)).OrderBy(x => x.ReservedAt)
                .Take(30);

            var data = from m in reserves
                select new {m.Id, Mates = m.Mates.Mask(), m.ReservedAt, m.Teacher};
            var response = new GeneralResponseModel
            {
                Data = data
            };
            return new JsonResult(response);
        }

        [HttpPost]
        [RequireCaptcha]
        [Route("reserve")]
        public async Task<IActionResult> PostReserveAsync([FromBody] ReserveRequestModel model)
        {
            var db = new LocalDbContext();

            bool exists = await db.Reserves.AnyAsync(x => x.ReservedAt == model.Date);
            var response = new GeneralResponseModel();
            if (exists)
            {
                response.Success = false;
                response.Message = "already reserved.";
            }
            else
            {
                try
                {
                    var reserveModel = new Reserves
                        {Mates = model.Mates, ReservedAt = model.Date, Passcode = model.PassCode, Teacher = model.Teacher};
                    await db.Reserves.AddAsync(reserveModel);
                    await db.SaveChangesAsync();
                    response.Data = reserveModel;
                    response.Message = "successfully reserved.";
                }
                catch (Exception)
                {

                }
            }

            return new JsonResult(response);
        }

        [HttpDelete]
        [RequireCaptcha]
        [Route("reserve")]
        public async Task<IActionResult> DeleteReserveAsync([FromQuery] ReserveDeleteRequestModel m)
        {
            var db = new LocalDbContext();

            var u = await db.Reserves.SingleOrDefaultAsync(x => x.Id == m.Id && x.Passcode == m.PassCode);
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
        [Route("reserve")]
        public async Task<IActionResult> PutReserveAsync([FromBody] ReserveRequestModel m, [FromQuery]int id)
        {
            var db = new LocalDbContext();

            var u = await db.Reserves.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.Passcode == m.PassCode);
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

            db.Reserves.Update(new Reserves(){Id = id, Mates = m.Mates, Passcode = u.Passcode, ReservedAt = m.Date, Teacher = m.Teacher});
            await db.SaveChangesAsync();

            response.Message = "Successfully modified reserve.";

            return new JsonResult(response);
        }
    }
}
