// HACKCU ARDUINO PROGRAM TO CONTROL A ROBOT USING TAP STRAP
// UDP WIFI TRANSMISSION BETWEEN ARDUINO AND VISUAL STUDIO
// EUDALD SANGENÍS - GUILLEM CORNELLA - HECTOR PASCUAL - LLUIS UMBERT

// Include all the libraries
#include <WiFiUdp.h>      // for programming of UDP routines
#include <Servo.h>        // to control the servo motors
#include <ESP8266WiFi.h>  // to connect to the WIFI module

// Define the pins of the motor shield module
#define enA 0     //D3
#define enB 14    //D5
#define in1 12    //D6
#define in2 13    //D7
#define in3 15    //D8
#define in4 16    //D0

// Define the speed of the motors, starting at 0
int motorSpeedA = 0;
int motorSpeedB = 0;

// Define the initial position degree of the servos
int initial_pos_cam = 0;
int initial_pos_steer =115;

// Define the router information
const char* ssid = "YOUR WIFI";     // The name of your WIFI router
const char* password = "YOUR PASSWORD";  // The password of your router

// Other definitions
#define ON_Board_LED 2            // Defining an On Board LED connected to the GPIO 2
Servo servo_cam;                  // The name of the servo that we are using for the camera control
Servo servo_steer;                // The name of the servo that we are using for robot steering
WiFiUDP udp;                      // Create a udp object
unsigned int localPort = 2000;    // The port to listen to incoming packets
char packetBuffer[50];            // Set up a buffer for incoming packets
String strCon;
String val_spc_str;
int angle;                        // Variable of the angle of the camera 



// Write the code to initialize the board, create the SETUP
void setup() {
  Serial.begin(115200);             // Sets the data rate in bits per second (baud) for serial data transmission.
  delay(500);                       // Wait for 500ms

  pinMode(enA, OUTPUT);             // Define the pins of the motor shield as outputs
  pinMode(enB, OUTPUT);
  pinMode(in1, OUTPUT);
  pinMode(in2, OUTPUT);
  pinMode(in3, OUTPUT);
  pinMode(in4, OUTPUT);
  pinMode(ON_Board_LED,OUTPUT);     // Define the Led on the board as an output
  
  digitalWrite(ON_Board_LED, HIGH); // Turn off the Led On Board
 
 
  servo_cam.attach(4);                  // Attach the servo to the pin GPIO 4, the same as D2 for the ESP8266 module
  servo_steer.attach(5);                // Attach the servo to the pin GPIO 5, the same as D1 for the ESP8266 module
  servo_cam.write(initial_pos_cam);      // Set the initial position to 0º
  servo_steer.write(initial_pos_steer);  // Set the initial position to 45º
  
  
  digitalWrite(ON_Board_LED, HIGH); // Turn off the Led On Board
  
  WiFi.begin(ssid, password);       // Connect to your WiFi router
  Serial.println("");               // Print a blank space
  
  // Wait for the connection
  Serial.print("Connecting");           // Print "Connecting" to the Serial Monitor
  while (WiFi.status() != WL_CONNECTED) // Make the Led on board Flash while connecting to the wifi router
  {
    Serial.print(".");                  // Print a dot "." while trying to connect
    digitalWrite(ON_Board_LED, LOW);    // Turn off the LED on board
    delay(250);                         // Wait for 250ms
    digitalWrite(ON_Board_LED, HIGH);   // Turn on the LED on board
    delay(250);                         // Wait for 250ms
  }  
  
  digitalWrite(ON_Board_LED, HIGH);     // Turn off the On Board LED when it is connected to the wifi router.
  
  // If successfully connected to the wifi router, the IP Address is displayed in the serial monitor
  Serial.println("");
  Serial.print("Successfully connected to : ");
  Serial.println(ssid);
  Serial.print("NodeMCU IP address : ");
  Serial.println(WiFi.localIP());
  
  udp.begin(localPort);               // Once connection is established, you can start listening to incoming packets.
  Serial.print("Local Port : ");      
  Serial.println(udp.localPort());    // Print the number of the port that's going to be used
}

// Write the code to run the program
void loop() 
{
  receive_packet();
}

//Waiting for incoming UDP packets
void receive_packet() 
{
    
  int packetSize = udp.parsePacket();
  if (packetSize) {
    IPAddress remoteIp = udp.remoteIP();
    int len = udp.read(packetBuffer, 255);
    if (len > 0) packetBuffer[len] = 0;
    strCon = packetBuffer;
    val_spc_str = packetBuffer;
    // Serial.println(packetBuffer); 
    
    if (strCon=="led on"){
      digitalWrite(ON_Board_LED, LOW);
    }
    else if (strCon=="led off"){
      digitalWrite(ON_Board_LED, HIGH);
    }
    else if (strCon=="A"){ //W
        motorSpeedA = 1000;
        motorSpeedB = 1000;
        // Set Motor A forward
        digitalWrite(in1, HIGH);
        digitalWrite(in2, LOW);
        // Set Motor B forward
        digitalWrite(in3, HIGH);
        digitalWrite(in4, LOW); 
    }
     else if (strCon=="E"){ //S
        motorSpeedA = 1000;
        motorSpeedB = 1000;
        // Set Motor A backward
        digitalWrite(in1, LOW);
        digitalWrite(in2, HIGH);
        // Set Motor B backward
        digitalWrite(in3, LOW);
        digitalWrite(in4, HIGH);
     }
     else if (strCon=="I"){ //A LEFT
      servo_steer.write(70);
     }
     else if (strCon=="O"){ //D RIGHT
      servo_steer.write(145);
     }
     else if (strCon=="U"){ //U CENTER
      servo_steer.write(initial_pos_steer);
     }
     else if (strCon=="T"){ //STOP
        motorSpeedA = 0;
        motorSpeedB = 0;
        // Set Motor A backward
        digitalWrite(in1, LOW);
        digitalWrite(in2, LOW);
        // Set Motor B backward
        digitalWrite(in3, LOW);
        digitalWrite(in4, LOW);
     }   
    String strings = val_spc_str;
    Serial.println(strings);
    strings.remove(0,3);
    int val_spc = strings.toInt();
    Serial.println(val_spc);
    
    motorSpeedA = map(val_spc, 0, 90, 500, 1000);
    motorSpeedB = map(val_spc, 0, 90, 500, 1000);
    
    analogWrite(enA, motorSpeedA); // Send PWM signal to motor A
    analogWrite(enB, motorSpeedB); // Send PWM signal to motor B

    delay(700);

    motorSpeedA = 0;
    motorSpeedB = 0;
    digitalWrite(in1, LOW);
    digitalWrite(in2, LOW);
    digitalWrite(in3, LOW);
    digitalWrite(in4, LOW);
    // digitalWrite(enA, LOW);
    // digitalWrite(enB, LOW);
}
}

// ARDUINO CODE END
