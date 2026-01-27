namespace badmintion.Interface
{
    public interface ITownService
    {
        Task<dynamic> GetAllByIdDistrict(int id);
    }
}
