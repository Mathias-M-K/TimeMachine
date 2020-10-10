const char string[] = ".. - . -- / --. .-.. --- ...- . / .-.. --- -.-. .- - .. --- -. / - --- .--. / ..-. .-. .. -.. --. . / ....- ..... ....-   ";


void setup() {
  Serial.begin(74880);
  pinMode(11, OUTPUT);
  pinMode(A0, OUTPUT);
}


void loop() {
  Serial.println("Begin");
  morseTranslator();
  Serial.println("End");
}

void morseTranslator(){
  for(int i = 0;i<strlen(string);i++){
    char c = string[i];

    if(c=='.'){
      Serial.println("Dot");
      dot();
    }
    if(c=='-'){
      Serial.println("Dash");
      dash();
    }
    if(isSpace(c)){
      Serial.println("Space");
      newLetter();
    }
    
  }
}

void dot() {
  digitalWrite(11, HIGH);
  delay(50);
  digitalWrite(11, LOW);
  delay(500);
}
void dash() {
  digitalWrite(11, HIGH);
  delay(300);
  digitalWrite(11, LOW);
  delay(500);
}

void newLetter(){
  digitalWrite(A0,HIGH);
  delay(300);
  digitalWrite(A0,LOW);
  delay(500);
}
