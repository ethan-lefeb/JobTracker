using JobTracker.Data;
using JobTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Services;

public class ApplicationService
{
    private readonly JobTrackerContext _db;

    public ApplicationService(JobTrackerContext db)
    {
        _db = db;
    }

    public List<Application> GetAllApplications()
    {
        return _db.Applications
            .Include(a => a.Notes)
            .Include(a => a.Contacts)
            .OrderBy(a => a.DateApplied)
            .ToList();
    }

    public Application AddApplication(string company, string position, ApplicationStatus status)
    {
        var app = new Application
        {
            Company = company,
            PositionTitle = position,
            Status = status
        };
        _db.Applications.Add(app);
        _db.SaveChanges();
        return app;
    }

    public void UpdateStatus(int appId, ApplicationStatus status)
    {
        var app = _db.Applications.Find(appId);
        if (app != null)
        {
            app.Status = status;
            _db.SaveChanges();
        }
    }

    public void AddNote(int appId, string content)
    {
        var note = new Note
        {
            ApplicationId = appId,
            Content = content
        };
        _db.Notes.Add(note);
        _db.SaveChanges();
    }

    public void AddContact(int appId, string name, string role, string? email = null)
    {
        var contact = new Contact
        {
            ApplicationId = appId,
            Name = name,
            Role = role,
            Email = email
        };
        _db.Contacts.Add(contact);
        _db.SaveChanges();
    }
}