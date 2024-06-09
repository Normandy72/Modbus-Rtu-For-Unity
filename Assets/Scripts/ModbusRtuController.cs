using System;
using UnityEngine;

public enum OperatingSystem
{
    Android,
    Windows        
}

public enum BaudRate
{
    BR_300 = 300,
    BR_600 = 600,
    BR_1200 = 1200,
    BR_2400 = 2400,
    BR_4800 = 4800,
    BR_9600 = 9600,
    BR_19200 = 19200,
    BR_38400 = 38400,
    BR_57600 = 57600,
    BR_115200 = 115200,
    BR_230400 = 230400,
    BR_460800 = 460800,
    BR_921600 = 921600
}

public class ModbusRtuController : MonoBehaviour
{  
    [SerializeField] private OperatingSystem _operatingSystem = OperatingSystem.Windows;
    [SerializeField] private BaudRate _baudRate = BaudRate.BR_300;

    [Space(10)]
    [Tooltip("Device Manager -> Ports (COM and LPT) -> USB Serial Port (COM<number>)")]
    [SerializeField] private string _windowsPortName = "COM3";

    [Tooltip("Most often the same name for each Android device")]
    [SerializeField] private string _androidPortName = "/dev/ttyUSB0";

    [Space(10)]
    [SerializeField] private byte _deviceAddress;

    protected ModbusRTU _modbusRtu;
    protected string _portName;

    public static event Action<ModbusRTU, byte> OnPortOpened;

    private void Start()
    {    
        _portName = SetPortName();

        _modbusRtu = new(_portName, (byte)_baudRate);

        if(_modbusRtu.Connect())
        {
            SetOnConnectActions();
        }          
    }

    ///<summary>Set actions to be executed on successful port connection.</summary>
    private void SetOnConnectActions()
    {
        // DO SOMETHING (e.g. call event)
        OnPortOpened? .Invoke(_modbusRtu, _deviceAddress);            
    }   

    ///<summary>Set the appropriate port name based on the operating system.</summary>
    private string SetPortName() => _operatingSystem == OperatingSystem.Windows ? _windowsPortName : _androidPortName;

    private void OnDestroy()
    {
        _modbusRtu?.Disconnect();            
    }
}