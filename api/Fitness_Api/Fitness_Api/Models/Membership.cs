namespace Fitness_Api.Models;

public class Membership
{
    public int Id { get; set; }
    public int ClientId { get; set; }
    public string Type { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public MembershipStatus Status { get; set; } = MembershipStatus.Active;
    public bool ReminderSent { get; set; }
}
