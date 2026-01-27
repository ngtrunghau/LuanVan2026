namespace badmintion.Lib.Core.DefaultRepository
{
    public static class ExceptionError
    {
        public static dynamic Exception(Exception e)
        {
            if (e.Message.Contains("is not a valid 24 digit hex string."))
            {
                return new ResponseMessageException().WithException(DefaultCode.ID_NOT_CORRECT_FORMAT);
            }
            if (e.Message.Contains("Object reference not set to an instance of an object."))
            {
                return new ResponseMessageException().WithCode(DefaultCode.ERROR_STRUCTURE).WithMessage("Cấu trúc gói tin không đúng quy định! Có một số thuộc tính sai so với khai báo của hệ thống ");
            }
            return new ResponseMessageException().WithCode(DefaultCode.EXCEPTION).WithMessage(e.Message);
        }
    }
}
