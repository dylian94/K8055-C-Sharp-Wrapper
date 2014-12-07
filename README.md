#K8055 C# Wrapper
A C# Wrapper for the Velleman K8055 (SDK v4) DLL that uses events for digital and analog input and a job queue for digital and analog output. Build to be used with WPF, not tested with Windows Forms.

##Table of Contents
- [How to use](#how-to-use)
- [Examples](#examples)
	- [Connecting to devices](#connecting-to-devices)
	- [Listening for connection errors](#listening-for-connection-errors)
	- [Listening for Digital input](#listening-for-digital-input)
	- [Adding Digital output to the output queue](#adding-digital-output-to-the-output-queue)
		- [Adding a single digital output to the queue](#adding-a-single-digital-output-to-the-queue)
			- [Method one](#method-one)
			- [Method two](#method-two)
		- [Adding multiple digital outputs to the queue](#adding-multiple-digital-outputs-to-the-queue)
			- [Method one](#method-one-1)
			- [Method two](#method-two-1)

##How to use
1. Build the DLL.
2. Add a reference to the DLL to your project.
3. [Download the Velleman K8055 (v4) SDK](http://www.velleman.eu/support/downloads/?code=K8055&type=9)
4. Extract the K8055D.dll from the "DLL_v4.0.0.0" folder.
5. Make 4 copies of the DLL file in your project folder and name like noted below*
    * K8055-1.dll
    * K8055-2.dll
    * K8055-3.dll
    * K8055-4.dll
6. Add the DLL files to your project and change the associated property "Copy to Output Directory" to "Copy if newer" or "Copy always".
7. Check the examples below to see how the DLL can be used.

\* Copying the dll file 4 times is needed to be able to connect to multiple devices at ones without having to open and close connections each time.

##Examples
###Connecting to devices
```csharp
K8055Communicator kcCommunicator = new K8055Communicator();

// The StartCommunnication method can be called multiple times if needed
kcCommunicator.StartCommunication(0); // This can be any of the available devices (0, 1, 2 or 3)

// And close the connection when not needed anymore
kcCommunicatior.StopCommunication(0); // This can be any of the available devices (0, 1, 2 or 3)
````

###Listening for connection errors
It is adviced to do this before calling the `StartCommunication()` method.

*Register the event handler:*
````csharp
kcCommunicator.Error += HandleDeviceConnectionError;
````

*Handle the event (showing a messagebox to indicate to the user that the device may not be connected):*
````csharp
private void HandleDeviceConnectionError(object oSender, EventArgs eaEventArguments)
{
    MessageBox.Show("Device connection failed!");
}
````

###Listening for Digital input
> **NOTE:** This event is triggered for input on all devices, you will need 
> to check the event arguments to check wich device triggered the event.

*Register the event handler:*
````csharp
kcCommunicator.DigitalInput += HandleInput;
````

*Handle the event (logging the input to the console output):*
````csharp
private void HandleInput(object oSender = null, DigitalInputEventArgs dieaEventArguments = null)
{
    Console.WriteLine();
    Console.WriteLine("Device: " + dieaEventArguments.Input.Device.ToString());
    Console.WriteLine("Port: " + dieaEventArguments.Input.Port.ToString());
    Console.WriteLine("Value: " + dieaEventArguments.Input.Value.ToString());
}
````

###Adding Digital output to the output queue
> **WARNING:** Make sure you have called the `StartCommunication()` method to open the connection
> to the device before trying to write to any of its digital outputs.
>
> **NOTE:** There are multiple overloads of the `AddDigitalOutput()` method. You can use
> wichever one you prefer, all overloads add the digital output as individual (per channel)
> outputs to the queue. The output is written to the device one channel at a time (the 
> current version of the library does not use the K8055 SDK provided `WriteAllDigital()` method). 

####Adding a single digital output to the queue
In the current version of the library all methods that add digital output to the queue use "Method one" eventually even the methods that add multiple digital values.

#####Method one:
````csharp
kcCommunicator.AddDigitalOutput(new DigitalIO()
{
   Device = 0,
   Port = 1,
   Value = true
});
````

#####Method two:
A more straightforward method that calls the above method eventually.
````csharp
kcCommunicator.AddDigitalOutput(0, 1, true);
````

####Adding multiple digital outputs to the queue

#####Method one:
You can add many outputs as you want, they are added to the queue in the same order as they are added to the list.
````csharp
kcCommunicator.AddDigitalOutput(new List<DigitalIO>()
{
   new DigitalIO()
   {
      Device = 0,
      Port = 1,
      Value = true
   },
   new DigitalIO()
   {
      Device = 0,
      Port = 2,
      Value = false
   }
});
````

#####Method two:
The second parameter accepts a boolean array with a max size of 8 (the number of ports). The index in the array is mapped to the ports on the device, if you only pass an array with a size of 3 only the values of the first 3 ports will be changed.
````csharp
kcCommunicator.AddDigitalOutput(0, new bool?[4] { false, true, false, false });
````

__*MORE COMMING SOON*__
