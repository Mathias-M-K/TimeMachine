#include <FastLED.h>

#define NUM_LEDS 60
#define DATA_PIN 13
#define CLOCK_PIN 2

CRGB leds[NUM_LEDS];

void setup() {
  delay(2000);
  LEDS.addLeds<APA102, DATA_PIN, CLOCK_PIN, RGB>(leds, NUM_LEDS);
}

void loop() {
  int mydelay = 20;

  for (int i = 0; i < 60; i++) {
    leds[i] = CRGB(255, 0, 0);
    FastLED.show();
    delay(mydelay);
  }

  for (int i = 0; i < 60; i++) {
    leds[i] = CRGB(0, 255, 0);
    FastLED.show();
    delay(mydelay);
  }

  for (int i = 0; i < 60; i++) {
    leds[i] = CRGB(0, 0, 255);
    FastLED.show();
    delay(mydelay);
  }

}
