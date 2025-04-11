import 'package:sentinel/templates/footerOnlyHome.dart';
import 'package:sentinel/auth_service.dart';
import 'package:sentinel/dashboard.dart';
import 'package:http/http.dart' as http;
import 'package:flutter/material.dart';
import 'dart:convert';

// API endpoint for user login
String apiURL = 'https://sentinal-api.mercantec.tech/api/Users/login';

class Login extends StatefulWidget {
  const Login({super.key});

  @override
  _LoginState createState() => _LoginState();
}

class _LoginState extends State<Login> {
  final AuthService _authService =
      AuthService(); // Instance of AuthService to manage tokens
  final TextEditingController usernameController =
      TextEditingController(); // Controller for username input
  final TextEditingController passwordController =
      TextEditingController(); // Controller for password input
  String result = ''; // To store the result from the API call

  @override
  void dispose() {
    // Dispose controllers to free up resources
    usernameController.dispose();
    passwordController.dispose();
    super.dispose();
  }

  // Function to send a POST request to the login API
  Future<void> _postUser() async {
    try {
      // Build the request body with user input
      Map<String, dynamic> body = {
        'username': usernameController.text.trim(),
        'password': passwordController.text.trim(),
      };

      // Send the POST request
      final response = await http.post(
        Uri.parse(apiURL),
        headers: {'Content-Type': 'application/json'},
        body: jsonEncode(body),
      );

      // Debug: Log the response
      print("Login Response: ${response.statusCode}");

      // Handle the response
      if (response.statusCode == 200 || response.statusCode == 201) {
        // Parse the response and extract the token
        final responseData = jsonDecode(response.body);
        String? token = responseData['token'];

        if (token != null) {
          // Save the token securely
          await _authService.saveToken(token);

          setState(() {
            result = 'Login Successful!';
          });

          // Navigate to the Dashboard screen
          Navigator.pushReplacement(
            context,
            MaterialPageRoute(builder: (context) => const Dashboard()),
          );
        }
      } else {
        // Handle error responses
        try {
          final Map<String, dynamic> errorData = jsonDecode(response.body);

          if (errorData.containsKey('message')) {
            setState(() {
              result = 'Login Failed: ${errorData['message']}';
            });
          } else {
            setState(() {
              result = 'Login Failed: ${response.body}';
            });
          }
        } catch (e) {
          setState(() {
            result = 'Login Failed: ${response.body}';
          });
        }
      }
    } catch (e) {
      setState(() {
        result = 'Error: $e';
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color.fromARGB(255, 0, 0, 0), // Black background
      body: SingleChildScrollView(
        child: Container(
          padding: const EdgeInsets.symmetric(
            horizontal: 40,
          ), // Horizontal padding
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
                    height: 70, // Logo height
                  ),
                  SizedBox(height: 20.0),
                  Text(
                    "Login",
                    style: TextStyle(
                      color: Color.fromARGB(
                        255,
                        64,
                        92,
                        218,
                      ), // Blue text color
                      fontSize: 30,
                      fontWeight: FontWeight.bold,
                    ),
                  ),
                  SizedBox(height: 20),
                ],
              ),
              const SizedBox(height: 20),
              // Login form container
              Center(
                child: Container(
                  width: 320, // Width of the form container
                  decoration: BoxDecoration(
                    color: Colors.white, // White background
                    borderRadius: BorderRadius.circular(18), // Rounded corners
                    boxShadow: [
                      BoxShadow(
                        color: Colors.grey.withOpacity(0.5), // Shadow color
                        spreadRadius: 5,
                        blurRadius: 7,
                        offset: Offset(0, 3), // Shadow position
                      ),
                    ],
                  ),
                  padding: const EdgeInsets.all(
                    20,
                  ), // Padding inside the container
                  child: Column(
                    children: <Widget>[
                      // Username input field
                      TextField(
                        controller: usernameController,
                        decoration: InputDecoration(
                          hintText: "Username",
                          border: OutlineInputBorder(
                            borderRadius: BorderRadius.circular(18),
                            borderSide: BorderSide.none,
                          ),
                          fillColor: Colors.blue.withOpacity(
                            0.1,
                          ), // Light blue background
                          filled: true,
                          prefixIcon: const Icon(
                            Icons.person,
                          ), // Icon for username
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
                          fillColor: Colors.blue.withOpacity(
                            0.1,
                          ), // Light blue background
                          filled: true,
                          prefixIcon: const Icon(
                            Icons.password,
                          ), // Icon for password
                        ),
                        obscureText: true, // Hide password input
                      ),
                      const SizedBox(height: 20.0),
                      // Display result message
                      Text(result, style: const TextStyle(fontSize: 16.0)),
                      const SizedBox(height: 20),
                      // Login button
                      SizedBox(
                        width: double.infinity,
                        child: ElevatedButton(
                          onPressed: _postUser, // Call the login function
                          style: ElevatedButton.styleFrom(
                            shape: const StadiumBorder(),
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            backgroundColor: const Color.fromARGB(
                              255,
                              64,
                              92,
                              218,
                            ), // Blue button color
                            foregroundColor: Colors.white, // White text color
                          ),
                          child: const Text(
                            "Login",
                            style: TextStyle(fontSize: 20),
                          ),
                        ),
                      ),
                      const SizedBox(height: 20),
                      // Signup prompt
                      const Text(
                        "Don't have a user?\n"
                        "Click sign up to create a user",
                        textAlign: TextAlign.center,
                        style: TextStyle(fontSize: 16, color: Colors.black54),
                      ),
                      const SizedBox(height: 20),
                      // Signup button
                      SizedBox(
                        width: double.infinity,
                        child: ElevatedButton(
                          onPressed: () {
                            Navigator.pushNamed(
                              context,
                              '/signup',
                            ); // Navigate to signup page
                          },
                          style: ElevatedButton.styleFrom(
                            shape: const StadiumBorder(),
                            padding: const EdgeInsets.symmetric(vertical: 16),
                            backgroundColor: const Color.fromARGB(
                              255,
                              64,
                              92,
                              218,
                            ), // Blue button color
                            foregroundColor: Colors.white, // White text color
                          ),
                          child: const Text(
                            "Sign Up",
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
