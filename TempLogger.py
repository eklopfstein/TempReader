from sense_hat import SenseHat
from time import sleep
import subprocess

def logTemp():
    
    sense = SenseHat()
    
    avgTemp = (sense.get_temperature() + sense.get_humidity()) / 2
    #To ensure accuracy we average the temp from the normal reading and the humidity reading
    output = subprocess.check_output("cat /sys/class/thermal/thermal_zone0/temp", shell=True)
    cpu = int(output) / 1000
    #Uses the tempurature of the cpu divided by 1000 to negate the effect of the heat from the cpu
    return ((avgTemp - ((cpu - avgTemp)/5.466)-12) * (9/5)) + 32
    #This equation negates the effect of the cpu temp and converts to Fahrenheit