using DotNETModernAPI.Domain.Models;

namespace DotNETModernAPI.Domain.Providers;

public interface IEmailProvider
{
    Task SendAsync(EmailRequestModel emailRequest);
}
