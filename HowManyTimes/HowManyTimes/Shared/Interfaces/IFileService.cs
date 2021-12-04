using System.IO;

namespace HowManyTimes.Shared.Interfaces
{
    internal interface IFileService
    {
        void SavePicture(string name, Stream data, string location = "temp");
    }
}
