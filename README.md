# SERIAL COMMUNICATION SYSTEM

# Contents

- In this project work, the development of an integrated test software for telemetry, tracking and command (TT&C) is developed and its test scenarios. The source code of the Goddard Space Flight Center flight software is provided as reference for TT&C.
- This project work develops two models, one for telemetry, and one for telecommand, in which user input data is encrypted and decrypted, respectively, comparing the handled data with predefined packet designs. The software is implemented on an Integrated Development Environment (IDE) using C# as the main programming language. The IDE handles the debugging and building parts of the system. The verification of the project has been done by developing two prototype interfaces that connect to virtual serial ports and display the input data in the correct format ready to be handled by the payload ground station.

# Required programs

If it needs modifications or a quick view through the code, you can simply install visual studio and open the .sln file.

While for testing, I installed an app that creates virtual serial ports called "HHD Virtual Serial Port Tools", that I used to connect both app to check connection and other stuff. I highly suggest the app if you need to check if the data is correctly sent according to your requirements and modifications.
