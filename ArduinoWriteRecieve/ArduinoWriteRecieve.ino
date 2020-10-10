#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <FastLED.h>
/*
   LED VALUES
*/
#define NUM_LEDS 120
#define DATA_PIN 14    //D5
#define CLOCK_PIN 12   //D6
CRGB leds[NUM_LEDS];


/*
   WIFI VALUES
*/
char ssid[] = "Martin Router King";
char password[] = "password";
const int port = 26;

int pingReturnValue = 0;
int requestDisconnect = 0; //Set to 1 to initiate controlled disconnect
bool clientDisconnectNotify = true;

String outgoing = "Ready";

WiFiServer server(port);
WiFiClient client;
// This simple code allow you to send data from Arduino to Unity3D.

// uncomment "NATIVE_USB" if you're using ARM CPU (Arduino DUE, Arduino M0, ..)
//#define NATIVE_USB

// uncomment "SERIAL_USB" if you're using non ARM CPU (Arduino Uno, Arduino Mega, ..)
#define SERIAL_USB




void setup() {
  LEDS.addLeds<APA102, DATA_PIN, CLOCK_PIN, BGR>(leds, NUM_LEDS);
  setLights(255, 0, 0);
#ifdef NATIVE_USB
  SerialUSB.begin(1); //Baudrate is irevelant for Native USB
#endif

#ifdef SERIAL_USB
  Serial.begin(74880); // You can choose any baudrate, just need to also change it in Unity.
  while (!Serial); // wait for Leonardo enumeration, others continue immediately
#endif

  outgoing = "Ready:0";
}

// Run forever
void loop() {
  sendData(outgoing);
  delay(5); // Choose your delay having in mind your ReadTimeout in Unity3D

  RecieveData();
}

void sendData(String data) {
#ifdef NATIVE_USB
  SerialUSB.println(data); // need a end-line because wrmlh.csharp use readLine method to receive data
#endif

#ifdef SERIAL_USB
  Serial.println(data); // need a end-line because wrmlh.csharp use readLine method to receive data
#endif
}

void RecieveData() {
  if (Serial.available() > 0) {
    String tempString = Serial.readString();

    if (getValue(tempString, ':', 0) == "WIFI") {

      String incomingSsid = getValue(tempString, ':', 1);
      String incomingPassword = getValue(tempString, ':', 2);

      incomingSsid.trim();
      incomingPassword.trim();

      char ssidArray[incomingSsid.length() + 1];
      char passwordArray[incomingPassword.length() + 1];

      incomingSsid.toCharArray(ssidArray, incomingSsid.length() + 1);
      incomingPassword.toCharArray(passwordArray, incomingPassword.length() + 1);

      InitiateWifiTwo(ssidArray, passwordArray);
    }
    if (getValue(tempString, ':', 0) == "Disconnect") {
      sendData("Goodbye:0");
    }
  }
}

void InitiateWifi() {
  setLights(0, 0, 255);
  WiFi.begin(ssid, password);
  //String wifiConnectionInfo = "Connecting: " + ssid + " // " + password;

  while (WiFi.status() != WL_CONNECTED) {
    //sendData(wifiConnectionInfo);
    sendData("Connecting now..");
    delay(500);
    //Serial.print(".");
  }
  //Serial.println("");
  //Serial.print("Connected to ");
  //Serial.println(ssid);

  //Serial.print("IP Address: ");
  //Serial.println(WiFi.localIP());

  //Serial.print("Port: ");
  //Serial.println(port);
  setLights(0, 255, 0);
  String wifiInfo = WiFi.localIP().toString() + ":" + "26";
  outgoing = wifiInfo;
}

void InitiateWifiTwo(char s[], char p[]) {
  setLights(0, 0, 255);
  WiFi.begin(s, p);

  String ssidString(s);
  String passwordString(p);

  String wifiConnectionInfo = "Connecting:-" + ssidString + "-//-" + passwordString + "-";

  int waitCounter = 0;
  while (WiFi.status() != WL_CONNECTED) {
    sendData(wifiConnectionInfo);
    delay(500);
    waitCounter++;

    if (waitCounter >= 20) {
      setLights(255, 0, 0);
      outgoing = "Connection Failed:0";
      return;
    }
  }

  setLights(0, 255, 0);
  String wifiInfo = "Connected:"+WiFi.localIP().toString() + ":" + "26";
  outgoing = wifiInfo;
}

void setLights(int r, int g, int b) {

  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CRGB(r, g, b);
  }
  FastLED.show();
}

String getValue(String data, char separator, int index)
{
  int found = 0;
  int strIndex[] = { 0, -1 };
  int maxIndex = data.length() - 1;

  for (int i = 0; i <= maxIndex && found <= index; i++) {
    if (data.charAt(i) == separator || i == maxIndex) {
      found++;
      strIndex[0] = strIndex[1] + 1;
      strIndex[1] = (i == maxIndex) ? i + 1 : i;
    }
  }
  return found > index ? data.substring(strIndex[0], strIndex[1]) : "";
}
