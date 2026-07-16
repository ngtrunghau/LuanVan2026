namespace badmintion.Interface
{
    public interface IDistrictService
    {
        Task<dynamic> GetAllByIdProvince(int id);
    }
}
