using System;
using System.Collections.Generic;
using System.Text;

namespace JobTracker.Models;

public class Contact
{
    public int Id { get; set; }

    public string Name { get; set; } = "";
    public string Role { get; set; } = "";
    public string? Email { get; set; }

    public int ApplicationId { get; set; }
    public Application Application { get; set; } = null!;
}