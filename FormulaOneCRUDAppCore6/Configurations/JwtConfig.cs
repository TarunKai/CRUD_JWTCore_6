﻿namespace FormulaOneCRUDAppCore6.Configurations
{
    public class JwtConfig
    {
        public string SecretKey { get; set; }
        public TimeSpan ExpiryTimeFrame { get; set; }
    }
}
