using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Portal.Services.AuthProviders
{
    public static class JwtParser
    {
        private static void ExtractRolesFromJwt(ICollection<Claim> claims, IDictionary<string, object> keyValuePairs)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out var roles);

            if (roles == null) return;

            var parsedRoles = roles.ToString()?.Trim().TrimStart('[').TrimEnd(']').Split(',');
            if (parsedRoles is { Length: >= 1 })
            {
                foreach (var parsedRole in parsedRoles)
                    claims.Add(new Claim(ClaimTypes.Role, parsedRole.Trim('"')));
            }

            keyValuePairs.Remove(ClaimTypes.Role);
        }

        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();

            if (string.IsNullOrEmpty(jwt))
                return claims;

            var items = jwt.Split('.');
            if (items.Length <= 1)
                throw new Exception("It appears the auth token is invalid");

            var payload = items[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);

            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
            if (keyValuePairs == null)
                return claims;

            ExtractRolesFromJwt(claims, keyValuePairs);

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
            return claims;
        }
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
    }
}
