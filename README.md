#K8055 C# Wrapper
A C# Wrapper for the Velleman K8055 (SDK v4) DLL that uses events for digital and analog input and a job queue for digital and analog output. Build to be used with WPF, not tested with Windows Forms.

##How to use
1. Build the DLL.
2. Add a reference to the DLL to your project.
3. Check the examples below to see how the DLL can be used.

##Examples
**Connecting to devices**
```csharp
K8055Communicator kcCommunicator = new K8055Communicator();

// The StartCommunnication method can be called multiple times if needed
kcCommunicator.StartCommunication(0); // This can be any of the available devices (0, 1, 2 or 3)

// And close the connection when not needed anymore
kcCommunicatior.StopCommunication(0); // This can be any of the available devices (0, 1, 2 or 3)
````

**Listening for connection errors**  
It is adviced to do this before calling the `StartCommunication()` method.

*Register the event handler:*
````csharp
_kcCommunicator.Error += HandleDeviceConnectionError;
````

*Handle the event (showing a messagebox to indicate to the user that the device may not be connected):*
````csharp
private void HandleDeviceConnectionError(object oSender, EventArgs eaEventArguments)
{
    MessageBox.Show("Device connection failed!");
}
````

**Listening for Digital input**  
> **NOTE:** This event is triggered for input on all devices you'll need 
> to check the event arguments to check wich device triggered the event.

*Register the event handler*
````csharp
_kcCommunicator.DigitalInput += HandleInput;
````

*Handle the event (logging the input to the console output)*
````csharp
private void HandleInput(object oSender = null, DigitalInputEventArgs dieaEventArguments = null)
{
    Console.Write("Device: " + dieaEventArguments.Input.Device);
    Console.Write(" | Port: " + dieaEventArguments.Input.Port.ToString());
    Console.Write(" | Value: " + dieaEventArguments.Input.Value.ToString());
    Console.Write("\r\n");
}
````
