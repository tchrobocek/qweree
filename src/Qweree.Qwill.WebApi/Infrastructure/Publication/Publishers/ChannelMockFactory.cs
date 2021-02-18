using System;
using System.Collections.Immutable;
using Qweree.Qwill.WebApi.Domain.Publishers;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Publishers
{
    public class ChannelMockFactory
    {
        private static readonly Random Random = new();

        private static readonly string[] Names = new[]
        {
            "Adelmarus",
            "Adrianus",
            "Ægidius",
            "Leonardus",
            "Isaacus",
            "Julius",
            "Ulricus",
            "Victor",
            "Victoria",
            "Xavierius",
            "Viviana",
            "Thomas",
            "Sara"
        };

        public static Channel CreateChannel()
        {
            var channelName = Names[Random.Next(Names.Length)];
            var ownerId = new Guid();
            return new Channel(Guid.NewGuid(), channelName, new[] {ownerId}.ToImmutableArray(), ownerId,
                DateTime.UtcNow, DateTime.UtcNow);
        }
    }
}