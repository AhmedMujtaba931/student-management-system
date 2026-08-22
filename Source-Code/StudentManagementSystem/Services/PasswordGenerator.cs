namespace StudentManagementSystem.Services
{
    public static class PasswordGenerator
    {
        private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string Lower = "abcdefghijkmnpqrstuvwxyz";
        private const string Digits = "23456789";
        private const string Symbols = "!@#$%*";

        public static string Generate()
        {
            var random = Random.Shared;
            var all = Upper + Lower + Digits + Symbols;

            var chars = new List<char>
            {
                Upper[random.Next(Upper.Length)],
                Lower[random.Next(Lower.Length)],
                Digits[random.Next(Digits.Length)],
                Symbols[random.Next(Symbols.Length)]
            };

            for (int i = 0; i < 6; i++)
            {
                chars.Add(all[random.Next(all.Length)]);
            }

            return new string(chars.OrderBy(_ => random.Next()).ToArray());
        }
    }
}