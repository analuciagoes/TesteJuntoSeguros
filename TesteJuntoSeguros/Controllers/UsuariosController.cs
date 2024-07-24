using Microsoft.AspNetCore.Mvc;
using TesteJuntoSeguros.Models;
using TesteJuntoSeguros.Services;

namespace TesteJuntoSeguros.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuariosNegocio usuariosService;

        public UsuariosController(IConfiguration configuration, IUsuariosNegocio usuariosService) 
        {
            this.usuariosService = usuariosService;
        }

        // GET: api/Usuarios
        [HttpGet]
        public IEnumerable<Usuario> ListarUsuarios()
        {
            var resultado = usuariosService.ListarUsuarios();
            return resultado;
        }

        // GET api/Usuarios/5
        [HttpGet("{id}")]
        public Usuario BuscarUsuarioPorId(int id)
        {
            var resultado = usuariosService.BuscarUsuarioPorId(id);
            return resultado;
        }

        // POST api/Usuarios
        [HttpPost]
        public void CriarUsuario([FromBody] Usuario usuario)
        {
            usuariosService.CriarUsuario(usuario);
        }

        // PUT api/Usuarios/5
        [HttpPut("{id}")]
        public void EditarUsuario(int id, [FromBody] Usuario usuario)
        {
            usuario.Id = id;
            usuariosService.EditarUsuario(usuario);
        }

        // DELETE api/Usuarios/5
        [HttpDelete("{id}")]
        public void ApagarUsuario(int id)
        {
            usuariosService.ApagarUsuario(id);
        }
    }
}
