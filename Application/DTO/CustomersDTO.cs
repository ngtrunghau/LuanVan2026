namespace badmintion.DTO
{
    public class CustomersDTO
    {
        public int Id { get; set; }

        public string FullName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;

        public bool? IsDeleted { get; set; }
    }
}
