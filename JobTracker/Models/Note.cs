using System;
using System.Collections.Generic;
using System.Text;

namespace JobTracker.Models;

public class Note
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int ApplicationId { get; set; }
    public Application Application { get; set; } = null!;
}