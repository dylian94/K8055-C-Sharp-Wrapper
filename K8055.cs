/*
Copyright (c) 2014, Dylian Melgert
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

* Redistributions of source code must retain the above copyright notice, this
  list of conditions and the following disclaimer.

* Redistributions in binary form must reproduce the above copyright notice,
  this list of conditions and the following disclaimer in the documentation
  and/or other materials provided with the distribution.

* Neither the name of K8055-C-Sharp-Wrapper nor the names of its
  contributors may be used to endorse or promote products derived from
  this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace K8055
{
    #region K8055 Device Communication class

    internal class K8055
    {
        private Type _tDevice;
        private int _iDevice;

        /// <summary>
        /// Initializes a new K8055 object for the specified device.
        /// </summary>
        /// <param name="iDevice"></param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Gets thrown when an invalid device channel was given.
        /// </exception>
        public K8055(int iDevice)
        {
            _iDevice = iDevice;

            if (iDevice >= 0 && iDevice <= 3)
            {
                _tDevice = Type.GetType("K8055.Device" + iDevice);
            }
            else
            {
                throw new ArgumentOutOfRangeException("iDevice", "The device channel " + iDevice.ToString() + " is not valid. Only channel 0, 1, 2 and 3 are available!");
            }
        }

        /// <summary>
        /// Calls a method on the earlier specified device.
        /// </summary>
        /// <param name="sMethodName">The method to call</param>
        /// <returns>The output of the chosen method</returns>
        public object CallDeviceMethod(string sMethodName)
        {
            return _tDevice.InvokeMember(sMethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, null);
        }

        /// <summary>
        /// Calls a method on the earlier specified device with the given parameter.
        /// </summary>
        /// <param name="sMethodName">The method to call</param>
        /// <param name="iParameter">The parameter to pass</param>
        /// <returns>The output of the chosen method</returns>
        public object CallDeviceMethod(string sMethodName, int iParameter)
        {
            return _tDevice.InvokeMember(sMethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[1] { iParameter });
        }

        /// <summary>
        /// Calls a method on the earlier specified device with the given parameters.
        /// </summary>
        /// <param name="sMethodName">The method to call</param>
        /// <param name="iParameter1">The first parameter to pass</param>
        /// <param name="iParameter2">The second parameter to pass</param>
        /// <returns>The output of the chosen method</returns>
        public object CallDeviceMethod(string sMethodName, int iParameter1, int iParameter2)
        {
            return _tDevice.InvokeMember(sMethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[2] { iParameter1, iParameter2 });
        }

        /// <summary>
        /// Calls a method on the earlier specified device with the given parameters.
        /// </summary>
        /// <param name="sMethodName">The method to call</param>
        /// <param name="iParameter1">The first parameter to pass</param>
        /// <param name="iParameter2">The second parameter to pass</param>
        /// <returns>The output of the chosen method</returns>
        public void CallDeviceMethod(string sMethodName, out int iParameter1, out int iParameter2)
        {
            iParameter1 = 0;
            iParameter2 = 0;

            _tDevice.InvokeMember(sMethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, new object[2] { iParameter1, iParameter2 });
        }

        /// <summary>
        /// Opens the communication link to the K8055 card. Loads the drivers needed to communicate via the USB port.
        /// This procedure must be performed before any attempts to communicate with the K8055 card. This function can
        /// also be used to selects the active K8055 card to read and write the data. All the communication routines
        /// after this function call are addressed to this card until the other card is selected by this function call. 
        /// </summary>
        /// <param name="iCardAddress">
        /// Value between 0 and 3 which corresponds to the jumper (SK5, SK6) setting on the K8055 board.
        /// </param>
        /// <returns>
        /// If succeeded the return value will be the card address read from the K8055 hardware. 
        /// Return value -1 indicates that K8055 card was not found.
        /// </returns>
        public int OpenDevice(int iCardAddress = -1)
        {
            if (iCardAddress == -1)
            {
                iCardAddress = _iDevice;
            }

            return (int)CallDeviceMethod("OpenDevice", iCardAddress);
        }

        /// <summary>
        /// Using this function all the K8055 cards can be opened. No need to use OpenDevice. 
        /// This function returns all the connected K8055 devices on the computer. The returned value is a bit field.
        /// 
        /// Returned value:
        /// • Bin 0000, Dec 0 : No devices was found 
        /// • Bin 0001, Dec 1 : Card address 0 was found. 
        /// • Bin 0010, Dec 2 : Card address 1 was found. 
        /// • Bin 0100, Dec 4 : Card address 2 was found. 
        /// • Bin 1000, Dec 8 : Card address 3 was found. 
        /// 
        /// Example : return value 9 = devices with address 0 and 3 are connected.
        /// </summary>
        /// <returns>
        /// • Bin 0000, Dec 0 : No devices was found 
        /// • Bin 0001, Dec 1 : Card address 0 was found. 
        /// • Bin 0010, Dec 2 : Card address 1 was found. 
        /// • Bin 0100, Dec 4 : Card address 2 was found. 
        /// • Bin 1000, Dec 8 : Card address 3 was found. 
        /// </returns>
        public int SearchDevices()
        {
            return (int)CallDeviceMethod("SearchDevices");
        }

        /// <summary>
        /// The function set the current controlled device. 
        /// The returned value is the device address, if this value is –1 no device with the address parameter was found.  
        /// </summary>
        /// <param name="iCardAddress">
        /// Value 0 to 3, which corresponds to the device address.
        /// </param>
        /// <returns>
        /// The returned value is the device address, if this value is –1 no device with the address parameter was found.
        /// </returns>
        public int SetCurrentDevice(int iCardAddress = -1)
        {
            if (iCardAddress == -1)
            {
                iCardAddress = _iDevice;
            }

            return (int)CallDeviceMethod("SetCurrentDevice", iCardAddress);
        }

        /// <summary>
        /// Unloads the communication routines for K8055 cards and unloads the driver needed to communicate via the USB port.
        /// This is the last action of the application program before termination.  
        /// </summary>
        public void CloseDevice()
        {
            CallDeviceMethod("CloseDevice");
        }

        /// <summary>
        /// The input voltage of the selected 8-bit Analog to Digital converter channel is converted to a value which lies between 0 and 255. 
        /// </summary>
        /// <param name="iChannel">
        /// Value between 1 and 2 which corresponds to the AD channel whose status is to be read. 
        /// </param>
        /// <returns>
        /// The corresponding Analog to Digital Converter data is read. 
        /// </returns>
        public int ReadAnalogChannel(int iChannel)
        {
            return (int)CallDeviceMethod("ReadAnalogChannel", iChannel);
        }

        /// <summary>
        /// The status of both Analog to Digital Converters are read to an array of long integers.  
        /// </summary>
        /// <param name="iData1">
        /// Reference to the integer (32-bit) where the data will be read.
        /// </param>
        /// <param name="iData2">
        /// Reference to the integer (32-bit) where the data will be read.</param>
        public void ReadAllAnalog(out int iData1, out int iData2)
        {
            iData1 = 0;
            iData2 = 0;

            CallDeviceMethod("ReadAllAnalog", out iData1, out iData2);
        }

        /// <summary>
        /// The indicated 8-bit Digital to Analog Converter channel is altered according to the new data. This
        /// means that the data corresponds to a specific voltage. The value 0 corresponds to a minimum output 
        /// voltage (0 Volt) and the value 255 corresponds to a maximum output voltage (+5V). A value of 'Data'
        /// lying in between these extremes can be translated by the following formula : Data / 255 x 5V. 
        /// </summary>
        /// <param name="iChannel">
        /// Value between 1 and 2 which corresponds to the 8-bit DA channel number whose data is to be set.
        /// </param>
        /// <param name="iData">
        /// Value between 0 and 255 which is to be sent to the 8-bit Digital to Analog Converter.
        /// </param>
        public void OutputAnalogChannel(int iChannel, int iData)
        {
            CallDeviceMethod("OutputAnalogChannel", iChannel, iData);
        }

        /// <summary>
        /// The indicated 8-bit Digital to Analog Converter channel is altered according to the new data. This
        /// means that the data corresponds to a specific voltage. The value 0 corresponds to a minimum output 
        /// voltage (0 Volt) and the value 255 corresponds to a maximum output voltage (+5V). A value of 'Data' 
        /// lying in between these extremes can be translated by the following formula : Data / 255 x 5V. 
        /// </summary>
        /// <param name="iData1">Value between 0 and 255 which is to be sent to the 8-bit Digital to Analog Converter.</param>
        /// <param name="iData2">Value between 0 and 255 which is to be sent to the 8-bit Digital to Analog Converter.</param>
        public void OutputAllAnalog(int iData1, int iData2)
        {
            CallDeviceMethod("OutputAllAnalog", iData1, iData2);
        }

        /// <summary>
        /// The selected DA-channel is set to minimum output voltage (0 Volt).
        /// </summary>
        /// <param name="iChannel">
        /// Value between 1 and 2 which corresponds to the 8-bit DA channel number in which the data is to be erased.
        /// </param>
        public void ClearAnalogChannel(int iChannel)
        {
            CallDeviceMethod("ClearAnalogChannel", iChannel);
        }

        /// <summary>
        /// Both DA-channels are set to minimum output voltage (0 Volt).
        /// </summary>
        public void ClearAllAnalog()
        {
            CallDeviceMethod("ClearAllAnalog");
        }

        /// <summary>
        /// The selected 8-bit Digital to Analog Converter channel is set to maximum output voltage. 
        /// </summary>
        /// <param name="iChannel">
        /// Value between 1 and 2 which corresponds to the 8-bit DA channel number in which the data is to be set to maximum. 
        /// </param>
        public void SetAnalogChannel(int iChannel)
        {
            CallDeviceMethod("SetAnalogChannel", iChannel);
        }

        /// <summary>
        /// All channels of the 8-bit Digital to Analog Converters are set to maximum output voltage.
        /// </summary>
        public void SetAllAnalog()
        {
            CallDeviceMethod("SetAllAnalog");
        }

        /// <summary>
        /// The channels of the digital output port are updated with the status of the corresponding bits in the data
        /// parameter. A high (1) level means that the microcontroller IC3 output is set, and a low (0) level means
        /// that the output is cleared. 
        /// </summary>
        /// <param name="iData">
        /// Value between 0 and 255 that is sent to the output port (8 channels). 
        /// </param>
        public void WriteAllDigital(int iData)
        {
            CallDeviceMethod("WriteAllDigital", iData);
        }

        /// <summary>
        /// The selected channel is cleared. 
        /// </summary>
        /// <param name="iChannel">
        /// Value between 1 and 8 which corresponds to the output channel that is to be cleared.
        /// </param>
        public void ClearDigitalChannel(int iChannel)
        {
            CallDeviceMethod("ClearDigitalChannel", iChannel);
        }

        /// <summary>
        /// All digital outputs are cleared.
        /// </summary>
        public void ClearAllDigital()
        {
            CallDeviceMethod("ClearAllDigital");
        }

        /// <summary>
        /// The selected digital output channel is set. 
        /// </summary>
        /// <param name="iChannel">
        /// Value between 1 and 8 which corresponds to the output channel that is to be set. 
        /// </param>
        public void SetDigitalChannel(int iChannel)
        {
            CallDeviceMethod("SetDigitalChannel", iChannel);
        }

        /// <summary>
        /// All the digital output channels are set.
        /// </summary>
        public void SetAllDigital()
        {
            CallDeviceMethod("SetAllDigital");
        }

        /// <summary>
        /// The status of the selected Input channel is read. 
        /// </summary>
        /// <param name="iChannel">
        /// Value between 1 and 5 which corresponds to the input channel whose status is to be read. 
        /// </param>
        /// <returns>
        /// TRUE means that the channel has been set and FALSE means that it has been cleared. 
        /// </returns>
        public bool ReadDigitalChannel(int iChannel)
        {
            return (bool)CallDeviceMethod("ReadDigitalChannel", iChannel);
        }

        /// <summary>
        /// The function returns the status of the digital inputs.
        /// </summary>
        /// <returns>
        /// The 5 LSB correspond to the status of the digital input channels. A high (1) means that the
        /// channel is HIGH, a low (0) means that the channel is LOW. 
        /// </returns>
        public int ReadAllDigital()
        {
            return (int)CallDeviceMethod("ReadAllDigital");
        }

        /// <summary>
        /// The selected pulse counter is reset. 
        /// </summary>
        /// <param name="iCounterNr">
        /// Value 1 or 2, which corresponds to the counter to be reset. 
        /// </param>
        public void ResetCounter(int iCounterNr)
        {
            CallDeviceMethod("ResetCounter", iCounterNr);
        }

        /// <summary>
        /// The function returns the status of the selected 16 bit pulse counter. 
        /// The counter number 1 counts the pulses fed to the input I1 and the counter number 2 counts the
        /// pulses fed to the input I2.
        /// </summary>
        /// <param name="iCounterNr">
        /// Value 1 or 2, which corresponds to the counter to be read.
        /// </param>
        /// <returns>
        /// The content of the 16 bit pulse counter. 
        /// </returns>
        public int ReadCounter(int iCounterNr)
        {
            return (int)CallDeviceMethod("ReadCounter", iCounterNr);
        }

        /// <summary>
        /// The counter inputs are debounced in the software to prevent false triggering when mechanical
        /// switches or relay inputs are used. The debounce time is equal for both falling and rising edges. The
        /// default debounce time is 2ms. This means the counter input must be stable for at least 2ms before it is
        /// recognised, giving the maximum count rate of about 200 counts per second.
        /// If the debounce time is set to 0, then the maximum counting rate is about 2000 counts per second. 
        /// </summary>
        /// <param name="iCounterNr">
        /// Value 1 or 2, which corresponds to the counter to be set.
        /// </param>
        /// <param name="iDebounceTime">
        /// Debounce time for the pulse counter.
        /// The DebounceTime value corresponds to the debounce time in milliseconds (ms) to be set for the
        /// pulse counter. Debounce time value may vary between 0 and 5000.  
        /// </param>
        public void SetCounterDebounceTime(int iCounterNr, int iDebounceTime)
        {
            CallDeviceMethod("SetCounterDebounceTime", iCounterNr, iDebounceTime);
        }
    }

    #endregion

    #region K8055 DLL Import class (Device 0)

    internal class Device0
    {
        [DllImport("K8055-4.dll")]
        public static extern int OpenDevice(int iCardAddress = 1);

        [DllImport("K8055-4.dll")]
        public static extern int SearchDevices();

        [DllImport("K8055-4.dll")]
        public static extern int SetCurrentDevice(int iCardAddress = 1);

        [DllImport("K8055-4.dll")]
        public static extern void CloseDevice();

        [DllImport("K8055-4.dll")]
        public static extern int ReadAnalogChannel(int iChannel);

        [DllImport("K8055-4.dll")]
        public static extern void ReadAllAnalog(out int iData1, out int iData2);

        [DllImport("K8055-4.dll")]
        public static extern void OutputAnalogChannel(int iChannel, int iData);

        [DllImport("K8055-4.dll")]
        public static extern void OutputAllAnalog(int iData1, int iData2);

        [DllImport("K8055-4.dll")]
        public static extern void ClearAnalogChannel(int iChannel);

        [DllImport("K8055-4.dll")]
        public static extern void ClearAllAnalog();

        [DllImport("K8055-4.dll")]
        public static extern void SetAnalogChannel(int iChannel);

        [DllImport("K8055-4.dll")]
        public static extern void SetAllAnalog();

        [DllImport("K8055-4.dll")]
        public static extern void WriteAllDigital(int iData);

        [DllImport("K8055-4.dll")]
        public static extern void ClearDigitalChannel(int iChannel);

        [DllImport("K8055-4.dll")]
        public static extern void ClearAllDigital();

        [DllImport("K8055-4.dll")]
        public static extern void SetDigitalChannel(int iChannel);

        [DllImport("K8055-4.dll")]
        public static extern void SetAllDigital();

        [DllImport("K8055-4.dll")]
        public static extern bool ReadDigitalChannel(int iChannel);

        [DllImport("K8055-4.dll")]
        public static extern int ReadAllDigital();

        [DllImport("K8055-4.dll")]
        public static extern void ResetCounter(int iCounterNr);

        [DllImport("K8055-4.dll")]
        public static extern int ReadCounter(int iCounterNr);

        [DllImport("K8055-4.dll")]
        public static extern void SetCounterDebounceTime(int iCounterNr, int iDebounceTime);
    }

    #endregion

    #region K8055 DLL Import class (Device 1)

    internal class Device1
    {
        [DllImport("K8055-1.dll")]
        public static extern int OpenDevice(int iCardAddress = 1);

        [DllImport("K8055-1.dll")]
        public static extern int SearchDevices();

        [DllImport("K8055-1.dll")]
        public static extern int SetCurrentDevice(int iCardAddress = 1);

        [DllImport("K8055-1.dll")]
        public static extern void CloseDevice();

        [DllImport("K8055-1.dll")]
        public static extern int ReadAnalogChannel(int iChannel);

        [DllImport("K8055-1.dll")]
        public static extern void ReadAllAnalog(out int iData1, out int iData2);

        [DllImport("K8055-1.dll")]
        public static extern void OutputAnalogChannel(int iChannel, int iData);

        [DllImport("K8055-1.dll")]
        public static extern void OutputAllAnalog(int iData1, int iData2);

        [DllImport("K8055-1.dll")]
        public static extern void ClearAnalogChannel(int iChannel);

        [DllImport("K8055-1.dll")]
        public static extern void ClearAllAnalog();

        [DllImport("K8055-1.dll")]
        public static extern void SetAnalogChannel(int iChannel);

        [DllImport("K8055-1.dll")]
        public static extern void SetAllAnalog();

        [DllImport("K8055-1.dll")]
        public static extern void WriteAllDigital(int iData);

        [DllImport("K8055-1.dll")]
        public static extern void ClearDigitalChannel(int iChannel);

        [DllImport("K8055-1.dll")]
        public static extern void ClearAllDigital();

        [DllImport("K8055-1.dll")]
        public static extern void SetDigitalChannel(int iChannel);

        [DllImport("K8055-1.dll")]
        public static extern void SetAllDigital();

        [DllImport("K8055-1.dll")]
        public static extern bool ReadDigitalChannel(int iChannel);

        [DllImport("K8055-1.dll")]
        public static extern int ReadAllDigital();

        [DllImport("K8055-1.dll")]
        public static extern void ResetCounter(int iCounterNr);

        [DllImport("K8055-1.dll")]
        public static extern int ReadCounter(int iCounterNr);

        [DllImport("K8055-1.dll")]
        public static extern void SetCounterDebounceTime(int iCounterNr, int iDebounceTime);
    }

    #endregion
    
    #region K8055 DLL Import class (Device 2)

    internal class Device2
    {
        [DllImport("K8055-2.dll")]
        public static extern int OpenDevice(int iCardAddress = 1);

        [DllImport("K8055-2.dll")]
        public static extern int SearchDevices();

        [DllImport("K8055-2.dll")]
        public static extern int SetCurrentDevice(int iCardAddress = 1);

        [DllImport("K8055-2.dll")]
        public static extern void CloseDevice();

        [DllImport("K8055-2.dll")]
        public static extern int ReadAnalogChannel(int iChannel);

        [DllImport("K8055-2.dll")]
        public static extern void ReadAllAnalog(out int iData1, out int iData2);

        [DllImport("K8055-2.dll")]
        public static extern void OutputAnalogChannel(int iChannel, int iData);

        [DllImport("K8055-2.dll")]
        public static extern void OutputAllAnalog(int iData1, int iData2);

        [DllImport("K8055-2.dll")]
        public static extern void ClearAnalogChannel(int iChannel);

        [DllImport("K8055-2.dll")]
        public static extern void ClearAllAnalog();

        [DllImport("K8055-2.dll")]
        public static extern void SetAnalogChannel(int iChannel);

        [DllImport("K8055-2.dll")]
        public static extern void SetAllAnalog();

        [DllImport("K8055-2.dll")]
        public static extern void WriteAllDigital(int iData);

        [DllImport("K8055-2.dll")]
        public static extern void ClearDigitalChannel(int iChannel);

        [DllImport("K8055-2.dll")]
        public static extern void ClearAllDigital();

        [DllImport("K8055-2.dll")]
        public static extern void SetDigitalChannel(int iChannel);

        [DllImport("K8055-2.dll")]
        public static extern void SetAllDigital();

        [DllImport("K8055-2.dll")]
        public static extern bool ReadDigitalChannel(int iChannel);

        [DllImport("K8055-2.dll")]
        public static extern int ReadAllDigital();

        [DllImport("K8055-2.dll")]
        public static extern void ResetCounter(int iCounterNr);

        [DllImport("K8055-2.dll")]
        public static extern int ReadCounter(int iCounterNr);

        [DllImport("K8055-2.dll")]
        public static extern void SetCounterDebounceTime(int iCounterNr, int iDebounceTime);
    }

    #endregion

    #region K8055 DLL Import class (Device 3)

    internal class Device3
    {
        [DllImport("K8055-3.dll")]
        public static extern int OpenDevice(int iCardAddress = 1);

        [DllImport("K8055-3.dll")]
        public static extern int SearchDevices();

        [DllImport("K8055-3.dll")]
        public static extern int SetCurrentDevice(int iCardAddress = 1);

        [DllImport("K8055-3.dll")]
        public static extern void CloseDevice();

        [DllImport("K8055-3.dll")]
        public static extern int ReadAnalogChannel(int iChannel);

        [DllImport("K8055-3.dll")]
        public static extern void ReadAllAnalog(out int iData1, out int iData2);

        [DllImport("K8055-3.dll")]
        public static extern void OutputAnalogChannel(int iChannel, int iData);

        [DllImport("K8055-3.dll")]
        public static extern void OutputAllAnalog(int iData1, int iData2);

        [DllImport("K8055-3.dll")]
        public static extern void ClearAnalogChannel(int iChannel);

        [DllImport("K8055-3.dll")]
        public static extern void ClearAllAnalog();

        [DllImport("K8055-3.dll")]
        public static extern void SetAnalogChannel(int iChannel);

        [DllImport("K8055-3.dll")]
        public static extern void SetAllAnalog();

        [DllImport("K8055-3.dll")]
        public static extern void WriteAllDigital(int iData);

        [DllImport("K8055-3.dll")]
        public static extern void ClearDigitalChannel(int iChannel);

        [DllImport("K8055-3.dll")]
        public static extern void ClearAllDigital();

        [DllImport("K8055-3.dll")]
        public static extern void SetDigitalChannel(int iChannel);

        [DllImport("K8055-3.dll")]
        public static extern void SetAllDigital();

        [DllImport("K8055-3.dll")]
        public static extern bool ReadDigitalChannel(int iChannel);

        [DllImport("K8055-3.dll")]
        public static extern int ReadAllDigital();

        [DllImport("K8055-3.dll")]
        public static extern void ResetCounter(int iCounterNr);

        [DllImport("K8055-3.dll")]
        public static extern int ReadCounter(int iCounterNr);

        [DllImport("K8055-3.dll")]
        public static extern void SetCounterDebounceTime(int iCounterNr, int iDebounceTime);
    }

    #endregion

    #region K8055 Queue item classes

    /// <summary>
    /// The general K8055 IO class
    /// </summary>
    public class IO
    {
        /// <summary>
        /// The device associated to this IO
        /// </summary>
        public int Device { get; set; }

        /// <summary>
        /// The port associated to this IO
        /// </summary>
        public int Port { get; set; }
    }

    /// <summary>
    /// The Digital K8055 IO class
    /// </summary>
    public class DigitalIO : IO
    {
        /// <summary>
        /// The value associated to this IO
        /// </summary>
        public bool Value { get; set; }
    }

    /// <summary>
    /// The Analog K8055 IO class
    /// </summary>
    public class AnalogIO : IO
    {
        /// <summary>
        /// The value associated to this IO
        /// </summary>
        public int Value { get; set; }
    }

    #endregion

    #region K8055 Event argument classes

    public class DigitalInputEventArgs : EventArgs
    {
        public DigitalIO Input { get; private set; }

        public DigitalInputEventArgs(DigitalIO dioInput)
        {
            Input = dioInput;
        }
    }

    public class AnalogInputEventArgs : EventArgs
    {
        public AnalogIO Input { get; private set; }

        public AnalogInputEventArgs(AnalogIO aioInput)
        {
            Input = aioInput;
        }
    }

    #endregion

    #region K8055 Communicator Class

    public class K8055Communicator
    {
        private bool[] _baCommunicatingWithDevices;

        private List<DigitalIO> _ldioDigitalOutputQueue;

        private List<AnalogIO> _laioAnalogOutputQueue;

        public delegate void DigitalInputEventHandler(object oSender, DigitalInputEventArgs dieaEventArguments);

        public delegate void AnalogInputEventHandler(object oSender, AnalogInputEventArgs aieaEventArguments);

        public delegate void ConnectionErrorEventHandler(object oSender, EventArgs eaEventArguments);

        public event DigitalInputEventHandler DigitalInput;

        public event AnalogInputEventHandler AnalogInput;

        public event ConnectionErrorEventHandler Error;

        /// <summary>
        /// The constructor for the K8055 Communicator
        /// </summary>
        public K8055Communicator()
        {
            _baCommunicatingWithDevices = new bool[4] { false, false, false, false };
            _ldioDigitalOutputQueue = new List<DigitalIO>();
            _laioAnalogOutputQueue = new List<AnalogIO>();
        }

        /// <summary>
        /// Asynchronous method that communicates with the specified device
        /// 
        /// When the value of one of the Digital or Anolog inputs changes an event is fired.
        /// The output queue will be written to the device in order of addition.
        /// </summary>
        /// <param name="iDevice">The channel of the device the communicate with</param>
        private async void CommunicateWithDevice(int iDevice)
        {
            await Task.Run(() =>
            {
                if (iDevice >= 0 && iDevice <= 3)
                {
                    K8055 kDevice = new K8055(iDevice);

                    BitArray baDigitalValues = new BitArray(new byte[1] { BitConverter.GetBytes(0)[0] });
                    int[] iaAnalogValues = new int[2] { 0, 0 };
                    
                    if (kDevice.OpenDevice(iDevice) != -1)
                    {
                        while (_baCommunicatingWithDevices[iDevice])
                        {
                            // Read all digital values and fire an event when one of them changes
                            
                            BitArray baNewDigitalValues = new BitArray(new byte[1] { BitConverter.GetBytes(kDevice.ReadAllDigital())[0] });

                            if (baNewDigitalValues.Count == 8)
                            {
                                for (int iBit = 4; iBit >= 0; iBit--)
                                {
                                    if (baNewDigitalValues[iBit] != baDigitalValues[iBit])
                                    {
                                        if (DigitalInput != null)
                                        {
                                            DigitalInputEventArgs dieaEventArgs = new DigitalInputEventArgs(new DigitalIO()
                                            {
                                                Device = iDevice,
                                                Port = iBit + 1,
                                                Value = baNewDigitalValues[iBit]
                                            });

                                            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                                            {
                                                DigitalInput(this, dieaEventArgs);
                                            }));
                                        }
                                    }
                                }
                            }

                            baDigitalValues = baNewDigitalValues;

                            List<DigitalIO> ldioDeviceDigitalOutputQueue = new List<DigitalIO>(_ldioDigitalOutputQueue.Where<DigitalIO>(dio => dio.Device == iDevice));

                            foreach (DigitalIO dioOutput in ldioDeviceDigitalOutputQueue)
                            {
                                if (dioOutput.Value)
                                {
                                    kDevice.SetDigitalChannel(dioOutput.Port);
                                }
                                else
                                {
                                    kDevice.ClearDigitalChannel(dioOutput.Port);
                                }

                                _ldioDigitalOutputQueue.Remove(dioOutput);
                            }

                            int[] iaNewAnalogValues = new int[2] { 0, 0 };

                            kDevice.ReadAllAnalog(out iaNewAnalogValues[0], out iaNewAnalogValues[1]);

                            for (int iPort = 0; iPort < 2; iPort++)
                            {
                                if (iaAnalogValues[iPort] != iaNewAnalogValues[iPort])
                                {
                                    if (AnalogInput != null)
                                    {
                                        AnalogInputEventArgs aieaEventArgs = new AnalogInputEventArgs(new AnalogIO()
                                        {
                                            Device = iDevice,
                                            Port = iPort,
                                            Value = iaNewAnalogValues[iPort]
                                        });

                                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                                        {
                                            AnalogInput(this, aieaEventArgs);
                                        }));
                                    }
                                }
                            }

                            List<AnalogIO> laioDeviceAnalogOutputQueue = new List<AnalogIO>(_laioAnalogOutputQueue.Where<AnalogIO>(aio => aio.Device == iDevice));

                            foreach (AnalogIO aioOutput in laioDeviceAnalogOutputQueue)
                            {
                                if (aioOutput.Value >= 0 && aioOutput.Value <= 255)
                                {
                                    kDevice.OutputAnalogChannel(aioOutput.Port, aioOutput.Value);
                                }

                                _laioAnalogOutputQueue.Remove(aioOutput);
                            }
                        }
                    }
                    else
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            if(Error != null)
                            {
                                Error(this, new EventArgs());
                            }
                        }));

                        StopCommunication(iDevice);
                    }

                    kDevice.CloseDevice();
                }
            });
        }

        /// <summary>
        /// This method starts the communication with the specified device
        /// </summary>
        /// <param name="iDevice">
        /// The channel number of the device to communicate with (0 till 3)
        /// </param>
        /// <returns>
        /// True if communication with the device has started,
        /// false if the communication was already started or an invalid channel number was given
        /// </returns>
        public bool StartCommunication(int iDevice)
        {
            bool bCommunicationStarted = false;

            if (iDevice >= 0 && iDevice <= 3 && !_baCommunicatingWithDevices[iDevice])
            {
                _baCommunicatingWithDevices[iDevice] = true;

                CommunicateWithDevice(iDevice);
            }

            return bCommunicationStarted;
        }

        /// <summary>
        /// This method stops the communication with the specified device
        /// </summary>
        /// <param name="iDevice">
        /// The channel number of the device to stop communication with (0 till 3)
        /// </param>
        /// <returns>
        /// True if communication with the device has been stopped,
        /// false if an invalid channel number was given
        /// </returns>
        public bool StopCommunication(int iDevice = 0)
        {
            bool bCommunicationStopped = false;

            if (iDevice >= 0 && iDevice <= 3)
            {
                _baCommunicatingWithDevices[iDevice] = false;

                bCommunicationStopped = true;
            }

            return bCommunicationStopped;
        }

        /// <summary>
        /// This method adds digital output to the device queue
        /// </summary>
        /// <param name="dioOutput">
        /// The digital output to add to the queue
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown when the device channel or port number specified in the output is invalid
        /// </exception>
        public void AddDigitalOutput(DigitalIO dioOutput)
        {
            if (dioOutput.Device >= 0 && dioOutput.Device <= 3)
            {
                if (dioOutput.Port >= 1 && dioOutput.Port <= 8)
                {
                    _ldioDigitalOutputQueue.Add(dioOutput);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The port number specified in the output has to have a value of 0 till 7!");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The device channel specified in the output has to have a value of 0 till 3!");
            }
        }

        /// <summary>
        /// This method adds a list of digital output to the device queue
        /// </summary>
        /// <param name="ldioOutput">
        /// The list with digital output to add to the queue
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown when the device channel or port number specified in one of the outputs is invalid
        /// </exception>
        public void AddDigitalOutput(List<DigitalIO> ldioOutput)
        {
            foreach (DigitalIO dioOutput in ldioOutput)
            {
                AddDigitalOutput(dioOutput);
            }
        }

        /// <summary>
        /// This method adds digital output to the device queue
        /// </summary>
        /// <param name="iDevice">
        /// The device channel
        /// </param>
        /// <param name="iPort">
        /// The port number
        /// </param>
        /// <param name="bValue">
        /// The wanted value
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown when the device channel or port number specified in the output is invalid
        /// </exception>
        public void AddDigitalOutput(int iDevice, int iPort, bool bValue)
        {
            AddDigitalOutput(new DigitalIO()
            {
                Device = iDevice,
                Port = iPort,
                Value = bValue
            });
        }

        /// <summary>
        /// This method adds multiple digital outputs to the device queue
        /// </summary>
        /// <param name="iDevice">
        /// The device channel
        /// </param>
        /// <param name="bValues">
        /// A bool array (size 8) with the wanted values for the digital outputs (a null value means not changing the output)
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown when the device channel or port number specified in one of the outputs is invalid
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This exception is thrown when the given array of values has a wrong size
        /// </exception>
        public void AddDigitalOutput(int iDevice, bool?[] bValues)
        {
            if (bValues.Length <= 8)
            {
                for (int iPort = 1; iPort <= bValues.Length; iPort++)
                {
                    if (bValues[iPort - 1] != null)
                    {
                        AddDigitalOutput(iDevice, iPort, (bool)bValues[iPort - 1]);
                    }
                }
            }
            else
            {
                throw new ArgumentException("The array with digital output values has an incorrect size! 8 max!", "bValues");
            }
        }

        /// <summary>
        /// This method adds analog output to the device queue
        /// </summary>
        /// <param name="aioOutput">
        /// The analog output to add to the queue
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown when the device channel, port number or value specified in the output is invalid
        /// </exception>
        public void AddAnalogOutput(AnalogIO aioOutput)
        {
            if (aioOutput.Device >= 0 && aioOutput.Device <= 3)
            {
                if (aioOutput.Port == 0 || aioOutput.Port == 1)
                {
                    if (aioOutput.Value >= 0 && aioOutput.Value <= 255)
                    {
                        _laioAnalogOutputQueue.Add(aioOutput);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("The value specified in the output has to have a value of 0 till 255!");
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The port number specified in the output has to have a value of 0 or 1!");
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("The device channel specified in the output has to have a value of 0 till 3!");
            }
        }

        /// <summary>
        /// This method adds a list of analog output to the device queue
        /// </summary>
        /// <param name="ldioOutput">
        /// The list with analog output to add to the queue
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown when the device channel, port number or value specified in one of the outputs is invalid
        /// </exception>
        public void AddAnalogOutput(List<AnalogIO> ldioOutput)
        {
            foreach (AnalogIO dioOutput in ldioOutput)
            {
                AddAnalogOutput(dioOutput);
            }
        }

        /// <summary>
        /// This method adds analog output to the device queue
        /// </summary>
        /// <param name="iDevice">
        /// The device channel
        /// </param>
        /// <param name="iPort">
        /// The port number
        /// </param>
        /// <param name="iValue">
        /// The wanted value
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown when the device channel, port number or value specified in the output is invalid
        /// </exception>
        public void AddAnalogOutput(int iDevice, int iPort, int iValue)
        {
            AddAnalogOutput(new AnalogIO()
            {
                Device = iDevice,
                Port = iPort,
                Value = iValue
            });
        }

        /// <summary>
        /// This method adds multiple digital outputs to the device queue
        /// </summary>
        /// <param name="iDevice">
        /// The device channel
        /// </param>
        /// <param name="bValues">
        /// A bool array (size 2) with the wanted values for the analog outputs
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// This exception is thrown when the device channel, port number or value specified in one of the outputs is invalid
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This exception is thrown when the given array of values has a wrong size
        /// </exception>
        public void AddAnalogOutput(int iDevice, int[] iValues)
        {
            if (iValues.Length != 2)
            {
                for (int iPort = 1; iPort <= 8; iPort++)
                {
                    AddAnalogOutput(iDevice, iPort, iValues[iPort - 1]);
                }
            }
            else
            {
                throw new ArgumentException("The array with analog output values has an incorrect size! 2 required!", "bValues");
            }
        }
    }

    #endregion
}