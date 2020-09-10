#include <FastLED.h>

#define btn1 13   //D0
#define btn2 15    //D1


//LED Values
#define NUM_LEDS 60
#define DATA_PIN 14    //D5
#define CLOCK_PIN 12   //D6
CRGB leds[NUM_LEDS];

//WaitingEffect Values
int ledOn = -1;
int ledOff = 0;
int ledWaveWidth = 7;
float roundTime = 15;

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

int btn1Value;
int btn2Value;

void setup() {
  pinMode(btn1, INPUT);
  pinMode(btn2, INPUT);
  Serial.begin(74880);

  delay(2000);
  LEDS.addLeds<APA102, DATA_PIN, CLOCK_PIN, BGR>(leds, NUM_LEDS);

}

void loop() {
  if(Serial.available() > 0){
    int temp = Serial.parseInt();

    if(temp == 0){
      return;
    }
    Serial.print("NEW VALUE:");
    Serial.println(temp);
    roundTime = temp;
  }

  
  ReadBtns();
  //PrintBtns();
  LightControl();


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
  Lights_Wait();
  FastLED.show();
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
        waveColorBack = CRGB(0,0,fadeValue);
      }

      //FastLED.show();

    } else {
      confirmEffectComplete = false;
      fadeValue -= 5;

      if (fadeValue < 0) {
        fadeValue = 0;
      }

      if(fadeIncreasing){
        return;
      }
      
      for (int i = 0; i < NUM_LEDS; i++) {
        leds[i] = CRGB(0, 0, fadeValue);
        waveColorBack = CRGB(0,0,fadeValue);
      }

      if(fadeValue == 0){
        fadeIncreasing = true;
      }
    }
  }
}

void Lights_Wait() {
  if(btn2Value == 1){
    waveColor = CRGB(225, 100, 0);
  }
  else{
    waveColor = waveColorBack;
  }

  if (millis() - lastHitTime > roundTime) {
    lastHitTime = millis();

    //Determine which led to turn on
    ledOn++;

    if (ledOn > 60) {
      ledOn = 0;
    }

    leds[ledOn] = waveColor;

    //Determine which led to turn off
    int controlInt = ledOn - ledWaveWidth;

    if (controlInt >= 0 && controlInt <= NUM_LEDS) {
      ledOff = controlInt;
      leds[ledOff] = waveColorBack;
    }
    if (controlInt < 0) {
      ledOff = NUM_LEDS + controlInt;
      leds[ledOff] = waveColorBack;
    }
  }
}
