namespace badmintion.Lib.Core.DefaultRepository
{
    public class ResultMessageResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public dynamic Data { get; set; }
        public List<ErrorModel> Detail { get; set; }

        public ResultMessageResponse WithCode(int resultCode)
        {
            if (!string.IsNullOrEmpty(resultCode.ToString()))
            {
                Code = resultCode;
            }

            return this;
        }
        public ResultMessageResponse WithMessage(string resultString)
        {
            if (!string.IsNullOrEmpty(resultString))
            {
                Message = resultString;
            }

            return this;
        }

        public ResultMessageResponse WithData(dynamic data)
        {
            if (data != null)
            {
                Data = data;
            }
            return this;
        }


        public ResultMessageResponse WithResponse(int code)
        {
            Code = 0;
            switch (code)
            {
                case 1:
                    Message = DefaultMessage.CREATE_SUCCESS;
                    break;
                case 2:
                    Message = DefaultMessage.UPDATE_SUCCESS;
                    break;
                case 3:
                    Message = DefaultMessage.DELETE_SUCCESS;
                    break;
                case 4:
                    Message = DefaultMessage.GET_DATA_SUCCESS;
                    break;
                case 5:
                    Message = DefaultMessage.LOGIN_SUCCESS;
                    break;
            }
            return this;
        }

        public ResultMessageResponse WithDetail(List<ErrorModel> listError)
        {
            if (listError.Count > 0)
            {
                Detail = listError;
            }

            return this;
        }

    }


}
