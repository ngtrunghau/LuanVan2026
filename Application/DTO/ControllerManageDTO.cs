namespace badmintion.DTO
{
    public class ControllerManageDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public bool? IsDeleted { get; set; }

        public string? Key { get; set; }
    }
}
