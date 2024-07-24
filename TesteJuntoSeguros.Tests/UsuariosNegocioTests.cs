
using Moq;
using System;
using System.Collections.Generic;
using TesteJuntoSeguros.Data;
using TesteJuntoSeguros.Models;
using TesteJuntoSeguros.Services;
using Xunit;
using Microsoft.Extensions.Logging;

namespace TesteJuntoSeguros.Tests;

public class UsuariosNegocioTests
{
    private readonly Mock<IUsuariosRepositorio> _mockRepositorio;
    private readonly Mock<ILogger<UsuariosNegocio>> _mockLogger;
    private readonly UsuariosNegocio _negocio;

    public UsuariosNegocioTests()
    {
        _mockRepositorio = new Mock<IUsuariosRepositorio>();
        _mockLogger = new Mock<ILogger<UsuariosNegocio>>();
        _negocio = new UsuariosNegocio(_mockRepositorio.Object, _mockLogger.Object);
    }

    [Fact]
    public void ListarUsuarios_ReturnsListOfUsuarios()
    {
        // Arrange
        var usuarios = new List<Usuario> { new Usuario(), new Usuario() };
        _mockRepositorio.Setup(repo => repo.ListarUsuarios()).Returns(usuarios);

        // Act
        var resultado = _negocio.ListarUsuarios();

        // Assert
        Assert.Equal(usuarios, resultado);
    }

    [Fact]
    public void BuscarUsuarioPorId_ValidId_ReturnsUsuario()
    {
        // Arrange
        var usuario = new Usuario { Id = 1 };
        _mockRepositorio.Setup(repo => repo.BuscarUsuarioPorId(1)).Returns(usuario);

        // Act
        var resultado = _negocio.BuscarUsuarioPorId(1);

        // Assert
        Assert.Equal(usuario, resultado);
    }

    [Fact]
    public void BuscarUsuarioPorId_InvalidId_ThrowsException()
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _negocio.BuscarUsuarioPorId(-1));
        Assert.Equal("O id não pode ser negativo", ex.Message);
    }

    [Fact]
    public void CriarUsuario_ValidUsuario_CreatesUsuario()
    {
        // Arrange
        var usuario = new Usuario { Nome = "User", Email = "user@example.com", Senha = "password" };

        // Act
        _negocio.CriarUsuario(usuario);

        // Assert
        _mockRepositorio.Verify(repo => repo.CriarUsuario(It.Is<Usuario>(u => u.Nome == "User" && u.Email == "user@example.com" )));
    }

    [Theory]
    [InlineData(null, "user@example.com", "password", "O nome não pode ser vazio")]
    [InlineData("User", null, "password", "O email não pode ser vazio")]
    [InlineData("User", "user@example.com", null, "A senha não pode ser vazio")]
    public void CriarUsuario_InvalidUsuario_ThrowsException(string nome, string email, string senha, string expectedMessage)
    {
        // Arrange
        var usuario = new Usuario { Nome = nome, Email = email, Senha = senha };

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _negocio.CriarUsuario(usuario));
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public void EditarUsuario_ValidUsuario_EditsUsuario()
    {
        // Arrange
        var usuario = new Usuario { Id = 1, Nome = "User", Email = "user@example.com", Senha = "password" };

        // Act
        _negocio.EditarUsuario(usuario);

        // Assert
        _mockRepositorio.Verify(repo => repo.EditarUsuario(It.Is<Usuario>(u => u.Id == 1 && u.Nome == "User" && u.Email == "user@example.com" && u.Senha == "password")));
    }

    [Theory]
    [InlineData(0, "User", "user@example.com", "password", "O id não pode ser negativo")]
    [InlineData(1, null, "user@example.com", "password", "O nome não pode ser vazio")]
    [InlineData(1, "User", null, "password", "O email não pode ser vazio")]
    [InlineData(1, "User", "user@example.com", null, "A senha não pode ser vazio")]
    public void EditarUsuario_InvalidUsuario_ThrowsException(int id, string nome, string email, string senha, string expectedMessage)
    {
        // Arrange
        var usuario = new Usuario { Id = id, Nome = nome, Email = email, Senha = senha };

        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _negocio.EditarUsuario(usuario));
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public void ApagarUsuario_ValidId_DeletesUsuario()
    {
        // Act
        _negocio.ApagarUsuario(1);

        // Assert
        _mockRepositorio.Verify(repo => repo.ApagarUsuario(1), Times.Once);
    }

    [Fact]
    public void ApagarUsuario_InvalidId_ThrowsException()
    {
        // Act & Assert
        var ex = Assert.Throws<Exception>(() => _negocio.ApagarUsuario(-1));
        Assert.Equal("O id não pode ser negativo", ex.Message);
    }

    [Fact]
    public void ValidarCredenciais_ValidCredentials_ReturnsTrue()
    {
        // Arrange
        var usuario = new Usuario { Email = "user@example.com", Senha = BCrypt.Net.BCrypt.HashPassword("password") };
        _mockRepositorio.Setup(repo => repo.BuscarUsuarioPorEmail("user@example.com")).Returns(usuario);

        // Act
        var result = _negocio.ValidarCredenciais("user@example.com", "password");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ValidarCredenciais_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        _mockRepositorio.Setup(repo => repo.BuscarUsuarioPorEmail("user@example.com")).Returns((Usuario)null);

        // Act
        var result = _negocio.ValidarCredenciais("user@example.com", "password");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AlterarSenha_ValidCredentials_ChangesPassword()
    {
        // Arrange
        var usuario = new Usuario { Email = "user@example.com", Senha = BCrypt.Net.BCrypt.HashPassword("password") };
        _mockRepositorio.Setup(repo => repo.BuscarUsuarioPorEmail("user@example.com")).Returns(usuario);
        _mockRepositorio.Setup(repo => repo.AtualizarSenha("user@example.com", It.IsAny<string>())).Returns(true);

        // Act
        var result = _negocio.AlterarSenha("user@example.com", "password", "newpassword");

        // Assert
        Assert.True(result);
        _mockRepositorio.Verify(repo => repo.AtualizarSenha("user@example.com", It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void AlterarSenha_InvalidCredentials_ReturnsFalse()
    {
        // Arrange
        _mockRepositorio.Setup(repo => repo.BuscarUsuarioPorEmail("user@example.com")).Returns((Usuario)null);

        // Act
        var result = _negocio.AlterarSenha("user@example.com", "password", "newpassword");

        // Assert
        Assert.False(result);
    }
}

