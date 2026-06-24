namespace badmintion.Interface.Core
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Verify(string password, string? storedPassword);
        bool NeedsRehash(string? storedPassword);
    }
}
