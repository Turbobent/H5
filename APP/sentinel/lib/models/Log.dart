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
}

class PostLog {
  String deviceId;
  DatePart date;
  DatePart endDate;
  TimePart armedTime;
  TimePart disarmedTime;
  bool isTriggered;
  TimePart? triggeredTime;

  PostLog({
    required this.deviceId,
    required this.date,
    required this.endDate,
    required this.armedTime,
    required this.disarmedTime,
    required this.isTriggered,
    this.triggeredTime,
  });
}

class DatePart {
  int year;
  int month;
  int day;

  DatePart({
    required this.year,
    required this.month,
    required this.day,
  });
}

class TimePart {
  int hour;
  int minute;

  TimePart({
    required this.hour,
    required this.minute,
  });
}

class Device {
  // Define the Device class properties and methods here
}