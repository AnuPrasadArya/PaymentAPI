namespace PaymentAPI.Helpers
{
    public static class Helper
    {
        public static string GenerateRefundcode()
        {
            var rnd = new Random();
            return rnd.Next(1000, 9999).ToString();
        }
    }
}
