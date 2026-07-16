namespace badmintion.Lib.Core
{
    public class IdFromBodyModel
    {
        public int Id { get; set; }
    }
    public class MoneyPayment
    {
        public double Money { get; set; }
    }

    public class ResponsePayment
    {
        public string Url { get; set; }
        public int OrderId { get; set; }
    }
}
