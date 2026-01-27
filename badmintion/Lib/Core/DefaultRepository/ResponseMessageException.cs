using System.ComponentModel.DataAnnotations;
using ValidationResult = FluentValidation.Results.ValidationResult;
namespace badmintion.Lib.Core.DefaultRepository
{
    public class ResponseMessageException : Exception
    {
        public int ResultCode { get; set; }
        public string ResultString { get; set; }

        public List<ErrorModel> Error { get; set; } = new List<ErrorModel>();

        public ResponseMessageException()
        {

        }
        public ResponseMessageException WithException(int resultCode)
        {
            ResultCode = resultCode;
            switch (ResultCode)
            {
                case 10:
                    ResultString = DefaultMessage.CREATE_FAILURE;
                    break;
                case 11:
                    ResultString = DefaultMessage.UPDATE_FAILURE;
                    break;
                case 12:
                    ResultString = DefaultMessage.DELETE_FAILURE;
                    break;
                case 13:
                    ResultString = DefaultMessage.USERNAME_NOT_FOUND;
                    break;
                case 14:
                    ResultString = DefaultMessage.WRONG_PASSWORD;
                    break;
                case 20:
                    ResultString = DefaultMessage.EXCEPTION;
                    break;
                case 21:
                    ResultString = DefaultMessage.DATA_EXISTED;
                    break;
                case 22:
                    ResultString = DefaultMessage.DATA_NOT_FOUND;
                    break;
                case 23:
                    ResultString = DefaultMessage.COMMON_NOT_FOUND;
                    break;
                case 24:
                    ResultString = DefaultMessage.ERRER_STRUCTURE;
                    break;
                case 25:
                    ResultString = DefaultMessage.ACCOUNT_IS_LOCKED;
                    break;
                case 26:
                    ResultString = DefaultMessage.ACCOUNT_NOT_AUTHORIZED;
                    break;
                case 27:
                    ResultString = DefaultMessage.DATA_FIELDS_NOT_INCORRECT;
                    break;
                case 28:
                    ResultString = DefaultMessage.ID_NOT_CORRECT_FORMAT;
                    break;
                case 29:
                    ResultString = DefaultMessage.NOT_HAVE_ACCESS;
                    break;
                case 30:
                    ResultString = DefaultMessage.CORRECT_NUMBER_QUESTION;
                    break;
                case 31:
                    ResultString = DefaultMessage.OVER_TIME;
                    break;
                case 32:
                    ResultString = DefaultMessage.TOTAL_BXH;
                    break;
                case 1:
                    ResultString = DefaultMessage.BEYOND_TIME;
                    break;
                case 2:
                    ResultString = DefaultMessage.REFRESH_TOKEN_OUT_TIME;
                    break;
                case 3:
                    ResultString = DefaultMessage.TOKEN_NOT_FOUND;
                    break;
                case 4:
                    ResultString = DefaultMessage.TOKEN_OR_REFRESH_TOKEN_NOT_FOUND;
                    break;
                case 33:
                    ResultString = DefaultMessage.TIME_SEU;
                    break;
                case 34:
                    ResultString = DefaultMessage.VERSION_INCORRECT;
                    break;
                case 35:
                    ResultString = DefaultMessage.HET_THOI_GIAN;
                    break;
                case 36:
                    ResultString = DefaultMessage.CAUTRUCDETHI;
                    break;
            }
            return this;
        }

        public ResponseMessageException WithCode(int code)
        {
            if (!string.IsNullOrEmpty(code.ToString()))
            {
                ResultCode = code;
            }
            return this;
        }

        public ResponseMessageException WithMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                ResultString = message;
            }
            return this;
        }
        public ResponseMessageException WithValidationResult(ValidationResult message)
        {
            if (message != null)
            {
                foreach (var item in message.Errors)
                {
                    ResultCode = DefaultCode.DATA_FIELDS_NOT_INCORRECT;
                    ResultString = DefaultMessage.DATA_FIELDS_NOT_INCORRECT;
                    Error.Add(new ErrorModel(item.PropertyName, item.ErrorMessage));
                }
            }
            return this;
        }

        public ResponseMessageException WithDetail(List<ErrorModel> error)
        {
            if (error != null && error.Count > 0)
            {
                Error = error;
            }
            return this;
        }


    }

}
