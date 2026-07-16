namespace badmintion.DTO
{
    public class AssistantAskRequest
    {
        public string Message { get; set; } = string.Empty;
        public int MaxResults { get; set; } = 5;
        public List<AssistantConversationMessage> History { get; set; } = new();
        public List<int> LastSuggestedProductIds { get; set; } = new();
    }

    public class AssistantConversationMessage
    {
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class AssistantProductSuggestion
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int SoldQuantity { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? ImageUrl { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class AssistantAskResponse
    {
        public string Message { get; set; } = string.Empty;
        public string Intent { get; set; } = string.Empty;
        public List<string> QuickReplies { get; set; } = new();
        public List<AssistantProductSuggestion> Suggestions { get; set; } = new();
    }

    public class RevenuePointDto
    {
        public string Label { get; set; } = string.Empty;
        public decimal Value { get; set; }
    }

    public class RevenueOverviewDto
    {
        public decimal TodayRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal YearRevenue { get; set; }

        public decimal RangeRevenue { get; set; }
        public decimal CompareRangeRevenue { get; set; }
        public decimal RangeChangeAmount { get; set; }
        public decimal RangeChangePercent { get; set; }

        public decimal CurrentMonthRevenue { get; set; }
        public decimal PreviousMonthRevenue { get; set; }
        public decimal MonthOverMonthAmount { get; set; }
        public decimal MonthOverMonthPercent { get; set; }
        public int CompletedOrderCount { get; set; }
        public int UnitsSold { get; set; }
        public decimal AverageOrderValue { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public DateTime? CompareFromDate { get; set; }
        public DateTime? CompareToDate { get; set; }
    }

    public class RevenueTrendResponseDto
    {
        public string GroupBy { get; set; } = "day";
        public List<RevenuePointDto> Points { get; set; } = new();
        public decimal Total { get; set; }
    }

    public class InventoryInsightItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public int CurrentStock { get; set; }
        public int SoldLookback { get; set; }
        public int ImportedLookback { get; set; }
        public decimal AvgDailySales { get; set; }
        public decimal StockCoverDays { get; set; }
        public decimal ForecastNeed { get; set; }
        public decimal SellThroughRate { get; set; }
        public string RiskLevel { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class InventoryInsightsResponseDto
    {
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }
        public int OverstockCount { get; set; }
        public int HealthyCount { get; set; }

        public List<InventoryInsightItemDto> LowStockProducts { get; set; } = new();
        public List<InventoryInsightItemDto> OverstockProducts { get; set; } = new();
        public List<RevenuePointDto> StockStatusChart { get; set; } = new();
        public List<RevenuePointDto> TopDemandChart { get; set; } = new();
    }

    public class StockAlertItemDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int CurrentStock { get; set; }
        public int AlertThreshold { get; set; }
        public string Severity { get; set; } = string.Empty;
        public bool IsAlert { get; set; }
    }

    public class StockAlertResponseDto
    {
        public int AlertCount { get; set; }
        public int OutOfStockCount { get; set; }
        public List<StockAlertItemDto> Items { get; set; } = new();
    }

    public class StockThresholdRequest
    {
        public int ProductId { get; set; }
        public int AlertThreshold { get; set; }
    }
}
