using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Unilever.v1.Common.SearchService
{
    public interface ISearchService
    {
        Task<List<object>> SearchUser(string searchTerm);
        Task<List<object>> SearchArea(string searchTerm);
        Task<List<object>> SearchNotification(string searchTerm);
        Task<List<object>> SearchCMS(string searchTerm);
    }
}