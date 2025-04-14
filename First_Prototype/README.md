# Multiplayer

A simple multiplayer game where two players control colored rectangles. Players can move in real-time and see each other's positions on the screen.

## Features

- Local server and multiple clients
- Real-time movement sync between two players
- Two rectangles on screen: Player 1 (Red), Player 2 (Blue)
- Each client sees the position of both players

## Requirements

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio Code or another IDE that supports C# and .NET


## Installation

1. Clone the repository or download the files.
2. Ensure you have the required .NET SDKs installed.
3. Open a terminal in Visual Studio Code or in the command prompt.


## How to Run

### Start the Server

Open a terminal, navigate into the `Server` directory, and run:

- cd Server

- dotnet restore

- dotnet run

### Start the Client(s)

Open **two terminals** to run two clients for testing. In each terminal, navigate into the `Client` directory and run:

- cd Client

- dotnet restore

- dotnet run

(You can also launch a second client by duplicating the terminal or opening two VS Code terminals.)

## Controls

- **Player 1 (Red Rectangle):** Move using **W, A, S, D**
- **Player 2 (Blue Rectangle):** Move using **Arrow Keys**

## Notes

- Always start the server **before** starting any clients.
- You can move both rectangles from the two open clients on the same computer.
- To close the app, just close the windows or press **CTRL + C** in the terminal.

## Status

This is an early-stage prototype for a Software Project module at university.  
Currently supports local multiplayer only (on a single machine with two client instances).


