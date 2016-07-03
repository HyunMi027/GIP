// ease of use
const int sensorPin = A0;
// baseline
const float baselineTemp = 22;

// Constructor
void setup()
{
  Serial.begin(9600);

  for(int i = 2; i < 5; i++)
  {
    pinMode(i, OUTPUT);
    digitalWrite(i, LOW);
  }
}

// Code loop
void loop()
{
  int sensorVal = analogRead(sensorPin);
  float voltage = (sensorVal/1024.0)*5.0;
  float temp = (voltage - 0.5)*100;
  
  String str = "Sensor value: ";
  Serial.println(str + sensorVal + ", voltage: " + voltage + ", degrees C: " + temp);

  if(temp < baselineTemp)
  {
    digitalWrite(2, LOW);
    digitalWrite(3, LOW);
    digitalWrite(4, LOW);
  }
  else
  {
    if(temp < baselineTemp+2)
    {
      digitalWrite(2, HIGH);
      digitalWrite(3, LOW);
      digitalWrite(4, LOW);
    }
    else
    {
      if (temp < baselineTemp+4)
      {
        digitalWrite(2, HIGH);
        digitalWrite(3, HIGH);
        digitalWrite(4, LOW);
      }
      else
      {
        digitalWrite(2, HIGH);
        digitalWrite(3, HIGH);
        digitalWrite(4, HIGH);
      }
    }
  }

  delay(1);
}
