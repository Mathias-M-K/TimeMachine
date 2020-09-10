#include <Servo.h>


#define OUTPUT_A 5 //data
#define OUTPUT_B 4  //clock
#define BTN 0  //btn

int counter = 0;
int oldCounter = 0;
float timesinceLastServoWrite = 0;
int aState;
int aLastState;

bool lockRotaryEncoder = false;

int currentLengthOfSequence;


//Servo
Servo myservo;  // create servo object to control a servo
// twelve servo objects can be created on most boards

int pos = 0;    // variable to store the servo position

void setup() {
  Serial.begin(74880);
  pinMode (OUTPUT_A, INPUT);
  pinMode (OUTPUT_B, INPUT);
  pinMode(BTN, INPUT);

  myservo.attach(2);  // attaches the servo on pin 9 to the servo object
  //myservo.write(0);
}

void loop() {
  //ReadEncoder();
  ReadSerial();

  if (counter != oldCounter) {
    Serial.println("NEW VAL!");
    oldCounter = counter;
    myservo.write(counter);
  }
}
void ReadSerial() {
  if (Serial.available() > 0)
  {
    int incomingVal = Serial.parseInt();

    if (incomingVal == 0) {
      return;
    }

    counter = incomingVal;
  }
}

void ReadEncoder() {
  aState = digitalRead(OUTPUT_A); // Reads the "current" state of the outputA
  // If the previous and the current state of the outputA are different, that means a Pulse has occured
  if (aState != aLastState) {
    // If the outputB state is different to the outputA state, that means the encoder is rotating clockwise
    if (digitalRead(OUTPUT_B) != aState) {
      counter ++;
    } else {
      counter --;
    }
    Serial.print("Position: ");
    Serial.println(counter);
  }
  aLastState = aState; // Updates the previous state of the outputA with the current state

  if (lockRotaryEncoder) {
    ContentPositionCorrection();
  }
}
void ContentPositionCorrection() {
  if (counter > 0) {
    counter = 0;
  }
  if (counter < -currentLengthOfSequence + 8) {
    counter = -currentLengthOfSequence + 8;
  }
}
