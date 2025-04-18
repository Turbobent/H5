﻿namespace API.Models
{
    public class User : Common
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string HashedPassword { get; set; }
        public string Salt { get; set; }
    }

    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Signup
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Edit
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
