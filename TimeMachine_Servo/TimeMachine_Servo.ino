/* Sweep
 by BARRAGAN <http://barraganstudio.com>
 This example code is in the public domain.

 modified 8 Nov 2013
 by Scott Fitzgerald
 http://www.arduino.cc/en/Tutorial/Sweep
*/

#include <Servo.h>

Servo myservo;  // create servo object to control a servo
// twelve servo objects can be created on most boards

int pos = 0;    // variable to store the servo position

void setup() {
  Serial.begin(74880);
  myservo.attach(2);  // attaches the servo on pin 9 to the servo object
  //myservo.write(90);
}

void loop() {

  if(Serial.available() > 0){
    int newPos = Serial.parseInt();

    if(newPos == 0){
      return;
    }
    
    myservo.write(newPos);
  }

  /*
  Serial.println("Going one");
  for (pos = 0; pos <= 360; pos += 1) { // goes from 0 degrees to 180 degrees
    // in steps of 1 degree
    Serial.println(pos);
    myservo.write(pos);              // tell servo to go to position in variable 'pos'
    delay(15);                       // waits 15ms for the servo to reach the position
  }
  Serial.println("Going two");
  for (pos = 360; pos >= 0; pos -= 1) { // goes from 180 degrees to 0 degrees
    Serial.println(pos);
    myservo.write(pos);              // tell servo to go to position in variable 'pos'
    delay(15);                       // waits 15ms for the servo to reach the position
  }*/
}
