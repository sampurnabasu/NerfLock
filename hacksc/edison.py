from datetime import datetime
from Servo import *
import time
from timeout import timeout
import os
import urllib
import json
#ref = firebase.FirebaseApplication('https://foreveralone.firebaseio.com', None)
def main():
    servo1 = Servo("First Servo")                                                   
    servo1.attach(5)                                                                
    servo2 = Servo("Second Servo")                                                  
    servo2.attach(3)                                                                
    servo1.write(86)                                                                
    servo2.write(85)
    servo3 = Servo("Third Servo")  
    servo3.attach(6)
    servo3.write(87)
    oldDoor = "False"
    while(1):
        #print "shoot"
        #shoot(servo1,servo2)
        #time.sleep(2)
        
        print "start"
        try:
            match,ready,confidence,door=callApi()
            print "good"
            print "door",door
            if door != oldDoor:
                print door,
                print oldDoor
                oldDoor=door
                moveDoor(door,servo3)
            if ready == 'True':
                if match == 'False' or confidence < .58:
                    shoot(servo1,servo2)
                    print "shoot"
        except:
            print "bad"
        
@timeout(2)
def callApi():
    urlStr = "https://foreveralone.firebaseio.com/.json"
    proxies = {'http': '189.112.3.87:3128','http' : '208.254.241.13:8080'}
    fileObj = urllib.urlopen(urlStr)
    for line in fileObj: #single line of "0", "1", or "2"
        print line
        parsed_json = json.loads(line)
        match = parsed_json['match']
        print match
        ready = parsed_json['ready']
        print ready
        confidence = parsed_json['confidence']
        print confidence
        door = parsed_json['door']
        print door
    return match,ready,confidence,door
def moveDoor(door,servo3):
    print door
    if(door=="True"):
        servo3.write(180)
        time.sleep(.224)
        servo3.write(87)
        time.sleep(.224);
    else:
        servo3.write(0)
        time.sleep(.224)
        servo3.write(87)
        time.sleep(.224);        
def shoot(servo1,servo2):
    servo1.write(180)
    time.sleep(.220)
    servo1.write(86)
    time.sleep(.450)
    servo2.write(0)
    time.sleep(.350)
    servo2.write(85)
    time.sleep(.200)
    servo2.write(180)
    time.sleep(.200)
    servo2.write(85)
    time.sleep(.200)
    servo2.write(0)
    time.sleep(.350)
    servo2.write(85)
    time.sleep(.200)
    servo2.write(180)
    time.sleep(.200)
    servo2.write(85)
    time.sleep(.200)
    servo2.write(0)
    time.sleep(.350)
    servo2.write(85)
    time.sleep(.200)
    servo2.write(180)
    time.sleep(.200)
    servo2.write(85)
    time.sleep(.200)
    servo1.write(0)
    time.sleep(.190)
    servo1.write(85)
    time.sleep(.450)
if __name__ == "__main__":
    main()