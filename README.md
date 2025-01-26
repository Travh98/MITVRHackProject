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
