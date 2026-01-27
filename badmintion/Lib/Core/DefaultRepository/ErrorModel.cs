namespace badmintion.Lib.Core.DefaultRepository
{
    public class ErrorModel
    {
        public string Field { get; set; }

        public string Message { get; set; }

        public ErrorModel(string field, string message)
        {
            Field = field;
            Message = message;
        }
    }
}
