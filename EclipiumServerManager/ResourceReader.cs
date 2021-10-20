using System.IO;

namespace EclipiumServerManager
{
    public static class ResourceReader
    {
        public static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using MemoryStream ms = new MemoryStream();
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return ms.ToArray();
        }
    }
}