#include "SHS.h"

SHS::SHS(const char *ssid, const char *password) : ssid(ssid), password(password) {}

void SHS::begin()
{
    CARRIER_CASE = false; // Deactivate carrier case if not mounted
    carrier.begin();
    Serial.begin(9600); // Initialize the serial communication

    // setupWiFi():
    connectWiFi(); // Connect to WiFi
    httpClient = new HttpClient(wifiClient, "https://sentinal-api.mercantec.tech/api/", 5099);
}

void SHS::connectWiFi()
{
    Serial.print("Connecting to ");
    Serial.println(ssid);

    carrier.display.setTextSize(2);
    carrier.display.setCursor(45, 80);
    carrier.display.setRotation(180);
    carrier.display.print("Connecting to ");
    carrier.display.setCursor(75, 100);
    carrier.display.setRotation(180);
    carrier.display.print(ssid);

    WiFi.begin(ssid, password);
    while (WiFi.status() != WL_CONNECTED) {
        delay(1000);
        Serial.print(".");
    }

    carrier.display.fillScreen(0x0000);
    Serial.println("");
    Serial.println("WiFi Connected.");
    Serial.println("IP address: ");
    Serial.println(WiFi.localIP());
}

void SHS::checkMotion()
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
        updateStatus(); // Send status update immediately
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
                    updateStatus(); // Reflect "armed without motion" status
                }
            }
        }
    }
}

void SHS::armSystem()
{
    isArmed = true;
    ledShowOn = true;
    motionDetected = false; // Reset motion detection

    // Turn off any previously lit LEDs
    turnOffLEDs(); // Ensure no other LEDs are lit before arming

    // Display "Armed" on the screen
    carrier.display.fillScreen(ST77XX_RED); // Set background color
    carrier.display.setTextSize(2);
    carrier.display.setTextColor(ST77XX_WHITE);
    carrier.display.setCursor(55, 80);
    carrier.display.setRotation(180);
    carrier.display.print("Device Armed");

    int remainingTime = motionDetectionDelay / 1000; // Convert to seconds for countdown

    // Countdown for "Change Status" prompt
    for (int i = remainingTime; i > 0; i--)
    {
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
    updateLEDs(redColor); // Using the new updateLEDs function

    updateStatus();
    checkMotion();
}

void SHS::disarmSystem()
{
    isArmed = false; // Disarm system
    ledShowOn = false;
    carrier.Buzzer.noSound(); // Stop the buzzer

    // Turn off any previously lit LEDs
    turnOffLEDs(); // Ensure no other LEDs are lit before arming

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
    for (int i = remainingTime; i > 0; i--)
    {
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
    updateLEDs(greenColor); // Using the new updateLEDs function

    updateStatus();
}

void SHS::turnOffLEDs()
{
    // Turn off all LEDs
    ledShowOn = false;

    for (int i = 0; i < NUMPIXELS; i++)
    {
        carrier.leds.setPixelColor(i, 0);
    }
    carrier.leds.show(); // Refresh LEDs

    // Display "Off" on the screen
    carrier.display.fillScreen(ST77XX_BLACK); // Clear screen
    carrier.display.setTextSize(2);
    carrier.display.setCursor(102, 120);
    carrier.display.setRotation(180);
    carrier.display.setTextColor(ST77XX_WHITE);
    carrier.display.print("OFF");

    Serial.println("Display/LED's Turned Off");
}

void SHS::updateLEDs(uint32_t color)
{
    // Turn on LEDs with the specified color
    for (int i = 0; i < NUMPIXELS; i++)
    {
        carrier.leds.setPixelColor(i, color);
    }
    carrier.leds.show(); // Refresh LEDs
}

String SHS::boolToString(bool isArmed, bool motionDetected)
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

void SHS::updateStatus()
{
    // Add deviceId to the JSON payload
    String postData = "{\"deviceId\": \"" + deviceID + "\", \"deviceStatus\": " + boolToString(isArmed, motionDetected) + "}";

    Serial.println("Trying to update status...");
    Serial.println("Requested data: " + postData);
    Serial.println("Device ID: " + deviceID);

    httpClient->beginRequest();
    httpClient->put("/api/Devices?id=" + deviceID);
    httpClient->sendHeader("Content-Type", "application/json");
    httpClient->sendHeader("Content-Length", postData.length());
    httpClient->sendHeader("accept", "*/*");
    httpClient->beginBody();
    httpClient->print(postData);
    httpClient->endRequest();

    int statusCode = httpClient->responseStatusCode();
    String response = httpClient->responseBody();

    Serial.print("Statuskode: ");
    Serial.println(statusCode);
    Serial.print("Svar: ");
    Serial.println(response);

    if (statusCode == 204)
    {
        Serial.println("Status updated succesfully.");
    }
    else
    {
        Serial.println("An error occured while updating status. Check your connection or the device's ID.");
    }
}
