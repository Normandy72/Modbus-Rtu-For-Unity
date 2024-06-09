# MODBUS RTU for UNITY

ModbusRTU is a library for working with the Modbus RTU protocol in C#. The library provides simple methods for reading and writing registers, coils, and other Modbus RTU elements via a serial port.

### INSTALLATION
You can copy the source code from the ModbusRTU.cs file directly into your project, or add the ModbusRTU.cs file as a reference to your project.

### USAGE
ATTENTION! It works only with .NET Framework (Project Settings -> Player -> Other Settings -> Api Compatibility Level -> .NET Framework).

1. Create an instance of the ModbusRTU class, specifying the serial port name and baud rate.
2. Call the `Connect()` method to establish a connection with the device.
3. Perform the necessary read or write operations on registers, coils, etc., using the appropriate methods of the ModbusRTU class.
4. After completing work with the device, call the `Disconnect()` method to close the connection.

```
using UnityEngine;
using System;

public class Example : MonoBehaviour
{
    private ModbusRTU _modbus;

    void Start()
    {
        _modbus = new ModbusRTU("COM1", 9600);

        if(_modbus.Connect())
        {
            // DO SOMETHING IF CONNECTED
        }
        else
        {
            // DO SOMETHING IF CONNECTION FAILED
        }
    }
}
```

### METHODS
#### Connect()
Establishes a connection with the device. Returns true if the connection is established successfully, and false otherwise.

#### Disconnect()
Closes the connection with the device.

### Reading and writing registers
#### ReadSingleRegister(byte deviceId, ushort startAddress)
Reads the value of a single register.

#### ReadSingleRegisterAsync(byte deviceId, ushort startAddress)
Asynchronously reads the value of a single register.

#### WriteSingleRegister(byte deviceId, ushort registerAddress, ushort value)
Writes a value to a single register.

#### WriteSingleRegisterAsync(byte deviceId, ushort registerAddress, ushort value)
Asynchronously writes a value to a single register.

#### WriteMultipleRegisters(byte deviceId, ushort startAddress, ushort[] values)
Writes values to multiple consecutive registers.

#### WriteMultipleRegistersAsync(byte deviceId, ushort startAddress, ushort[] values)
Asynchronously writes values to multiple consecutive registers.

### Reading and writing coils
#### ReadCoils(byte deviceId, ushort startAddress, ushort coilCount)
Reads the values of coils.

#### ReadCoilsAsync(byte deviceId, ushort startAddress, ushort coilCount)
Asynchronously reads the values of coils.

#### WriteSingleCoil(byte deviceId, ushort coilAddress, bool value)
Writes a value to a single coil.

#### WriteSingleCoilAsync(byte deviceId, ushort coilAddress, bool value)
Asynchronously writes a value to a single coil.

#### WriteMultipleCoils(byte deviceId, ushort startAddress, bool[] values)
Writes values to multiple coils.

#### WriteMultipleCoilsAsync(byte deviceId, ushort startAddress, bool[] values)
Asynchronously writes values to multiple coils.

### Other functions
#### ReadExceptionStatus(byte deviceId)
Reads the exception status (only for Modbus RTU).

#### ReadExceptionStatusAsync(byte deviceId)
Asynchronously reads the exception status (only for Modbus RTU).

#### DiagnosticsEcho(byte deviceId, byte[] data)
Performs a diagnostic echo request.

#### DiagnosticsEchoAsync(byte deviceId, byte[] data)
Asynchronously performs a diagnostic echo request.