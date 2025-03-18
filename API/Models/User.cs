﻿namespace API.Models
{
    public class User : Common
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class Signup
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
