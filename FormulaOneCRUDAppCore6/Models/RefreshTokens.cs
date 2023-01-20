namespace FormulaOneCRUDAppCore6.Models
{
    public class RefreshToken
    {
        public int ID { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public string JwtId { get; set; }//this will contain all the ID of the previous token present in subject of GenerateJwtToken method
        public bool IsUsed { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
