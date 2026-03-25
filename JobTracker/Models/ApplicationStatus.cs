using System;
using System.Collections.Generic;
using System.Text;

namespace JobTracker.Models;

public enum ApplicationStatus
{
    Applied,
    Interviewing,
    Offer,
    Rejected,
    Withdrawn
}