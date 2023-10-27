namespace webserver.Models;
/// <summary>
/// BZEmployee user class. Such users can CRUD anything in the database
/// </summary>
public class BZEmployee : BZEAccount {

    public float salary { get; set; }

}