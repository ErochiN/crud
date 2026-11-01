namespace CRDUProject
{
    public interface IUserManager
    {
        Task<IResult> AddUser(HttpContext context);
        Task<bool> IniUser(string email, string password);
        Task<int> GetUserId(string email);
    }
}
