#include "content.h"

String deviceID = "1g";

Sentinel::Sentinel(const char *ssid, const char *password)
{
    // WiFi-forbindelse fjernet herfra, da det håndteres i script.ino
    // Gem SSID og password til senere brug hvis nødvendigt
    _ssid = ssid;
    _password = password;
}

void Sentinel::begin()
{
    CARRIER_CASE = false;
    carrier.begin();
    pinMode(motionPin, INPUT);
    Serial.begin(9600);
}

float Sentinel::readMovement()
{
    unsigned long currentMillis = millis();

    if (currentMillis - lastMovementCheck >= 5000)
    {
        movementValue = analogRead(motionPin); // Read the actual movement sensor value
    }

    return movementValue; // Return the actual sensor value
}