using System;
using Orion.Domain;
using Orion.Services;

class Program
{
    static void Main(string[] args)
    {
        var networkService = new NetworkService();
        networkService.StartNetwork();

        networkService.FoundNodeEvent += (Node node) =>
        {
            Console.WriteLine($"Name: {node.Name}       IP: {node.Ip}");
            
        };

        Console.ReadKey();
    }
}
