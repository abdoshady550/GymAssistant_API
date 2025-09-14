namespace GymAssistant_API.Model.Identity;


public class JWT
{
    public string Key { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public double ExpiresInMin { get; set; }

}

