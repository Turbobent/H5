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
}

class SharedPassword {
  final String passwordId;
  final String hashedPassword;
  final DateTime createdAt;
  final List<Device> devices;

  SharedPassword({
    required this.passwordId,
    required this.hashedPassword,
    DateTime? createdAt,
    this.devices = const [],
  }) : createdAt = createdAt ?? DateTime.now();
}

class DeviceMakePassword {
  final String deviceId;
  final String password;

  DeviceMakePassword({
    required this.deviceId,
    required this.password,
  });
}

class DeviceLogin {
  final String deviceId;
  final String password;

  DeviceLogin({
    required this.deviceId,
    required this.password,
  });
}

class PostDevice {
  final String name;
  final String deviceId;
  final bool status;
  final String password;

  PostDevice({
    required this.name,
    required this.deviceId,
    this.status = false,
    required this.password,
  });
}

class UpdateName {
  final String newName;

  UpdateName({
    required this.newName,
  });
}

class UpdateSta {
  final bool status;

  UpdateSta({
    required this.status,
  });
}

class UpdatePassword {
  final String newPassword;

  UpdatePassword({
    required this.newPassword,
  });
}