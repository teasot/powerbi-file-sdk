using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerBIFileSDK
{
    internal static class FileHelper
    {
        internal static void PerformActionOnLayout(string FileLocation, Action<JObject> ActionToPerform)
        {
            using (FileStream ZipFileStream = new FileStream(FileLocation, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (ZipArchive Archive = new ZipArchive(ZipFileStream, ZipArchiveMode.Update))
            {
                JObject LayoutObject;
                using (StreamReader Reader = new StreamReader(Archive.GetEntry(ArchiveFileLocations.Layout).Open(), System.Text.Encoding.Unicode))
                using (JsonTextReader JsonReader = new JsonTextReader(new StringReader(Reader.ReadToEnd())))
                {
                    JsonReader.FloatParseHandling = FloatParseHandling.Decimal;

                    LayoutObject = JObject.Load(JsonReader);

                    ActionToPerform(LayoutObject);
                }

                Archive.GetEntry(ArchiveFileLocations.Layout).Delete();

                System.Text.Encoding UnicodeWithoutBom = new System.Text.UnicodeEncoding(false, false);
                byte[] LayoutByteArray = UnicodeWithoutBom.GetBytes(LayoutObject.ToString());

                using (MemoryStream Stream = new MemoryStream(LayoutByteArray))
                using (Stream Writer = Archive.CreateEntry(ArchiveFileLocations.Layout).Open())
                    Stream.CopyTo(Writer);

                Archive.GetEntry(ArchiveFileLocations.SecurityBindings)?.Delete();
            }
        }
    }
}
