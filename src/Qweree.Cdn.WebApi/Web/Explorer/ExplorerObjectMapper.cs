using System;
using Qweree.Cdn.WebApi.Application.Explorer;

namespace Qweree.Cdn.WebApi.Web.Explorer
{
    public static class ExplorerObjectMapper
    {
        public static IExplorerObjectDto ToDto(IExplorerObject explorerObject)
        {
            if (explorerObject is ExplorerFile file)
                return ToDto(file);
            if (explorerObject is ExplorerDirectory directory)
                return ToDto(directory);
            throw new ArgumentException($"Mapper for type {explorerObject.GetType()} is not implemented.");
        }

        public static IExplorerObjectDto ToDto(ExplorerDirectory explorerDirectory)
        {
            return new ExplorerDirectoryDto
            {
                Path = explorerDirectory.Path,
                CreatedAt = explorerDirectory.CreatedAt,
                ModifiedAt = explorerDirectory.ModifiedAt,
                TotalCount = explorerDirectory.TotalCount,
                TotalSize = explorerDirectory.TotalSize
            };
        }

        public static IExplorerObjectDto ToDto(ExplorerFile explorerFile)
        {
            return new ExplorerFileDto
            {
                Id = explorerFile.Id,
                Path = explorerFile.Path,
                Size = explorerFile.Size,
                CreatedAt = explorerFile.CreatedAt,
                MediaType = explorerFile.MediaType,
                ModifiedAt = explorerFile.ModifiedAt
            };
        }
    }
}