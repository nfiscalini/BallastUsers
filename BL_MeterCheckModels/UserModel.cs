namespace BL_MeterCheckModels
{
    public class UserModel
    {
        public int User_id { get; set; }
        public string? Names { get; set; }
        public string? Lastname { get; set; }
        public string? Loginname { get; set; }
        public string? Password { get; set; }
        public DateTime? Created { get; set; }
    }
}