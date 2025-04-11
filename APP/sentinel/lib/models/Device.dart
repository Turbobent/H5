class Device {
  final String deviceId;
  final bool status;
  final String name;
  final String? sharedPasswordId;
  final SharedPassword? sharedPassword;

  Device({
    required this.deviceId,
    required this.status,
    required this.name,
    this.sharedPasswordId,
    this.sharedPassword,
  });

  factory Device.fromJson(Map<String, dynamic> json) {
    return Device(
      deviceId: json['deviceId'],
      status: json['status'],
      name: json['name'],
      sharedPasswordId: json['sharedPasswordId'],
      sharedPassword:
          json['sharedPassword'] != null
              ? SharedPassword.fromJson(json['sharedPassword'])
              : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'deviceId': deviceId,
      'status': status,
      'name': name,
      'sharedPasswordId': sharedPasswordId,
      'sharedPassword': sharedPassword?.toJson(),
    };
  }
}

class SharedPassword {
  final String passwordId;
  final String hashedPassword;
  final DateTime createdAt;
  final List<Device> devices;

  SharedPassword({
    required this.passwordId,
    required this.hashedPassword,
    required this.createdAt,
    this.devices = const [],
  });

  factory SharedPassword.fromJson(Map<String, dynamic> json) {
    return SharedPassword(
      passwordId: json['passwordId'],
      hashedPassword: json['hashedPassword'],
      createdAt: DateTime.parse(json['createdAt']),
      devices:
          (json['devices'] as List<dynamic>?)
              ?.map((device) => Device.fromJson(device))
              .toList() ??
          [],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'passwordId': passwordId,
      'hashedPassword': hashedPassword,
      'createdAt': createdAt.toIso8601String(),
      'devices': devices.map((device) => device.toJson()).toList(),
    };
  }
}

// Endpoints

// GET /api/Devices
// POST /api/Devices
// GET /api/Devices/{deviceId}
// PUT /api/Devices/UpdateStatus/{deviceId}
// PUT /api/Devices/UpdateName/{deviceId}
// DELETE /api/Devices/{id}