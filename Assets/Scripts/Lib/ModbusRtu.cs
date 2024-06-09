using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System;

public class ModbusRTU
{
    private SerialPort _serialPort;

    private const ushort SINGLE_REGISTER_NUMBER = 1;
    private const int TIME_TO_SLEEP = 100;  // ms        

    #region Modbus functions
    private const byte READ_COILS_FUNCTION = 0x01;
    private const byte READ_DISCRETE_INPUTS_FUNCTION = 0x02;
    private const byte READ_SINGLE_REGISTER_FUNCTION = 0x03;
    private const byte READ_INPUT_REGISTERS_FUNCTION = 0x04;
    private const byte WRITE_SINGLE_COIL_FUNCTION = 0x05;
    private const byte WRITE_SINGLE_REGISTER_FUNCTION = 0x06;
    private const byte WRITE_MULTIPLE_COILS_FUNCTION = 0x0F;
    private const byte WRITE_MULTIPLE_REGISTERS_FUNCTION = 0x10;
    private const byte READ_EXCEPTION_STATUS_FUNCTION = 0x07;
    private const byte DIAGNOSTICS_FUNCTION = 0x08;
    #endregion

    public ModbusRTU(string portName, int baudRate)
    {
        _serialPort = new SerialPort(portName, baudRate)
        {
            Parity = Parity.None,
            DataBits = 8,
            StopBits = StopBits.One
        };
    }

    public bool Connect()
    {
        try
        {
            if(!_serialPort.IsOpen)
            {
                _serialPort.Open();
            }

            if(_serialPort.IsOpen)
            {
                Debug.Log("Modbus RTU: Connected.");
                return true;
            }
            else
            {
                Debug.LogError("Modbus RTU: Connection failed. Port is not open.");
                return false;
            }
        }
        catch(Exception ex)
        {
            Debug.LogError($"Modbus RTU: Connection error. {ex.Message}");
            return false;
        }
    }

    public void Disconnect()
    {
        _serialPort.Close();
        Debug.Log("Modbus RTU: Disconnected.");
    }

