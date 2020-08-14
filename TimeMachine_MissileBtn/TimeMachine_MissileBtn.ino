void setup() {
  pinMode(16, INPUT);
  pinMode(5, INPUT);
  Serial.begin(9600);

}

void loop() {
  Serial.print("b1: "); Serial.print(digitalRead(16));
  Serial.print(" | b2: "); Serial.println(digitalRead(5));

}
