namespace webserver.Models;
/// <summary>
/// Client users must be registered by a BZEmployee or Company
/// Clients can read, and only read, Companies and Real Estates
/// </summary>
public class Client : BZEAccount {

    public string Occupation { get; set; } = string.Empty;

}