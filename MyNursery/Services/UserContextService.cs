using Microsoft.AspNetCore.Http;

namespace MyNursery.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession Session => _httpContextAccessor.HttpContext?.Session!;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? FullName
        {
            get => Session.GetString("UserFullName");
            set => Session.SetString("UserFullName", value ?? string.Empty);
        }

        public string? Email
        {
            get => Session.GetString("UserEmail");
            set => Session.SetString("UserEmail", value ?? string.Empty);
        }

        public string? Role
        {
            get => Session.GetString("UserRole");
            set => Session.SetString("UserRole", value ?? string.Empty);
        }

        public string? Area
        {
            get => Session.GetString("Area");
            set => Session.SetString("Area", value ?? string.Empty);
        }
    }
}
