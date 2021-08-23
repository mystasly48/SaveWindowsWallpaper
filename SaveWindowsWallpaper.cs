using System;
using System.IO;
using System.Drawing;
public class Program {
  public static void Main() {
    string SearchDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages\\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\\LocalState\\Assets");
    string SaveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "SaveWindowsWallpaper");
    string HorizontalDirectory = Path.Combine(SaveDirectory, "Horizontal");
    string VerticalDirectory = Path.Combine(SaveDirectory, "Vertical");
    string[] files = Directory.GetFiles(SearchDirectory);

    if (!Directory.Exists(SaveDirectory)) Directory.CreateDirectory(SaveDirectory);
    if (!Directory.Exists(HorizontalDirectory)) Directory.CreateDirectory(HorizontalDirectory);
    if (!Directory.Exists(VerticalDirectory)) Directory.CreateDirectory(VerticalDirectory);

    foreach (string file in files) {
      string fileName = file.Substring(SearchDirectory.Length+2) + ".jpeg";
      string tempFilePath = Path.Combine(SaveDirectory, fileName);
      File.Copy(file, tempFilePath);
      try {
        using (Image image = Image.FromFile(tempFilePath)) {
          if (image.Width >= 1920 && image.Height >= 1080) {
            image.Dispose();
            string saveFilePath = Path.Combine(HorizontalDirectory, fileName);
            if (File.Exists(saveFilePath)) {
              File.Delete(tempFilePath);
              Console.WriteLine("Skipped: Duplicate");
            } else {
              File.Move(tempFilePath, saveFilePath);
              Console.WriteLine("Moved: Horizontal");
            }           
          } else if (image.Width >= 1080 && image.Height >= 1920) {
            image.Dispose();
            string saveFilePath = Path.Combine(VerticalDirectory, fileName);
            if (File.Exists(saveFilePath)) {
              File.Delete(tempFilePath);
              Console.WriteLine("Skipped: Duplicate");
            } else {
              File.Move(tempFilePath, saveFilePath);
              Console.WriteLine("Moved: Vertical");
            }
          } else {
            image.Dispose();
            File.Delete(tempFilePath);
            Console.WriteLine("Deleted: Too small");
          }
        }
      } catch (OutOfMemoryException) {
        File.Delete(tempFilePath);
        Console.WriteLine("Deleted: Not image");
      }
    }
  }
}
