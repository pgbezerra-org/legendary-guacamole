namespace webserver.Models;

public class Client
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // Additional properties specific to Client
    public string PhoneNumber { get; set; } = string.Empty;
    public string Occupation { get; set; } = string.Empty;
}