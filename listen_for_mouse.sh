#!/bin/bash

# Identify the mouse device (you may need to change this depending on your system)
MOUSE_DEVICE="/dev/input/eventX"  # Replace 'X' with the correct number for your mouse device

# Check if the device exists
if [ ! -e "$MOUSE_DEVICE" ]; then
    echo "Mouse device not found: $MOUSE_DEVICE"
    exit 1
fi

# Listen for events from the mouse device
echo "Listening for mouse clicks on $MOUSE_DEVICE..."

# Use cat to read input events and detect mouse button presses
sudo cat "$MOUSE_DEVICE" | while read -r line; do
    # Mouse button press event codes are usually 330 or 332 for button presses
    if echo "$line" | grep -q "EV_KEY"; then
        button=$(echo "$line" | grep -oP '(?<=code\s)\d+')
        value=$(echo "$line" | grep -oP '(?<=value\s)\d+')
        
        # Button press (value=1) or release (value=0)
        if [ "$value" -eq 1 ]; then
            echo "Mouse button $button pressed"
        fi
    fi
done
