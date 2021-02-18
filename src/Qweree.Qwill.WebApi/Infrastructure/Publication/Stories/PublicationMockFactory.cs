using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Qweree.Qwill.WebApi.Domain.Stories;

namespace Qweree.Qwill.WebApi.Infrastructure.Publication.Stories
{
    public class PublicationMockFactory
    {
        private static readonly Random Random = new();

        private static readonly string[] Images =
        {
            "https://i.picsum.photos/id/1000/5626/3635.jpg",
            "https://i.picsum.photos/id/1004/5616/3744.jpg",
            "https://i.picsum.photos/id/1015/6000/4000.jpg",
            "https://i.picsum.photos/id/1024/1920/1280.jpg"
        };

        private static readonly string[] Languages =
        {
            "en",
            "de",
            "cz"
        };

        private static readonly string[] Titles =
        {
            "Donec tincidunt mi vel sapien ornare congue",
            "Sed at magna vitae orci molestie ",
            "Donec pulvinar porttitor ultrices."
        };

        private static readonly string[] Paragraphs =
        {
            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec tincidunt mi vel sapien ornare congue. Aenean venenatis sed neque non porttitor. Vivamus porttitor, risus id sodales posuere, nisl dolor bibendum est, in luctus urna nunc non nunc. Integer in odio aliquet, sollicitudin neque ac, convallis dui. Aliquam erat volutpat. Phasellus cursus metus efficitur, sodales ante a, blandit tortor. Nullam ut pellentesque purus. Cras lacinia pellentesque erat non vulputate. Cras sed orci ac lacus varius finibus sit amet eu mi. Pellentesque tortor sapien, porttitor at ante vel, faucibus sodales eros. Suspendisse fermentum consequat varius. Fusce ornare ipsum arcu, eget ultricies sem ullamcorper quis.",
            "Sed in orci id nunc iaculis dictum. Aliquam aliquet eros in mauris facilisis porta. Sed at magna vitae orci molestie sollicitudin vel et mi. Cras placerat placerat tortor sit amet placerat. Pellentesque massa velit, venenatis non vestibulum nec, dapibus non arcu. Aliquam lacinia sapien nec scelerisque posuere. Duis leo libero, dictum sed gravida quis, molestie non odio.",
            "Etiam euismod, orci nec mattis congue, nunc lacus lobortis sem, id scelerisque erat erat eget odio. Duis a dolor libero. Donec gravida diam tellus, vitae congue tellus malesuada in. Mauris feugiat, erat sit amet lobortis luctus, ipsum justo suscipit purus, non aliquet eros tellus non sem. Maecenas condimentum mauris sit amet ligula posuere vestibulum. In tincidunt ipsum vitae tempus cursus. Nulla ut orci facilisis, sagittis ex sit amet, luctus dolor. Donec pulvinar porttitor ultrices. Fusce sodales maximus tortor, sed placerat sapien ullamcorper iaculis. Integer nec sollicitudin nunc."
        };


        public static Domain.Stories.Publication CreatePublication(Guid channelId)
        {
            var pages = new List<string>();
            for (var i = 0; i < Random.Next(5) + 1; i++) pages.Add(Paragraphs[Random.Next(Paragraphs.Length)]);

            var language0 = Languages[Random.Next(Languages.Length)];
            var language1 = Languages[Random.Next(Languages.Length)];

            return new Domain.Stories.Publication(
                Guid.NewGuid(),
                channelId,
                Guid.NewGuid(),
                new DateTime(2010 + Random.Next(10), Random.Next(12) + 1, Random.Next(28) + 1),
                new DateTime(2010 + Random.Next(10), Random.Next(12) + 1, Random.Next(28) + 1),
                new DateTime(2010 + Random.Next(10), Random.Next(12) + 1, Random.Next(28) + 1),
                new List<PublicationTranslation>
                {
                    new(
                        language0,
                        Titles[Random.Next(Titles.Length)],
                        Paragraphs[Random.Next(Paragraphs.Length)],
                        Images[Random.Next(Images.Length)],
                        pages.ToImmutableArray(),
                        new DateTime(2010 + Random.Next(10), Random.Next(12) + 1, Random.Next(28) + 1),
                        new DateTime(2010 + Random.Next(10), Random.Next(12) + 1, Random.Next(28) + 1)),

                    new(
                        language1,
                        Titles[Random.Next(Titles.Length)],
                        Paragraphs[Random.Next(Paragraphs.Length)],
                        Images[Random.Next(Images.Length)],
                        pages.ToImmutableArray(),
                        new DateTime(2010 + Random.Next(10), Random.Next(12) + 1, Random.Next(28) + 1),
                        new DateTime(2010 + Random.Next(10), Random.Next(12) + 1, Random.Next(28) + 1))
                }.ToImmutableArray());
        }
    }
}