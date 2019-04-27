using System.Text;

namespace Muwesome.Protocol.Utility {
  internal static class PointerHelper {
    public static unsafe string GetString(byte* data, int maxLength, Encoding encoding) {
      int stringLength = 0;
      for (; stringLength < maxLength && data[stringLength] != 0; stringLength++) { }
      return Encoding.ASCII.GetString(data, stringLength);
    }

    public static unsafe void SetString(byte* data, int maxLength, string value, Encoding encoding) {
      fixed (char* input = value) {
        int bytesWritten = Encoding.ASCII.GetBytes(input, value.Length, data, maxLength - 1);
        data[bytesWritten] = 0;
      }
    }
  }
}