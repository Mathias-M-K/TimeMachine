#include <FastLED.h>

#define btn1 16
#define btn2 5


//LED Values
#define NUM_LEDS 60
#define DATA_PIN 4
#define CLOCK_PIN 0

CRGB leds[NUM_LEDS];

void setup() {
  pinMode(btn1, INPUT);
  pinMode(btn2, INPUT);
  Serial.begin(74880);

  delay(2000);
  LEDS.addLeds<APA102, DATA_PIN, CLOCK_PIN, BGR>(leds, NUM_LEDS);

}

void loop() {
  printBtns();

  if (digitalRead(btn1) == 1 && digitalRead(btn2) == 1) {
    for (int i = 0; i < 60; i++) {
      leds[i] = CRGB(255, 0, 0);
      FastLED.show();
    }
  } else if (digitalRead(btn1) == 1 || digitalRead(btn2) == 1) {
    for (int i = 0; i < 60; i++) {
      leds[i] = CRGB(0, 255, 0);
      FastLED.show();
    }
  }else{
    for (int i = 0; i < 60; i++) {
      leds[i] = CRGB(0, 0, 0);
      FastLED.show();
    }
  }
}


void printBtns() {
  Serial.print("b1: "); Serial.print(digitalRead(btn1));
  Serial.print(" | b2: "); Serial.println(digitalRead(btn2));
}
