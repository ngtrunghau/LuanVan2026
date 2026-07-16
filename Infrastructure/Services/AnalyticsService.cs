using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.EntityFrameworkCore;

namespace badmintion.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly BadmintionNlContext _context;

        public AnalyticsService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }

        public async Task<dynamic> GetRevenueOverview(DateTime? fromDate, DateTime? toDate, DateTime? compareFromDate, DateTime? compareToDate)
        {
            try
            {
                var today = DateTime.Today;
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var yearStart = new DateTime(today.Year, 1, 1);

                var (rangeFrom, rangeTo) = NormalizeDateRange(fromDate, toDate, 30);
                DateTime compareFrom;
                DateTime compareTo;

                if (compareFromDate.HasValue && compareToDate.HasValue)
                {
                    compareFrom = compareFromDate.Value.Date;
                    compareTo = compareToDate.Value.Date;
                    if (compareFrom > compareTo)
                    {
                        (compareFrom, compareTo) = (compareTo, compareFrom);
                    }
                }
                else
                {
                    var rangeDays = (rangeTo - rangeFrom).Days + 1;
                    compareTo = rangeFrom.AddDays(-1);
                    compareFrom = compareTo.AddDays(-(rangeDays - 1));
                }

                var todayRevenue = await SumRevenueAsync(today, today);
                var monthRevenue = await SumRevenueAsync(monthStart, today);
                var yearRevenue = await SumRevenueAsync(yearStart, today);

                var rangeRevenue = await SumRevenueAsync(rangeFrom, rangeTo);
                var compareRangeRevenue = await SumRevenueAsync(compareFrom, compareTo);
                var rangeChangeAmount = rangeRevenue - compareRangeRevenue;
                var rangeChangePercent = CalculatePercentChange(rangeRevenue, compareRangeRevenue);
                var rangeToExclusive = rangeTo.AddDays(1);
                var deliveredRange = GetDeliveredOrdersQuery(rangeFrom, rangeToExclusive);
                var completedOrderCount = await deliveredRange.CountAsync();
                var unitsSold = await (
                    from oi in _context.OrderItems.AsNoTracking()
                    where oi.IsDeleted == false && oi.Orders.HasValue
                    join o in deliveredRange on oi.Orders.Value equals o.Id
                    select oi.Quantity ?? 0
                ).SumAsync();
                var averageOrderValue = completedOrderCount > 0
                    ? rangeRevenue / completedOrderCount
                    : 0m;

                var currentMonthRevenue = await SumRevenueAsync(monthStart, today);
                var currentMonthSpanDays = (today - monthStart).Days;
                var previousMonthStart = monthStart.AddMonths(-1);
                var previousMonthLastDay = new DateTime(previousMonthStart.Year, previousMonthStart.Month, DateTime.DaysInMonth(previousMonthStart.Year, previousMonthStart.Month));
                var previousMonthComparableEnd = previousMonthStart.AddDays(currentMonthSpanDays);
                if (previousMonthComparableEnd > previousMonthLastDay)
                {
                    previousMonthComparableEnd = previousMonthLastDay;
                }

                var previousMonthRevenue = await SumRevenueAsync(previousMonthStart, previousMonthComparableEnd);
                var monthOverMonthAmount = currentMonthRevenue - previousMonthRevenue;
                var monthOverMonthPercent = CalculatePercentChange(currentMonthRevenue, previousMonthRevenue);

                return new RevenueOverviewDto
                {
                    TodayRevenue = Round(todayRevenue),
                    MonthRevenue = Round(monthRevenue),
                    YearRevenue = Round(yearRevenue),
                    RangeRevenue = Round(rangeRevenue),
                    CompareRangeRevenue = Round(compareRangeRevenue),
                    RangeChangeAmount = Round(rangeChangeAmount),
                    RangeChangePercent = Round(rangeChangePercent),
                    CurrentMonthRevenue = Round(currentMonthRevenue),
                    PreviousMonthRevenue = Round(previousMonthRevenue),
                    MonthOverMonthAmount = Round(monthOverMonthAmount),
                    MonthOverMonthPercent = Round(monthOverMonthPercent),
                    CompletedOrderCount = completedOrderCount,
                    UnitsSold = unitsSold,
                    AverageOrderValue = Round(averageOrderValue),
                    FromDate = rangeFrom,
                    ToDate = rangeTo,
                    CompareFromDate = compareFrom,
                    CompareToDate = compareTo
                };
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException().WithCode(DefaultCode.EXCEPTION).WithMessage(e.ResultString).WithDetail(e.Error);
            }
            catch (Exception e)
            {
                throw ExceptionError.Exception(e);
            }
        }

        public async Task<dynamic> GetRevenueTrend(string? groupBy, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var normalizedGroupBy = (groupBy ?? "day").Trim().ToLowerInvariant();
                if (normalizedGroupBy != "day" && normalizedGroupBy != "month" && normalizedGroupBy != "year")
                {
                    normalizedGroupBy = "day";
                }

                var (rangeFrom, rangeTo) = NormalizeDateRange(fromDate, toDate, 30);
                var toExclusive = rangeTo.AddDays(1);
                var deliveredOrders = GetDeliveredOrdersQuery(rangeFrom, toExclusive);

                var points = new List<RevenuePointDto>();

                if (normalizedGroupBy == "year")
                {
                    var yearlyData = await deliveredOrders
                        .GroupBy(o => new
                        {
                            Y = o.ShippingDetails
                                .Where(sd => sd.IsDeleted == false && sd.Status == 3 && sd.DateShip.HasValue)
                                .Max(sd => sd.DateShip!.Value).Year
                        })
                        .Select(g => new
                        {
                            g.Key.Y,
                            Value = g.Sum(x => x.TotalAmount ?? 0m)
                        })
                        .OrderBy(x => x.Y)
                        .ToListAsync();

                    points = yearlyData
                        .Select(x => new RevenuePointDto
                        {
                            Label = x.Y.ToString(),
                            Value = Round(x.Value)
                        })
                        .ToList();
                    points = FillMissingRevenuePoints(points, rangeFrom, rangeTo, "year");
                }
                else if (normalizedGroupBy == "month")
                {
                    var monthlyData = await deliveredOrders
                        .GroupBy(o => new
                        {
                            Y = o.ShippingDetails
                                .Where(sd => sd.IsDeleted == false && sd.Status == 3 && sd.DateShip.HasValue)
                                .Max(sd => sd.DateShip!.Value).Year,
                            M = o.ShippingDetails
                                .Where(sd => sd.IsDeleted == false && sd.Status == 3 && sd.DateShip.HasValue)
                                .Max(sd => sd.DateShip!.Value).Month
                        })
                        .Select(g => new
                        {
                            g.Key.Y,
                            g.Key.M,
                            Value = g.Sum(x => x.TotalAmount ?? 0m)
                        })
                        .OrderBy(x => x.Y)
                        .ThenBy(x => x.M)
                        .ToListAsync();

                    points = monthlyData
                        .Select(x => new RevenuePointDto
                        {
                            Label = $"{x.M:00}/{x.Y}",
                            Value = Round(x.Value)
                        })
                        .ToList();
                    points = FillMissingRevenuePoints(points, rangeFrom, rangeTo, "month");
                }
                else
                {
                    var dailyData = await deliveredOrders
                        .GroupBy(o => new
                        {
                            Y = o.ShippingDetails
                                .Where(sd => sd.IsDeleted == false && sd.Status == 3 && sd.DateShip.HasValue)
                                .Max(sd => sd.DateShip!.Value).Year,
                            M = o.ShippingDetails
                                .Where(sd => sd.IsDeleted == false && sd.Status == 3 && sd.DateShip.HasValue)
                                .Max(sd => sd.DateShip!.Value).Month,
                            D = o.ShippingDetails
                                .Where(sd => sd.IsDeleted == false && sd.Status == 3 && sd.DateShip.HasValue)
                                .Max(sd => sd.DateShip!.Value).Day
                        })
                        .Select(g => new
                        {
                            g.Key.Y,
                            g.Key.M,
                            g.Key.D,
                            Value = g.Sum(x => x.TotalAmount ?? 0m)
                        })
                        .OrderBy(x => x.Y)
                        .ThenBy(x => x.M)
                        .ThenBy(x => x.D)
                        .ToListAsync();

                    points = dailyData
                        .Select(x => new RevenuePointDto
                        {
                            Label = $"{x.D:00}/{x.M:00}/{x.Y}",
                            Value = Round(x.Value)
                        })
                        .ToList();
                    points = FillMissingRevenuePoints(points, rangeFrom, rangeTo, "day");
                }

                return new RevenueTrendResponseDto
                {
                    GroupBy = normalizedGroupBy,
                    Points = points,
                    Total = Round(points.Sum(x => x.Value))
                };
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException().WithCode(DefaultCode.EXCEPTION).WithMessage(e.ResultString).WithDetail(e.Error);
            }
            catch (Exception e)
            {
                throw ExceptionError.Exception(e);
            }
        }

        public async Task<dynamic> GetRevenueByCategory(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var (rangeFrom, rangeTo) = NormalizeDateRange(fromDate, toDate, 30);
                var toExclusive = rangeTo.AddDays(1);
                var deliveredOrders = GetDeliveredOrdersQuery(rangeFrom, toExclusive);

                var rawRevenueByCategory = await (
                    from oi in _context.OrderItems.AsNoTracking()
                    where oi.IsDeleted == false && oi.Orders.HasValue && oi.ProductsId.HasValue
                    join o in deliveredOrders on oi.Orders.Value equals o.Id
                    join p in _context.Products.AsNoTracking() on oi.ProductsId.Value equals p.Id
                    join c in _context.Categories.AsNoTracking() on p.CategoriesId equals c.Id into categoryJoin
                    from category in categoryJoin.DefaultIfEmpty()
                    group oi by (category != null ? category.Name : "Chua phan loai") into g
                    select new
                    {
                        Label = g.Key,
                        Value = g.Sum(x => (x.Price ?? 0m) * (x.Quantity ?? 0))
                    }
                )
                .OrderByDescending(x => x.Value)
                .ToListAsync();

                // Round là method C# tùy chỉnh nên EF Core không thể dịch sang SQL.
                // Chỉ làm tròn sau khi GroupBy/Sum đã được database thực hiện.
                var revenueByCategory = rawRevenueByCategory
                    .Select(x => new RevenuePointDto
                    {
                        Label = x.Label,
                        Value = Round(x.Value)
                    })
                    .ToList();

                var orderRevenue = await deliveredOrders.SumAsync(x => x.TotalAmount ?? 0m);
                var itemRevenue = revenueByCategory.Sum(x => x.Value);
                var shippingAndAdjustment = Round(orderRevenue - itemRevenue);
                if (shippingAndAdjustment != 0m)
                {
                    revenueByCategory.Add(new RevenuePointDto
                    {
                        Label = "Phí vận chuyển/điều chỉnh",
                        Value = shippingAndAdjustment
                    });
                }

                return revenueByCategory;
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException().WithCode(DefaultCode.EXCEPTION).WithMessage(e.ResultString).WithDetail(e.Error);
            }
            catch (Exception e)
            {
                throw ExceptionError.Exception(e);
            }
        }

        public async Task<dynamic> GetRevenueByPaymentMethod(DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                var (rangeFrom, rangeTo) = NormalizeDateRange(fromDate, toDate, 30);
                var deliveredOrders = await GetDeliveredOrdersQuery(rangeFrom, rangeTo.AddDays(1))
                    .Select(x => new
                    {
                        x.Id,
                        Amount = x.TotalAmount ?? 0m
                    })
                    .ToListAsync();
                var orderIds = deliveredOrders.Select(x => x.Id).ToList();
                var paymentMethods = await _context.Payments.AsNoTracking()
                    .Where(x => x.IsDeleted == false && x.OrdersId.HasValue && orderIds.Contains(x.OrdersId.Value))
                    .GroupBy(x => x.OrdersId!.Value)
                    .Select(x => new
                    {
                        OrderId = x.Key,
                        Method = x.OrderByDescending(p => p.Id).Select(p => p.PaymentMethod).FirstOrDefault()
                    })
                    .ToDictionaryAsync(x => x.OrderId, x => x.Method);

                return deliveredOrders
                    .GroupBy(x => NormalizePaymentMethod(paymentMethods.GetValueOrDefault(x.Id)))
                    .Select(x => new RevenuePointDto
                    {
                        Label = x.Key,
                        Value = Round(x.Sum(i => i.Amount))
                    })
                    .OrderByDescending(x => x.Value)
                    .ToList();
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException().WithCode(DefaultCode.EXCEPTION).WithMessage(e.ResultString).WithDetail(e.Error);
            }
            catch (Exception e)
            {
                throw ExceptionError.Exception(e);
            }
        }

        public async Task<dynamic> GetInventoryInsights(int lookbackDays, int forecastDays)
        {
            try
            {
                var safeLookbackDays = Math.Clamp(lookbackDays <= 0 ? 30 : lookbackDays, 7, 365);
                var safeForecastDays = Math.Clamp(forecastDays <= 0 ? 14 : forecastDays, 3, 90);

                var toDate = DateTime.Today;
                var fromDate = toDate.AddDays(-(safeLookbackDays - 1));
                var toExclusive = toDate.AddDays(1);

                var deliveredOrdersLookback = GetDeliveredOrdersQuery(fromDate, toExclusive);

                var soldMap = await (
                    from oi in _context.OrderItems.AsNoTracking()
                    where oi.IsDeleted == false && oi.Orders.HasValue && oi.ProductsId.HasValue
                    join o in deliveredOrdersLookback on oi.Orders.Value equals o.Id
                    group oi by oi.ProductsId.Value into g
                    select new
                    {
                        ProductId = g.Key,
                        Quantity = g.Sum(x => x.Quantity ?? 0)
                    }
                ).ToDictionaryAsync(x => x.ProductId, x => x.Quantity);

                var importedMap = await _context.HistoryImports.AsNoTracking()
                    .Where(x => x.IsDeleted == false
                                && x.ProductId.HasValue
                                && x.DateImport.HasValue
                                && x.DateImport.Value >= fromDate
                                && x.DateImport.Value < toExclusive)
                    .GroupBy(x => x.ProductId!.Value)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Quantity = g.Sum(x => x.QuantityImport ?? 0)
                    })
                    .ToDictionaryAsync(x => x.ProductId, x => x.Quantity);

                var warehouseStockMap = await _context.WareHouses.AsNoTracking()
                    .Where(x => x.IsDeleted == false && x.ProductId.HasValue)
                    .GroupBy(x => x.ProductId!.Value)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Stock = g.Sum(x => x.RemainQuantity ?? 0)
                    })
                    .ToDictionaryAsync(x => x.ProductId, x => x.Stock);

                var thresholdMap = await _context.InventoryAlerts.AsNoTracking()
                    .Where(x => x.IsDeleted == false && x.ProductsId.HasValue)
                    .GroupBy(x => x.ProductsId!.Value)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        Threshold = g.OrderByDescending(x => x.Id).Select(x => x.AlertThreshold ?? 0).FirstOrDefault()
                    })
                    .ToDictionaryAsync(x => x.ProductId, x => x.Threshold);

                var products = await (
                    from p in _context.Products.AsNoTracking()
                    join c in _context.Categories.AsNoTracking() on p.CategoriesId equals c.Id into categoryJoin
                    from category in categoryJoin.DefaultIfEmpty()
                    where p.IsDeleted == false
                    select new
                    {
                        p.Id,
                        p.Name,
                        p.StockQuantity,
                        p.ImageUrl,
                        CategoryName = category != null ? category.Name : "Chua phan loai"
                    }
                ).ToListAsync();

                var lowStockProducts = new List<InventoryInsightItemDto>();
                var overstockProducts = new List<InventoryInsightItemDto>();

                foreach (var product in products)
                {
                    var soldQuantity = soldMap.TryGetValue(product.Id, out var sold) ? sold : 0;
                    var importedQuantity = importedMap.TryGetValue(product.Id, out var imported) ? imported : 0;
                    var fallbackStock = product.StockQuantity ?? 0;
                    var currentStock = warehouseStockMap.TryGetValue(product.Id, out var stockByWarehouse) ? stockByWarehouse : fallbackStock;
                    if (currentStock < 0)
                    {
                        currentStock = 0;
                    }

                    var avgDailySales = soldQuantity > 0 ? (decimal)soldQuantity / safeLookbackDays : 0m;
                    var forecastNeed = Round(avgDailySales * safeForecastDays);
                    var stockCoverDays = avgDailySales > 0m
                        ? Round(currentStock / avgDailySales)
                        : (currentStock > 0 ? 9999m : 0m);
                    var estimatedOpeningStock = Math.Max(0, currentStock + soldQuantity - importedQuantity);
                    var availableQuantity = estimatedOpeningStock + importedQuantity;
                    var sellThroughRate = availableQuantity > 0
                        ? Round(Math.Min(1m, (decimal)soldQuantity / availableQuantity))
                        : 0m;

                    var threshold = thresholdMap.TryGetValue(product.Id, out var dynamicThreshold)
                        ? Math.Max(dynamicThreshold, 1)
                        : 5;

                    var isLowStock = currentStock == 0
                                     || (avgDailySales > 0m && stockCoverDays <= safeForecastDays)
                                     || (currentStock <= threshold && soldQuantity > 0);

                    var isOverstock = !isLowStock
                                      && (
                                          (avgDailySales == 0m && currentStock >= threshold * 4)
                                          || stockCoverDays >= 120m
                                          || (importedQuantity > 0 && sellThroughRate < 0.35m && currentStock > forecastNeed * 2m)
                                      );

                    if (isLowStock)
                    {
                        var lowReason = avgDailySales > 0m
                            ? $"Bán trung bình {Round(avgDailySales):0.##}/ngày, dự báo hết hàng sau {Round(stockCoverDays):0.##} ngày."
                            : $"Tồn kho hiện tại ({currentStock}) thấp hơn ngưỡng cảnh báo ({threshold}).";

                        lowStockProducts.Add(new InventoryInsightItemDto
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                            CategoryName = product.CategoryName,
                            CurrentStock = currentStock,
                            SoldLookback = soldQuantity,
                            ImportedLookback = importedQuantity,
                            AvgDailySales = Round(avgDailySales),
                            StockCoverDays = Round(stockCoverDays),
                            ForecastNeed = Round(forecastNeed),
                            SellThroughRate = Round(sellThroughRate),
                            RiskLevel = stockCoverDays <= 7m ? "high" : "medium",
                            Reason = lowReason,
                            ImageUrl = product.ImageUrl
                        });
                    }

                    if (isOverstock)
                    {
                        var overReason = avgDailySales == 0m
                            ? "Sản phẩm chưa phát sinh bán hàng trong kỳ phân tích nhưng tồn kho đang cao."
                            : $"Tồn kho quay chậm ({Round(stockCoverDays):0.##} ngày), tỷ lệ xuất bán {Round(sellThroughRate * 100m):0.##}%.";

                        overstockProducts.Add(new InventoryInsightItemDto
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                            CategoryName = product.CategoryName,
                            CurrentStock = currentStock,
                            SoldLookback = soldQuantity,
                            ImportedLookback = importedQuantity,
                            AvgDailySales = Round(avgDailySales),
                            StockCoverDays = Round(stockCoverDays),
                            ForecastNeed = Round(forecastNeed),
                            SellThroughRate = Round(sellThroughRate),
                            RiskLevel = stockCoverDays >= 180m || avgDailySales == 0m ? "high" : "medium",
                            Reason = overReason,
                            ImageUrl = product.ImageUrl
                        });
                    }
                }

                var totalProducts = products.Count;
                var lowStockCount = lowStockProducts.Count;
                var overstockCount = overstockProducts.Count;
                var healthyCount = Math.Max(0, totalProducts - lowStockCount - overstockCount);

                var topDemandChart = products
                    .Select(p => new
                    {
                        p.Name,
                        Sold = soldMap.TryGetValue(p.Id, out var soldQty) ? soldQty : 0
                    })
                    .Where(x => x.Sold > 0)
                    .OrderByDescending(x => x.Sold)
                    .Take(8)
                    .Select(x => new RevenuePointDto
                    {
                        Label = x.Name,
                        Value = x.Sold
                    })
                    .ToList();

                return new InventoryInsightsResponseDto
                {
                    TotalProducts = totalProducts,
                    LowStockCount = lowStockCount,
                    OverstockCount = overstockCount,
                    HealthyCount = healthyCount,
                    LowStockProducts = lowStockProducts
                        .OrderBy(x => x.StockCoverDays)
                        .ThenByDescending(x => x.SoldLookback)
                        .Take(20)
                        .ToList(),
                    OverstockProducts = overstockProducts
                        .OrderByDescending(x => x.StockCoverDays)
                        .ThenByDescending(x => x.CurrentStock)
                        .Take(20)
                        .ToList(),
                    StockStatusChart = new List<RevenuePointDto>
                    {
                        new RevenuePointDto { Label = "Sap het hang", Value = lowStockCount },
                        new RevenuePointDto { Label = "Ton du", Value = overstockCount },
                        new RevenuePointDto { Label = "Can bang", Value = healthyCount }
                    },
                    TopDemandChart = topDemandChart
                };
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException().WithCode(DefaultCode.EXCEPTION).WithMessage(e.ResultString).WithDetail(e.Error);
            }
            catch (Exception e)
            {
                throw ExceptionError.Exception(e);
            }
        }

        private IQueryable<Order> GetDeliveredOrdersQuery(DateTime fromInclusive, DateTime toExclusive)
        {
            return _context.Orders.AsNoTracking()
                .Where(o => o.IsDeleted == false
                            && o.ShippingDetails.Any(sd => sd.IsDeleted == false
                                                          && sd.Status == 3
                                                          && sd.DateShip.HasValue
                                                          && sd.DateShip.Value >= fromInclusive
                                                          && sd.DateShip.Value < toExclusive));
        }

        private async Task<decimal> SumRevenueAsync(DateTime fromInclusive, DateTime toInclusive)
        {
            var toExclusive = toInclusive.Date.AddDays(1);
            var from = fromInclusive.Date;

            return await GetDeliveredOrdersQuery(from, toExclusive)
                .SumAsync(o => o.TotalAmount ?? 0m);
        }

        private static (DateTime from, DateTime to) NormalizeDateRange(DateTime? fromDate, DateTime? toDate, int defaultDays)
        {
            var safeDefaultDays = defaultDays <= 0 ? 30 : defaultDays;

            var to = toDate?.Date ?? DateTime.Today;
            var from = fromDate?.Date ?? to.AddDays(-(safeDefaultDays - 1));

            if (from > to)
            {
                (from, to) = (to, from);
            }

            return (from, to);
        }

        private static decimal CalculatePercentChange(decimal current, decimal previous)
        {
            if (previous == 0m)
            {
                return current == 0m ? 0m : 100m;
            }

            return ((current - previous) / previous) * 100m;
        }

        private static decimal Round(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        public async Task<dynamic> GetStockAlerts(bool includeAll, int defaultThreshold)
        {
            var safeDefaultThreshold = Math.Clamp(defaultThreshold, 0, 100000);
            var warehouseStock = await _context.WareHouses
                .AsNoTracking()
                .Where(x => x.IsDeleted == false && x.ProductId.HasValue)
                .GroupBy(x => x.ProductId!.Value)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Stock = group.Sum(x => x.RemainQuantity ?? 0)
                })
                .ToDictionaryAsync(x => x.ProductId, x => x.Stock);

            var thresholdMap = await _context.InventoryAlerts
                .AsNoTracking()
                .Where(x => x.IsDeleted == false && x.ProductsId.HasValue)
                .GroupBy(x => x.ProductsId!.Value)
                .Select(group => new
                {
                    ProductId = group.Key,
                    Threshold = group
                        .OrderByDescending(x => x.Id)
                        .Select(x => x.AlertThreshold ?? safeDefaultThreshold)
                        .FirstOrDefault()
                })
                .ToDictionaryAsync(x => x.ProductId, x => x.Threshold);

            var products = await _context.Products
                .AsNoTracking()
                .Where(x => x.IsDeleted == false)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.ImageUrl,
                    x.StockQuantity
                })
                .ToListAsync();

            var items = products.Select(product =>
            {
                var currentStock = Math.Max(
                    0,
                    warehouseStock.GetValueOrDefault(
                        product.Id,
                        product.StockQuantity ?? 0));
                var threshold = Math.Max(
                    0,
                    thresholdMap.GetValueOrDefault(product.Id, safeDefaultThreshold));
                var isAlert = currentStock <= threshold;
                return new StockAlertItemDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    CurrentStock = currentStock,
                    AlertThreshold = threshold,
                    Severity = currentStock == 0
                        ? "critical"
                        : currentStock <= Math.Max(1, threshold / 2)
                            ? "high"
                            : "warning",
                    IsAlert = isAlert
                };
            })
            .Where(x => includeAll || x.IsAlert)
            .OrderByDescending(x => x.IsAlert)
            .ThenBy(x => x.CurrentStock)
            .ThenBy(x => x.ProductName)
            .ToList();

            return new StockAlertResponseDto
            {
                AlertCount = items.Count(x => x.IsAlert),
                OutOfStockCount = items.Count(x => x.IsAlert && x.CurrentStock == 0),
                Items = items
            };
        }

        public async Task<dynamic> SetStockThreshold(StockThresholdRequest model)
        {
            if (model.ProductId <= 0 || model.AlertThreshold < 0)
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.ERROR_STRUCTURE)
                    .WithMessage("Ngưỡng cảnh báo tồn kho không hợp lệ.");
            }

            var productExists = await _context.Products.AnyAsync(x =>
                x.Id == model.ProductId && x.IsDeleted == false);
            if (!productExists)
            {
                throw new ResponseMessageException()
                    .WithException(DefaultCode.DATA_NOT_FOUND);
            }

            var alert = await _context.InventoryAlerts
                .Where(x => x.ProductsId == model.ProductId && x.IsDeleted == false)
                .OrderByDescending(x => x.Id)
                .FirstOrDefaultAsync();
            if (alert == null)
            {
                alert = new InventoryAlert
                {
                    ProductsId = model.ProductId,
                    IsDeleted = false
                };
                await _context.InventoryAlerts.AddAsync(alert);
            }

            alert.AlertThreshold = model.AlertThreshold;
            await _context.SaveChangesAsync();
            return alert;
        }

        private static List<RevenuePointDto> FillMissingRevenuePoints(
            IReadOnlyCollection<RevenuePointDto> source,
            DateTime from,
            DateTime to,
            string groupBy)
        {
            var map = source.ToDictionary(x => x.Label, x => x.Value);
            var result = new List<RevenuePointDto>();

            if (groupBy == "year")
            {
                for (var year = from.Year; year <= to.Year; year++)
                {
                    var label = year.ToString();
                    result.Add(new RevenuePointDto { Label = label, Value = map.GetValueOrDefault(label) });
                }
                return result;
            }

            if (groupBy == "month")
            {
                var cursor = new DateTime(from.Year, from.Month, 1);
                var end = new DateTime(to.Year, to.Month, 1);
                while (cursor <= end)
                {
                    var label = $"{cursor.Month:00}/{cursor.Year}";
                    result.Add(new RevenuePointDto { Label = label, Value = map.GetValueOrDefault(label) });
                    cursor = cursor.AddMonths(1);
                }
                return result;
            }

            for (var cursor = from.Date; cursor <= to.Date; cursor = cursor.AddDays(1))
            {
                var label = $"{cursor.Day:00}/{cursor.Month:00}/{cursor.Year}";
                result.Add(new RevenuePointDto { Label = label, Value = map.GetValueOrDefault(label) });
            }
            return result;
        }

        private static string NormalizePaymentMethod(string? method)
        {
            return (method ?? string.Empty).Trim().ToLowerInvariant() switch
            {
                "cod" => "Thanh toán khi nhận hàng",
                "bank" => "Chuyển khoản",
                _ => "Khác/chưa xác định"
            };
        }
    }
}
