#include <FastLED.h> 

#define NUM_LEDS 6
#define DATA_PIN 11
#define CLOCK_PIN 13

CRGB leds[NUM_LEDS];

void setup() {
    delay(2000);
    LEDS.addLeds<APA102,DATA_PIN, CLOCK_PIN,RGB>(leds,NUM_LEDS);
}

void loop() {
    leds[0] = CRGB(255,0,0);
    leds[1] = CRGB(0,255,0);
    leds[2] = CRGB(0,255,0);
    leds[3] = CRGB(0,0,255);
    leds[4] = CRGB(0,0,255);
    leds[5] = CRGB(0,0,255);
    FastLED.show();
    delay(1000);
}
