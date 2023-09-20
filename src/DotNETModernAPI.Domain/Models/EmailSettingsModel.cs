namespace DotNETModernAPI.Domain.Models;

public class EmailSettingsModel
{
    public string UserName { get; set; }
    public string From { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
}
