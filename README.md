## Privacy

The issue of privacy, especially regarding future use cases of the monitor, like transmitting Health data, is from great
concern. Therefore, it is aimed to at least provide some kind of encryption.

- ### Encryption
  Currently, no encryption is in place when transmitting data like the screen of the VR application. It is planned that
the transmission will be encrypted via TLS.

- ### Monitor Identification
  The identification of the monitor is implemented via a simple 4-Digit ID that is generated on Start Up.
It is also known when connecting to the monitor what kind of transmissions the monitor expects.

## General Architecture

- ### Connection
  The connection is first initialised by the monitor sending a broadcast message in the current network. 
This broadcast message is then accepted by the Unity Application that again then sends a Keep Alive message to the 
monitor. When the monitor receives this first initial keep alive message he in turn also sends a keep Alive message 
to the Unity Application. When from either point the keep alive message is missing for more than 15 seconds
the connection is broken and the Application is searching for a new Server and vice versa. 
Connection also can be broke up manually by sending a disconnect message to the other device.
