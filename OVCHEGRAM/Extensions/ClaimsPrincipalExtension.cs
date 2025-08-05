using System.Security.Claims;

namespace OVCHEGRAM.Extensions;

public static class ClaimsPrincipalExtension
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        return int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier));
    }
}