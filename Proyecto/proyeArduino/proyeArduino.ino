#include <DHT11.h>
DHT11 dht11(2);
int trig = 6;
int echo = 5;
int pot = A1;
int ldr = A0;
long tiempo;
long distancia;
int abierto;
int luz;
long lectPot;
long lectLDR;
float temperatura;
float humedad;
int lecturaBot;
int leds[] = {13,14,15,16,17};
void setup() {
Serial.begin(9600);
pinMode(trig,OUTPUT);
pinMode(echo,INPUT);
pinMode(pot,INPUT);
pinMode(ldr,INPUT);
for(int i = 0; i < 5; i++)
  pinMode(leds[i],OUTPUT);
while(!Serial){}

}

void loop() {
  if(Serial.available()){
    lecturaBot = Serial.parseInt();
    for(int i = 4; i >= 0; i--){
      if(round((lecturaBot/pow(10,i)))%10 == 1)
        digitalWrite(leds[i],HIGH);
      else
        digitalWrite(leds[i],LOW); 
    }
  }
  digitalWrite(trig,HIGH);
  delayMicroseconds(10);
  digitalWrite(trig,LOW);
  tiempo = pulseIn(echo,HIGH);
  distancia = tiempo/59;
  distancia = constrain(distancia,0,150);
  if(distancia < 10)
    abierto = 1;
  else
    abierto = 0;
  lectPot = analogRead(pot);
  lectLDR = analogRead(ldr);
  if(lectLDR <800)
    luz = 1;
  else
    luz = 0;
  //dht11.read(humedad,temperatura);
  lectPot = map(lectPot,0,1024,0,100);
  lectLDR = map(lectLDR,0,1024,0,100);
  Serial.println(String(abierto)+","+String(lectPot)+","+String(lectLDR));
  delay(100);

}
