using System;
using Orion.Domain;
using Orion.Services;

class Program
{
    static void Main(string[] args)
    {
        var _networkService = new NetworkService();
        _networkService.StartNetwork();
        Console.WriteLine("============================================");
        Console.WriteLine("Starting Network");
        Console.WriteLine("============================================\n");

        _networkService.FoundNodeEvent += (Node node) =>
        {
            Console.WriteLine($"Name: {node.Name}       IP: {node.Ip}");
        };

        _networkService.BroadcastSelfNode();
        Console.WriteLine("============================================");
        Console.WriteLine("Broadcasting Self Node");
        Console.WriteLine("============================================\n");

        Console.ReadKey();
    }
}
