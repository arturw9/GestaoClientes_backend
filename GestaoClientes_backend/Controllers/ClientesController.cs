using GestaoClientes_backend.Data;
using GestaoClientes_backend.Models;
using GestaoClientes_backend.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GestaoClientes_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ClientesController : MainControllerApi
    {
        private readonly ILogger<ClientesController> _logger;
        private readonly ApiDbContext _context;

        public ClientesController(ILogger<ClientesController> logger, ApiDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [Route("/Clientes/Inserir")]
        [HttpPost]
        public async Task<ActionResult> Inserir(ClienteViewModel clienteViewModel)
        {
            try
            {
                var emailDuplicado = await _context.Clientes
                    .Where(x => x.Email == clienteViewModel.Email)
                    .FirstOrDefaultAsync();

                if (emailDuplicado != null)
                    return BadRequest($"E-mail cadastrado para outro cliente: {clienteViewModel.Email}");

                var addCliente = ClienteViewModelToCliente(null, clienteViewModel, null);

                var logradourosJson = JsonConvert.SerializeObject(addCliente.Logradouro);

                // Criação dos parâmetros para a stored procedure
                var idParam = new SqlParameter("@Id", addCliente.Id);
                var clienteParam = new SqlParameter("@Nome", addCliente.Nome);
                var emailParam = new SqlParameter("@Email", addCliente.Email);
                var logotipoParam = new SqlParameter("@Logotipo", addCliente.Logotipo);
                var logradourosParam = new SqlParameter("@Logradouros", logradourosJson);
                var operacaoParam = new SqlParameter("@Operacao", "Inserir");

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC ClientePROCEDURE @Id, @Nome, @Email, @Logotipo, @Logradouros, @Operacao",
                    idParam, clienteParam, emailParam, logotipoParam, logradourosParam, operacaoParam);

                return Ok(clienteViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("/Clientes/Atualizar")]
        [HttpPut]
        public async Task<ActionResult> Atualizar(Guid id, [FromBody] AtualizarClienteViewModel atualizarClienteViewModel)
        {
            try
            {
                var emailDuplicado = await _context.Clientes
                    .Where(x => x.Email == atualizarClienteViewModel.Email && x.Id != id)
                    .FirstOrDefaultAsync();

                if (emailDuplicado != null)
                    return BadRequest($"E-mail cadastrado para outro cliente: {atualizarClienteViewModel.Email}");

                var editedCliente = ClienteViewModelToCliente(id, null, atualizarClienteViewModel);

                var idParam = new SqlParameter("@Id", editedCliente.Id);
                var nomeParam = new SqlParameter("@Nome", editedCliente.Nome);
                var emailParam = new SqlParameter("@Email", editedCliente.Email);
                var logotipoParam = new SqlParameter("@Logotipo", editedCliente.Logotipo);
                var logradourosParam = new SqlParameter("@Logradouros", DBNull.Value);
                var operacaoParam = new SqlParameter("@Operacao", "Editar");

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC ClientePROCEDURE @Id, @Nome, @Email, @Logotipo, @Logradouros, @Operacao",
                    idParam, nomeParam, emailParam, logotipoParam, logradourosParam, operacaoParam);

                return Ok(atualizarClienteViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("/Clientes/Visualizar")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Visualizar(string? email)
        {
            try
            {
                var getCliente = new List<Cliente>();
                var cliente = new Cliente();
                if (email != null && email != "")
                {
                    cliente = await _context.Clientes.Include(x => x.Logradouro).FirstOrDefaultAsync(x => x.Email == email);
                    if (cliente == null)
                        return BadRequest("Cliente não encontrado.");

                    getCliente.Add(cliente);
                }
                else
                {
                    getCliente = await _context.Clientes.Include(x => x.Logradouro).ToListAsync();
                }

                var clienteViewModelToCliente = ClientesModelToClienteViewModelList(getCliente);
                return Ok(clienteViewModelToCliente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Route("/Clientes/Remover")]
        [HttpDelete]
        public async Task<ActionResult> Remover(Guid id)
        {
            try
            {
                var cliente = await _context.Clientes
                    .Where(x => x.Id == id)
                    .FirstOrDefaultAsync();

                if (cliente == null)
                    return NotFound($"Cliente com o Id {id} não encontrado.");

                var idParam = new SqlParameter("@Id", id);
                var nomeParam = new SqlParameter("@Nome", cliente.Nome);
                var emailParam = new SqlParameter("@Email", cliente.Email);
                var logotipoParam = new SqlParameter("@Logotipo", cliente.Logotipo);
                var logradourosParam = new SqlParameter("@Logradouros", DBNull.Value);
                var operacaoParam = new SqlParameter("@Operacao", "Remover");

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC ClientePROCEDURE @Id, @Nome, @Email, @Logotipo, @Logradouros, @Operacao",
                    idParam, nomeParam, emailParam, logotipoParam, logradourosParam, operacaoParam);

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private Cliente ClienteViewModelToCliente(Guid? id, ClienteViewModel clienteViewModel, AtualizarClienteViewModel atualizarClienteViewModel)
        {
            var newLogradouraList = new List<Logradouro>();

            if (id == Guid.Empty || id == null)
            {
                var newCliente = new Cliente()
                {
                    Id = Guid.NewGuid(),
                    Nome = clienteViewModel.Nome,
                    Email = clienteViewModel.Email,
                    Logotipo = clienteViewModel.Logotipo,
                };

                foreach (var logradouro in clienteViewModel.Logradouros)
                {
                    var newLogradoura = new Logradouro();
                    newLogradoura.IdLogradouro = logradouro.IdLogradouro.HasValue && logradouro.IdLogradouro.Value != Guid.Empty
                        ? logradouro.IdLogradouro.Value
                        : Guid.NewGuid();
                    newLogradoura.IdCliente = newCliente.Id;
                    newLogradoura.Rua = logradouro.Rua;
                    newLogradoura.Quadra = logradouro.Quadra;
                    newLogradoura.Lote = logradouro.Lote;
                    newLogradoura.Numero = logradouro.Numero;
                    newLogradoura.Bairro = logradouro.Bairro;
                    newLogradouraList.Add(newLogradoura);
                }
                newCliente.Logradouro = newLogradouraList;

                return newCliente;
            }
            else
            {
                var editCliente = _context.Clientes.Find(id);
                editCliente.Nome = atualizarClienteViewModel.Nome;
                editCliente.Email = atualizarClienteViewModel.Email;
                editCliente.Logotipo = atualizarClienteViewModel.Logotipo;

                return editCliente;
            }
        }

        private List<ClienteViewModel> ClientesModelToClienteViewModelList(List<Cliente> clientes)
        {
            var clienteViewModelList = new List<ClienteViewModel>();

            foreach (var cliente in clientes)
            {
                var newLogradouraViewModelList = new List<LogradouroViewModel>();

                foreach (var logradouro in cliente.Logradouro)
                {
                    var newLogradoura = new LogradouroViewModel
                    {
                        IdLogradouro = logradouro.IdLogradouro,
                        IdCliente = cliente.Id,
                        Rua = logradouro.Rua,
                        Quadra = logradouro.Quadra,
                        Lote = logradouro.Lote,
                        Numero = logradouro.Numero,
                        Bairro = logradouro.Bairro
                    };
                    newLogradouraViewModelList.Add(newLogradoura);
                }

                var newClienteViewModel = new ClienteViewModel
                {
                    Id = cliente.Id,
                    Nome = cliente.Nome,
                    Email = cliente.Email,
                    Logotipo = cliente.Logotipo,
                    Logradouros = newLogradouraViewModelList
                };

                clienteViewModelList.Add(newClienteViewModel);
            }

            return clienteViewModelList;
        }
    }
}