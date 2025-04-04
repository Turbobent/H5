import 'package:flutter/material.dart';
import 'package:http/http.dart' as http;
import 'package:sentinel/show_device.dart';
import 'auth_service.dart';
import 'dart:convert';
import 'package:sentinel/templates/footer.dart';
import 'package:sentinel/templates/header.dart';
import 'package:jwt_decoder/jwt_decoder.dart';

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
  List<Map<String, String>> searchResults = [];

  @override
  void initState() {
    super.initState();
    _fetchUserDevices();
  }

  // This function fetches the user's Arduino device codes from the API
  Future<void> _fetchUserDevices() async {
    String? token = await AuthService().getToken();
    if (token == null) return;

    // Decode the token and get the userId as an int
    Map<String, dynamic> decodedToken = JwtDecoder.decode(token);
    int userId = int.parse(
      decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]
          .toString(),
    );

    print("User ID: $userId"); // Print userId for debugging

    try {
      // Fetch user-device mappings
      var userDeviceResponse = await http.get(
        Uri.parse('${apiURL}User_Device/device-ids/$userId'),
        headers: {"Content-Type": "application/json"},
      );

      // Check if the response is successful
      if (userDeviceResponse.statusCode == 200) {
        List<dynamic> userDeviceData = jsonDecode(userDeviceResponse.body);

        // Extract the device IDs for the user
        List<String> userDeviceIds =
            userDeviceData
                .where(
                  (device) => device["user_id"] == userId,
                ) // Compare as int
                .map<String>((device) => device["arduino_id"].toString())
                .toList();

        // Fetch all devices
        var allDevicesResponse = await http.get(
          Uri.parse('${apiURL}Devices'),
          headers: {"Content-Type": "application/json"},
        );

        // Check if the response is successful
        if (allDevicesResponse.statusCode == 200) {
          List<dynamic> allDeviceData = jsonDecode(allDevicesResponse.body);

            // Filter the devices based on userDeviceIdsq
            setState(() {
            userDevices = allDeviceData
              .where((device) => userDeviceIds.contains(device["id"].toString()))
              .map((device) => UserDevice.fromJson(device).toMap()).cast<Map<String, String>>()
              .toList();

            // Sort devices by updatedAt in descending order
            userDevices.sort(
              (a, b) => DateTime.parse(b["updatedAt"]!)
                .compareTo(DateTime.parse(a["updatedAt"]!)),
            );
            });

        } else {
          setState(() {
            errorMessage = "Failed to fetch device details.";
          });
        }
      } else {
        setState(() {
          errorMessage =
              "Error fetching user-device mappings: ${userDeviceResponse.body}";
        });
      }
    } catch (e) {
      setState(() {
        errorMessage = "Exception: $e";
      });
    }
  }

  // Function to update the device status
  Future<void> updateDeviceStatus(String deviceId, String newStatus) async {
    try {
      var response = await http.put(
        Uri.parse(
          'https://sentinal-api.mercantec.tech/api/Devices/$deviceId/status',
        ),
        headers: {"Content-Type": "application/json"},
        body: jsonEncode({"status": newStatus}),
      );

      if (response.statusCode == 200) {
        print("Device status updated successfully.");
        setState(() {
          // Update the status in the userDevices list
          userDevices =
              userDevices.map((device) {
                if (device["arduino_id"] == deviceId) {
                  device["status"] = newStatus;
                }
                return device;
              }).toList();
        });
      } else {
        print("Failed to update device status: ${response.body}");
      }
    } catch (e) {
      print("Exception: $e");
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const Header(),
      body: SafeArea(
        child: Column(
          children: [
            Padding(
              padding: const EdgeInsets.all(8.0),
              child: SearchAnchor(
                builder: (BuildContext context, SearchController controller) {
                  return SearchBar(
                    controller: controller,
                    padding: const WidgetStatePropertyAll<EdgeInsets>(
                      EdgeInsets.symmetric(horizontal: 16.0),
                    ),
                    onTap: () {
                      controller.openView();
                    },
                    onChanged: (_) {
                      controller.openView();
                    },
                    leading: const Icon(Icons.search),
                  );
                },
                suggestionsBuilder: (
                  BuildContext context,
                  SearchController controller,
                ) {
                  final query = controller.text.toLowerCase();
                  if (query.isEmpty) {
                    return [];
                  }
                  final suggestions =
                      userDevices
                          .where(
                            (device) =>
                                device["title"]?.toLowerCase().contains(
                                  query,
                                ) ??
                                false,
                          )
                          .toList();
                  return List<Widget>.generate(suggestions.length, (int index) {
                    final String item =
                        suggestions[index]["title"] ?? "Untitled Device";
                    return ListTile(
                      title: Text(item),
                      onTap: () {
                        controller.closeView(item);
                        Navigator.push(
                          context,
                          MaterialPageRoute(
                            builder:
                                (context) => ShowDevice(
                                  deviceId: suggestions[index]["arduino_id"]!,
                                ),
                          ),
                        );
                      },
                    );
                  });
                },
              ),
            ),
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
                      : Padding(
                        padding: const EdgeInsets.all(8.0),
                        child: GridView.builder(
                          gridDelegate:
                              const SliverGridDelegateWithFixedCrossAxisCount(
                                crossAxisCount: 3,
                                crossAxisSpacing: 12,
                                mainAxisSpacing: 12,
                                childAspectRatio: 0.7,
                              ),
                          itemCount: userDevices.length,
                          itemBuilder: (context, index) {
                            return Container(
                              padding: const EdgeInsets.all(8.0),
                              decoration: BoxDecoration(
                                color:
                                    Colors.grey[200], // Light grey background
                                borderRadius: BorderRadius.circular(8.0),
                                boxShadow: [
                                  BoxShadow(
                                    color: Colors.grey.withOpacity(0.5),
                                    spreadRadius: 2,
                                    blurRadius: 5,
                                    offset: const Offset(
                                      0,
                                      3,
                                    ), // Shadow position
                                  ),
                                ],
                              ),
                              child: Column(
                                mainAxisAlignment:
                                    MainAxisAlignment.spaceBetween,
                                children: [
                                  // Device Name
                                  Text(
                                    userDevices[index]["title"] ??
                                        "Untitled Device",
                                    style: const TextStyle(
                                      fontSize: 16,
                                      fontWeight: FontWeight.bold,
                                    ),
                                    textAlign: TextAlign.center,
                                  ),
                                  const SizedBox(height: 8),
                                  // Device Status
                                  Text(
                                    "Status: ${userDevices[index]["status"] ?? "Unknown"}",
                                    style: const TextStyle(
                                      fontSize: 14,
                                      color: Colors.black54,
                                    ),
                                  ),
                                  const Spacer(),
                                  // Arm and Disarm Buttons
                                  Row(
                                    mainAxisAlignment:
                                        MainAxisAlignment.spaceEvenly,
                                    children: [
                                      ElevatedButton(
                                        onPressed: () {
                                          // Call updateDeviceStatus to arm the device
                                          updateDeviceStatus(
                                            userDevices[index]["arduino_id"]!,
                                            "armed",
                                          );
                                        },
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor: Colors.green,
                                        ),
                                        child: const Text("Arm"),
                                      ),
                                      ElevatedButton(
                                        onPressed: () {
                                          // Call updateDeviceStatus to disarm the device
                                          updateDeviceStatus(
                                            userDevices[index]["arduino_id"]!,
                                            "disarmed",
                                          );
                                        },
                                        style: ElevatedButton.styleFrom(
                                          backgroundColor: Colors.red,
                                        ),
                                        child: const Text("Disarm"),
                                      ),
                                    ],
                                  ),
                                ],
                              ),
                            );
                          },
                        ),
                      ),
            ),
          ],
        ),
      ),
      bottomNavigationBar: const Footer(),
    );
  }
}
