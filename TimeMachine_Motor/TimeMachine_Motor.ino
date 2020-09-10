//Serial control of motor speed
//Upload this sketch and then open up the Serial Monitor. Type in a value (0 - 255) and hit enter.

int motorSpeed;

void setup()
{
  Serial.begin(74880);  //initiate Serial communication
  Serial.println("Type in a vale from 0 - 255 and hit enter.");
  pinMode(9, OUTPUT);
}

void loop()
{
  if (Serial.available() > 0)
  {
    motorSpeed = Serial.parseInt();

/*
    if (motorSpeed == 0) {
      return;
    }
*/

    Serial.print("Setting motor speed to: ");
    Serial.println(motorSpeed);
    analogWrite(9, motorSpeed);
  }
}
