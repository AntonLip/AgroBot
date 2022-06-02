using AgroBot.Models.ModelsDB;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgroBot.Models.Interfaces.IService
{
    public interface IReportService
    {
        Task<string> GetReportByLogistAsync(ApplicationUser logist, IList<Route> route);
        Task<string> GetReportByDriverAsync(ApplicationUser driver, IList<Route> route);
        Task<string> GetReportAllRoutes(IList<Route> route);
        void RemoveReportFile(string filename);
    }
}
