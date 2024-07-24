using Microsoft.AspNetCore.Mvc;
using TesteJuntoSeguros.Models;

namespace TesteJuntoSeguros.Services
{
    public interface IUsuariosNegocio
    {
        IEnumerable<Usuario> ListarUsuarios();
        Usuario BuscarUsuarioPorId(int id);
        void CriarUsuario(Usuario usuario);
        void EditarUsuario(Usuario usuario);
        void ApagarUsuario(int id);
        bool ValidarCredenciais(string email, string senha);
        bool AlterarSenha(string email, string currentPassword, string newPassword);
    }
}