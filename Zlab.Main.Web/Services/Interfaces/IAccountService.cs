using System.Threading.Tasks;
using Zlab.Main.Web.Models;

namespace Zlab.Main.Web.Service.Interfaces
{
    public interface IAccountService
    {
        Task<string> SignUpAsync(SignupModel model);
        Task<string> SigninAsync(SigninModel model);
        Task<bool> SendEmailAsync(string email);
    }
}
