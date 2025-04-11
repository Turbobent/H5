import 'package:sentinel/templates/footer.dart';
import 'package:sentinel/templates/header.dart';
import 'package:jwt_decoder/jwt_decoder.dart';
import 'package:sentinel/auth_service.dart';
import 'package:http/http.dart' as http;
import 'package:flutter/material.dart';
import 'package:sentinel/login.dart';
import 'dart:convert';

// Base API URL
const String apiURL = 'https://sentinal-api.mercantec.tech/api/';

class Account extends StatefulWidget {
  const Account({super.key});

  @override
  _AccountState createState() => _AccountState();
}

class _AccountState extends State<Account> {
  // Controllers for text fields
  late TextEditingController emailController;
  late TextEditingController usernameController;
  late TextEditingController passwordController;

  // State variables
  bool isPasswordVisible = false; // To toggle password visibility
  bool isChanged = false; // Tracks if any input field has been changed
  String? successMessage; // Message to display on successful operation
  String? errorMessage; // Message to display on error

  @override
  void initState() {
    super.initState();
    // Initialize text controllers
    emailController = TextEditingController();
    usernameController = TextEditingController();
    passwordController = TextEditingController();
    _loadUserData(); // Load user data on initialization
  }

  // Function to load user data from the token
  Future<void> _loadUserData() async {
    String? token = await AuthService().getToken(); // Retrieve token

    if (token != null) {
      print("JWT Token: $token"); // Debug: Print full token

      if (!JwtDecoder.isExpired(token)) {
        // Decode the token if it's not expired
        Map<String, dynamic> decodedToken = JwtDecoder.decode(token);
        print("Decoded Token: $decodedToken"); // Debug: Print decoded token

        setState(() {
          // Populate username from the token
          emailController.text = decodedToken['email'] ?? 'No email in token';
          usernameController.text =
              decodedToken['name'] ?? 'No username in token';
        });
      } else {
        print("Token is expired"); // Debug: Token expired
      }
    } else {
      print("No token found"); // Debug: No token available
    }
  }

  // Function to log out the user
  void logout() async {
    await AuthService().deleteToken(); // Delete the token
    Navigator.pushReplacement(
      context,
      MaterialPageRoute(
        builder: (context) => const Login(), // Redirect to login page
      ),
    );
  }

  // Function to save changes to the user's account
  void saveChanges() async {
    print("saveChanges() started...");

    // Step 1: Retrieve the token
    String? token = await AuthService().getToken();
    if (token == null) {
      print("ERROR: Token is null!"); // Debug: Token not found
      return;
    }
    print("Token retrieved successfully.");

    // Step 2: Decode the token to get user ID
    Map<String, dynamic> decodedToken = JwtDecoder.decode(token);
    print("Decoded token: $decodedToken");

    String? userId =
        decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];
    if (userId == null) {
      print("ERROR: User ID is null!"); // Debug: User ID not found
      return;
    }
    print("User ID extracted: $userId");

    // Step 3: Prepare request data
    Map<String, String> updatedData = {
      "email": emailController.text,
      "username": usernameController.text, // Updated username
      "password": passwordController.text, // Updated password
    };
    print("Updated data: $updatedData");

    // Step 4: Send the API request
    String apiUrl = "${apiURL}Users/$userId"; // Endpoint for updating user
    print("Sending request to: $apiUrl");

    try {
      var response = await http.put(
        Uri.parse(apiUrl),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token", // Include token in headers
        },
        body: jsonEncode(updatedData), // Send updated data as JSON
      );

      print("Response Status Code: ${response.statusCode}");
      print("Response Body: ${response.body}");

      if (response.statusCode == 200 || response.statusCode == 204) {
        // If successful, log out the user
        logout();
      }
    } catch (e) {
      print("ERROR: Exception occurred: $e"); // Debug: Exception occurred
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const Header(),
      body: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.center,
          children: [
            const Text(
              "Profile",
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
                color: const Color.fromARGB(255, 64, 92, 218),
              ),
            ),
            const SizedBox(height: 20),
            Container(
              padding: const EdgeInsets.all(20),
              decoration: BoxDecoration(
                color: Colors.grey[200],
                borderRadius: BorderRadius.circular(10),
              ),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  const Text(
                    "Edit the information you wish to change below to update your account (Password is optional).",
                    style: TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.bold,
                      color: const Color.fromARGB(255, 64, 92, 218),
                    ),
                  ),
                  const SizedBox(height: 10),
                  const Text(
                    "Email",
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: const Color.fromARGB(255, 64, 92, 218),
                    ),
                  ),
                  TextField(
                    controller: emailController,
                    decoration: const InputDecoration(
                      border: OutlineInputBorder(),
                      isDense: true,
                    ),
                    onChanged: (_) => setState(() => isChanged = true),
                  ),
                  const SizedBox(height: 10),
                  const Text(
                    "Username",
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: const Color.fromARGB(255, 64, 92, 218),
                    ),
                  ),
                  TextField(
                    controller: usernameController,
                    decoration: const InputDecoration(
                      border: OutlineInputBorder(),
                      isDense: true,
                    ),
                    onChanged: (_) => setState(() => isChanged = true),
                  ),
                  const SizedBox(height: 10),
                  const Text(
                    "Password",
                    style: TextStyle(
                      fontWeight: FontWeight.bold,
                      color: const Color.fromARGB(255, 64, 92, 218),
                    ),
                  ),
                  TextField(
                    controller: passwordController,
                    obscureText: !isPasswordVisible, // Hide or show password
                    decoration: InputDecoration(
                      border: const OutlineInputBorder(),
                      isDense: true,
                      suffixIcon: IconButton(
                        icon: Icon(
                          isPasswordVisible
                              ? Icons.visibility
                              : Icons.visibility_off,
                        ),
                        onPressed: () {
                          setState(() {
                            isPasswordVisible = !isPasswordVisible;
                          });
                        },
                      ),
                    ),
                    onChanged: (_) => setState(() => isChanged = true),
                  ),
                  const SizedBox(height: 20),
                  ElevatedButton(
                    onPressed: isChanged ? saveChanges : null,
                    style: ElevatedButton.styleFrom(
                      backgroundColor: isChanged ? Colors.blue : Colors.grey,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(vertical: 12),
                    ),
                    child: const Center(child: Text("Save Changes")),
                  ),
                  const SizedBox(height: 20),
                  ElevatedButton(
                    onPressed: logout, // Call logout function
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.red,
                      foregroundColor: Colors.white,
                      padding: const EdgeInsets.symmetric(vertical: 12),
                    ),
                    child: const Center(child: Text("Logout")),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 10),
            if (successMessage != null)
              Text(
                successMessage!,
                style: const TextStyle(
                  color: Colors.green,
                  fontWeight: FontWeight.bold,
                ),
              ),
            if (errorMessage != null)
              Text(
                errorMessage!,
                style: const TextStyle(
                  color: Colors.red,
                  fontWeight: FontWeight.bold,
                ),
              ),
          ],
        ),
      ),
      bottomNavigationBar: const Footer(),
    );
  }
}
