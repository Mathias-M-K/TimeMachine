#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <FastLED.h>
#include <Servo.h>

#define btn1 13   //D0
#define btn2 15   //D1

//LED Values
#define NUM_LEDS 120
#define DATA_PIN 14    //D5
#define CLOCK_PIN 12   //D6
CRGB leds[NUM_LEDS];

//WaitingEffect Values
int ledLeftOn = -1;
int ledLeftOff = 0;
int ledRightOn = 121;
int ledRightOff = 120;
int ledWaveWidth = 7;
float roundTime = 15;

//Timetravel values
bool timeTravelActivated = false;
String ledTravelDirection = "up";
float timeTravelStartTime = 0;

CRGB waveColor = CRGB(225, 100, 0);
CRGB waveColorBack = CRGB(0, 0, 0);

float lastHitTime = 0;

//ConfimConnected values
int fadeSpeed = 5;
int fadeValue = 0;
int fadeIncreasing = true;
int confirmEffectProgressionCounter = 0;
bool confirmEffectComplete = false;
float lastTimeSinceHit = 0;


/*
   WIFI VALUES
*/
char ssid[] = "Martin Router King";
char password[] = "password";
const int port = 26;

int pingReturnValue = 0;
int requestDisconnect = 0; //Set to 1 to initiate controlled disconnect
bool clientDisconnectNotify = true;

String outgoing = "Ready:0";

WiFiServer server(port);
WiFiClient client;

/*
   BUTTON VALUES
*/
int btn1Value;
int btn2Value;

/*
   Servo
*/
Servo myservo;  // create servo object to control a servo

/*
   Serial Communication
*/
// This simple code allow you to send data from Arduino to Unity3D.

// uncomment "NATIVE_USB" if you're using ARM CPU (Arduino DUE, Arduino M0, ..)
//#define NATIVE_USB

// uncomment "SERIAL_USB" if you're using non ARM CPU (Arduino Uno, Arduino Mega, ..)
#define SERIAL_USB

bool SerialCommsActive = true;

void setup() {
  pinMode(btn1, INPUT);
  pinMode(btn2, INPUT);
  myservo.attach(2);
  myservo.write(95);
  LEDS.addLeds<APA102, DATA_PIN, CLOCK_PIN, BGR>(leds, NUM_LEDS);

#ifdef NATIVE_USB
  SerialUSB.begin(1); //Baudrate is irevelant for Native USB
#endif

#ifdef SERIAL_USB
  Serial.begin(74880); // You can choose any baudrate, just need to also change it in Unity.
  while (!Serial); // wait for Leonardo enumeration, others continue immediately
#endif

  outgoing = "Ready:0";

  setLights(255, 0, 0);
  delay(2000);
  // setLights(0, 0, 0);

  // Start the TCP server
  server.begin();
}


void loop() {
  
  if (SerialCommsActive) {
    sendData(outgoing);
    delay(5); // Choose your delay having in mind your ReadTimeout in Unity3D

    RecieveData();
    return;
  }


  // Listen for connecting clients
  client = server.available();

  if (client) {
    //Serial.println("");
    //Serial.println("Client connected");
    clientDisconnectNotify = false;

    while (client.connected()) {

      //External methods
      ReadBtns();
      LightControl();

      if (client.available() > 0) {
        Serial.print("Data Available:");
        String tempString;
        while (client.available() > 0) {
          char c = client.read();
          tempString += c;
        }
        Serial.println(tempString);

        if (getValue(tempString, ':', 0) == "Ping") {
          pingReturnValue = getValue(tempString, ':', 1).toInt();
        }
        if (getValue(tempString, ':', 0) == "Servo") {
          myservo.write(getValue(tempString, ':', 1).toInt());
        }
        if (getValue(tempString, ':', 0) == "TimeTravel") {
          StartTimeTravel();
        }

      }

      String toPc = "VAL:" + String(btn1Value) + ":" + String(btn2Value) + ":" + "Status_Message" + ":" + String(requestDisconnect) + ":" + pingReturnValue;
      // Send the distance to the client, along with a break to separate our messages
      client.print(toPc);
      client.print('\r');

      if (requestDisconnect == 1) {
        client.stop();
        requestDisconnect = 0;
      }

      // Delay before the next reading
      //delay(10);
    }
  } else {
    if (!clientDisconnectNotify) {
      Serial.println("Client Disconnected");
      clientDisconnectNotify = true;
    }

    setLights(255, 255, 0);

  }

  if (WiFi.status() != WL_CONNECTED) {
    Serial.println("");
    Serial.print("Wifi disconnected, trying to reconnect");

    while (WiFi.status() != WL_CONNECTED) {
      delay(500);
      Serial.print(".");
    }
    Serial.println(" Wifi reconnect successful");
    Serial.println("");
  }


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
  String wifiInfo = "Connected:" + WiFi.localIP().toString() + ":" + "26";
  outgoing = wifiInfo;
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
      SerialCommsActive = false;
    }
  }
}

void ReadBtns() {
  btn1Value = digitalRead(btn1);
  btn2Value = digitalRead(btn2);
}

void PrintBtns() {
  Serial.print("b1: "); Serial.print(btn1Value);
  Serial.print(" | b2: "); Serial.println(btn2Value);
}

void LightControl() {
  ConfimConnected();

  if (timeTravelActivated) {
    Lights_Go();
  } else {
    Lights_Wait();
  }
  //Lights_Wait();
  FastLED.show();
}

void StartTimeTravel() {
  timeTravelStartTime = millis();
  timeTravelActivated = true;
}

