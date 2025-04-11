import 'dart:convert';

import 'package:intl/intl.dart';

import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:jwt_decoder/jwt_decoder.dart';

import 'package:sentinel/templates/header.dart';
import 'package:sentinel/templates/footer.dart';

import 'auth_service.dart';

import 'models/User_Device.dart';

const String apiURL = 'https://sentinal-api.mercantec.tech/api/';

class Dashboard extends StatefulWidget {
  const Dashboard({super.key});

  @override
  DashboardState createState() => DashboardState();
}

class DashboardState extends State<Dashboard> {
  String? errorMessage;
  List<Map<String, String>> userDevices = [];

  @override
  void initState() {
    super.initState();
    _fetchUserDevices();
  }

  // Fetch the user's devices
  Future<void> _fetchUserDevices() async {
    // Get the token from secure storage
    String? token = await AuthService().getToken();

    // Debugging: Print the token to the console
    print("Token: $token");

    // Check if the token is null
    if (token == null) {
      setState(() {
        errorMessage = "Token is null. Please log in again.";
      });
      return;
    }

    // Check if the token is expired
    if (JwtDecoder.isExpired(token)) {
      setState(() {
        errorMessage = "Token has expired. Please log in again.";
      });
      return;
    }

    // Decode the token and extract the userId and username
    Map<String, dynamic> decodedToken = JwtDecoder.decode(token);
    int userId = int.parse(
      decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
          .toString(),
    );
    String username = decodedToken["name"] ?? "Unknown User";

    try {
      // Fetch user-device mappings
      var userDeviceResponse = await http.get(
        Uri.parse('${apiURL}User_Device/device-ids/$userId'),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        },
      );
      print("User Device Response: ${userDeviceResponse.body}");

      if (userDeviceResponse.statusCode == 200) {
        // Parse the response as a list of device IDs
        List<String> userDeviceIds = List<String>.from(
          jsonDecode(userDeviceResponse.body),
        );

        // Fetch details for each device
        List<Map<String, String>> devices = [];
        for (String deviceId in userDeviceIds) {
          var deviceResponse = await http.get(
            Uri.parse('${apiURL}Devices/$deviceId'),
            headers: {
              "Content-Type": "application/json",
              "Authorization": "Bearer $token",
            },
          );
          if (deviceResponse.statusCode == 200) {
            Map<String, dynamic> deviceData = jsonDecode(deviceResponse.body);
            devices.add({
              "deviceId": deviceData["deviceId"]?.toString() ?? "",
              "deviceName": deviceData["name"]?.toString() ?? "Unknown",
              "updatedAt": deviceData["updatedAt"]?.toString() ?? "",
            });
          } else {
            print("Failed to fetch details for device ID: $deviceId");
          }
        }

        setState(() {
          userDevices = devices;

          // Sort devices by updatedAt in descending order
          userDevices.sort((a, b) {
            DateTime updatedAtA = DateTime.parse(
              a["updatedAt"] ?? "1970-01-01T00:00:00Z",
            );
            DateTime updatedAtB = DateTime.parse(
              b["updatedAt"] ?? "1970-01-01T00:00:00Z",
            );
            return updatedAtB.compareTo(updatedAtA);
          });
        });
      } else {
        setState(() {
          errorMessage = "No devices found for user: $username";
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = "Exception: $e";
      });
    }
  }

  // Fetch details of a specific device
  Future<Map<String, dynamic>?> _fetchDeviceDetails(String deviceId) async {
    // Get the token from secure storage
    String? token = await AuthService().getToken();

    try {
      var response = await http.get(
        Uri.parse('${apiURL}Devices/$deviceId'),
        headers: {
          "Content-Type": "application/json",
          "Authorization": "Bearer $token",
        },
      );
      print("Device Details Response for $deviceId: ${response.body}");

      if (response.statusCode == 200) {
        return jsonDecode(response.body);
      } else {
        print("Failed to fetch device details: ${response.body}");
        return null;
      }
    } catch (e) {
      print("Exception: $e");
      return null;
    }
  }

  // Show device details in a dialog
  void _showDeviceDetails(String deviceId) async {
    Map<String, dynamic>? deviceDetails = await _fetchDeviceDetails(deviceId);

    if (deviceDetails != null) {
      print("Device Details: $deviceDetails");

      // Parse and format the updatedAt date
      String formattedDate = "Unknown";
      if (deviceDetails["updatedAt"] != null &&
          deviceDetails["updatedAt"]!.isNotEmpty) {
        try {
          DateTime updatedAt = DateTime.parse(deviceDetails["updatedAt"]!);
          formattedDate = DateFormat(
            'd MMMM yyyy, HH:mm', // European style with 24-hour clock
          ).format(updatedAt);
        } catch (e) {
          print("Error parsing date: $e");
        }
      }

      // Show the device details in a dialog
      showDialog(
        context: context,
        builder: (BuildContext context) {
          return AlertDialog(
            title: Text(deviceDetails["title"] ?? "Device Details"),
            content: Column(
              mainAxisSize: MainAxisSize.min,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text("Status: ${deviceDetails["status"] ?? "Unknown"}"),
                Text(
                  "Last Updated: $formattedDate",
                ), // Use the formatted date here
              ],
            ),
            actions: [
              TextButton(
                onPressed: () {
                  Navigator.of(context).pop();
                },
                child: const Text("Close"),
              ),
            ],
          );
        },
      );
    } else {
      print("Failed to fetch device details for $deviceId");
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const Header(),
      body: SafeArea(
        child: Column(
          children: [
            const SizedBox(height: 20),
            const Text(
              'Your Devices',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
                color: Color.fromARGB(255, 0, 0, 0),
              ),
            ),
            const SizedBox(height: 20),
            Expanded(
              child:
                  userDevices.isEmpty
                      ? Center(
                        child: Text(
                          errorMessage ?? "No devices found.",
                          style: const TextStyle(
                            fontSize: 16,
                            color: Colors.black54,
                          ),
                        ),
                      )
                      : ListView.builder(
                        itemCount: userDevices.length,
                        itemBuilder: (context, index) {
                          // Parse and format the updatedAt date
                          String formattedDate = "Unknown";
                          if (userDevices[index]["updatedAt"] != null &&
                              userDevices[index]["updatedAt"]!.isNotEmpty) {
                            try {
                              DateTime updatedAt = DateTime.parse(
                                userDevices[index]["updatedAt"]!,
                              );
                              formattedDate = DateFormat(
                                'd MMMM yyyy, HH:mm', // European style with 24-hour clock
                              ).format(updatedAt);
                            } catch (e) {
                              print("Error parsing date: $e");
                            }
                          }

                          return Card(
                            color: const Color.fromARGB(255, 64, 92, 218),
                            margin: const EdgeInsets.symmetric(
                              vertical: 10,
                              horizontal: 20,
                            ),
                            child: ListTile(
                              title: Text(
                                userDevices[index]["deviceName"] ?? "Unknown",
                                style: const TextStyle(color: Colors.white),
                              ),
                              subtitle: Text(
                                "Last Updated: $formattedDate",
                                style: const TextStyle(color: Colors.white70),
                              ),
                              trailing: IconButton(
                                icon: const Icon(Icons.info),
                                color: Colors.white,
                                onPressed: () {
                                  _showDeviceDetails(
                                    userDevices[index]["deviceId"]!,
                                  );
                                },
                              ),
                            ),
                          );
                        },
                      ),
            ),
          ],
        ),
      ),
      bottomNavigationBar: const Footer(),
    );
  }
}
