using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

public class ReverseProtocol
{
    private Socket _socket;
    private const int Port = 12345;
    private string _playerSide;
    private Country _country; // Store the current player's country
    private Socket _clientSocket; // Store the client socket for later use
    private const int IncomeIncrease = 3000000; // Income to be added each turn

    public void Serve()
    {
        ListAvailableIPs();

        Console.Write("Enter your IP address to listen on: ");
        string ipAddress = Console.ReadLine();

        IPAddress ip = IPAddress.Parse(ipAddress);
        _socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        _socket.Bind(new IPEndPoint(ip, Port));
        _socket.Listen(1);

        Console.WriteLine($"Server started, listening on {ipAddress}:{Port}...");

        _clientSocket = _socket.Accept();
        Console.WriteLine("Client connected.");

        HandleClient(_clientSocket);
    }

    public void Connect(string serverIp)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _socket.Connect(serverIp, Port);
        Console.WriteLine("Connected to server.");

        HandleServer(_socket);
    }

    private void HandleClient(Socket clientSocket)
    {
        SendMessage(clientSocket, "Hello from the server! You are now part of the Cold War!");
        AssignSide(clientSocket, "Server");
        WaitForEnter(); // Wait for player to press enter
        StartGame(clientSocket);
    }

    private void HandleServer(Socket serverSocket)
    {
        string welcomeMessage = "Hello from the client! Welcome to the Cold War!";
        SendMessage(serverSocket, welcomeMessage);
        AssignSide(serverSocket, "Client");
        WaitForEnter(); // Wait for player to press enter
        StartGame(serverSocket);
    }

    private void AssignSide(Socket socket, string role)
    {
        string[] sides = { "USA", "USSR" };

        if (role == "Server")
        {
            _playerSide = sides[1]; // Server is USSR
            _country = new USSR(250000000, new List<string> { "Moscow", "Saint Petersburg", "Kyiv" }, 4000, "Mikhail Gorbachev");
            SendMessage(socket, $"You have been assigned: {sides[1]}");
            Console.WriteLine($"Server chose: {sides[1]}");
        }
        else
        {
            Console.WriteLine("Choose your side:");
            Console.WriteLine("1. USA (United States of America)");
            Console.WriteLine("2. USSR (Soviet Union)");
            string choice = Console.ReadLine();

            if (choice.Trim() == "1")
            {
                _playerSide = sides[0]; // Client chooses USA
                _country = new USA(300000000, new List<string> { "New York", "Los Angeles", "Chicago" }, 5000, "Ronald Reagan");
                SendMessage(socket, sides[0]);
                Console.WriteLine($"Client chose: {sides[0]}");
            }
            else if (choice.Trim() == "2")
            {
                _playerSide = sides[1]; // Client chooses USSR
                _country = new USSR(250000000, new List<string> { "Moscow", "Saint Petersburg", "Kyiv" }, 4000, "Mikhail Gorbachev");
                SendMessage(socket, sides[1]);
                Console.WriteLine($"Client chose: {sides[1]}");
            }
            else
            {
                Console.WriteLine("Invalid choice. Assigning default side: USA.");
                _playerSide = sides[0]; // Default to USA
                _country = new USA(300000000, new List<string> { "New York", "Los Angeles", "Chicago" }, 5000, "Ronald Reagan");
                SendMessage(socket, sides[0]);
                Console.WriteLine($"Client chose: {sides[0]}");
            }
        }
    }

    private void WaitForEnter()
    {
        Console.WriteLine("Press Enter to start the game...");
        Console.ReadLine();
    }

    private void StartGame(Socket socket)
    {
        Console.WriteLine("Turn 1");
        Console.WriteLine($"{_playerSide} starts");

        // Increase income at the beginning of the turn
        _country.Income += IncomeIncrease; // Update income

        // Show updated stats after income increase
        ShowCountryStats(socket); // Show stats after income increase

        string gameState = "Choose your action:\n1. Wait\n2. Buy Warheads\n3. Attack";
        SendMessage(socket, gameState);
        Console.WriteLine(gameState); // Print options for debugging

        // Get the player's action
        string action = ReceiveMessage(socket); // Read the action from the client
        HandleAction(socket, action);
    }

    private void HandleAction(Socket socket, string action)
    {
        switch (action)
        {
            case "1": // Wait
                Console.WriteLine($"{_playerSide} chose to wait.");
                break;
            case "2": // Buy Warheads
                // Handle the logic for buying warheads (update capacity, etc.)
                Console.WriteLine($"{_playerSide} chose to buy warheads.");
                break;
            case "3": // Attack
                Console.WriteLine($"{_playerSide} chose to attack."); // Handle attack logic here
                break;
            default:
                Console.WriteLine("Invalid action selected.");
                break;
        }

        // Show stats after actions, print only once
        ShowCountryStats(socket); // Show stats after actions
    }

    private void ShowCountryStats(Socket socket)
    {
        // Prepare the country stats to send to the player
        string stats = $"Current Leader: {(_country is USA ? ((USA)_country).President : ((USSR)_country).Leader)}\n" +
                       $"Current Income: {_country.Income:C}\n" +
                       $"Population: {_country.Population}\n" +
                       $"Warheads Capacity: {_country.WarheadsCapacity}";

        // Send the stats to the socket
        SendMessage(socket, stats);
        Console.WriteLine(stats); // Print stats on server side for debugging
    }

    private string ReceiveMessage(Socket socket)
    {
        byte[] buffer = new byte[1024];
        int bytesRead = socket.Receive(buffer);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }

    private void SendMessage(Socket socket, string message)
    {
        byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
        socket.Send(messageBuffer);
    }

    private void ListAvailableIPs()
    {
        Console.WriteLine("Available IP addresses:");
        foreach (var ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                Console.WriteLine($" - {ip}");
            }
        }
    }

    public void Close()
    {
        _socket?.Close();
    }
}
