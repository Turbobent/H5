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
    return UserDevice(
      deviceId: json['deviceId'],
      device: json['device'],
      user: json['user'],
      userId: json['userId'],
      id: json['id'],
      createdAt: json['createdAt'] != null ? DateTime.parse(json['createdAt']) : null,
      updatedAt: json['updatedAt'] != null ? DateTime.parse(json['updatedAt']) : null,
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

  Future<void> toMap() async {}
}