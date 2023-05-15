using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Common
{
    public interface IGetAllUsers
    {
        Task<List<object>> SearchUser();
    }
}