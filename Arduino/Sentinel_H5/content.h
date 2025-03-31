#ifndef content_H
#define content_H

#include <Arduino_MKRIoTCarrier.h>
#include <SPI.h>
#include <WiFiNINA.h>

#include "config.h" // Include config file

// Device Constants
#define NUMPIXELS 5

class Sentinel
{
private:
    // Tilføj variabler til at gemme SSID og password
    const char *_ssid;
    const char *_password;

    // Arduino color variables
    uint32_t yellowColor = 0xFFFF00; 
    uint32_t greenColor = 0x00FF00;  
    uint32_t redColor = 0xFF0000;    

    // LED
    bool ledShowOn = false;

    // Motion Detection
    int motionValue = 0;
    bool motionDetected = false;
    static const int motionPin = A6;

    // Factory Settings
    int motionDetectionDelay = 5000; 
    int motionDetectionSensitivity = 200;
    int motionDetectionDistance = 200;
    int alarmDuration = 300000; 

    // Acceleration
    int accelerationValue = 0;
    bool accelerationDetected = false;

    int accelerationDetectionDelay = 5000; 
    int accelerationDetectionSensitivity = 200;

    float accelerationValue = 0;
    bool isMoving = false;

    // Device
    String deviceID;

public:
    // Change function return types to match the ones in the implementation file
    long lastMovementCheck;
    long startTime;
    int movementCount;

    // Global state variables for toggle handling
    bool armButtonPressed = false;
    bool disarmButtonPressed = false;
    bool ledButtonPressed = false;
    bool isArmed = false;

    MKRIoTCarrier carrier;

    // Network Configuration
    WiFiSSLClient wifiClient;

    Sentinel(const char *ssid, const char *password);
    void begin();
    float readMovement();  // Return type changed to int
    void checkMotion();
    void armSystem();
    void disarmSystem();
    String boolToString(bool isArmed, bool motionDetected);
};

#endif