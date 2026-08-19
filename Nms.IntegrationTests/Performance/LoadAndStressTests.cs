using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Performance;

public class LoadAndStressTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly ITestOutputHelper _output;

    public LoadAndStressTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    [Theory]
    [InlineData("/swagger/index.html", 50, 500)]
    [InlineData("/swagger/index.html", 150, 1500)]
    public async Task Endpoint_UnderConcurrentLoad_MeetsSlaThresholds(
        string endpoint,
        int concurrencyLevel,
        int totalRequests)
    {
        var client = _factory.CreateClient();
        var latencies = new ConcurrentBag<long>();
        var statusCodes = new ConcurrentBag<HttpStatusCode>();

        using var semaphore = new SemaphoreSlim(concurrencyLevel);
        var overallStopwatch = Stopwatch.StartNew();

        var tasks = Enumerable.Range(0, totalRequests).Select(async _ =>
        {
            await semaphore.WaitAsync();
            var sw = Stopwatch.StartNew();
            try
            {
                var response = await client.GetAsync(endpoint);
                sw.Stop();
                latencies.Add(sw.ElapsedMilliseconds);
                statusCodes.Add(response.StatusCode);
            }
            catch
            {
                sw.Stop();
                latencies.Add(sw.ElapsedMilliseconds);
                statusCodes.Add(HttpStatusCode.InternalServerError);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        overallStopwatch.Stop();

        // Metric calculations
        var sortedLatencies = latencies.OrderBy(x => x).ToList();
        var serverErrors = statusCodes.Count(code => (int)code >= 500);
        var errorRate = (double)serverErrors / totalRequests * 100;
        var rps = totalRequests / (overallStopwatch.ElapsedMilliseconds / 1000.0);

        var p50 = sortedLatencies[(int)(sortedLatencies.Count * 0.50)];
        var p95 = sortedLatencies[(int)(sortedLatencies.Count * 0.95)];
        var p99 = sortedLatencies[(int)(sortedLatencies.Count * 0.99)];

        _output.WriteLine($"--- Load Test Results for: {endpoint} ---");
        _output.WriteLine($"Concurrency: {concurrencyLevel} | Total: {totalRequests} requests");
        _output.WriteLine($"Throughput: {rps:F2} req/sec | Elapsed: {overallStopwatch.ElapsedMilliseconds} ms");
        _output.WriteLine($"Error Rate: {errorRate:F2}% ({serverErrors} failed)");
        _output.WriteLine($"Latency: P50 = {p50} ms | P95 = {p95} ms | P99 = {p99} ms | Max = {sortedLatencies.Last()} ms");

        // SLA Assertions
        Assert.True(errorRate < 1.0, $"Error rate exceeded SLA limit: {errorRate:F2}%");
        Assert.True(p95 < 1000, $"P95 latency exceeded 1000ms threshold: {p95} ms");
        Assert.True(p99 < 2000, $"P99 latency exceeded 2000ms threshold: {p99} ms");
    }
}