void ConfimConnected() {


  if (millis() - lastTimeSinceHit > fadeSpeed) {
    lastTimeSinceHit = millis();

    if (btn1Value == 1) {

      //Return if effect is complete
      if (confirmEffectComplete) {
        return;
      }

      //Either increase or decrease the fade value
      if (fadeIncreasing) {
        fadeValue += 5;
      } else {
        fadeValue -= 5;
      }

      //Enforcing lower boundary
      if (fadeValue < 20) {
        fadeValue = 20;
        fadeIncreasing = true;
        fadeSpeed = 5;
      }

      //Enforcing upper boundary
      if (fadeValue > 255) {
        fadeValue = 255;
        fadeIncreasing = false;
        fadeSpeed = 20;
      }

      //Stop the effect when the correct parameters have been met
      if (!fadeIncreasing && fadeValue < 80) {
        confirmEffectComplete = true;
      }

      for (int i = 0; i < NUM_LEDS; i++) {
        leds[i] = CRGB(0, 0, fadeValue);
        waveColorBack = CRGB(0, 0, fadeValue);
      }

      //FastLED.show();

    } else {
      confirmEffectComplete = false;
      fadeValue -= 5;

      if (fadeValue < 0) {
        fadeValue = 0;
      }

      if (fadeIncreasing) {
        return;
      }

      for (int i = 0; i < NUM_LEDS; i++) {
        leds[i] = CRGB(0, 0, fadeValue);
        waveColorBack = CRGB(0, 0, fadeValue);
      }

      if (fadeValue == 0) {
        fadeIncreasing = true;
      }
    }
  }
}

void Lights_Wait() {
  if (btn2Value == 1) {
    waveColor = CRGB(225, 100, 0);
  }
  else {
    waveColor = waveColorBack;
  }

  if (millis() - lastHitTime > roundTime) {
    lastHitTime = millis();

    //Determine which led to turn on
    ledLeftOn++;
    ledRightOn--;

    if (ledLeftOn > 60) {
      ledLeftOn = 0;
    }
    if (ledRightOn < 60) {
      ledRightOn = 120;
    }

    leds[ledLeftOn] = waveColor;
    leds[ledRightOn] = waveColor;

    //Determine which led to turn off
    int controlIntLeft = ledLeftOn - ledWaveWidth;
    int controlIntRight = ledRightOn + ledWaveWidth;

    //Left
    if (controlIntLeft >= 0 && controlIntLeft <= 60) {
      ledLeftOff = controlIntLeft;
      leds[ledLeftOff] = waveColorBack;
    }
    if (controlIntLeft < 0) {
      ledLeftOff = 60 + controlIntLeft;
      leds[ledLeftOff] = waveColorBack;
    }

    //Right
    if (controlIntRight >= 60 && controlIntRight <= 120) {
      ledRightOff = controlIntRight;
      leds[ledRightOff] = waveColorBack;
    }
    if (controlIntRight > 120) {
      ledRightOff = 60 + (controlIntRight - 120);
      leds[ledRightOff] = waveColorBack;
    }

  }
}

void Lights_Go() {

  float timeProgression = millis() - timeTravelStartTime;

  roundTime = map(timeProgression, 0, 10000, 15, 0);

  if (timeProgression > 10000 && timeProgression < 12000) {
    float newFadeValue = map(timeProgression, 10000, 12000, 255, 0);

    for (int i = 0; i < NUM_LEDS; i++) {
      leds[i] = CRGB(0, newFadeValue, 0);
    }
    return;
  }

  if (timeProgression > 12000 && timeProgression < 13000) {
    float newFadeValue = map(timeProgression, 12000, 13000, 0, 80);

    for (int i = 0; i < NUM_LEDS; i++) {
      leds[i] = CRGB(0, 0, newFadeValue);
    }
    return;
  }

  if (timeProgression > 14000) {
    roundTime = 15;
    timeTravelActivated = false;
    return;
  }

  if (millis() - lastHitTime > roundTime) {
    lastHitTime = millis();

    if (ledLeftOn + 1 == 61) {
      ledTravelDirection = "down";

      ledLeftOn = 60 - ledWaveWidth + 1;
      ledRightOn = 60 + ledWaveWidth - 1;
    }
    if (ledLeftOn - 1 == -1) {
      ledTravelDirection = "up";

      ledLeftOn = ledWaveWidth - 1;
      ledRightOn = 120 - ledWaveWidth + 1;
    }

    if (ledTravelDirection == "up") {
      ledLeftOn++;
      ledLeftOff = ledLeftOn - ledWaveWidth;

      ledRightOn--;
      ledRightOff = ledRightOn + ledWaveWidth;

      if (ledRightOff > 119) {
        ledRightOff = 119;
      }
    }

    if (ledTravelDirection == "down") {
      ledLeftOn--;
      ledLeftOff = ledLeftOn + ledWaveWidth;

      ledRightOn++;

      //Stupid code that is stupid

      if (ledRightOn > 119) {
        ledRightOn = 119;
      }

      ledRightOff = ledRightOn - ledWaveWidth;
    }


    leds[ledLeftOn] = waveColor;
    leds[ledLeftOff] = waveColorBack;


    if (ledRightOn > 119 || ledRightOff > 119) {
      Serial.println("Wtf error");
    }

    leds[ledRightOn] = waveColor;
    leds[ledRightOff] = waveColorBack;
  }
}

void setLights(int r, int g, int b) {

  for (int i = 0; i < NUM_LEDS; i++) {
    leds[i] = CRGB(r, g, b);
  }
  FastLED.show();
}

void InitiateWifi() {
  WiFi.begin(ssid, password);
  Serial.print("Connecting");

  setLights(255, 0, 0);

  while (WiFi.status() != WL_CONNECTED) {
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

  String wifiInfo = WiFi.localIP() + ":" + port;
  Serial.println(wifiInfo);
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