    #region Read coils 0x01
    public bool[] ReadCoils(byte deviceId, ushort startAddress, ushort numberOfCoils)
    {
        List<byte> request = new()
        {
            deviceId,
            READ_COILS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(numberOfCoils >> 8),
            (byte)numberOfCoils
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        int responseLength = 5 + (numberOfCoils + 7) / 8;
        byte[] response = new byte[responseLength];
        _serialPort.Read(response, 0, responseLength);

        if (!CheckCRC(response, responseLength - 2, (ushort)(response[responseLength - 1] << 8 | response[responseLength - 2])))
        {
            Debug.Log("Modbus RTU: Read Coils CRC error.");
            return null;
        }
        else
        {
            Debug.Log("Modbus RTU: Read Coils successful.");
        }

        bool[] coils = new bool[numberOfCoils];
        for (int i = 0; i < numberOfCoils; i++)
        {
            int byteIndex = 3 + i / 8;
            int bitIndex = i % 8;
            coils[i] = (response[byteIndex] & (1 << bitIndex)) != 0;
        }

        return coils;
    }

    public async Task<bool[]> ReadCoilsAsync(byte deviceId, ushort startAddress, ushort numberOfCoils)
    {
        List<byte> request = new()
        {
            deviceId,
            READ_COILS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(numberOfCoils >> 8),
            (byte)numberOfCoils
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        int responseLength = 5 + (numberOfCoils + 7) / 8;
        byte[] response = new byte[responseLength];
        await Task.Run(() => _serialPort.Read(response, 0, responseLength));

        if (!CheckCRC(response, responseLength - 2, (ushort)(response[responseLength - 1] << 8 | response[responseLength - 2])))
        {
            Debug.Log("Modbus RTU: Read Coils CRC error.");
            return null;
        }
        else
        {
            Debug.Log("Modbus RTU: Read Coils successful.");
        }

        bool[] coils = new bool[numberOfCoils];
        for (int i = 0; i < numberOfCoils; i++)
        {
            int byteIndex = 3 + i / 8;
            int bitIndex = i % 8;
            coils[i] = (response[byteIndex] & (1 << bitIndex)) != 0;
        }

        return coils;
    }
    #endregion

    #region Read discrete inputs 0x02
    public bool[] ReadDiscreteInputs(byte deviceId, ushort startAddress, ushort numberOfInputs)
    {
        List<byte> request = new()
        {
            deviceId,
            READ_DISCRETE_INPUTS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(numberOfInputs >> 8),
            (byte)numberOfInputs
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        int responseLength = 5 + (numberOfInputs + 7) / 8;
        byte[] response = new byte[responseLength];
        _serialPort.Read(response, 0, responseLength);

        if (!CheckCRC(response, responseLength - 2, (ushort)(response[responseLength - 1] << 8 | response[responseLength - 2])))
        {
            Debug.Log("Modbus RTU: Read Discrete Inputs CRC error.");
            return null;
        }
        else
        {
            Debug.Log("Modbus RTU: Read Discrete Inputs successful.");
        }

        bool[] inputs = new bool[numberOfInputs];
        for (int i = 0; i < numberOfInputs; i++)
        {
            int byteIndex = 3 + i / 8;
            int bitIndex = i % 8;
            inputs[i] = (response[byteIndex] & (1 << bitIndex)) != 0;
        }

        return inputs;
    }

    public async Task<bool[]> ReadDiscreteInputsAsync(byte deviceId, ushort startAddress, ushort numberOfInputs)
    {
        List<byte> request = new()
        {
            deviceId,
            READ_DISCRETE_INPUTS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(numberOfInputs >> 8),
            (byte)numberOfInputs
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        int responseLength = 5 + (numberOfInputs + 7) / 8;
        byte[] response = new byte[responseLength];
        await Task.Run(() => _serialPort.Read(response, 0, responseLength));

        if (!CheckCRC(response, responseLength - 2, (ushort)(response[responseLength - 1] << 8 | response[responseLength - 2])))
        {
            Debug.Log("Modbus RTU: Read Discrete Inputs CRC error.");
            return null;
        }
        else
        {
            Debug.Log("Modbus RTU: Read Discrete Inputs successful.");
        }

        bool[] inputs = new bool[numberOfInputs];
        for (int i = 0; i < numberOfInputs; i++)
        {
            int byteIndex = 3 + i / 8;
            int bitIndex = i % 8;
            inputs[i] = (response[byteIndex] & (1 << bitIndex)) != 0;
        }

        return inputs;
    }
    #endregion

    #region Read single register 0x03
    public int ReadSingleRegister(byte deviceId, ushort startAddress)
    {
        List<byte> request = new()
        {
            deviceId,                         
            READ_SINGLE_REGISTER_FUNCTION,                            
            (byte)(startAddress >> 8),
            (byte)startAddress,
            0x00,                          
            (byte)SINGLE_REGISTER_NUMBER
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        _serialPort.Read(response, 0, 8);

        if(!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Read Single Register CRC error.");
            return -1;
        }
        else
        {
            Debug.Log("Modbus RTU: Read Single Register successful.");
        }

        return (response[3] << 8) | response[4];
    }

    public async Task<int> ReadSingleRegisterAsync(byte deviceId, ushort startAddress)
    {
        List<byte> request = new()
        {
            deviceId,
            READ_SINGLE_REGISTER_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            0x00,
            (byte)SINGLE_REGISTER_NUMBER
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        await Task.Run(() => _serialPort.Read(response, 0, 8));

        if(!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Read Single Register CRC error.");
            return -1;
        }
        else
        {
            Debug.Log("Modbus RTU: Read Single Register successful.");
        }

        return (response[3] << 8) | response[4];
    }
    #endregion
    
    #region Read input registers 0x04
    public ushort[] ReadInputRegisters(byte deviceId, ushort startAddress, ushort numberOfRegisters)
    {
        List<byte> request = new()
        {
            deviceId,
            READ_INPUT_REGISTERS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(numberOfRegisters >> 8),
            (byte)numberOfRegisters
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        int responseLength = 5 + 2 * numberOfRegisters;
        byte[] response = new byte[responseLength];
        _serialPort.Read(response, 0, responseLength);

        if (!CheckCRC(response, responseLength - 2, (ushort)(response[responseLength - 1] << 8 | response[responseLength - 2])))
        {
            Debug.Log("Modbus RTU: Read Input Registers CRC error.");
            return null;
        }
        else
        {
            Debug.Log("Modbus RTU: Read Input Registers successful.");
        }

        ushort[] registers = new ushort[numberOfRegisters];
        for (int i = 0; i < numberOfRegisters; i++)
        {
            registers[i] = (ushort)((response[3 + i * 2] << 8) | response[4 + i * 2]);
        }

        return registers;
    }

    public async Task<ushort[]> ReadInputRegistersAsync(byte deviceId, ushort startAddress, ushort numberOfRegisters)
    {
        List<byte> request = new()
        {
            deviceId,
            READ_INPUT_REGISTERS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(numberOfRegisters >> 8),
            (byte)numberOfRegisters
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        int responseLength = 5 + 2 * numberOfRegisters;
        byte[] response = new byte[responseLength];
        await Task.Run(() => _serialPort.Read(response, 0, responseLength));

        if (!CheckCRC(response, responseLength - 2, (ushort)(response[responseLength - 1] << 8 | response[responseLength - 2])))
        {
            Debug.Log("Modbus RTU: Read Input Registers CRC error.");
            return null;
        }
        else
        {
            Debug.Log("Modbus RTU: Read Input Registers successful.");
        }

        ushort[] registers = new ushort[numberOfRegisters];
        for (int i = 0; i < numberOfRegisters; i++)
        {
            registers[i] = (ushort)((response[3 + i * 2] << 8) | response[4 + i * 2]);
        }

        return registers;
    }
    #endregion

    #region Write single coil 0x05
    public void WriteSingleCoil(byte deviceId, ushort coilAddress, bool state)
    {
        List<byte> request = new()
        {
            deviceId,
            WRITE_SINGLE_COIL_FUNCTION,
            (byte)(coilAddress >> 8),
            (byte)coilAddress,
            state ? (byte)0xFF : (byte)0x00,
            0x00
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        _serialPort.Read(response, 0, 8);

        if (!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Write Single Coil CRC error.");
        }
        else
        {
            Debug.Log("Modbus RTU: Write Single Coil successful.");
        }
    }

    public async Task WriteSingleCoilAsync(byte deviceId, ushort coilAddress, bool state)
    {
        List<byte> request = new()
        {
            deviceId,
            WRITE_SINGLE_COIL_FUNCTION,
            (byte)(coilAddress >> 8),
            (byte)coilAddress,
            state ? (byte)0xFF : (byte)0x00,
            0x00
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        await Task.Run(() => _serialPort.Read(response, 0, 8));

        if (!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Write Single Coil CRC error.");
        }
        else
        {
            Debug.Log("Modbus RTU: Write Single Coil successful.");
        }
    }
    #endregion

    #region Write single register 0x06
    public void WriteSingleRegister(byte deviceId, ushort registerAddress, ushort value)
    {
        List<byte> request = new()
        {
            deviceId,
            WRITE_SINGLE_REGISTER_FUNCTION,
            (byte)(registerAddress >> 8),
            (byte)registerAddress,
            (byte)(value >> 8),
            (byte)value
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        _serialPort.Read(response, 0, 8);

        if(!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Write Single Register CRC error.");
        }
        else
        {
            Debug.Log("Modbus RTU: Write Single Register successful.");
        }
    }

    public async Task WriteSingleRegisterAsync(byte deviceId, ushort registerAddress, ushort value)
    {
        List<byte> request = new()
        {
            deviceId,
            WRITE_SINGLE_REGISTER_FUNCTION,
            (byte)(registerAddress >> 8),
            (byte)registerAddress,
            (byte)(value >> 8),
            (byte)value
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        await Task.Run(() => _serialPort.Read(response, 0, 8));

        if(!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Write Single Register CRC error.");
        }
        else
        {
            Debug.Log("Modbus RTU: Write Single Register successful.");
        }
    }
    #endregion

    #region Write multiple coils 0x0F
    public void WriteMultipleCoils(byte deviceId, ushort startAddress, bool[] states)
    {
        int byteCount = (states.Length + 7) / 8;
        List<byte> request = new()
        {
            deviceId,
            WRITE_MULTIPLE_COILS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(states.Length >> 8),
            (byte)states.Length,
            (byte)byteCount
        };

        for (int i = 0; i < byteCount; i++)
        {
            byte coilByte = 0;
            for (int bit = 0; bit < 8; bit++)
            {
                if (i * 8 + bit < states.Length && states[i * 8 + bit])
                {
                    coilByte |= (byte)(1 << bit);
                }
            }
            request.Add(coilByte);
        }

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        _serialPort.Read(response, 0, 8);

        if (!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Write Multiple Coils CRC error.");
        }
        else
        {
            Debug.Log("Modbus RTU: Write Multiple Coils successful.");
        }
    }

    public async Task WriteMultipleCoilsAsync(byte deviceId, ushort startAddress, bool[] states)
    {
        int byteCount = (states.Length + 7) / 8;
        List<byte> request = new()
        {
            deviceId,
            WRITE_MULTIPLE_COILS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(states.Length >> 8),
            (byte)states.Length,
            (byte)byteCount
        };

        for (int i = 0; i < byteCount; i++)
        {
            byte coilByte = 0;
            for (int bit = 0; bit < 8; bit++)
            {
                if (i * 8 + bit < states.Length && states[i * 8 + bit])
                {
                    coilByte |= (byte)(1 << bit);
                }
            }
            request.Add(coilByte);
        }

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        await Task.Run(() => _serialPort.Read(response, 0, 8));

        if (!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Write Multiple Coils CRC error.");
        }
        else
        {
            Debug.Log("Modbus RTU: Write Multiple Coils successful.");
        }
    }
    #endregion

    #region Write multiple registers 0x10
    public void WriteMultipleRegisters(byte deviceId, ushort startAddress, ushort[] values)
    {
        int byteCount = values.Length * 2;
        List<byte> request = new()
        {
            deviceId,
            WRITE_MULTIPLE_REGISTERS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(values.Length >> 8),
            (byte)values.Length,
            (byte)byteCount
        };

        foreach (ushort value in values)
        {
            request.Add((byte)(value >> 8));
            request.Add((byte)value);
        }

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        _serialPort.Read(response, 0, 8);

        if (!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Write Multiple Registers CRC error.");
        }
        else
        {
            Debug.Log("Modbus RTU: Write Multiple Registers successful.");
        }
    }

    public async Task WriteMultipleRegistersAsync(byte deviceId, ushort startAddress, ushort[] values)
    {
        int byteCount = values.Length * 2;
        List<byte> request = new()
        {
            deviceId,
            WRITE_MULTIPLE_REGISTERS_FUNCTION,
            (byte)(startAddress >> 8),
            (byte)startAddress,
            (byte)(values.Length >> 8),
            (byte)values.Length,
            (byte)byteCount
        };

        foreach (ushort value in values)
        {
            request.Add((byte)(value >> 8));
            request.Add((byte)value);
        }

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        byte[] response = new byte[8];
        await Task.Run(() => _serialPort.Read(response, 0, 8));

        if (!CheckCRC(response, 6, (ushort)(response[7] << 8 | response[6])))
        {
            Debug.Log("Modbus RTU: Write Multiple Registers CRC error.");
        }
        else
        {
            Debug.Log("Modbus RTU: Write Multiple Registers successful.");
        }
    }
    #endregion

    #region Read exception status 0x07
    public byte ReadExceptionStatus(byte deviceId)
    {
        List<byte> request = new()
        {
            deviceId,
            READ_EXCEPTION_STATUS_FUNCTION
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        byte[] response = new byte[5];
        _serialPort.Read(response, 0, 5);

        if (!CheckCRC(response, 3, (ushort)(response[4] << 8 | response[3])))
        {
            Debug.Log("Modbus RTU: Read Exception Status CRC error.");
            return 0xFF;  // 0xFF means error
        }
        else
        {
            Debug.Log("Modbus RTU: Read Exception Status successful.");
        }

        return response[2];
    }

    public async Task<byte> ReadExceptionStatusAsync(byte deviceId)
    {
        List<byte> request = new()
        {
            deviceId,
            READ_EXCEPTION_STATUS_FUNCTION
        };

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        byte[] response = new byte[5];
        await Task.Run(() => _serialPort.Read(response, 0, 5));

        if (!CheckCRC(response, 3, (ushort)(response[4] << 8 | response[3])))
        {
            Debug.Log("Modbus RTU: Read Exception Status CRC error.");
            return 0xFF;  // 0xFF means error
        }
        else
        {
            Debug.Log("Modbus RTU: Read Exception Status successful.");
        }

        return response[2];
    }
    #endregion

    #region Diagnostics 0x08
    public byte[] DiagnosticsEcho(byte deviceId, byte[] data)
    {
        List<byte> request = new()
        {
            deviceId,
            DIAGNOSTICS_FUNCTION,
            0x00,  // Sub-function high byte (0 for Echo)
            0x00,  // Sub-function low byte (0 for Echo)
        };

        foreach (byte b in data)
        {
            request.Add(b);
        }

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        _serialPort.Write(request.ToArray(), 0, request.Count);

        Thread.Sleep(TIME_TO_SLEEP);

        byte[] response = new byte[5 + data.Length];
        _serialPort.Read(response, 0, response.Length);

        if (!CheckCRC(response, response.Length - 2, (ushort)(response[^1] << 8 | response[^2])))
        {
            Debug.Log("Modbus RTU: Diagnostics Echo CRC error.");
            return null;
        }
        else
        {
            Debug.Log("Modbus RTU: Diagnostics Echo successful.");
        }

        byte[] echoData = new byte[data.Length];
        Array.Copy(response, 4, echoData, 0, data.Length);
        return echoData;
    }

    public async Task<byte[]> DiagnosticsEchoAsync(byte deviceId, byte[] data)
    {
        List<byte> request = new()
        {
            deviceId,
            DIAGNOSTICS_FUNCTION,
            0x00,  // Sub-function high byte (0 for Echo)
            0x00,  // Sub-function low byte (0 for Echo)
        };

        foreach (byte b in data)
        {
            request.Add(b);
        }

        ushort crc = CalculateCRC(request.ToArray(), request.Count);
        request.Add((byte)(crc & 0xFF));
        request.Add((byte)(crc >> 8));

        await Task.Run(() => _serialPort.Write(request.ToArray(), 0, request.Count));

        await Task.Delay(TIME_TO_SLEEP);

        byte[] response = new byte[5 + data.Length];
        await Task.Run(() => _serialPort.Read(response, 0, response.Length));

        if (!CheckCRC(response, response.Length - 2, (ushort)(response[^1] << 8 | response[^2])))
        {
            Debug.Log("Modbus RTU: Diagnostics Echo CRC error.");
            return null;
        }
        else
        {
            Debug.Log("Modbus RTU: Diagnostics Echo successful.");
        }

        byte[] echoData = new byte[data.Length];
        Array.Copy(response, 4, echoData, 0, data.Length);
        return echoData;
    }
    #endregion

    #region CRC
    private ushort CalculateCRC(byte[] data, int length)
    {
        ushort crc = 0xFFFF;

        for(int i = 0; i < length; i++)
        {
            crc ^= data[i];

            for(int j = 0; j < 8; j++)
            {
                if((crc & 0x0001) == 1)
                {
                    crc >>= 1;
                    crc ^= 0xA001;
                }
                else
                {
                    crc >>= 1;
                }
            }
        }

        return crc;
    }

    private bool CheckCRC(byte[] data, int length, ushort receivedCRC)
    {
        ushort crc = CalculateCRC(data, length);
        return crc == receivedCRC;
    }
    #endregion
}