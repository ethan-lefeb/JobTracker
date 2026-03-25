using Microsoft.EntityFrameworkCore;
using JobTracker.Models;

namespace JobTracker.Data;

public class JobTrackerContext : DbContext
{
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=jobtracker.db");
}