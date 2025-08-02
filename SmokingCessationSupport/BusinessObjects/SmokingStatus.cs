﻿using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class SmokingStatus
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int CigarettesPerDay { get; set; }

    public string? HealthStatus { get; set; }

    public DateTime RecordDate { get; set; }

    public virtual User User { get; set; } = null!;
}
