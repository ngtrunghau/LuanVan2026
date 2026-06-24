using badmintion.DTO;
using badmintion.Interface;
using badmintion.Lib.Core.DefaultRepository;
using badmintion.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace badmintion.Services
{
    public class AssistantService : IAssistantService
    {
        private static readonly HashSet<string> StopWords = new(StringComparer.Ordinal)
        {
            "anh", "ban", "cho", "co", "cua", "duoc", "gi", "giup", "hang", "hay", "khong",
            "la", "loai", "minh", "mot", "muon", "nao", "nay", "nhe", "nhung", "san", "pham",
            "shop", "toi", "tim", "tu", "van", "voi", "va", "ve", "can", "dang", "the", "hon"
        };
        private static readonly HashSet<string> CategoryStopWords = new(StringComparer.Ordinal)
        {
            "cau", "long", "san", "pham", "phu", "kien", "dung", "cu", "the", "thao"
        };

        private readonly BadmintionNlContext _context;

        public AssistantService(BadmintionNlContext context, IHttpContextAccessor contextAccessor)
        {
            _context = context;
        }

        public async Task<dynamic> Ask(AssistantAskRequest model)
        {
            try
            {
                if (model == default || string.IsNullOrWhiteSpace(model.Message))
                {
                    throw new ResponseMessageException().WithException(DefaultCode.ERROR_STRUCTURE);
                }

                if (model.Message.Length > 1000)
                {
                    return CreateTextResponse(
                        "Tin nhắn hơi dài. Bạn hãy mô tả ngắn gọn loại sản phẩm, ngân sách hoặc nhu cầu chơi để mình tư vấn chính xác hơn.",
                        "invalid");
                }

                var maxResults = Math.Clamp(model.MaxResults <= 0 ? 5 : model.MaxResults, 1, 10);
                var message = NormalizeText(model.Message);
                var historyText = string.Join(" ", model.History
                    .Where(x => string.Equals(x.Role, "user", StringComparison.OrdinalIgnoreCase))
                    .TakeLast(6)
                    .Select(x => NormalizeText(x.Content)));
                var contextualText = $"{historyText} {message}".Trim();

                var candidates = await LoadCatalog();
                if (candidates.Count == 0)
                {
                    return CreateTextResponse(
                        "Hiện cửa hàng chưa có sản phẩm đang kinh doanh trong danh mục.",
                        "empty_catalog");
                }

                var categories = candidates
                    .Where(x => !string.IsNullOrWhiteSpace(x.CategoryName))
                    .GroupBy(x => x.CategoryName)
                    .OrderBy(x => x.Key)
                    .ToList();

                if (IsGreeting(message))
                {
                    return CreateTextResponse(
                        $"Chào bạn! Mình có thể tư vấn {BuildCategorySummary(categories)}, tìm theo ngân sách, màu sắc, sản phẩm còn hàng hoặc bán chạy. Bạn đang cần sản phẩm nào?",
                        "greeting",
                        BuildGeneralQuickReplies(categories));
                }

                if (ContainsAny(message, "cua hang co gi", "shop co gi", "danh muc", "ban nhung gi", "kinh doanh gi"))
                {
                    var categoryDetails = string.Join("; ", categories.Select(x => $"{x.Key} ({x.Count()} sản phẩm)"));
                    return CreateTextResponse(
                        $"Cửa hàng hiện phục vụ: {categoryDetails}. Bạn có thể nói thêm ngân sách hoặc tên sản phẩm để mình lọc sát hơn.",
                        "catalog_overview",
                        BuildGeneralQuickReplies(categories));
                }

                var lastSuggestions = candidates
                    .Where(x => model.LastSuggestedProductIds.Contains(x.ProductId))
                    .ToList();
                var matchedCategory = FindCategory(message, categories.Select(x => x.Key));
                if (string.IsNullOrWhiteSpace(matchedCategory))
                {
                    matchedCategory = FindCategory(contextualText, categories.Select(x => x.Key));
                }

                var budget = ExtractBudgetRange(message);
                if (!budget.HasValue)
                {
                    budget = ExtractBudgetRange(contextualText);
                }

                var intent = DetectIntent(message);
                var queryTokens = ExtractSearchTokens(message);
                var contextualTokens = ExtractSearchTokens(contextualText);
                var asksInStock = ContainsAny(message, "con hang", "co san", "ton kho", "mua duoc");
                var asksOutOfStock = ContainsAny(message, "het hang", "tam het");
                var asksCheaper = ContainsAny(message, "re hon", "gia thap hon", "mem hon");
                var asksMoreExpensive = ContainsAny(message, "cao cap hon", "dat hon", "gia cao hon");

                decimal? referencePrice = null;
                if (lastSuggestions.Count > 0)
                {
                    referencePrice = asksCheaper
                        ? lastSuggestions.Where(x => x.Price.HasValue).Min(x => x.Price)
                        : lastSuggestions.Where(x => x.Price.HasValue).Max(x => x.Price);
                }

                var scored = candidates
                    .Select(x => new RankedCandidate
                    {
                        Candidate = x,
                        Score = ScoreCandidate(
                            x,
                            message,
                            queryTokens,
                            contextualTokens,
                            matchedCategory,
                            budget,
                            intent,
                            asksInStock,
                            asksOutOfStock,
                            asksCheaper,
                            asksMoreExpensive,
                            referencePrice)
                    })
                    .Where(x => IsEligible(
                        x.Candidate,
                        matchedCategory,
                        budget,
                        asksInStock,
                        asksOutOfStock,
                        asksCheaper,
                        asksMoreExpensive,
                        referencePrice))
                    .ToList();

                if (intent == "best_seller")
                {
                    scored = scored
                        .OrderByDescending(x => x.Candidate.SoldQuantity)
                        .ThenByDescending(x => x.Candidate.StockQuantity)
                        .ThenByDescending(x => x.Score)
                        .ToList();
                }
                else if (intent == "cheapest")
                {
                    scored = scored
                        .Where(x => x.Candidate.Price.HasValue)
                        .OrderBy(x => x.Candidate.Price)
                        .ThenByDescending(x => x.Score)
                        .ToList();
                }
                else
                {
                    scored = scored
                        .OrderByDescending(x => x.Score)
                        .ThenByDescending(x => x.Candidate.StockQuantity > 0)
                        .ThenByDescending(x => x.Candidate.SoldQuantity)
                        .ThenBy(x => x.Candidate.Price ?? decimal.MaxValue)
                        .ToList();
                }

                var hasSpecificCriteria = matchedCategory != null
                    || budget.HasValue
                    || queryTokens.Count > 0
                    || asksInStock
                    || asksOutOfStock
                    || intent is "best_seller" or "cheapest";

                if (hasSpecificCriteria && intent == "search")
                {
                    scored = scored.Where(x => x.Score >= 12).ToList();
                }

                var suggestions = scored
                    .Take(maxResults)
                    .Select(x => MapSuggestion(x.Candidate, intent, budget, asksCheaper, asksMoreExpensive))
                    .ToList();

                if (suggestions.Count == 0)
                {
                    var categoryHint = matchedCategory == null ? string.Empty : $" thuộc danh mục {matchedCategory}";
                    return CreateTextResponse(
                        $"Mình chưa tìm thấy sản phẩm{categoryHint} khớp yêu cầu trong dữ liệu hiện có của cửa hàng. Bạn thử đổi khoảng giá hoặc mô tả rộng hơn nhé.",
                        "no_result",
                        BuildGeneralQuickReplies(categories));
                }

                return new AssistantAskResponse
                {
                    Intent = intent,
                    Message = BuildReplyMessage(intent, matchedCategory, budget, suggestions),
                    Suggestions = suggestions,
                    QuickReplies = BuildFollowUpReplies(intent, matchedCategory, suggestions)
                };
            }
            catch (ResponseMessageException e)
            {
                throw new ResponseMessageException()
                    .WithCode(DefaultCode.EXCEPTION)
                    .WithMessage(e.ResultString)
                    .WithDetail(e.Error);
            }
            catch (Exception e)
            {
                throw ExceptionError.Exception(e);
            }
        }

        private async Task<List<AssistantCandidate>> LoadCatalog()
        {
            var soldQuantities = await _context.OrderItems
                .AsNoTracking()
                .Where(x => x.IsDeleted == false
                    && x.ProductsId.HasValue
                    && x.OrdersNavigation != null
                    && x.OrdersNavigation.IsDeleted == false
                    && x.OrdersNavigation.ShippingDetails.Any(s => s.Status == 3))
                .GroupBy(x => x.ProductsId!.Value)
                .Select(x => new
                {
                    ProductId = x.Key,
                    Quantity = x.Sum(i => i.Quantity ?? 0)
                })
                .ToDictionaryAsync(x => x.ProductId, x => x.Quantity);

            var products = await (
                from p in _context.Products.AsNoTracking()
                join c in _context.Categories.AsNoTracking() on p.CategoriesId equals c.Id into categoryJoin
                from category in categoryJoin.DefaultIfEmpty()
                where p.IsDeleted == false && (category == null || category.IsDeleted == false)
                select new AssistantCandidate
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Descriptions = p.Descriptions,
                    CategoryName = category != null ? category.Name : string.Empty,
                    Price = p.Price,
                    StockQuantity = Math.Max(0, p.StockQuantity ?? 0),
                    ImageUrl = p.ImageUrl,
                    Color = p.Color
                }
            ).ToListAsync();

            foreach (var product in products)
            {
                product.SoldQuantity = soldQuantities.GetValueOrDefault(product.ProductId);
                product.SearchableText = NormalizeText(
                    $"{product.Name} {product.CategoryName} {product.Color} {StripHtml(product.Descriptions)}");
            }

            return products;
        }

        private static int ScoreCandidate(
            AssistantCandidate candidate,
            string message,
            IReadOnlyCollection<string> queryTokens,
            IReadOnlyCollection<string> contextualTokens,
            string? category,
            BudgetRange? budget,
            string intent,
            bool asksInStock,
            bool asksOutOfStock,
            bool asksCheaper,
            bool asksMoreExpensive,
            decimal? referencePrice)
        {
            var score = 0;
            var normalizedName = NormalizeText(candidate.Name);

            if (message.Contains(normalizedName, StringComparison.Ordinal) && normalizedName.Length >= 3)
            {
                score += 80;
            }

            foreach (var token in queryTokens)
            {
                if (normalizedName.Contains(token, StringComparison.Ordinal))
                {
                    score += 14;
                }
                else if (candidate.SearchableText.Contains(token, StringComparison.Ordinal))
                {
                    score += 6;
                }
            }

            foreach (var token in contextualTokens.Except(queryTokens))
            {
                if (candidate.SearchableText.Contains(token, StringComparison.Ordinal))
                {
                    score += 2;
                }
            }

            if (category != null && NormalizeText(candidate.CategoryName) == NormalizeText(category))
            {
                score += 35;
            }

            if (budget.HasValue && candidate.Price.HasValue)
            {
                if (budget.Value.Contains(candidate.Price.Value))
                {
                    score += 30;
                }

                var target = budget.Value.Target;
                if (target > 0)
                {
                    var ratio = Math.Abs(candidate.Price.Value - target) / target;
                    score += ratio <= 0.1m ? 18 : ratio <= 0.25m ? 10 : 0;
                }
            }

            if (asksInStock)
            {
                score += candidate.StockQuantity > 0 ? 25 : -50;
            }

            if (asksOutOfStock)
            {
                score += candidate.StockQuantity == 0 ? 25 : 0;
            }

            if (intent == "best_seller")
            {
                score += Math.Min(candidate.SoldQuantity, 50);
            }

            if (intent == "cheapest" && candidate.Price.HasValue)
            {
                score += 10;
            }

            if (referencePrice.HasValue && candidate.Price.HasValue)
            {
                if (asksCheaper && candidate.Price.Value < referencePrice.Value)
                {
                    score += 35;
                }
                if (asksMoreExpensive && candidate.Price.Value > referencePrice.Value)
                {
                    score += 35;
                }
            }

            if (candidate.StockQuantity > 0)
            {
                score += 8;
            }

            return score;
        }

        private static bool IsEligible(
            AssistantCandidate candidate,
            string? category,
            BudgetRange? budget,
            bool asksInStock,
            bool asksOutOfStock,
            bool asksCheaper,
            bool asksMoreExpensive,
            decimal? referencePrice)
        {
            if (category != null && NormalizeText(candidate.CategoryName) != NormalizeText(category))
            {
                return false;
            }

            if (budget.HasValue && candidate.Price.HasValue && !budget.Value.ContainsWithTolerance(candidate.Price.Value))
            {
                return false;
            }

            if (asksInStock && candidate.StockQuantity <= 0)
            {
                return false;
            }

            if (asksOutOfStock && candidate.StockQuantity > 0)
            {
                return false;
            }

            if (referencePrice.HasValue && candidate.Price.HasValue)
            {
                if (asksCheaper && candidate.Price.Value >= referencePrice.Value)
                {
                    return false;
                }
                if (asksMoreExpensive && candidate.Price.Value <= referencePrice.Value)
                {
                    return false;
                }
            }

            return true;
        }

        private static AssistantProductSuggestion MapSuggestion(
            AssistantCandidate candidate,
            string intent,
            BudgetRange? budget,
            bool asksCheaper,
            bool asksMoreExpensive)
        {
            var reasons = new List<string>();

            if (!string.IsNullOrWhiteSpace(candidate.CategoryName))
            {
                reasons.Add($"Danh mục {candidate.CategoryName}.");
            }

            if (budget.HasValue && candidate.Price.HasValue && budget.Value.Contains(candidate.Price.Value))
            {
                reasons.Add("Giá nằm trong khoảng bạn yêu cầu.");
            }
            else if (asksCheaper)
            {
                reasons.Add("Có giá thấp hơn nhóm sản phẩm vừa xem.");
            }
            else if (asksMoreExpensive)
            {
                reasons.Add("Thuộc lựa chọn cao cấp hơn nhóm vừa xem.");
            }

            if (intent == "best_seller" && candidate.SoldQuantity > 0)
            {
                reasons.Add($"Đã bán {candidate.SoldQuantity} sản phẩm trong các đơn hoàn tất.");
            }

            reasons.Add(candidate.StockQuantity > 0
                ? $"Còn {candidate.StockQuantity} sản phẩm."
                : "Hiện đang hết hàng.");

            return new AssistantProductSuggestion
            {
                ProductId = candidate.ProductId,
                Name = candidate.Name,
                Price = candidate.Price ?? 0m,
                StockQuantity = candidate.StockQuantity,
                SoldQuantity = candidate.SoldQuantity,
                CategoryName = candidate.CategoryName,
                Color = candidate.Color,
                ImageUrl = candidate.ImageUrl,
                Reason = string.Join(" ", reasons)
            };
        }

        private static string BuildReplyMessage(
            string intent,
            string? category,
            BudgetRange? budget,
            IReadOnlyCollection<AssistantProductSuggestion> suggestions)
        {
            var subject = string.IsNullOrWhiteSpace(category) ? "sản phẩm" : category;
            var priceText = budget.HasValue ? $" trong khoảng {budget.Value.ToDisplayText()}" : string.Empty;

            return intent switch
            {
                "best_seller" => $"Dựa trên các đơn đã hoàn tất, đây là {suggestions.Count} {subject} bán chạy phù hợp nhất{priceText}.",
                "cheapest" => $"Đây là {suggestions.Count} {subject} có giá thấp nhất đang có trong dữ liệu cửa hàng{priceText}.",
                "stock" => $"Mình tìm thấy {suggestions.Count} {subject} đúng với yêu cầu tồn kho của bạn{priceText}.",
                _ => $"Mình tìm thấy {suggestions.Count} {subject} phù hợp từ danh mục thực tế của cửa hàng{priceText}."
            };
        }

        private static AssistantAskResponse CreateTextResponse(
            string message,
            string intent,
            List<string>? quickReplies = null)
        {
            return new AssistantAskResponse
            {
                Message = message,
                Intent = intent,
                QuickReplies = quickReplies ?? new List<string>()
            };
        }

        private static List<string> BuildGeneralQuickReplies(
            IEnumerable<IGrouping<string, AssistantCandidate>> categories)
        {
            var replies = categories
                .Take(2)
                .Select(x => $"Gợi ý {x.Key} còn hàng")
                .ToList();

            replies.Add("Sản phẩm bán chạy");
            replies.Add("Sản phẩm giá thấp nhất");
            return replies.Take(4).ToList();
        }

        private static List<string> BuildFollowUpReplies(
            string intent,
            string? category,
            IReadOnlyCollection<AssistantProductSuggestion> suggestions)
        {
            var replies = new List<string>();
            if (suggestions.Any(x => x.StockQuantity > 0))
            {
                replies.Add("Chỉ xem sản phẩm còn hàng");
            }

            if (suggestions.Any(x => x.Price > 0))
            {
                replies.Add("Có lựa chọn rẻ hơn không?");
                replies.Add("Cho tôi loại cao cấp hơn");
            }

            if (intent != "best_seller")
            {
                replies.Add(category == null ? "Sản phẩm nào bán chạy?" : $"{category} nào bán chạy?");
            }

            return replies.Distinct().Take(4).ToList();
        }

        private static string BuildCategorySummary(
            IEnumerable<IGrouping<string, AssistantCandidate>> categories)
        {
            var names = categories.Select(x => x.Key).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            return names.Count == 0 ? "các sản phẩm cầu lông hiện có" : string.Join(", ", names);
        }

        private static string DetectIntent(string message)
        {
            if (ContainsAny(message, "ban chay", "pho bien", "mua nhieu", "hot"))
            {
                return "best_seller";
            }

            if (ContainsAny(message, "re nhat", "gia thap nhat", "it tien nhat"))
            {
                return "cheapest";
            }

            if (ContainsAny(message, "con hang", "het hang", "ton kho", "co san"))
            {
                return "stock";
            }

            return "search";
        }

        private static bool IsGreeting(string message)
        {
            return Regex.IsMatch(message, @"^(xin chao|chao|hello|hi|hey|alo)(\s+(shop|ban|ad|minh))?[!. ]*$");
        }

        private static string? FindCategory(string message, IEnumerable<string> categories)
        {
            var normalizedCategories = categories
                .Select(x => new { Original = x, Normalized = NormalizeText(x) })
                .Where(x => x.Normalized.Length > 1)
                .ToList();

            var exactMatch = normalizedCategories
                .Where(x => message.Contains(x.Normalized, StringComparison.Ordinal))
                .OrderByDescending(x => x.Normalized.Length)
                .FirstOrDefault();
            if (exactMatch != null)
            {
                return exactMatch.Original;
            }

            var messageTokens = Regex.Split(message, @"[^a-z0-9]+")
                .Where(x => x.Length >= 2)
                .ToHashSet(StringComparer.Ordinal);

            return normalizedCategories
                .Select(x => new
                {
                    x.Original,
                    MatchCount = Regex.Split(x.Normalized, @"[^a-z0-9]+")
                        .Where(token => token.Length >= 2 && !CategoryStopWords.Contains(token))
                        .Count(messageTokens.Contains)
                })
                .Where(x => x.MatchCount > 0)
                .OrderByDescending(x => x.MatchCount)
                .Select(x => x.Original)
                .FirstOrDefault();
        }

        private static List<string> ExtractSearchTokens(string message)
        {
            return Regex.Split(message, @"[^a-z0-9]+")
                .Where(x => x.Length >= 2
                    && !StopWords.Contains(x)
                    && !Regex.IsMatch(x, @"^\d+$")
                    && !ContainsAny(x, "trieu", "nghin", "tram"))
                .Distinct()
                .Take(20)
                .ToList();
        }

        private static BudgetRange? ExtractBudgetRange(string message)
        {
            var values = ExtractMoneyValues(message);
            if (values.Count >= 2 && ContainsAny(message, "tu ", "den ", "khoang"))
            {
                return new BudgetRange(values.Min(), values.Max());
            }

            if (values.Count == 0)
            {
                return null;
            }

            var value = values[0];
            if (ContainsAny(message, "duoi ", "toi da", "khong qua", "tam duoi"))
            {
                return new BudgetRange(0, value);
            }

            if (ContainsAny(message, "tren ", "tu ", "it nhat"))
            {
                return new BudgetRange(value, decimal.MaxValue);
            }

            return new BudgetRange(value * 0.75m, value * 1.25m, value);
        }

        private static List<decimal> ExtractMoneyValues(string message)
        {
            var values = new List<decimal>();
            var matches = Regex.Matches(
                message,
                @"(\d+(?:[.,]\d+)?)\s*(trieu|tr|m|nghin|ngan|k)?\b|(\d{1,3}(?:[.,]\d{3})+|\d{5,10})",
                RegexOptions.IgnoreCase);

            foreach (Match match in matches)
            {
                var raw = match.Groups[1].Success ? match.Groups[1].Value : match.Groups[3].Value;
                var unit = match.Groups[2].Value;
                decimal value;

                if (!string.IsNullOrWhiteSpace(unit))
                {
                    raw = raw.Replace(',', '.');
                    if (!decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    {
                        continue;
                    }

                    value *= unit is "trieu" or "tr" or "m" ? 1_000_000m : 1_000m;
                }
                else
                {
                    raw = raw.Replace(".", string.Empty).Replace(",", string.Empty);
                    if (!decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value))
                    {
                        continue;
                    }
                }

                values.Add(value);
            }

            return values;
        }

        private static string StripHtml(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? string.Empty
                : Regex.Replace(value, "<.*?>", " ");
        }

        private static string NormalizeText(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var decomposed = input.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var ch in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(ch == 'đ' ? 'd' : ch);
                }
            }

            return Regex.Replace(
                sb.ToString().Normalize(NormalizationForm.FormC),
                @"\s+",
                " ").Trim();
        }

        private static bool ContainsAny(string text, params string[] values)
        {
            return values.Any(value => text.Contains(value, StringComparison.Ordinal));
        }

        private sealed class RankedCandidate
        {
            public AssistantCandidate Candidate { get; set; } = null!;
            public int Score { get; set; }
        }

        private sealed class AssistantCandidate
        {
            public int ProductId { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Descriptions { get; set; }
            public string CategoryName { get; set; } = string.Empty;
            public decimal? Price { get; set; }
            public int StockQuantity { get; set; }
            public int SoldQuantity { get; set; }
            public string? ImageUrl { get; set; }
            public string? Color { get; set; }
            public string SearchableText { get; set; } = string.Empty;
        }

        private readonly record struct BudgetRange(decimal Min, decimal Max, decimal? Preferred = null)
        {
            public decimal Target => Preferred ?? (Max == decimal.MaxValue ? Min : (Min + Max) / 2);

            public bool Contains(decimal value) => value >= Min && value <= Max;

            public bool ContainsWithTolerance(decimal value)
            {
                if (Max == decimal.MaxValue)
                {
                    return value >= Min;
                }

                var tolerance = Preferred.HasValue ? Preferred.Value * 0.1m : 0m;
                return value >= Math.Max(0, Min - tolerance) && value <= Max + tolerance;
            }

            public string ToDisplayText()
            {
                if (Min == 0)
                {
                    return $"dưới {Max:N0} ₫";
                }

                if (Max == decimal.MaxValue)
                {
                    return $"từ {Min:N0} ₫";
                }

                return $"{Min:N0}–{Max:N0} ₫";
            }
        }
    }
}
