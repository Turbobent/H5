#ifndef SHS_H
#define SHS_H

#include <Arduino_MKRIoTCarrier.h>
#include <SPI.h>
#include <WiFiNINA.h>
#include <ArduinoHttpClient.h>

// Device Constants
#define NUMPIXELS 5

class SHS
{
private:
    // Arduino color variables
    uint32_t yellowColor = 0xFFFF00; // Yellow color for detected motion
    uint32_t greenColor = 0x00FF00;  // Green color for disarmed state
    uint32_t redColor = 0xFF0000;    // Red color for armed state
    bool ledShowOn = false;

    // Motion Sensor
    bool motionDetected = false;
    const int motionSensorPin = A6;
    int motionLevel = 0;

    // Accelerometer Threshold
    float accelThreshold = 1.2;  // Adjust based on testing

    // Factory Settings
    int motionDetectionDelay = 10000; // Delay in milliseconds (10 seconds)
    int motionDetectionSensitivity = 200; // How much movement needs to happen before the sensor cares
    int motionDetectionDistance = 200; // how far away the sensor can pick up movement from
    int alarmDuration = 300000; // 5 minutes in milliseconds

    const char *ssid;
    const char *password;

public:
    // Global state variables for toggle handling
    bool armButtonPressed = false;
    bool disarmButtonPressed = false;
    bool ledButtonPressed = false;
    bool passcodeEntered = false;
    bool isArmed = false;

    MKRIoTCarrier carrier;

    // Network Configuration
    WiFiSSLClient wifiClient;
    HttpClient *httpClient;

    // Constructor to initialize WiFi credentials
    SHS(const char *ssid, const char *password);

    // Initialize the system and WiFi connection
    void begin();

    // Connect to the Wi-Fi network
    void connectWiFi();

    // Read sensor values (motion, accelerometer, etc.)
    void readSensors();

    // Check for motion sensor state
    void checkMotion();

    // Arm the system
    void armSystem();

    // Disarm the system
    void disarmSystem();

    // Turn off LEDs
    void turnOffLEDs();

    // Convert boolean values to strings
    String boolToString(bool isArmed, bool motionDetected);

    // Update the system status (e.g., when triggered)
    void updateStatus();

    // Get the current accelerometer threshold
    float getAccelThreshold() {
        return accelThreshold;
    }

    // Set the motion detection sensitivity threshold
    void setMotionDetectionSensitivity(int sensitivity) {
        motionDetectionSensitivity = sensitivity;
    }
};

#endif // SENSORDATA_H
