import os
import time

def export_gpio(pin):
    """Export the GPIO pin."""
    try:
        with open("/sys/class/gpio/export", "w") as f:
            f.write(str(pin))
    except FileExistsError:
        pass  # Pin is already exported

def set_gpio_direction(pin, direction):
    """Set GPIO direction (in or out)."""
    direction_path = f"/sys/class/gpio/gpio{pin}/direction"
    with open(direction_path, "w") as f:
        f.write(direction)

def read_gpio_value(pin):
    """Read the value of a GPIO pin."""
    value_path = f"/sys/class/gpio/gpio{pin}/value"
    with open(value_path, "r") as f:
        return int(f.read().strip())

def write_gpio_value(pin, value):
    """Write a value (0 or 1) to a GPIO pin."""
    value_path = f"/sys/class/gpio/gpio{pin}/value"
    with open(value_path, "w") as f:
        f.write(str(value))
def unexport_gpio(pin):
    """Unexport the GPIO pin."""
    with open("/sys/class/gpio/unexport", "w") as f:
        f.write(str(pin))

# GPIO pins
toggle_switch = 16  # Toggle switch
led_pin = 17        # LED connected to GPIO 17
tact_switches = [18, 19]  # Tact switches

try:
    # Export GPIO pins
    export_gpio(toggle_switch)
    export_gpio(led_pin)
    for pin in tact_switches:
        export_gpio(pin)

    # Set GPIO directions
    set_gpio_direction(toggle_switch, "in")
    set_gpio_direction(led_pin, "out")
    for pin in tact_switches:
        set_gpio_direction(pin, "in")

    # Initialize LED to OFF
    write_gpio_value(led_pin, 0)

    # Track previous states for tact switches to detect button press
    prev_tact_states = {pin: 0 for pin in tact_switches}

    print("Monitoring switches... Press Ctrl+C to stop.")
    while True:
        # Read toggle switch state
        toggle_state = read_gpio_value(toggle_switch)

        # Control LED based on toggle switch state
        if toggle_state == 1:  # If toggle switch is ON
            write_gpio_value(led_pin, 1)  # Turn ON LED
        else:
            write_gpio_value(led_pin, 0)  # Turn OFF LED

        # Detect single press for tact switches
        for pin in tact_switches:
            current_state = read_gpio_value(pin)
            if current_state == 1 and prev_tact_states[pin] == 0:
                print(f"Tact switch {pin} pressed!")
                # Send a signal or handle the event here
            prev_tact_states[pin] = current_state

        # Small delay to reduce CPU usage
        time.sleep(0.05)

except KeyboardInterrupt:
    print("Exiting...")

finally:
    # Cleanup: Unexport GPIO pins
    unexport_gpio(toggle_switch)
    unexport_gpio(led_pin)
    for pin in tact_switches:
        unexport_gpio(pin)
