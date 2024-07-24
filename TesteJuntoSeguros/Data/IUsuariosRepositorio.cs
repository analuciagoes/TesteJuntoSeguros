using Microsoft.AspNetCore.Mvc;
using TesteJuntoSeguros.Models;

namespace TesteJuntoSeguros.Data
{
    public interface IUsuariosRepositorio
    {
        IEnumerable<Usuario> ListarUsuarios();
        Usuario BuscarUsuarioPorId(int id);
        Usuario BuscarUsuarioPorEmail(string email);
        void CriarUsuario(Usuario usuario);
        void EditarUsuario(Usuario usuario);
        void ApagarUsuario(int id);
        bool AtualizarSenha(string email, string novaSenha);
    }
}
