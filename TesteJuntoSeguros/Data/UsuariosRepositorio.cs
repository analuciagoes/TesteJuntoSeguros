using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using TesteJuntoSeguros.Models;

namespace TesteJuntoSeguros.Data
{

    public class UsuariosRepositorio : IUsuariosRepositorio
    {
        private readonly string _connectionString;
        private readonly ILogger<UsuariosRepositorio> _logger;
        private readonly Func<IDbConnection> _connectionFactory;

        public UsuariosRepositorio(IConfiguration configuration, ILogger<UsuariosRepositorio> logger)
        {
            _connectionString = configuration.GetConnectionString("UsuariosDatabase");
            _logger = logger;
        }
        public IEnumerable<Usuario> ListarUsuarios()
        {
            var listaUsuarios = new List<Usuario>();

            using (var connection = new SqlConnection(_connectionString))
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

        public Usuario BuscarUsuarioPorId(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
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

        public void CriarUsuario(Usuario usuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("insert into Usuarios (Nome, Email, Senha) values (@Nome, @Email, @Senha)", connection);
                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Senha", usuario.Senha);
                command.ExecuteNonQuery();
            }
        }

        public void EditarUsuario(Usuario usuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = """
                      UPDATE [dbo].[Usuarios]
                      SET [Nome] = @Nome
                         ,[Email] = @Email
                      WHERE Id = @Id
                    """;
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Nome", usuario.Nome);
                command.Parameters.AddWithValue("@Email", usuario.Email);
                command.Parameters.AddWithValue("@Id", usuario.Id);
                command.ExecuteNonQuery();
            }
        }

        public void ApagarUsuario(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DELETE FROM [TesteJuntoSeguros].[dbo].[Usuarios] WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        public Usuario BuscarUsuarioPorEmail(string email)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string sql = """
                    SELECT
                         [Nome]
                        ,[Email]
                        ,[Id]
                        ,[Senha]
                    FROM [TesteJuntoSeguros].[dbo].[Usuarios]
                    WHERE Email = @Email
                    """;
                var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@Email", email);
                var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var nome = reader.GetString(0);
                    var id = reader.GetInt32(2);
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

        public bool AtualizarSenha(string email, string novaSenha)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(novaSenha);
                string updateQuery = "UPDATE Usuarios SET Senha = @Senha WHERE Email = @Email";
                using (SqlCommand command = new SqlCommand(updateQuery, connection))
                {
                    command.Parameters.Add("@Email", SqlDbType.VarChar).Value = email;
                    command.Parameters.Add("@Senha", SqlDbType.VarChar).Value = newHashedPassword;

                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }
    }
}
