using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
public class Program {
  public  static void Main() {
    string file = Console.ReadLine();
    byte[] hash;
    using (Image image = Image.FromFile(file))
    using (Bitmap bmp = (Bitmap)image) {
      BitmapData bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
      int bsize = bd.Stride * bmp.Height;
      byte[] bytes = new byte[bsize];
      try {
        Marshal.Copy(bd.Scan0, bytes, 0, bsize);
      } finally {
        bmp.UnlockBits(bd);
      }
      MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
      hash = md5.ComputeHash(bytes);
    }
    Console.WriteLine(Encoding.ASCII.GetString(hash));
    Console.WriteLine(string.Join("", hash.Select(b => b.ToString()).ToArray()));
  }
}
