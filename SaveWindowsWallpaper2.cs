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
  // 設定ファイルに関する変数
  private static Settings settings = new Settings();
  private static string SettingsPath;

  // 検索対象フォルダや保存先フォルダ等の変数
  private const string DIRECTORY_UID = "cw5n1h2txyewy";
  private static string SearchDirectory, SaveDirectory, HorizontalDirectory, VerticalDirectory;
  private static string[] ImagePaths;

  public static void Main() {
    PrepareImagePaths();
    if (ImagePaths.Length > 0) {
      loadSettings();
      JudgeAndDeal();
      saveSettings();
      Console.WriteLine("Completed.");
    } else {
      Console.WriteLine("Files not found.");
    }
    //Console.WriteLine("Press any key to exit.");
    //Console.ReadKey();
  }

  // 検索対象フォルダの指定や、保存先フォルダの指定等
  private static void PrepareImagePaths() {
    SearchDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages\\Microsoft.Windows.ContentDeliveryManager_" + DIRECTORY_UID + "\\LocalState\\Assets");
    SaveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "SaveWindowsWallpaper");
    HorizontalDirectory = Path.Combine(SaveDirectory, "Horizontal");
    VerticalDirectory = Path.Combine(SaveDirectory, "Vertical");
    ImagePaths = Directory.GetFiles(SearchDirectory);
    SettingsPath = Path.Combine(SaveDirectory, "Settings.xml");

    if (!Directory.Exists(SaveDirectory)) {
      Directory.CreateDirectory(SaveDirectory);
    }
    if (!Directory.Exists(HorizontalDirectory)) {
      Directory.CreateDirectory(HorizontalDirectory);
    }
    if (!Directory.Exists(VerticalDirectory)) {
      Directory.CreateDirectory(VerticalDirectory);
    }
  }

  // 画像の状態（内容等）を確認して、それに対応した処理を行う
  private static void JudgeAndDeal() {
    // 新規ファイルの全探索
    foreach (string file in ImagePaths) {
      // フォルダ名を除いて、ファイル名のみ取得する
      string fileName = file.Substring(SearchDirectory.Length + 2);
      string saveFilePath = "";
      try {
        using (Image image = Image.FromFile(file)) {
          if (isHighDefinitionImage(image.Width, image.Height)) {
            string hash = getHash(file);
            if (settings.Hashes.Contains(hash)) {
              Console.WriteLine("Deleted: Duplicate");
            } else if (settings.BlackHashes.Contains(hash)) {
              Console.WriteLine("Deleted: Blacklist");
            } else {
              if (isHorizontalImage(image.Width, image.Height)) {
                saveFilePath = getOptimumPath(HorizontalDirectory, fileName);
                Console.WriteLine("Moved: Horizontal");
              } else if (isVerticalImage(image.Width, image.Height)) {
                saveFilePath = getOptimumPath(VerticalDirectory, fileName);
                Console.WriteLine("Moved: Vertical");
              } else {
                Console.WriteLine("Unknown error! Path: " + file);
                continue;
              }
              settings.Hashes.Add(hash);
            }
          } else {
            Console.WriteLine("Deleted: Too small");
          }
        }
      } catch (OutOfMemoryException) {
        Console.WriteLine("Deleted: Not image");
      }
      if (saveFilePath != "") {
      	File.Move(file, saveFilePath);
      } else {
      	File.Delete(file);
      }
    }
  }

  private static bool isHighDefinitionImage(int width, int height) =>
    (isHorizontalImage(width, height) || isVerticalImage(width, height));

  private static bool isHorizontalImage(int width, int height) =>
    (width >= 1920 && height >= 1080);

  private static bool isVerticalImage(int width, int height) =>
    (width >= 1080 && height >= 1920);

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

  // 保存に最適な名前を取得（重複していなければそのまま返却、重複していれば生成して返却）
  private static string getOptimumPath(string directory, string name) {
    string path = Path.Combine(directory, name + ".jpeg");
    while (true) {
      if (File.Exists(path)) {
        path = Path.Combine(directory, getRandomCode());
        if (path.Substring(path.Length-5) != ".jpeg") {
          path += ".jpeg";
        }
      } else {
        return path;
      }
    }
  }

  // 適当な文字列を返却
  private static string getRandomCode() {
    var result = "";
    var codeChar = "0123456789abcdefghijklmnopqrstuvwxyz";
    var rand = new Random();
    for (var i = 0; i < 64; i++) {
      var pos = rand.Next(codeChar.Length);
      var code = codeChar[pos];
      result += code;
    }
    return result;
  }

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
