#include "content.h"

int deviceID = 1;

Sentinel::Sentinel(const char *ssid, const char *password) {
    // WiFi-forbindelse fjernet herfra, da det håndteres i script.ino
    // Gem SSID og password til senere brug hvis nødvendigt
    _ssid = ssid;
    _password = password;
}

void Sentinel::begin() {
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
        lastMovementCheck = currentMillis;
        int motion = digitalRead(motionPin); 
        if (motion == HIGH) 
        { 
            movementCount++;
        }
    }
    return movementCount;  // Ensure it returns a value
}

// Check motion in front of the sensor and start alarm
void Sentinel::checkMotion() 
{
    int motion = analogRead(motionPin);

    if (motion > motionDetectionSensitivity && !motionDetected) 
    {
        motionDetected = true;
        Serial.println("Motion Detected!");

        carrier.display.fillScreen(ST77XX_YELLOW);
        carrier.display.setCursor(20, 120);
        carrier.display.setRotation(180);
        carrier.display.setTextColor(ST77XX_BLACK);
        carrier.display.println("Movement Detected!");

        for (int i = 0; i < NUMPIXELS; i++) 
        {
            carrier.leds.setPixelColor(i, yellowColor);
        }
        
        carrier.leds.show();
        delay(200);

        // Keep sounding the buzzer and updating status while armed
        while (isArmed && motionDetected) 
        {
            delay(500);
            carrier.Buttons.update();

            if (carrier.Buttons.onTouchDown(TOUCH0)) 
            {
                disarmSystem();
                break;
            }

            motion = analogRead(motionPin);

            // Check multiple times to confirm no motion before resetting `motionDetected`
            if (motion < motionDetectionSensitivity) 
            {
                delay(1000); // Add a delay to confirm no motion is detected over time
                motion = analogRead(motionPin);
                if (motion < motionDetectionSensitivity)
                {
                    motionDetected = false;
                }
            }
        }
    }
}

// Arm the system
void Sentinel::armSystem() 
{
    isArmed = true;
    ledShowOn = true;
    motionDetected = false; // Reset motion detection

    // Display "Armed" on the screen
    carrier.display.fillScreen(ST77XX_RED); // Set background color
    carrier.display.setTextSize(2);
    carrier.display.setTextColor(ST77XX_WHITE);
    carrier.display.setCursor(55, 80);
    carrier.display.setRotation(180);
    carrier.display.print("Device Armed");

    int remainingTime = motionDetectionDelay / 1000; // Convert to seconds for countdown

    // Countdown for "Change Status" prompt
    for (int i = remainingTime; i > 0; i--) {
        // Clear previous countdown area
        carrier.display.fillRect(70, 120, 150, 60, ST77XX_RED);

        // Display "Change Status" on a new line
        carrier.display.setCursor(30, 120);
        carrier.display.print("Detecting motion ");

        // Display "In:" on the next line, followed by the countdown
        carrier.display.setCursor(85, 140);
        carrier.display.print("in: ");
        carrier.display.print(i);

        delay(1000);  // Wait for one second (display only)
    }

    // Turn on red LEDs to indicate armed state
    for (int i = 0; i < NUMPIXELS; i++) 
    {
        carrier.leds.setPixelColor(i, redColor);
    }
    
    carrier.leds.show(); // Refresh LEDs
    checkMotion();
}

// Disarm the system
void Sentinel::disarmSystem() 
{
    isArmed = false; // Disarm system
    ledShowOn = false;
    carrier.Buzzer.noSound(); // Stop the buzzer

    // Display "Disarmed" on the screen
    carrier.display.fillScreen(ST77XX_GREEN); // Set background color
    carrier.display.setTextSize(2);
    carrier.display.setCursor(70, 80);
    carrier.display.setRotation(180);
    carrier.display.setTextColor(ST77XX_WHITE);
    carrier.display.print("Disarmed");

    Serial.println("Device Disarmed");

    int remainingTime = motionDetectionDelay / 1000; // Convert to seconds for countdown

    // Countdown for "Change Status" prompt
    for (int i = remainingTime; i > 0; i--) {
        // Clear previous countdown area
        carrier.display.fillRect(70, 120, 150, 60, ST77XX_GREEN);

        // Display "Change Status" on a new line
        carrier.display.setCursor(37, 120);
        carrier.display.print("Change Status");

        // Display "In:" on the next line, followed by the countdown
        carrier.display.setCursor(85, 140);
        carrier.display.print("In: ");
        carrier.display.print(i);

        delay(1000);  // Wait for one second (display only)
    }

    // Turn on green LEDs to indicate disarmed state
    for (int i = 0; i < NUMPIXELS; i++) 
    {
        carrier.leds.setPixelColor(i, greenColor);
    }
    carrier.leds.show(); // Refresh LEDs
}

String Sentinel::boolToString(bool isArmed, bool motionDetected)
{
    if (isArmed && motionDetected)
    {
        return "2";
    }

    else if (isArmed)
    {
        return "1";
    }

    else
    {
        return "0";
    }
}

 