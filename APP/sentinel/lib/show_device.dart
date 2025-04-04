import 'package:sentinel/auth_service.dart';
import 'package:sentinel/templates/footer.dart';
import 'package:sentinel/templates/header.dart';
import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'dart:convert';

class ShowDevice extends StatefulWidget {
  final String deviceId; // The ID of the device to be displayed

  const ShowDevice({super.key, required this.deviceId});

  @override
  _ShowDeviceState createState() => _ShowDeviceState();
}

class _ShowDeviceState extends State<ShowDevice> {
  // Error message to display in case of an error
  String? errorMessage;
  String result = ''; // Result of the API call
  String titleHint = "Fetching title..."; // Placeholder for the device title
  String textHint = "Fetching text..."; // Placeholder for the device text

  @override
  void initState() {
    super.initState();
    _getDevice(); // Fetch device details when the widget is initialized
  }

  // Function to fetch device details from the API
  Future<void> _getDevice() async {
    String? token = await AuthService().getToken(); // Retrieve the token
    if (token == null) return; // Exit if no token is found

    try {
      // Send a GET request to fetch device details
      var response = await http.get(
        Uri.parse(
          'https://sentinal-api.mercantec.tech/api/Devices/${widget.deviceId}',
        ),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token", // Include token in the headers
        },
      );

      if (response.statusCode == 200) {
        // Parse the response body if the request is successful
        var data = jsonDecode(response.body);
        setState(() {
          textHint = data['text'] ?? ''; // Update the text field
          titleHint = data['title'] ?? ''; // Update the title field
        });
      } else {
        // Handle error response
        setState(() {
          errorMessage = "Device failed to load.";
        });
      }
    } catch (e) {
      // Handle exceptions during the API call
      setState(() {
        errorMessage = "An error occurred: $e";
      });
    }
  }

  // Function to display the content of the device in a dialog
  void _showContentDialog(String content) {
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text("Device Content"), // Dialog title
          content: SingleChildScrollView(
            child: SelectableText(
              content, // Display the content
              style: const TextStyle(fontSize: 18, color: Colors.black),
            ),
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop(); // Close the dialog
              },
              child: const Text("OK"),
            ),
          ],
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const Header(), // Custom header widget
      body: Column(
        children: [
          // Row for additional actions (currently empty)
          Row(mainAxisAlignment: MainAxisAlignment.end, children: []),
          // Display device details
          Padding(
            padding: const EdgeInsets.all(16.0),
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                // Display the device title
                Text(
                  titleHint,
                  style: const TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.bold,
                  ),
                ),
                const SizedBox(height: 10),
                // Display the device text
                Text(textHint, style: const TextStyle(fontSize: 16)),
                const SizedBox(height: 20),
                // Show content button
                ElevatedButton(
                  onPressed: () {
                    _showContentDialog(
                      textHint,
                    ); // Show the content in a dialog
                  },
                  child: const Text("Show Content"),
                ),
                // Display error message if any
                if (errorMessage != null)
                  Text(
                    errorMessage!,
                    style: const TextStyle(color: Colors.red),
                  ),
              ],
            ),
          ),
        ],
      ),
      bottomNavigationBar: const Footer(), // Custom footer widget
    );
  }
}
