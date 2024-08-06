namespace IServices;

public interface ITokenHelper
{
    long GetUserIdFromToken(string token);
}
