
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using TesteJuntoSeguros;
using TesteJuntoSeguros.Models;
using TesteJuntoSeguros.Services;
using Xunit;

namespace TesteJuntoSeguros.Tests;

public class UsuariosControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly Mock<IUsuariosNegocio> _mockUsuariosService;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public UsuariosControllerTests(WebApplicationFactory<Program> factory)
    {
        _mockUsuariosService = new Mock<IUsuariosNegocio>();
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton(typeof(IUsuariosNegocio), _mockUsuariosService.Object);
            });
        });
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task ListarUsuarios_ReturnsSuccessStatusCode()
    {
        // Arrange
        var usuarios = new List<Usuario>
        {
            new Usuario { Id = 1, Nome = "Usuario 1", Email = "usuario1@teste.com", Senha = "senha" },
            new Usuario { Id = 2, Nome = "Usuario 2", Email = "usuario2@teste.com", Senha = "senha" }
        };

        _mockUsuariosService.Setup(service => service.ListarUsuarios()).Returns(usuarios);

        // Act
        var response = await _client.GetAsync("/api/usuarios");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var usuariosReponse = JsonSerializer.Deserialize<List<Usuario>>(responseString, _jsonSerializerOptions);
        Assert.Equal(usuarios.Count(), usuariosReponse.Count());
    }

    [Fact]
    public async Task BuscarUsuarioPorId_ReturnsSuccessStatusCode()
    {
        // Arrange
        var usuario = new Usuario { Id = 1, Nome = "Usuario 1", Email = "usuario1@teste.com", Senha = "senha" };
        _mockUsuariosService.Setup(service => service.BuscarUsuarioPorId(1)).Returns(usuario);

        // Act
        var response = await _client.GetAsync("/api/usuarios/1");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var usuarioReponse = JsonSerializer.Deserialize<Usuario>(responseString, _jsonSerializerOptions);
        Assert.Equal(usuario.Nome, usuarioReponse.Nome);
    }

    [Fact]
    public async Task CriarUsuario_ReturnsSuccessStatusCode()
    {
        // Arrange
        var usuario = new Usuario { Nome = "Usuario 1", Email = "usuario1@teste.com", Senha = "senha" };
        var content = new StringContent(JsonSerializer.Serialize(usuario), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/usuarios", content);

        // Assert
        response.EnsureSuccessStatusCode();
        _mockUsuariosService.Verify(service => service.CriarUsuario(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task EditarUsuario_ReturnsSuccessStatusCode()
    {
        // Arrange
        var userId = 1;
        var usuario = new Usuario { Id = userId, Nome = "Usuario 1", Email = "usuario1@teste.com", Senha = "senha" };
        var content = new StringContent(JsonSerializer.Serialize(usuario), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync($"/api/usuarios/{userId}", content);

        // Assert
        response.EnsureSuccessStatusCode();
        _mockUsuariosService.Verify(service => service.EditarUsuario(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task ApagarUsuario_ReturnsSuccessStatusCode()
    {
        // Arrange
        var userId = 1;

        // Act
        var response = await _client.DeleteAsync($"/api/usuarios/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        _mockUsuariosService.Verify(service => service.ApagarUsuario(userId), Times.Once);
    }

}
