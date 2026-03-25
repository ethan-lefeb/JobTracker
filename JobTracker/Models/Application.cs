using System;
using System.Collections.Generic;
using System.Text;

namespace JobTracker.Models;

public class Application
{
    public int Id { get; set; }

    public string Company { get; set; } = "";
    public string PositionTitle { get; set; } = "";
    public ApplicationStatus Status { get; set; }

    public DateTime DateApplied { get; set; } = DateTime.UtcNow;
    public bool Interviewed { get; set; }

    public List<Note> Notes { get; set; } = new();
    public List<Contact> Contacts { get; set; } = new();
}