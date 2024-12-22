using Newtonsoft.Json.Converters;

namespace GestaoClientes_backend.Dtos
{
    public interface IErrorResponse
    {
        string Code { get; set; }
        string Message { get; set; }
        string Path { get; set; }
        string OperationId { get; set; }
    }

    class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter()
        {
            base.DateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        }
    }
    public class CustomErrorResponse : IErrorResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Path { get; set; }
        public string OperationId { get; set; }
    }
}