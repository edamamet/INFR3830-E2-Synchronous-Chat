# About
This is a basic TCP chat service written in C#. 

It's using the .NET 9.0 framework and built with sockets.

> The codebase at the moment is configured to run on the loopback interface, but can be configured to run on other interfaces (wlan0)

# Examples
![1](./.github/1.png)
[Video example](https://youtu.be/4f39dKGbxag)

# Usage
Make sure to run the server instance first. The client will not run without a server:
```
# You will need 2 terminal instances for this to work (one for the server, one for the client)

# Terminal 1
dotnet run --project Server

# Terminal 2
dotnet run --project Client
```

Upon connection, you can send whatever messages you want! (except for "exit")

To close a connection, you can use the word "exit", or you can use ctrl+c
