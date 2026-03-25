using JobTracker.Data;
using JobTracker.Models;
using JobTracker.Services;
using Microsoft.EntityFrameworkCore;
using System.IO;

var dbPath = Path.Combine(AppContext.BaseDirectory, "jobtracker.db");
using var db = new JobTrackerContext(dbPath);

db.Database.Migrate();

var service = new ApplicationService(db);

while (true)
{
    Console.WriteLine("\n--- Job Tracker ---");
    Console.WriteLine("1. List Applications");
    Console.WriteLine("2. Add Application");
    Console.WriteLine("3. Update Status");
    Console.WriteLine("4. Add Note");
    Console.WriteLine("5. Add Contact");
    Console.WriteLine("0. Exit");
    Console.Write("Select an option: ");
    var input = Console.ReadLine();

    switch (input)
    {
        case "1":
            var apps = service.GetAllApplications();
            foreach (var app in apps)
            {
                Console.WriteLine($"{app.Id}: {app.Company} - {app.PositionTitle} ({app.Status})");
                if (app.Notes.Any())
                    Console.WriteLine($"  Notes: {string.Join("; ", app.Notes.Select(n => n.Content))}");
                if (app.Contacts.Any())
                    Console.WriteLine($"  Contacts: {string.Join(", ", app.Contacts.Select(c => c.Name))}");
            }
            break;

        case "2":
            Console.Write("Company: ");
            var company = Console.ReadLine()!;
            Console.Write("Position: ");
            var position = Console.ReadLine()!;
            Console.WriteLine("Status (0: Applied, 1: Interviewing, 2: Offer, 3: Rejected, 4: Withdrawn): ");
            var statusInput = int.Parse(Console.ReadLine()!);
            service.AddApplication(company, position, (ApplicationStatus)statusInput);
            Console.WriteLine("Application added.");
            break;

        case "3":
            Console.Write("Application ID: ");
            var id = int.Parse(Console.ReadLine()!);
            Console.WriteLine("New Status (0: Applied, 1: Interviewing, 2: Offer, 3: Rejected, 4: Withdrawn): ");
            var newStatus = int.Parse(Console.ReadLine()!);
            service.UpdateStatus(id, (ApplicationStatus)newStatus);
            Console.WriteLine("Status updated.");
            break;

        case "4":
            Console.Write("Application ID: ");
            var noteId = int.Parse(Console.ReadLine()!);
            Console.Write("Note content: ");
            var content = Console.ReadLine()!;
            service.AddNote(noteId, content);
            Console.WriteLine("Note added.");
            break;

        case "5":
            Console.Write("Application ID: ");
            var contactId = int.Parse(Console.ReadLine()!);
            Console.Write("Contact name: ");
            var name = Console.ReadLine()!;
            Console.Write("Role: ");
            var role = Console.ReadLine()!;
            Console.Write("Email (optional): ");
            var email = Console.ReadLine();
            service.AddContact(contactId, name, role, string.IsNullOrWhiteSpace(email) ? null : email);
            Console.WriteLine("Contact added.");
            break;

        case "0":
            return;

        default:
            Console.WriteLine("Invalid option.");
            break;
    }
}