using GestaoClientes_backend.Helpers;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net;
using System.Text;
using System.Text.Json;

namespace GestaoClientes_backend.Controllers
{
    [ApiController]
    public abstract class MainControllerApi : ControllerBase
    {
        protected ICollection<string> Erros = new List<string>();

        protected ActionResult CustomResponse(object result = null)
        {
            if (IsValidOperation())
            {
                return Ok(result);
            }

            Success = false;
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                AdicionarErroProcessamento(erro.ErrorMessage);
                Success = false;
            }
            return CustomResponse();
        }

        protected ActionResult CustomBadRequest(string code, string message)
        {
            return CustomErrorResponse(HttpStatusCode.BadRequest, code, message);
        }
        protected ActionResult CustomNotFound(string message)
        {
            return CustomErrorResponse(HttpStatusCode.NotFound, "PGP#", message);
        }
        protected ActionResult CustomErrorResponse(HttpStatusCode statusCode, string code, string message)
        {
            var operationId = HttpContext.Features.Get<RequestTelemetry>().Context.Operation.Id;
            var response = GeneralResponseHelper.GenerateGenericErrorResponse(code, message, operationId, HttpContext.Request.Path);
            return StatusCode((int)statusCode, response);
        }

        protected bool IsValidOperation()
        {
            return !Erros.Any();
        }

        protected void AdicionarErroProcessamento(string erro)
        {
            Erros.Add(erro);
        }

        protected void LimparErrosProcessamento()
        {
            Erros.Clear();
            Success = true;
        }

        protected ActionResult<T> ResponseGet<T>(T result)
        {
            if (result == null)
            {
                Success = false;
                return NotFound();
            }

            return Ok(result);
        }

        protected ActionResult<IEnumerable<T>> ResponseGet<T>(IEnumerable<T> result)
        {

            if (result == null || !result.Any())
                return NoContent();

            return Ok(result);
        }

        protected ActionResult<T> ResponsePost<T>(string action, object route, T result)
        {
            if (IsValidOperation())
            {
                if (result == null)
                    return NoContent();

                return CreatedAtAction(action, route, result);
            }

            Success = false;
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }

        protected ActionResult ResponsePutPatch()
        {
            if (IsValidOperation())
            {
                return NoContent();
            }

            Success = false;
            return BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            }));
        }

        protected ActionResult<T> ResponseDelete<T>(T item)
        {
            if (IsValidOperation())
            {
                if (item == null)
                    return NoContent();

                return Ok(item);
            }

            Success = false;
            return BadRequest(new Dictionary<string, string[]>
            {
                { "Mensagens", Erros.ToArray() }
            });
        }

        protected void NotifyModelStateErrors()
        {
            var erros = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var erro in erros)
            {
                var erroMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                AdicionarErroProcessamento(erroMsg);
            }
        }

        protected ActionResult ModelStateErrorResponseError()
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        protected StringContent ObterConteudo(object dado)
        {
            return new StringContent(
                JsonSerializer.Serialize(dado),
                Encoding.UTF8,
                "application/json");
        }

        public bool Success { get; set; }
        public string Message { get; set; }

    }
}