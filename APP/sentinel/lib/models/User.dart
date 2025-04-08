class User {
  final String email;
  final String username;
  final String hashedPassword;
  final String salt;

  User({
    required this.email,
    required this.username,
    required this.hashedPassword,
    required this.salt,
  });
}

class Login {
  final String username;
  final String password;

  Login({
    required this.username,
    required this.password,
  });
}

class Signup {
  final String email;
  final String username;
  final String password;

  Signup({
    required this.email,
    required this.username,
    required this.password,
  });
}

class Edit {
  final String email;
  final String username;
  final String password;

  Edit({
    required this.email,
    required this.username,
    required this.password,
  });
}