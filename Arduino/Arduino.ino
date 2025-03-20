#include "SHS.h"

// Instantiate the SHS class with WiFi credentials
SHS shs("MAGS-OLC", "Merc1234!");

// This function is called once at startup
void setup() {
    shs.begin();
    
    // Initialize the IMU (Accelerometer)
    if (!shs.carrier.IMUmodule.begin()) {
        Serial.println("IMU initialization failed!");
        while (1); // Halt program if IMU fails to initialize
    }
}

// This function runs repeatedly
void loop() {
    delay(100); // Delay to avoid constant polling
    shs.carrier.Buttons.update(); // Check button states

    // Disarms the system
    if (shs.carrier.Buttons.onTouchDown(TOUCH0) && !shs.disarmButtonPressed) { 
        shs.disarmSystem(); // Disarm system
        shs.disarmButtonPressed = true;
        delay(500);  // Add delay to debounce button press
    } 

    // Arms the system
    if (shs.carrier.Buttons.onTouchDown(TOUCH1) && !shs.armButtonPressed) { 
        shs.armSystem(); // Arm system
        shs.armButtonPressed = true;
        delay(500);  // Add delay to debounce button press
    } 

    // Turn off LEDs and Display (only when disarmed)
    if (shs.carrier.Buttons.onTouchDown(TOUCH2) && !shs.ledButtonPressed) { 
        shs.turnOffLEDs(); // Turn off LEDs
        shs.ledButtonPressed = true;
        delay(500);  // Add delay to debounce button press
    }

    // Read accelerometer values
    float accelX, accelY, accelZ;
    shs.carrier.IMUmodule.readAcceleration(accelX, accelY, accelZ);
    
    // Calculate total acceleration magnitude
    float totalAccel = sqrt(accelX * accelX + accelY * accelY + accelZ * accelZ);
    
    Serial.print("Acceleration: ");
    Serial.println(totalAccel);

    // Trigger alarm if system is armed and movement is detected
    if (shs.isArmed && totalAccel > shs.accelThreshold) {
        Serial.println("Movement detected! Triggering alarm.");
        
        // Use the built-in audio system to play a sound
        shs.carrier.speaker.tone(1000, 500);  // Frequency of 1000 Hz for 500 ms
        delay(5000);  // Keep alarm on for 5 seconds
        shs.carrier.speaker.noTone();  // Stop the sound after 5 seconds

        // Send movement data to the server
        sendMovementAlert(totalAccel); 
    }

    delay(200); // Delay to avoid high CPU usage
}

// Send movement alert data to the server
void sendMovementAlert(float movementValue) {
    if (WiFi.status() == WL_CONNECTED) {
        shs.httpClient->beginRequest();
        shs.httpClient->post("/movement");
        shs.httpClient->sendHeader("Content-Type", "application/json");
        shs.httpClient->sendHeader("Content-Length", 50);
        shs.httpClient->beginBody();
        shs.httpClient->print("{\"deviceID\": \"12345\", \"movement\": ");
        shs.httpClient->print(movementValue);
        shs.httpClient->print("}");
        shs.httpClient->endRequest();

        int statusCode = shs.httpClient->responseStatusCode();
        Serial.print("Server Response: ");
        Serial.println(statusCode);
    } else {
        Serial.println("WiFi not connected, cannot send data.");
    }
}
