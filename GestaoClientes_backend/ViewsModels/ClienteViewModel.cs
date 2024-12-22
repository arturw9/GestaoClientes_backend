namespace GestaoClientes_backend.ViewsModels
{
    public class ClienteViewModel
    {
        public Guid? Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Logotipo { get; set; }
        public List<LogradouroViewModel>? Logradouros { get; set; }
    }
}