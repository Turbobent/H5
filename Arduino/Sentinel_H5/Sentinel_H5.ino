#include "content.h"
#include <PubSubClient.h> // Inkluderer PubSubClient-biblioteket
#include <ArduinoJson.h>
#include <WiFiUdp.h>
#include <NTPClient.h>

Sentinel sentinel(WIFI_SSID, WIFI_PASSWORD);

const char* mqtt_server = MQTT_SERVER; 
const char* topic = "ArduinoData"; 
const char* mqtt_username = MQTT_USERNAME;
const char* mqtt_password = MQTT_PASSWORD;

WiFiClient wifiClient;
PubSubClient client(wifiClient);

WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "pool.ntp.org");
const long gmtOffset_sec = 3600; 

void setup() {
  Serial.begin(9600);
  Serial.println("Arduino starter op...");

  sentinel.begin();
  Serial.println("Sentinel initialiseret");
  
  // Konfigurer display
  sentinel.carrier.display.setRotation(0);
  sentinel.carrier.display.fillScreen(0x0000); 
  sentinel.carrier.display.setTextColor(0xFFFF); 
  
  // Opret forbindelse til WiFi
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  
  // Vent på forbindelse
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.println("Forbinder til WiFi...");
  }
  
  Serial.println("Forbundet til WiFi!");
  Serial.print("IP-adresse: ");
  Serial.println(WiFi.localIP());
  Serial.print("Signal styrke (RSSI): ");
  Serial.println(WiFi.RSSI());

  // Opret forbindelse til MQTT-broker
  client.setServer(mqtt_server, 1883);
  if (client.connect("ArduinoClient", mqtt_username, mqtt_password)) {
    Serial.println("Forbundet til MQTT-broker!");
  } else {
    Serial.print("Fejl ved forbindelse til MQTT-broker, rc=");
    Serial.print(client.state());
  }

  // Efter WiFi forbindelse er etableret
  timeClient.begin();
  timeClient.setTimeOffset(gmtOffset_sec);
}

void loop() {
  
  client.loop();
  
  // Opdater tid før JSON generering
  timeClient.update();
  
  Serial.println("------ Ny måling ------");
  Serial.print("Tidsstempel: ");
  Serial.println(timeClient.getFormattedTime());
  
  // Læs sensordata
  float movementValue = sentinel.readMovement();
  Serial.print("Bevægelse registreret: ");
  Serial.println(movementValue);
  
  // Opdater display
  sentinel.carrier.display.fillScreen(0x0000);
  
  // Movement
  sentinel.carrier.display.setTextSize(2);
  sentinel.carrier.display.setCursor(30, 30);
  sentinel.carrier.display.print("Movement: ");
  sentinel.carrier.display.print(movementValue, 1);
  
  // People
  sentinel.carrier.display.setCursor(30, 90);
  sentinel.carrier.display.print("People: ");
  //carrier.display.print(movementCount, 1);
  
  // Acceleration
  sentinel.carrier.display.setCursor(30, 150);
  sentinel.carrier.display.print("Acceleration: ");
  //carrier.display.print(accelerationValue);
  
  // Tilføj data til JSON dokument
  StaticJsonDocument<200> doc;
  doc["timestamp"] = timeClient.getEpochTime();
  doc["movement"] = movementValue;
  
  // Konverter JSON til string
  char jsonBuffer[200];
  serializeJson(doc, jsonBuffer);
  
  // Opdater JSON diagnostisk info
  Serial.println("JSON data genereret:");
  Serial.println(jsonBuffer);
  
  // Send data
  if (client.publish(topic, jsonBuffer)) {
    Serial.print("Data sendt til MQTT emne '");
    Serial.print(topic);
    Serial.println("'");
  } else {
    Serial.print("Fejl ved sending af data til MQTT, klient status: ");
    Serial.println(client.state());
    Serial.println("Kontrollér forbindelse og broker konfiguration");
  }

  Serial.println("Venter 5 sekunder før næste måling...");
  Serial.println();
  
  // Vent 5 sekunder før næste måling
  delay(5000);
}

 