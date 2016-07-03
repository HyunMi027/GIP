using System;
using System.IO.Ports;

namespace ConnectionTest
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            SerialPort port = new SerialPort("COM3", 9600);
            port.Open();
            while (port.IsOpen)
            {
                Console.WriteLine(port.ReadLine());
            }
        }
    }
}

/* Arduino code

    void setup() 
    {
        Serial.begin(9600);
    }

    void loop()
    {
        Serial.println(millis());
    }
 */
