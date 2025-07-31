namespace MyNursery.Services
{
    public interface IUserContextService
    {
        string? FullName { get; set; }
        string? Email { get; set; }
        string? Role { get; set; }
        string? Area { get; set; }
    }
}
