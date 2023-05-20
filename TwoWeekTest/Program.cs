using Microsoft.EntityFrameworkCore;
using TwoWeekTest.Database;

var db = new LocalDbContext();


var model = new Reserves() { Mates = "김성현 윤서준 임태현" };
var date = DateOnly.FromDateTime(DateTime.Today).DayNumber - new DateOnly().AddDays(14).DayNumber;
var dateForward = DateOnly.FromDateTime(DateTime.Today).DayNumber + new DateOnly().AddDays(14).DayNumber;

var twoWeek = db.Reserves.OrderBy(x=> x.ReservedAt).Where(x => x.ReservedAt.DayNumber > date && x.ReservedAt.DayNumber < dateForward).Select(x=> x.Mates);
foreach (var reserves in twoWeek)
{
    var Mates = reserves.Split();
    var match = from m in model.Mates.Split()
        where Mates.Contains(m)
        select (m, true);

    foreach ((string m, bool) valueTuple in match)
    {
        Console.WriteLine($"{valueTuple.m} {valueTuple.Item2}");
    }
}
