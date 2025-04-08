#ifndef content_H
#define content_H

#include <Arduino_MKRIoTCarrier.h>
#include <SPI.h>
#include <WiFiNINA.h>
#include <PubSubClient.h> // Inkluderer PubSubClient-biblioteket
#include <ArduinoJson.h>
#include <WiFiUdp.h>
#include <NTPClient.h>

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

    // Movement Detection
    int movementValue = 0;
    bool movementDetected = false;
    static const int motionPin = A6;

    // Movement Count
    long lastMovementCheck = 0;  // Track last check time
    int movementCount = 0;  // Movement count

    // Factory Settings
    int movementDetectionDelay = 5000; 
    int movementDetectionSensitivity = 200;
    int movementDetectionDistance = 200;
    int alarmDuration = 300000; 

    // Acceleration
    int accelerationValue = 0;
    bool accelerationDetected = false;

    int accelerationDetectionDelay = 5000; 
    int accelerationDetectionSensitivity = 200;

    bool isMoving = false;

    // Device
    String deviceID;

public:
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
    void checkMovement();
    void armSystem();
    void disarmSystem();
    String boolToString(bool isArmed, bool movementDetected);
};

#endif