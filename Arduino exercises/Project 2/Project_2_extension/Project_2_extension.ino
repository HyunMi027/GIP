// Globals
int mode = 0;

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
  if (digitalRead(2) == HIGH)
  {
    // switch was pressed, blink all LEDs
    digitalWrite(3, HIGH);
    digitalWrite(4, HIGH);
    digitalWrite(5, HIGH);
    delay(250);
    digitalWrite(3, LOW);
    digitalWrite(4, LOW);
    digitalWrite(5, LOW);
    delay(250);
    digitalWrite(3, HIGH);
    digitalWrite(4, HIGH);
    digitalWrite(5, HIGH);
    delay(250);
    digitalWrite(3, LOW);
    digitalWrite(4, LOW);
    digitalWrite(5, LOW);
    delay(750);

    // switch mode variable
    if (mode == 2)
    {
      mode = 0;
    }
    else
    {
      ++mode;
    }
  }
  else
  {
    if (mode == 0)
    {
      digitalWrite(4, HIGH);
      digitalWrite(5, HIGH);
      delay(450);
      digitalWrite(4, LOW);
      digitalWrite(5, LOW);
      delay(200);
    }

    if (mode == 1)
    {
      digitalWrite(5, LOW);
      digitalWrite(4, HIGH);
      delay(500);

      digitalWrite(4, LOW);
      digitalWrite(3, HIGH);
      delay(500);

      digitalWrite(3, LOW);
      digitalWrite(4, HIGH);
      delay(500);

      digitalWrite(4, LOW);
      digitalWrite(5, HIGH);
      delay(500);
    }

    if (mode == 2)
    {
      digitalWrite(3, HIGH);
      digitalWrite(4, HIGH);
      digitalWrite(5, HIGH);
      delay(250);
      digitalWrite(3, LOW);
      digitalWrite(4, LOW);
      digitalWrite(5, LOW);
      delay(250);
    }
  }
}
