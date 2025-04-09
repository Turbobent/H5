#ifndef content_H
#define content_H

#include <Arduino_MKRIoTCarrier.h>
#include <PubSubClient.h>
#include <ArduinoJson.h>
#include <NTPClient.h>
#include <WiFiNINA.h>
#include <WiFiUdp.h>
#include <SPI.h>

#include "config.h" // Include config file

// Device Constants
#define NUMPIXELS 5

class Sentinel
{
private:
    // Tilføj variabler til at gemme SSID og password
    const char *_ssid;
    const char *_password;

    static const int motionPin = A6;
    
    // Movement Detection
    int movementValue = 0;

    // Movement Count
    long lastMovementCheck = 0;  // Track last check time
    int movementCount = 0;  // Movement count

public:
    MKRIoTCarrier carrier;

    // Network Configuration
    WiFiSSLClient wifiClient;

    Sentinel(const char *ssid, const char *password);
    void begin();
    float readMovement();  // Return type changed to int
};

#endif