using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IServices;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class TokenHelper : ITokenHelper
    {
        private readonly string _secretKey;
        private readonly ILogger<TokenHelper> _logger;

        public TokenHelper(ILogger<TokenHelper> logger)
        {
            _secretKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("new_secret_key_of_at_least_32_characters_long"));
            _logger = logger;
        }

        public long GetUserIdFromToken(string token)
        {
            try
            {
                _logger.LogInformation("Starting token validation.");
                var handler = new JwtSecurityTokenHandler();
                var key = Convert.FromBase64String(_secretKey);

                _logger.LogInformation("Validating token: {Token}", token);
                var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                
                foreach (var claim in claimsPrincipal.Claims)
                {
                    _logger.LogInformation("Claim Type: {ClaimType}, Claim Value: {ClaimValue}", claim.Type, claim.Value);
                }

               
                var jwtToken = validatedToken as JwtSecurityToken;
                if (jwtToken == null)
                {
                    _logger.LogError("Invalid token: Not a JWT token.");
                    throw new UnauthorizedAccessException("Invalid token: Not a JWT token.");
                }
                
                var userIdClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;
                _logger.LogInformation("User ID Claim: {UserIdClaim}", userIdClaim);


                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogError("Invalid token: User ID claim is missing.");
                    throw new UnauthorizedAccessException("Invalid token: User ID claim is missing.");
                }

                if (!long.TryParse(userIdClaim, out var userId))
                {
                    _logger.LogError("Invalid token: User ID is not a valid long value.");
                    throw new UnauthorizedAccessException("Invalid token: User ID is not a valid long value.");
                }

                _logger.LogInformation("Token validated successfully for user ID: {UserId}", userId);
                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed.");
                throw;
            }
        }
    }
}




