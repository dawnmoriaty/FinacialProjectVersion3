using FinacialProjectVersion3.Utils;
using FinacialProjectVersion3.ViewModels.Dashboard;

namespace FinacialProjectVersion3.Services
{
    public interface IDashboardService
    {
        Task<ServiceResult<DashboardViewModel>> GetDashboardData(int userId);
    }
}
