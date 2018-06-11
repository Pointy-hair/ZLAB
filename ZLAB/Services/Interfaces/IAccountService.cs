using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Zlab.Web.Main.Model.Dtos;
using Zlab.Web.Main.Model.Models;

namespace Zlab.Web.Main.Service.Interfaces
{
    public interface IAccountService
    {
        Task<SignupDTO> SignUpAsync(SignupModel modl);
        Task<bool> SendEmailAsync(string email);
    }
}
