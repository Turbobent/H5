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

  factory User.fromJson(Map<String, dynamic> json) {
    return User(
      email: json['email'],
      username: json['username'],
      hashedPassword: json['hashedPassword'],
      salt: json['salt'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'email': email,
      'username': username,
      'hashedPassword': hashedPassword,
      'salt': salt,
    };
  }
}

// Endpoints

// GET /api/Users
// GET /api/Users/{id}
// PUT /api/Users/{id}
// DELETE /api/Users/{id}
// POST /api/Users/signUp
// POST /api/Users/login