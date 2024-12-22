using System.ComponentModel.DataAnnotations;

namespace GestaoClientes_backend.Models
{
    public class Cliente
    {
        [Key]
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Logotipo { get; set; }
        public ICollection<Logradouro> Logradouro { get; set; }
    }
}