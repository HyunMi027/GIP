// {R, G, B}
const int LED[] = {11, 9, 10};
const int Sensor[] = {A0, A1, A2};

int LEDValue[] = {0, 0, 0};
int SensorValue[] = {0, 0, 0};

void setup()
{
  Serial.begin(9600);

  pinMode(LED[0], OUTPUT);
  pinMode(LED[1], OUTPUT);
  pinMode(LED[2], OUTPUT);
}

void loop()
{
  SensorValue[0] = analogRead(Sensor[0]);
  SensorValue[1] = analogRead(Sensor[1]);
  SensorValue[2] = analogRead(Sensor[2]);
  
  LEDValue[0] = SensorValue[0]/4;
  LEDValue[1] = SensorValue[1]/4;
  LEDValue[2] = SensorValue[2]/4;

  SerialPrintDebug();
  
  analogWrite(LED[0], LEDValue[0]);
  analogWrite(LED[1], LEDValue[1]);
  analogWrite(LED[2], LEDValue[2]);
}

void SerialPrintDebug()
{
  String str = "Raw: {";
  str = str + SensorValue[0] + ", " + SensorValue[1] + ", " + SensorValue[2] + "}\tMapped: {" + LEDValue[0] + ", " + LEDValue[1] + ", " + LEDValue[2] + "}";
  Serial.println(str);
}
