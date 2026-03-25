using JobTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Data;

public class JobTrackerContext : DbContext
{
    private readonly string _dbPath;

    public JobTrackerContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<Contact> Contacts => Set<Contact>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_dbPath}");
}