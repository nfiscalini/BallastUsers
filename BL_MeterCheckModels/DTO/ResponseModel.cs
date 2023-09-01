namespace BL_MeterCheckModels.DTO
{
    public class ResponseModel<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }

        public ResponseModel(T data)
        {
            Data = data;
            Message = string.Empty;
        }
    }
}
