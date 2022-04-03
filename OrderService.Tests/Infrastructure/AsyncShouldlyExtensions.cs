using System.Threading.Tasks;
using Shouldly;

namespace OrderService.Tests.Infrastructure;

public static class AsyncShouldlyExtensions
{
    public static async Task ShouldBeTrue(this Task<bool> task)
    {
        bool actual = await task;
        actual.ShouldBeTrue();
    }
}