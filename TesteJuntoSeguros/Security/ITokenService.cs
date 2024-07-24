namespace TesteJuntoSeguros.Security
{
    public interface ITokenService
    {
        string GenerateToken(string email);
    }
}
