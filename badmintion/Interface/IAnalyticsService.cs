using badmintion.DTO;

namespace badmintion.Interface
{
    public interface IAnalyticsService
    {
        Task<dynamic> GetRevenueOverview(DateTime? fromDate, DateTime? toDate, DateTime? compareFromDate, DateTime? compareToDate);
        Task<dynamic> GetRevenueTrend(string? groupBy, DateTime? fromDate, DateTime? toDate);
        Task<dynamic> GetRevenueByCategory(DateTime? fromDate, DateTime? toDate);
        Task<dynamic> GetRevenueByPaymentMethod(DateTime? fromDate, DateTime? toDate);
        Task<dynamic> GetInventoryInsights(int lookbackDays, int forecastDays);
        Task<dynamic> GetStockAlerts(bool includeAll, int defaultThreshold);
        Task<dynamic> SetStockThreshold(StockThresholdRequest model);
    }
}
