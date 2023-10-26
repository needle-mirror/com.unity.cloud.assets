using System;

namespace Unity.Cloud.Assets
{
    public interface IFileCreation : IFileUpdate
    {
        string Path { get; }
    }

    interface IFileCreationWithDetails: IFileCreation
    {
        string UserChecksum { get; }
        long SizeBytes { get; }
    }
}
