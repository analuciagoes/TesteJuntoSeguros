using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TesteJuntoSeguros.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TesteJuntoSeguros.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        //static List<Usuario> usuarios = new List<Usuario>();
        // GET: api/<UsuariosController>
        [HttpGet]
        public IEnumerable<Usuario> ListarUsuarios()
        {
            var listaUsuarios = new List<Usuario>();
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=TesteJuntoSeguros;Integrated Security=SSPI;AttachDBFilename=C:\Users\ana_g\TesteJuntoSeguros.mdf";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = """
                    SELECT
                         [Nome]
                        ,[Email]
                        ,[Id]
                        ,[Senha]
                    FROM [TesteJuntoSeguros].[dbo].[Usuarios]
                    """;
                var command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                while (reader.Read()) 
                {
                    var nome = reader.GetString(0);
                    var email = reader.GetString(1);
                    var id = reader.GetInt32(2);
                    var senha = reader.GetString(3);
                    var usuario = new Usuario()
                    {
                        Id = id,
                        Nome = nome,
                        Email = email,
                        Senha = senha,
                    };
                    listaUsuarios.Add(usuario);
                }
            }
            return listaUsuarios;
        }

        // GET api/<UsuariosController>/5
        [HttpGet("{id}")]
        public Usuario BuscarUsuarioPorId(int id)
        {
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=TesteJuntoSeguros;Integrated Security=SSPI;AttachDBFilename=C:\Users\ana_g\TesteJuntoSeguros.mdf";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = """
                    SELECT
                         [Nome]
                        ,[Email]
                        ,[Id]
                        ,[Senha]
                    FROM [TesteJuntoSeguros].[dbo].[Usuarios]
                    WHERE Id = @Id
                    """;
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Id", id);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var nome = reader.GetString(0);
                    var email = reader.GetString(1);
                    var senha = reader.GetString(3);
                    var usuario = new Usuario()
                    {
                        Id = id,
                        Nome = nome,
                        Email = email,
                        Senha = senha,
                    };
                    return usuario;
                }
            }
            return null;
        }

        // POST api/<UsuariosController>
        [HttpPost]
        public void CriarUsuario([FromBody] Usuario usuario)
        {
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=TesteJuntoSeguros;Integrated Security=SSPI;AttachDBFilename=C:\Users\ana_g\TesteJuntoSeguros.mdf";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("insert into Usuarios (Nome, Email, Senha) values (@Nome, @Email, @Senha)", connection);
                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@Email", usuario.Email); 
                command.Parameters.AddWithValue("@Senha", usuario.Senha);
                command.ExecuteNonQuery();
            }
        }

        // PUT api/<UsuariosController>/5
        [HttpPut("{id}")]
        public void EditarUsuario(int id, [FromBody] Usuario usuario)
        {
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=TesteJuntoSeguros;Integrated Security=SSPI;AttachDBFilename=C:\Users\ana_g\TesteJuntoSeguros.mdf";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string sql = """
                      UPDATE [dbo].[Usuarios]
                      SET [Nome] = @Nome
                         ,[Email] = @Email
                         ,[Senha] = @Senha
                      WHERE Id = @Id
                    """;
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Senha", usuario.Senha);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        // DELETE api/<UsuariosController>/5
        [HttpDelete("{id}")]
        public void ApagarUsuario(int id)
        {
            string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=TesteJuntoSeguros;Integrated Security=SSPI;AttachDBFilename=C:\Users\ana_g\TesteJuntoSeguros.mdf";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM [TesteJuntoSeguros].[dbo].[Usuarios] WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
