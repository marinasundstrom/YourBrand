using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Domain.ValueObjects;

namespace YourBrand.ChatApp.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedData(ApplicationDbContext context)
    {
        //context.Channels.Add(new Channel(new ChannelId("73b202c5-3ef1-4cd8-b1ed-04c05f47e981"), "General"));
        //context.Channels.Add(new Channel(new ChannelId("0b896f9b-d248-4b11-b96b-6fa1902af606"), "Blah"));

        await context.SaveChangesAsync();
    }
}