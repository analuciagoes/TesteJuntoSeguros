using TesteJuntoSeguros.Data;
using TesteJuntoSeguros.Models;

namespace TesteJuntoSeguros.Services
{
    public class UsuariosNegocio : IUsuariosNegocio
    {
        private readonly IUsuariosRepositorio _usuariosRepositorio;
        private readonly ILogger<UsuariosNegocio> _logger;

        public UsuariosNegocio(IUsuariosRepositorio usuariosRepositorio, ILogger<UsuariosNegocio> logger)
        {
            _usuariosRepositorio = usuariosRepositorio;
            _logger = logger;
        }

        public IEnumerable<Usuario> ListarUsuarios()
        {
            var resultado = _usuariosRepositorio.ListarUsuarios();
            return resultado;
        }

        public Usuario BuscarUsuarioPorId(int id)
        {
            if(id <= 0 )
            {
                throw new Exception("O id não pode ser negativo");
            }

            var resultado = _usuariosRepositorio.BuscarUsuarioPorId(id);
            return resultado;
        }

        public void CriarUsuario(Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.Nome))
            {
                throw new Exception("O nome não pode ser vazio");
            }

            if (string.IsNullOrEmpty(usuario.Email))
            {
                throw new Exception("O email não pode ser vazio");
            }

            if (string.IsNullOrEmpty(usuario.Senha))
            {
                throw new Exception("A senha não pode ser vazio");
            }

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
            _usuariosRepositorio.CriarUsuario(usuario);
        }

        public void EditarUsuario(Usuario usuario)
        {
            if (usuario.Id <= 0)
            {
                throw new Exception("O id não pode ser negativo");
            }

            if (string.IsNullOrEmpty(usuario.Nome))
            {
                throw new Exception("O nome não pode ser vazio");
            }

            if (string.IsNullOrEmpty(usuario.Email))
            {
                throw new Exception("O email não pode ser vazio");
            }

            _usuariosRepositorio.EditarUsuario(usuario);
        }

        public void ApagarUsuario(int id)
        {
            if (id <= 0)
            {
                throw new Exception("O id não pode ser negativo");
            }

            _usuariosRepositorio.ApagarUsuario(id);
        }

        public bool ValidarCredenciais(string email, string senha)
        {
            var usuario = _usuariosRepositorio.BuscarUsuarioPorEmail(email);
            if (usuario == null)
            {
                _logger.LogInformation("Usuario não encontrado");
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(senha, usuario.Senha);
        }

        public bool AlterarSenha(string email, string senhaAtual, string novaSenha)
        {
            var usuario = _usuariosRepositorio.BuscarUsuarioPorEmail(email);
            if (usuario is null)
            {
                _logger.LogInformation("Usuario não encontrado");
                return false;
            }

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(senhaAtual, usuario.Senha))
            {
                return false;
            }

            return _usuariosRepositorio.AtualizarSenha(email, novaSenha);
        }
    }
}
