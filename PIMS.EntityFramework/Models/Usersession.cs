namespace PIMS.EntityFramework.Models
{
    public partial class Usersession
    {
        public Guid UserSessionId { get; set; }
        public int TokenId { get; set; }

        public Guid UserId { get; set; }

        public string Token { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public virtual User User { get; set; } = null!;
    }
}