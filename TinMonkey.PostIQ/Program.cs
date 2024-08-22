// Read from console until enter pressed twice and process data input so far.

using BenchmarkDotNet.Running;
using TinMonkey.PostIQ;

// Add CTRL-C break handler
Console.CancelKeyPress += (sender, e) =>
{
    Console.WriteLine("Exiting...");
    Environment.Exit(0);
};


if (args.Length > 0 && args[0] == "bench")
{
    _ = BenchmarkRunner.Run<Benchmarks>();
}
else
{
    var benchmark = new Benchmarks();
    benchmark.FunctionalAddressMatching();
}
