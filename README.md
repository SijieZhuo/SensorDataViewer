# SensorDataViewer

## What is a SensorDataViewer ?
SensorDataViewer is an C# Windows Presentation Foundation(WPF) application that allows user
to view the data user collected from different devices.

Currently, the device that can be connected to this application includes: Shimmer 3, and Android phone.

The data collected and displayed would be from the shimmer, android phone, and the PC (which includes system log and Chrome log).

SensorDataViewer was developed for the research project conducted at the University of Auckland.

**Currently only tested in Windows 10**

## Software Installation
the latest version of the application can be found in the release page. 
https://github.com/SijieZhuo/SensorDataViewer/releases
1. Extract the zip file
2. Run "stressProject.sln" with Visual Studio
3. build the project


## Development Setup
We recommend using Visual Studios to contribute to MonitorApp.
Clone this repository on a Windows 10 computer and you will be good to go.

## Functionalities
The main purpose of this application at this stage is collecting and storing data from different sensors (include the PC itself).
Also, this application would display the data from these sensors to the user

The data would be from 4 different parts: Shimmer sensor, Android phone, system log, and Chrome log.

### Shimmer
The shimmer 3 device would collect 5 different set of data for now. The configuration of the device is set by this application.

The Data includes:

- PPG signal
- GSR signal
- Accelerometer signal(3 sets of data, x, y, z).

This application uses the official Shimmer API the manufactuer provided. It is used to connect the decive to PC (via bluetooth), 
and configurate the shimmer device, and tranfer the data to PC.

### Android phone
The Android phone collects the data via an app, which can be found in the link:
https://github.com/SijieZhuo/sound_activity_recorder

The data collected from the app would include:

- Sound (include loudness/decibels, and sound frequency)
- Accelerometer
- Gyroscope
- Megnetometer
- Network status (downloading and uploading in bytes)
- Screen status (screen is on/off/lockscreen)
- Current Foreground App
- User touch event (gesture start time, x, y coordinate, end time, x, y coordinate, and gesture type)

### System Log
The system log would record the system status regard to the application that the computer is focused on.
This part of the code is referenced from another project developed by part 4 software engineering student.
the link is the following: https://github.com/rtan265/MonitorApp

The data would include: 

- Recorded time
- Module Name
- Window Name
- the status of the application (focused/visible)

### Chrome Log
The Chrome log would record the browser activities, the code is also referenced from the part 4 software engineering student, 
The link is the following: https://github.com/rtan265/MonitorPlugin (original version)

The modified version is in the following link: https://github.com/SijieZhuo/StressChromeExtension

The installation of the extension: 

1. Download the zip from the link above and unzip it
2. Go to "chrome://extensions/" using Google Chrome
3. Click on the "Load unpacked"
4. browse to the location of the file
5. the logging would start once the extension is installed, and will stop after it is uninstalled

This part of the application would require Google Chrome installed, and install a specific Chrome Extension.

The data would include:

- Timestamps of activity
- Number of tabs currently open
- URL of currently viewed tab
- Hostname of currently viewed tab
- Tab Title of currectly viewed tab
- TabID of currently viewed tab

## Recorded File
The data will be saved in .csv file. There would be a folder in the root directory called "Records", 
and each save would create a folder inside "Records", and it would include a number of .csv file.

Depending on the number of sensors connected to application, the number of .csv file 
would be different. 
