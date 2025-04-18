import 'package:sentinel/auth_service.dart';
import 'package:sentinel/templates/footerOnlyHome.dart';
import 'package:http/http.dart' as http;
import 'package:flutter/material.dart';
import 'dart:convert';

// API endpoint for user signup
String apiURL = 'https://sentinal-api.mercantec.tech/api/Users/signUp';

class Signup extends StatefulWidget {
  const Signup({super.key});

  @override
  _SignupState createState() => _SignupState();
}

class _SignupState extends State<Signup> {
  // Controllers for text fields
  final TextEditingController emailController = TextEditingController();
  final TextEditingController usernameController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  // Variable to store the result of the API call
  String result = '';

  @override
  void dispose() {
    // Dispose controllers to free up resources
    usernameController.dispose();
    emailController.dispose();
    passwordController.dispose();
    super.dispose();
  }

  // Function to send a POST request to the signup API
  Future<void> _postUser() async {
    try {
      // Build the request body with user input
      Map<String, dynamic> body = {
        'email': emailController.text.trim(), // Still required for signup
        'username': usernameController.text.trim(),
        'password': passwordController.text.trim(),
      };

      // Send the POST request to the signup API
      final response = await http.post(
        Uri.parse(apiURL),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(body),
      );

      // Handle the response
      if (response.statusCode == 200 || response.statusCode == 201) {
        // Success response
        setState(() {
          result = 'Success!';
        });

        // Automatically log the user in using username and password
        await _loginUser(body['username'], body['password']);
      } else {
        // Handle error response
        try {
          final Map<String, dynamic> errorData = jsonDecode(response.body);
          final Map<String, dynamic> errors = errorData['errors'];

          String formattedErrors = errors.entries
              .map((entry) => '${entry.key}: ${entry.value}')
              .join('\n');

          setState(() {
            result = 'Signup Failed:\n$formattedErrors';
          });
        } catch (e) {
          setState(() {
            result = 'Signup Failed: ${response.body}';
          });
        }
      }
    } catch (e) {
      setState(() {
        result = 'Error: $e';
      });
    }
  }

  Future<void> _loginUser(String username, String password) async {
    try {
      // Build the request body for login
      Map<String, dynamic> body = {'username': username, 'password': password};

      // Send the POST request to the login API
      final response = await http.post(
        Uri.parse('https://sentinal-api.mercantec.tech/api/Users/login'),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(body),
      );

      // Debug: Log the response
      print("Login Response: ${response.statusCode}");

      // Handle the response
      if (response.statusCode == 200) {
        // Parse the response to get the token
        final Map<String, dynamic> responseData = jsonDecode(response.body);
        String token = responseData['token'];

        // Save the token securely using AuthService
        final authService = AuthService();
        await authService.saveToken(token);

        // Navigate to the Dashboard screen
        Navigator.pushReplacementNamed(context, '/dashboard');
      } else {
        // Handle login failure
        setState(() {
          result = 'Login Failed: ${response.body}';
        });
      }
    } catch (e) {
      setState(() {
        result = 'Error during login: $e';
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color.fromARGB(255, 0, 0, 0),
      body: SingleChildScrollView(
        child: Container(
          padding: const EdgeInsets.symmetric(horizontal: 40),
          width: double.infinity,
          child: Column(
            mainAxisAlignment: MainAxisAlignment.spaceEvenly,
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: <Widget>[
              // Logo and title section
              const Column(
                children: <Widget>[
                  SizedBox(height: 60.0),
                  Image(
                    image: AssetImage('assets/Sentinel_logo.png'),
                    height: 70,
                  ),
                  Text(
                    "Sign up",
                    style: TextStyle(
                      color: Color.fromARGB(255, 64, 92, 218),
                      fontSize: 30,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  SizedBox(height: 10),
                ],
              ),
              const SizedBox(height: 20),
              // Signup form container
              Center(
                child: Container(
                  width: 320, // Width of the form container
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(18),
                    boxShadow: [
                      BoxShadow(
                        color: Colors.grey.withOpacity(0.5),
                        spreadRadius: 5,
                        blurRadius: 7,
                        offset: const Offset(0, 3), // Shadow position
                      ),
                    ],
                  ),
                  padding: const EdgeInsets.all(20),
                  child: Column(
                    children: <Widget>[
                      // Email input field
                      TextField(
                        controller: emailController,
                        decoration: InputDecoration(
                          hintText: "Email",
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(18),
                            borderSide: BorderSide.none,
                          ),
                          fillColor: Colors.blue.withOpacity(0.1),
                          filled: true,
                          prefixIcon: const Icon(Icons.email),
                        ),
                      ),
                      const SizedBox(height: 20),
                      // Username input field
                      TextField(
                        controller: usernameController,
                        decoration: InputDecoration(
                          hintText: "Username",
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(18),
                            borderSide: BorderSide.none,
                          ),
                          fillColor: Colors.blue.withOpacity(0.1),
                          filled: true,
                          prefixIcon: const Icon(Icons.person),
                        ),
                      ),
                      const SizedBox(height: 20),
                      // Password input field
                      TextField(
                        controller: passwordController,
                        decoration: InputDecoration(
                          hintText: "Password",
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(18),
                            borderSide: BorderSide.none,
                          ),
                          fillColor: Colors.blue.withOpacity(0.1),
                          filled: true,
                          prefixIcon: const Icon(Icons.password),
                        ),
                        obscureText: true, // Hide password input
                      ),
                      const SizedBox(height: 20.0),
                      // Display result message
                      Text(result, style: const TextStyle(fontSize: 16.0)),
                      const SizedBox(height: 20),
                      // Signup button
                      SizedBox(
                        width: double.infinity,
                        child: ElevatedButton(
                          onPressed: _postUser, // Call the signup function
                          style: ElevatedButton.styleFrom(
                            shape: const StadiumBorder(),
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            backgroundColor: const Color.fromARGB(
                              255,
                              64,
                              92,
                              218,
                            ),
                            foregroundColor: Colors.white,
                          ),
                          child: const Text(
                            "Sign up",
                            style: TextStyle(fontSize: 20),
                          ),
                        ),
                      ),
                      const SizedBox(height: 20),
                      // Login prompt
                      const Text(
                        "Already have an account?\n"
                        "Click login to sign in",
                        textAlign: TextAlign.center,
                        style: TextStyle(fontSize: 16, color: Colors.black54),
                      ),
                      const SizedBox(height: 20),
                      // Login button
                      SizedBox(
                        width: double.infinity,
                        child: ElevatedButton(
                          onPressed: () {
                            Navigator.pushNamed(context, '/login');
                          },
                          style: ElevatedButton.styleFrom(
                            shape: const StadiumBorder(),
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            backgroundColor: const Color.fromARGB(
                              255,
                              64,
                              92,
                              218,
                            ),
                            foregroundColor: Colors.white,
                          ),
                          child: const Text(
                            "Login",
                            style: TextStyle(fontSize: 20),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
      bottomNavigationBar: const FooterOnlyHome(), // Footer widget
    );
  }
}
