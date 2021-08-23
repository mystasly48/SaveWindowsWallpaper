using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

public class Program {
  private static Settings settings = new Settings();
  private static string SettingsPath;

  public static void Main() {
    string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "SaveWindowsWallpaper");
    SettingsPath = Path.Combine(saveDirectory, "Settings.xml");
    if (!Directory.Exists(saveDirectory)) {
      Console.WriteLine("There is not settings file.");
      return;
    }
    loadSettings();

    Console.WriteLine("Enter a image path to add to blacklist.");
    string imagePath = Console.ReadLine();
    string hash = getHash(imagePath);
    if (settings.BlackHashes.Contains(hash)) {
      Console.WriteLine("Already added to blacklist.");
      return;
    } else if (settings.Hashes.Contains(hash)) {
      settings.BlackHashes.Add(hash);
      settings.Hashes.Remove(hash);
      string verorhori = imagePath.Substring(saveDirectory.Length + 2);
      string fileName = (verorhori[0] == 'H') ? verorhori.Substring(12) : verorhori.Substring(10);
      string saveFile = Path.Combine(Path.Combine(saveDirectory, "Blacklist"), fileName);
      File.Move(imagePath, saveFile);
      Console.WriteLine("Add to blacklist was successful.");
    } else {
      Console.WriteLine("The image is not added to SaveWindowsWallpaper directory.");
      return;
    }
    saveSettings();
    Console.WriteLine("Press the any key to exit.");
    Console.ReadKey();
  }

  // 画像のハッシュ値を取得
  private static string getHash(string file) {
    Bitmap img = new Bitmap(file);
    BitmapData bd = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
    try {
      int bsize = bd.Stride * img.Height;
      byte[] bytes = new byte[bsize];
      Marshal.Copy(bd.Scan0, bytes, 0, bsize);

      MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
      byte[] hash = md5.ComputeHash(bytes);

      return string.Join("", hash.Select(b => b.ToString()).ToArray());
    } finally {
      img.UnlockBits(bd);
      img.Dispose();
    }
  }

  // 設定の読み込み
  private static void loadSettings() {
    if (File.Exists(SettingsPath)) {
      var serializer = new XmlSerializer(typeof(Settings));
      var reader = new StreamReader(SettingsPath);
      settings = (Settings)serializer.Deserialize(reader);
      reader.Close();
    } else {
      saveSettings();
      loadSettings();
    }
  }

  // 設定の保存
  private static void saveSettings() {
    var serializer = new XmlSerializer(typeof(Settings));
    var writer = new StreamWriter(SettingsPath, false, Encoding.UTF8);
    serializer.Serialize(writer, settings);
    writer.Close();
  }
}

public class Settings {
  public List<string> Hashes = new List<string>();
  public List<string> BlackHashes = new List<string>();
}