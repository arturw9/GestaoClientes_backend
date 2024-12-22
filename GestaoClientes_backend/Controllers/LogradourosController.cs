using GestaoClientes_backend.Data;
using GestaoClientes_backend.Models;
using GestaoClientes_backend.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GestaoClientes_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class LogradourosController : MainControllerApi
    {
        private readonly ILogger<LogradourosController> _logger;
        private readonly ApiDbContext _context;

        public LogradourosController(ILogger<LogradourosController> logger, ApiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("/Logradouros/Inserir")]
        [HttpPost]
        public async Task<ActionResult> Inserir(Guid idCliente, LogradouroViewModel logradouroViewModel)
        {
            try
            {
                if (idCliente == Guid.Empty)
                {
                    return BadRequest("IdCliente não pode ser um GUID vazio.");
                }

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@IdLogradouro", SqlDbType.UniqueIdentifier) { Value = DBNull.Value },
                    new SqlParameter("@IdCliente", SqlDbType.UniqueIdentifier) { Value = idCliente },
                    new SqlParameter("@Rua", SqlDbType.NVarChar) { Value = logradouroViewModel.Rua ?? (object)DBNull.Value },
                    new SqlParameter("@Quadra", SqlDbType.NVarChar) { Value = logradouroViewModel.Quadra ?? (object)DBNull.Value },
                    new SqlParameter("@Lote", SqlDbType.NVarChar) { Value = logradouroViewModel.Lote ?? (object)DBNull.Value },
                    new SqlParameter("@Numero", SqlDbType.Int) { Value = logradouroViewModel.Numero.HasValue ? (object)logradouroViewModel.Numero.Value : DBNull.Value },
                    new SqlParameter("@Bairro", SqlDbType.NVarChar) { Value = logradouroViewModel.Bairro ?? (object)DBNull.Value },
                    new SqlParameter("@Operacao", SqlDbType.NVarChar) { Value = "Inserir" }
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Logradouro_PROCEDURE @IdLogradouro, @IdCliente, @Rua, @Quadra, @Lote, @Numero, @Bairro, @Operacao",
                    parameters
                );

                return Ok(idCliente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("/Logradouros/Atualizar")]
        [HttpPut]
        public async Task<ActionResult> Atualizar(Guid idCliente, LogradouroViewModel logradouroViewModel)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@IdLogradouro", SqlDbType.UniqueIdentifier) { Value = logradouroViewModel.IdLogradouro },
                    new SqlParameter("@IdCliente", SqlDbType.UniqueIdentifier) { Value = idCliente },
                    new SqlParameter("@Rua", SqlDbType.NVarChar) { Value = logradouroViewModel.Rua ?? (object)DBNull.Value },
                    new SqlParameter("@Quadra", SqlDbType.NVarChar) { Value = logradouroViewModel.Quadra ?? (object)DBNull.Value },
                    new SqlParameter("@Lote", SqlDbType.NVarChar) { Value = logradouroViewModel.Lote ?? (object)DBNull.Value },
                    new SqlParameter("@Numero", SqlDbType.Int) { Value = logradouroViewModel.Numero.HasValue ? (object)logradouroViewModel.Numero.Value : DBNull.Value },
                    new SqlParameter("@Bairro", SqlDbType.NVarChar) { Value = logradouroViewModel.Bairro ?? (object)DBNull.Value },
                    new SqlParameter("@Operacao", SqlDbType.NVarChar) { Value = "Atualizar" }
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC Logradouro_PROCEDURE @IdLogradouro, @IdCliente, @Rua, @Quadra, @Lote, @Numero, @Bairro, @Operacao",
                    parameters
                );

                return Ok("Logradouro atualizado com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("/Logradouros/Remover")]
        [HttpDelete]
        public async Task<ActionResult> Remover(Guid idLogradouro, Guid idCliente)
        {
            try
            {
                if (idLogradouro == Guid.Empty || idCliente == Guid.Empty)
                {
                    return BadRequest("IdLogradouro ou IdCliente inválidos.");
                }

                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@IdLogradouro", SqlDbType.UniqueIdentifier) { Value = idLogradouro },
                    new SqlParameter("@IdCliente", SqlDbType.UniqueIdentifier) { Value = idCliente },
                    new SqlParameter("@Rua", SqlDbType.NVarChar) { Value = (object)DBNull.Value },
                    new SqlParameter("@Quadra", SqlDbType.NVarChar) { Value = (object)DBNull.Value },
                    new SqlParameter("@Lote", SqlDbType.NVarChar) { Value = (object)DBNull.Value },
                    new SqlParameter("@Numero", SqlDbType.Int) { Value = (object)DBNull.Value },
                    new SqlParameter("@Bairro", SqlDbType.NVarChar) { Value = (object)DBNull.Value },
                    new SqlParameter("@Operacao", SqlDbType.NVarChar, 50) { Value = "Remover" }
                };

                var result = await _context.Set<Logradouro>()
                    .FromSqlRaw("EXEC Logradouro_PROCEDURE @IdLogradouro, @IdCliente, @Rua, @Quadra, @Lote, @Numero, @Bairro, @Operacao", parameters)
                    .ToListAsync();

                if (result.Count == 0)
                {
                    return NotFound("Logradouro não encontrado.");
                }

                return Ok("Logradouro removido com sucesso.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("/Logradouros/Visualizar")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Visualizar(Guid? idCliente)
        {
            try
            {
                var getLogradouro = new List<Logradouro>();
                if (idCliente != Guid.Empty && idCliente != null)
                {
                    getLogradouro = await _context.Logradouros.Where(x => x.IdCliente == idCliente).ToListAsync();
                    if (getLogradouro == null)
                        return BadRequest("Logradouro não encontrado.");
                }
                else
                {
                    getLogradouro = await _context.Logradouros.ToListAsync();
                }
                var logradouroViewModelToLogradouro = LogradourosToLogradouroViewModelList(getLogradouro);
                return Ok(logradouroViewModelToLogradouro);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private List<LogradouroViewModel> LogradourosToLogradouroViewModelList(List<Logradouro> logradouros)
        {
            var logradouroViewModelList = new List<LogradouroViewModel>();

            foreach (var logradouro in logradouros)
            {
                var logradouroViewModel = new LogradouroViewModel()
                {
                    IdLogradouro = logradouro.IdLogradouro,
                    IdCliente = logradouro.IdCliente,
                    Rua = logradouro.Rua,
                    Quadra = logradouro.Quadra,
                    Lote = logradouro.Lote,
                    Numero = logradouro.Numero,
                    Bairro = logradouro.Bairro
                };

                logradouroViewModelList.Add(logradouroViewModel);
            }

            return logradouroViewModelList;
        }
    }
}