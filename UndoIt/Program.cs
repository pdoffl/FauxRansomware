using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.IO.Pipes;

namespace UndoIt
{
    internal static class Program
    {
        // PInvoke for changing system parameters ? Used for changing wallpaper later on.
        public const int SPI_SETDESKWALLPAPER = 20;
        public const int SPIF_UPDATEINIFILE = 1;
        public const int SPIF_SENDCHANGE = 2;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)] public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [STAThread]
        static void Main()
        {
            try
            {
                // Generate random number, and let the program slow down a bit.
                Random rnd = new Random();
                Thread.Sleep(rnd.Next(1, 7) * 1000);

                // Search the current user's Desktop for corrupted files
                string file_searchingPath = Environment.GetEnvironmentVariable("UserProfile") + @"\Desktop";

                // Store the full path of the corrupted files in their respective string arrays
                string[] excel_corruptedFiles = System.IO.Directory.GetFiles(file_searchingPath, "*.xlsx.encrypted");
                string[] word_corruptedFiles = System.IO.Directory.GetFiles(file_searchingPath, "*.docx.encrypted");
                string[] pdf_corruptedFiles = System.IO.Directory.GetFiles(file_searchingPath, "*.pdf.encrypted");

                // XOR-ing the corrupted file with the predefined byte to restore the original content
                // Logic Reference: https://gist.github.com/nakov/1d39c4513cff83b8a735d7dc883dfe18
                for (int i = 0; i < excel_corruptedFiles.Length; i++)
                {
                    string old_filePath = excel_corruptedFiles[i];
                    string new_filePath = excel_corruptedFiles[i].Replace(".encrypted", "");

                    using (var oldFile = new FileStream(old_filePath, FileMode.Open, FileAccess.Read))
                    using (var newFile = new FileStream(new_filePath, FileMode.CreateNew, FileAccess.Write))
                    {
                        byte[] temp_buffer = new byte[4096];

                        while (true)
                        {
                            int bytesRead = oldFile.Read(temp_buffer, 0, 4096);

                            if (bytesRead == 0)
                            {
                                break;
                            }

                            else
                            {
                                for (int j = 0; j < bytesRead; j++)
                                {
                                    temp_buffer[j] = (byte)(temp_buffer[j] ^ 68);
                                }

                                newFile.Write(temp_buffer, 0, bytesRead);
                            }
                        }

                        oldFile.Close();
                        newFile.Close();
                        File.Delete(old_filePath);
                    }
                }

                for (int i = 0; i < word_corruptedFiles.Length; i++)
                {
                    string old_filePath = word_corruptedFiles[i];
                    string new_filePath = word_corruptedFiles[i].Replace(".encrypted", ""); ;

                    using (var oldFile = new FileStream(old_filePath, FileMode.Open, FileAccess.Read))
                    using (var newFile = new FileStream(new_filePath, FileMode.CreateNew, FileAccess.Write))
                    {
                        byte[] temp_buffer = new byte[4096];

                        while (true)
                        {
                            int bytesRead = oldFile.Read(temp_buffer, 0, 4096);

                            if (bytesRead == 0)
                            {
                                break;
                            }

                            else
                            {
                                for (int j = 0; j < bytesRead; j++)
                                {
                                    temp_buffer[j] = (byte)(temp_buffer[j] ^ 69);
                                }

                                newFile.Write(temp_buffer, 0, bytesRead);
                            }
                        }

                        oldFile.Close();
                        newFile.Close();
                        File.Delete(old_filePath);
                    }
                }

                for (int i = 0; i < pdf_corruptedFiles.Length; i++)
                {
                    string old_filePath = pdf_corruptedFiles[i];
                    string new_filePath = pdf_corruptedFiles[i].Replace(".encrypted", "");

                    using (var oldFile = new FileStream(old_filePath, FileMode.Open, FileAccess.Read))
                    using (var newFile = new FileStream(new_filePath, FileMode.CreateNew, FileAccess.Write))
                    {
                        byte[] temp_buffer = new byte[4096];

                        while (true)
                        {
                            int bytesRead = oldFile.Read(temp_buffer, 0, 4096);

                            if (bytesRead == 0)
                            {
                                break;
                            }

                            else
                            {
                                for (int j = 0; j < bytesRead; j++)
                                {
                                    temp_buffer[j] = (byte)(temp_buffer[j] ^ 70);
                                }

                                newFile.Write(temp_buffer, 0, bytesRead);
                            }
                        }

                        oldFile.Close();
                        newFile.Close();
                        File.Delete(old_filePath);
                    }
                }

                // Won't be restoring the old shadow volumes post deletion, as once they're gone, they're gone.

                // Logic Reference: https://github.com/philhansen/WallpaperChanger
                // The "ransomware" created a backup of the current wallpaper before replacing it with the "ransomware" wallpaper.
                // Sets the original wallpaper back from the backup path
                string originalWallpaper_backupPath = Environment.GetEnvironmentVariable("Temp") + @"\TranscodedWallpaper.jpg.backup";
                string originalWallpaper_savingPath = Environment.GetEnvironmentVariable("Temp") + @"\OriginalWallpaper.bmp";

                // For Windows 10
                if (!File.Exists(originalWallpaper_backupPath))
                {
                    originalWallpaper_backupPath = Environment.GetEnvironmentVariable("Temp") + @"\TranscodedWallpaper.backup";
                }

                Image originalWallpaper = System.Drawing.Image.FromFile(originalWallpaper_backupPath);
                originalWallpaper.Save(originalWallpaper_savingPath, System.Drawing.Imaging.ImageFormat.Bmp);

                // Using PInvoke, changing system parameters to set wallpaper
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, originalWallpaper_savingPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

                // Delete the wallpaper files, once done.
                File.Delete(originalWallpaper_savingPath);

                // Program slows down again
                Thread.Sleep(rnd.Next(1, 7) * 1000);

                // Deletes the "ransomware" note.
                string ransomNotePath = Environment.GetEnvironmentVariable("UserProfile") + @"\Desktop\contact_info.txt";
                File.Delete(ransomNotePath);

                // Program slows down again
                Thread.Sleep(rnd.Next(1, 7) * 1000);

                // Delete the wallpaper files, once done. (Doesn't work: File in use by this executable)
                //File.Delete(originalWallpaper_backupPath);

                // Final message box pop-up
                string message = "System restored. Thank you for the $$$!";
                string title = "Information";
                MessageBox.Show(message, title);
            }

            catch (Exception Ex)
            {
                // Prints the exception message in the Windows message box.
                // Might be invoked if the file path to save ransom note, is not accessible or doesn't not exist. 
                MessageBox.Show(Ex.ToString(), "Exception Warning!", 0, MessageBoxIcon.Warning);
                System.Environment.Exit(1);
            }
        }
    }
}
