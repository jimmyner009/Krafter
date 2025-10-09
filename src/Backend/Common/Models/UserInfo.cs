namespace Backend.Common.Models;

public class UserInfo : IIdDto
{
    public string Id { get; set; }
    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    public DateTime CreatedOn { get; set; }

    ////Email
    public string Email { get; set; } = "iambipinpaul@outlook.com";

    ////permissions

    public List<string> Permissions { get; set; } = new List<string>();

    ////roles
    public List<string> Roles { get; set; } = new List<string>();

    ////tokenExpiryTime
    public DateTime TokenExpiryTime { get; set; }
}