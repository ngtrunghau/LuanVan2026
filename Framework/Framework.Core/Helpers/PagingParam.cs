namespace badmintion.Lib.Core
{
    public class PagingParam
    {
        public int Start { get; set; } = 1;
        public int Limit { get; set; } = 10;


        public string? SortBy { get; set; }

        public bool SortDesc { get; set; }
        public string? Content { get; set; }
        public int? Level { get; set; } = null;



        public int? IdDonViCha { get; set; } = null;

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }


        public int Skip
        {
            get
            {
                return (Start > 0 ? Start - 1 : 0) * Limit;
            }
        }
    }


    public class PagingParamDefault : PagingParam
    {
        public string? NewsSectionMobiId { get; set; } = null;

        public bool IsPublic { get; set; } = false;
        public string? MenuId { get; set; }

        public string? NewsId { get; set; }
        public string? ServiceId { get; set; } = null;
    }

    public class PagingModel<T>
    {
        public long TotalRows { get; set; }

        public IEnumerable<T> Data { get; set; }
    }
}
