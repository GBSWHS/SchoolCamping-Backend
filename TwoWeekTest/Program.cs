using Microsoft.EntityFrameworkCore;
using TwoWeekTest.Database;

var db = new LocalDbContext();


var model = new Reserves() { Mates = "김성현1 윤서1준 임태1현" };
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
    Console.WriteLine("Period");
}