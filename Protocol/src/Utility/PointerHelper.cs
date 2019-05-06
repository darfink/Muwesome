using System.Text;

namespace Muwesome.Protocol.Utility {
  internal static class PointerHelper {
    public static unsafe string GetString(byte* data, int maxLength, Encoding encoding, byte[] xorCipher = null) {
      xorCipher?.ApplyCipher(data, maxLength);
      int stringLength = 0;
      for (; stringLength < maxLength && data[stringLength] != 0; stringLength++) { }

      string result = encoding.GetString(data, stringLength);
      xorCipher?.ApplyCipher(data, maxLength);
      return result;
    }

    public static unsafe void SetString(byte* data, int maxLength, string value, Encoding encoding, byte[] xorCipher = null, bool withNull = true) {
      fixed (char* input = value) {
        int bytesWritten = encoding.GetBytes(input, value.Length, data, maxLength - (withNull ? 1 : 0));
        if (withNull) {
          data[bytesWritten] = 0;
        }

        xorCipher?.ApplyCipher(data, maxLength);
      }
    }

    private unsafe static void ApplyCipher(this byte[] xorCipher, byte* data, int length) {
      for (int i = 0; i < length; i++) {
        data[i] ^= xorCipher[i % xorCipher.Length];
      }
    }
  }
}