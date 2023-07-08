# BLETemperatureSensor
Listen for Inkbird IBS TH2 temperature sensor advertisements and parse temp, humidity and battery

First version queries device for additional info, but in my what comes back is quite opaque.  One service offers notifications which I subscribe to but I never see any updates coming that way.

Second version just listens for advertisements from a specified bluetooth address and then if the TH2 is found it captures the temp, humidty and battery levels for display on the console.

TODO:  compensate for F/C and figure out where calibration bytes are hidden
