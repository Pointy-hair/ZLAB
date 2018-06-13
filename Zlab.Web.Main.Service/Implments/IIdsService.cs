using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zlab.Web.Service.Implments
{
    public interface IIdsService
    {
        Task<int> GetIdAsync(string key);
    }
}
