using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vialidad.Utils.Extensions
{
    public static class ConvertersExtensionMethods
    {
        public static long ToLong(this string sLong)
        {
            long.TryParse(sLong, out long longResult);
            return longResult;
        }

        public static int ToInt(this string sInt)
        {
            int.TryParse(sInt, out int intResult);
            return intResult;
        }

        public static decimal ToDecimal(this string sDecimal)
        {
            decimal.TryParse(sDecimal, out decimal intResult);
            return intResult;
        }

        public static double ToDouble(this string sDouble)
        {
            double.TryParse(sDouble, out double intResult);
            return intResult;
        }

        public static Stream ToStream(this byte[] bytesArray)
        {
            return new MemoryStream(bytesArray);
        }

        public static byte[] ToArrayBytes(this Stream stream)
        {
            var buffer = new byte[32768];
            var ms = new MemoryStream();
            int bytesRead;
            do
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);

                ms.Write(buffer, 0, bytesRead);
            } while (bytesRead > 0);

            var result = ms.ToArray();
            ms.Close();

            return result;
        }

        public static string ToBase64(this Stream fileContents)
        {
            var buffer = new byte[32768];
            var ms = new MemoryStream();
            int bytesRead;
            do
            {
                bytesRead = fileContents.Read(buffer, 0, buffer.Length);
                ms.Write(buffer, 0, bytesRead);
            }
            while (bytesRead > 0);

            var imageBytes = ms.ToArray();

            ms.Close();

            var base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }
    }
}
