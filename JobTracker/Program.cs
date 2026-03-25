using JobTracker.Data;
using JobTracker.Models;

using var db = new JobTrackerContext();

var app = new Application
{
    Company = "Twitch",
    PositionTitle = "Software Engineer",
    Status = ApplicationStatus.Applied,
    Interviewed = false
};

db.Applications.Add(app);
db.SaveChanges();

Console.WriteLine("Application saved!");