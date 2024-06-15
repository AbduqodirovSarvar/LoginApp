using System.Security.Claims;

namespace LoginApp.Services.Security;

public class CurrentUserService
{
    public Guid UserId { get; set; }
    public CurrentUserService(IHttpContextAccessor _contextAccessor)
    {
        var httpContext = _contextAccessor.HttpContext;
        var userClaims = httpContext?.User.Claims;
        var idClaim = userClaims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
        if (idClaim != null && Guid.TryParse(idClaim.Value, out Guid value))
        {
            UserId = value;
        }
    }
}
