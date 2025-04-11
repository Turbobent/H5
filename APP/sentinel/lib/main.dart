import 'package:sentinel/templates/footerOnlyHome.dart';
import 'package:sentinel/dashboard.dart';
import 'package:flutter/material.dart';
import 'package:sentinel/signup.dart';
import 'package:sentinel/login.dart';

// Entry point of the application
void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false, // Disable the debug banner
      initialRoute: '/', // Set the initial route to the home screen
      routes: {
        '/': (context) => const HomeScreen(), // Home screen route
        '/signup': (context) => const Signup(), // Signup screen route
        '/login': (context) => const Login(), // Login screen route
        '/dashboard': (context) => const Dashboard(), // Dashboard screen
      },
    );
  }
}

class HomeScreen extends StatelessWidget {
  const HomeScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: const Color.fromARGB(255, 0, 0, 0), // Black background
      body: Column(
        mainAxisAlignment:
            MainAxisAlignment.center, // Center content vertically
        children: <Widget>[
          // Display the Sentinel logo
          Image.asset(
            'assets/Sentinel.png',
            height: 200, // Logo height
            width: 500, // Logo width
          ),
          const Align(alignment: Alignment.center), // Center alignment
          const SizedBox(height: 30), // Add spacing between elements
          // Login button
          SizedBox(
            height: 50, // Button height
            width: 140, // Button width
            child: ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color.fromARGB(
                  255,
                  64,
                  92,
                  218,
                ), // Blue button color
                foregroundColor: Colors.white, // White text color
              ),
              child: const Text(
                'Login',
                style: TextStyle(fontSize: 20), // Button text style
              ),
              onPressed: () {
                Navigator.pushNamed(
                  context,
                  '/login',
                ); // Navigate to the login screen
              },
            ),
          ),
          const SizedBox(height: 30), // Add spacing between buttons
          // Signup button
          SizedBox(
            height: 50, // Button height
            width: 140, // Button width
            child: ElevatedButton(
              style: ElevatedButton.styleFrom(
                backgroundColor: const Color.fromARGB(
                  255,
                  64,
                  92,
                  218,
                ), // Blue button color
                foregroundColor: Colors.white, // White text color
              ),
              child: const Text(
                'Sign up',
                style: TextStyle(fontSize: 20), // Button text style
              ),
              onPressed: () {
                Navigator.pushNamed(
                  context,
                  '/signup',
                ); // Navigate to the signup screen
              },
            ),
          ),
        ],
      ),
      bottomNavigationBar: const FooterOnlyHome(), // Footer widget
    );
  }
}
