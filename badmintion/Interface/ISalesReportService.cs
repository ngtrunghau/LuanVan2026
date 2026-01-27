using badmintion.DTO;
using badmintion.Models;

namespace badmintion.Interface
{
    public interface ISalesReportService
    {
        Task<dynamic> Create(SalesReport model);
        Task<dynamic> Update(SalesReport model);
    }
}
