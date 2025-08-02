using System;
using System.Collections.Generic;

namespace BusinessObjects;

public partial class QuitPlan
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime TargetDate { get; set; }

    public string MainGoal { get; set; } = null!;
    public int TargetCigarettesPerDay { get; set; }
    public virtual User User { get; set; } = null!;
}
