import 'package:sentinel/models/Device.dart';
import 'package:flutter/material.dart';

class Log {
  String deviceId;
  Device device;
  DateTime date;
  DateTime endDate;
  TimeOfDay armedTime;
  TimeOfDay disarmedTime;
  bool isTriggered;
  TimeOfDay? triggeredTime;

  Log({
    required this.deviceId,
    required this.device,
    required this.date,
    required this.endDate,
    required this.armedTime,
    required this.disarmedTime,
    required this.isTriggered,
    this.triggeredTime,
  });

  factory Log.fromJson(Map<String, dynamic> json) {
    return Log(
      deviceId: json['deviceId'],
      device: Device.fromJson(json['device']),
      date: DateTime.parse(json['date']),
      endDate: DateTime.parse(json['endDate']),
      armedTime: TimeOfDay(
        hour: json['armedTime']['hour'],
        minute: json['armedTime']['minute'],
      ),
      disarmedTime: TimeOfDay(
        hour: json['disarmedTime']['hour'],
        minute: json['disarmedTime']['minute'],
      ),
      isTriggered: json['isTriggered'],
      triggeredTime:
          json['triggeredTime'] != null
              ? TimeOfDay(
                hour: json['triggeredTime']['hour'],
                minute: json['triggeredTime']['minute'],
              )
              : null,
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'deviceId': deviceId,
      'device': device.toJson(),
      'date': date.toIso8601String(),
      'endDate': endDate.toIso8601String(),
      'armedTime': {'hour': armedTime.hour, 'minute': armedTime.minute},
      'disarmedTime': {
        'hour': disarmedTime.hour,
        'minute': disarmedTime.minute,
      },
      'isTriggered': isTriggered,
      'triggeredTime':
          triggeredTime != null
              ? {'hour': triggeredTime!.hour, 'minute': triggeredTime!.minute}
              : null,
    };
  }
}

// Endpoints

// GET /api/Logs
// POST /api/Logs
// GET /api/Logs/device/{deviceId}
// GET /api/Logs/{id}
// PUT /api/Logs/{id}
// DELETE /api/Logs/{id}