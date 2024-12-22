using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using GestaoClientes_backend.Dtos;


namespace GestaoClientes_backend.Helpers
{
    public static class GeneralResponseHelper
    {
        public const string RESPONSE_ERROR_UNEXPECTED = "Ocorreu um erro inesperado";
        public const string RESPONSE_ERROR_UNKNOWN = "Ocorreu um erro desconhecido";

        public static string GenerateDefaultErrorResponse(HttpContext context)
        {
            switch (context.Response.StatusCode)
            {
                case 403:
                    return GenerateDefaultErrorResponse(context, RESPONSE_ERROR_UNEXPECTED);
                default:
                    return GenerateDefaultErrorResponse(context, RESPONSE_ERROR_UNKNOWN);
            }
        }

        public static string GenerateDefaultErrorResponse(HttpContext context, string shortMessage)
        {
            var operationId = context.Features.Get<RequestTelemetry>().Context.Operation.Id;
            var message = $"{shortMessage}. Tente novamente em alguns instantes. " +
                          "Se o problema continuar entre em contato com o atendimento ao cliente informando " +
                          $"o código {operationId}. Basta enviar um print!";

            var responseError = GenerateGenericErrorResponse(message, operationId, context.Request.Path);
            return ObjectToJson(responseError);
        }

        public static CustomErrorResponse GenerateGenericErrorResponse(HttpContext context, string message)
        {
            var operationId = context.Features.Get<RequestTelemetry>().Context.Operation.Id;
            return GenerateGenericErrorResponse(message, operationId, context.Request.Path);
        }

        public static CustomErrorResponse GenerateGenericErrorResponse(string message, string operationId,
            string requestPath)
        {
            return GenerateGenericErrorResponse("PGP0000", message, operationId, requestPath);
        }

        public static string GenerateGenericErrorResponseAsString(string code, string message, string operationId,
            string requestPath)
        {
            var responseError = GenerateGenericErrorResponse(code, message, operationId, requestPath);
            return ObjectToJson(responseError);
        }

        public static CustomErrorResponse GenerateGenericErrorResponse(string code, string message, string operationId,
            string requestPath)
        {
            return new CustomErrorResponse()
            {
                Code = code,
                Message = message,
                Path = requestPath,
                OperationId = operationId
            };
        }

        public static string ObjectToJson(object obj)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };

            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });
        }
    }
}