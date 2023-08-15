using System.Threading.Tasks;

namespace Boutique.Services;

public interface IEmailSender
{
    Task SendEmailAsync(string to, string subject, string body, string from = null);
    Task SendMessageAsync(string name, string from, string subject, string body);
}