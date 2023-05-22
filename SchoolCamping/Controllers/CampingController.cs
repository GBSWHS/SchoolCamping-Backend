using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                select new {m.Id, Mates = m.Mates.Mask(), m.ReservedAt, Teacher = m.Teacher.Mask()};
            var response = new GeneralResponseModel
            {
                Data = data
            };
            return new JsonResult(response);
        }

        [HttpGet]
        [Route("reserve/{id}")]
        public async Task<IActionResult> GetReserveAsync(int id, [FromQuery] string passcode)
        {
            var db = new LocalDbContext();

            var u = await db.Reserves.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id && x.Passcode == passcode);
            var response = new GeneralResponseModel();
            if (u == null)
            {
                response.Success = false;
                response.Message = "Unknown Reserve.";
                return new JsonResult(response);
            }

            response.Data = u;
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
                    var date = DateOnly.FromDateTime(DateTime.Today).DayNumber - new DateOnly().AddDays(14).DayNumber;
                    var dateForward = DateOnly.FromDateTime(DateTime.Today).DayNumber + new DateOnly().AddDays(14).DayNumber;

                    var twoWeek = db.Reserves.OrderBy(x => x.ReservedAt).Where(x => x.ReservedAt.DayNumber > date && x.ReservedAt.DayNumber < dateForward).Select(x => x.Mates);
                    List<string> dup = new List<string>();
                    foreach (var reserves in twoWeek)
                    {
                        var Mates = reserves.Split();
                        
                        var var = from m in model.Mates.Split()
                            where Mates.Contains(m)
                            select m;

                        dup.AddRange(var);
                    }

                    
                    dup = dup.Distinct().ToList();

                    
                    if (dup.Any())
                    {
                        response.Success = false;
                        response.Message = "two-week period.";
                        response.Data = dup;
                        return new JsonResult(response);
                    }


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
