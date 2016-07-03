int switchState = 0;

// Constructor
void setup() 
{
  // set pinmodes
  pinMode(2, INPUT);
  pinMode(3, OUTPUT);
  pinMode(4, OUTPUT);
  pinMode(5, OUTPUT);
}

// Code loop
void loop()
{
  switchState = digitalRead(2);

  if(switchState == LOW)
  {
    // button was pressed, toggle LEDs
    digitalWrite(3, HIGH);
    digitalWrite(4, LOW);
    digitalWrite(5, LOW);
  }
  else
  {
    // button was not pressed, toggle LEDs
    digitalWrite(3, LOW);
    digitalWrite(4, LOW);
    digitalWrite(5, HIGH);

    // sleep 250ms
    delay(250);

    // toggle LEDs
    digitalWrite(4, HIGH);
    digitalWrite(5, LOW);

    // sleep 250ms
    delay(250);
  }
}
