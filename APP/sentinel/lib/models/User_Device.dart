import 'dart:developer';

class UserDevice {
  String? deviceId;
  String? device;
  String? user;
  int? userId;
  int? id;
  DateTime? createdAt;
  DateTime? updatedAt;

  UserDevice({
    this.deviceId,
    this.device,
    this.user,
    this.userId,
    this.id,
    this.createdAt,
    this.updatedAt,
  });

  factory UserDevice.fromJson(Map<String, dynamic> json) {
    log('JSON Data: $json'); // Use log for debugging
    return UserDevice(
      deviceId: json['deviceId']?.toString(), // Handle null safely
      device: json['device']?.toString(), // Handle null safely
      user: json['user']?.toString(), // Handle null safely
      userId:
          json['userId'] is String
              ? int.tryParse(
                json['userId'],
              ) // Convert String to int if necessary
              : json['userId'], // Use as int if already an int
      id: json['id'] as int?,
      createdAt:
          json['createdAt'] != null ? DateTime.parse(json['createdAt']) : null,
      updatedAt:
          json['updatedAt'] != null ? DateTime.parse(json['updatedAt']) : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'deviceId': deviceId,
      'device': device,
      'user': user,
      'userId': userId,
      'id': id,
      'createdAt': createdAt?.toIso8601String(),
      'updatedAt': updatedAt?.toIso8601String(),
    };
  }

  Map<String, dynamic> toMap() {
    return toJson();
  }
}

// Endpoints

// GET /api/User_Device
// GET /api/User_Device/device-ids/{userId}
// DELETE /api/User_Device/{deviceId}