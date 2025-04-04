import 'package:flutter_secure_storage/flutter_secure_storage.dart';

// AuthService class to manage JWT tokens securely
class AuthService {
  // Instance of FlutterSecureStorage to handle secure storage
  static const FlutterSecureStorage _storage = FlutterSecureStorage();

  // Save token to secure storage
  Future<void> saveToken(String token) async {
    await _storage.write(
      key: "jwt_token",
      value: token,
    ); // Store the token with the key "jwt_token"
  }

  // Delete token from secure storage
  Future<void> deleteToken() async {
    await _storage.delete(
      key: "jwt_token",
    ); // Remove the token associated with the key "jwt_token"
  }

  // Retrieve token from secure storage
  Future<String?> getToken() async {
    return await _storage.read(
      key: "jwt_token",
    ); // Read and return the token stored with the key "jwt_token"
  }
}
