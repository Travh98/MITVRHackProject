# Copy Paste VR Experience
Quickly prototype Virtual Reality scenes using AI Object Detection on the Qualcomm RB3 Gen 2 board.

Our ergonomic copy paste handheld tool allows the user to scan objects in the real world, detect and classify the scanned object, and send it to the virtual reality scene.

## Data Pipeline
* 📷Qualcomm RB3 Gen 2 board scans the physical space like a handheld camera
* When the user presses the copy button, a serial message is sent from the ESP32 to the Qualcomm (USB-C to USB-A, baudrate 115200)
* 📶The Qualcomm receives the copy signal and publishes the latest detected high-confidence object to the Redis server
* 📚When the user presses the paste button, the signal goes through the Qualcomm and into the Redis Server
* The VR Unity scene is subscribed to the Redis server and listens for the user's button presses
* 📦When the paste signal is recieved, the Unity project takes the AI identified name and uses the SketchFab API to spawn a 3D model in Virtual Reality
* 🎨This allows the user to rapidly prototype ideas in virtual reality 

## Getting Started
### How to get into Qualcomm RB3 Gen 2
* Plug in Qualcomm HDMI to USB-C
* Plug in Qualcomm USB-C to USB-A
* Use `usbipd` to see connected USB devices
* `usbipd bind --busid <bus number>` to bind the USB port
* `usbipd attach --busid <bus number> --wsl` to bind USB to WSL
This helps us share USB ports from a Windows machine to it's WSL Linux

### Use `adb` on WSL for debugging the Qualcomm
* `adb kill-server` to kill running adb servers (need to do this often)
* `adb devices` to see connected devices
* `adb shell` to log into the Qualcomm device

Leverage [Qualcomm's Sample AI Python scripts](https://docs.qualcomm.com/bundle/publicresource/topics/80-70015-50/python-sample-applications.html)

In WSL, start vscode from WSL, go to the Qualcomm extension and Application configuration multimedia to see the sample projects

### Setup ESP32 with Hardware Controls
* Connect ESP32 (USB-C) to the Qualcomm's USB-A port
* Set the Qualcomm's USB Port's baudrate to 115200: `stty -F /dev/ttyUSB0 115200`
* Listen to the output on the serial port: `cat /dev/ttyUSB0`

### Setup the Qualcomm to detect and classify objects with AI
* Connect Qualcomm's USB-C to a Windows machine's USB-A
* Use `usbipd` to share the Window's USB-A port with the Window's WSL
* In WSL Linux, ensure you can see the device with `adb devices`
* Log into the Qualcomm's shell environment with `adb shell`
* Connect the Qualcomm to the same network as the Redis Server machine
* Push the `run_detection.sh` onto the Qualcomm device using the `adb push` command
* Run the `run_detection.sh` bash script to start the Qualcomm AI Object Detection
* The script will pipe the AI detection data to the Redis Server
* If you need to remove carriage returns `^M` run the sed command to remove them `sed 's/\r$//' run_detection.sh`

### Setup the Redis Server
* Make sure the Redis Server machine and the Qualcomm are connected on the same network
* With Redis installed on the machine, start Redis with `sudo service redis-server start`
* 
