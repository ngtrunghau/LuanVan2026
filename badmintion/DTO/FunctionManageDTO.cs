namespace badmintion.DTO
{
    public class FunctionManageDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Router { get; set; } = null!;

        public int ControllerId { get; set; }

        public bool? IsDeleted { get; set; }

        public int UnitRoleId { get; set; }

        public string? Key { get; set; }
    }
}
