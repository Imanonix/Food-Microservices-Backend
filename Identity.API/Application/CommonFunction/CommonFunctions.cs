using Application.DTOs;
using Application.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CommonFunction
{
    public class CommonFunctions
    {
        public static List<string> extensionsList = new List<string>() { ".jpg", ".jpeg", ".png", ".webp", ".svg", ".bmp", ".pdf", ".mp3", ".mp4" };

        public static string FileValidation(IFormFile file)
        {
            try
            {
                var fileExtension = Path.GetExtension(file.FileName);
                Console.WriteLine($"🟢 [FileValidation] File extension: {fileExtension}");

                if (!extensionsList.Contains(fileExtension))
                {
                    Console.WriteLine($"❌ [FileValidation] Invalid extension: {fileExtension}");
                    throw new ValidationException("Invalid file type. Please upload an image with one of the following extensions: .jpeg, .jpg, .png, .webp, .svg");
                }

                if (fileExtension == ".pdf")
                {
                    fileExtension = ".pdf";
                    Console.WriteLine($"ℹ️ [FileValidation] Extension converted to .webp");
                }

                if (fileExtension == ".mp3" || fileExtension == ".mp4")
                {
                    fileExtension = ".mp4";
                    Console.WriteLine($"ℹ️ [FileValidation] Extension converted to .mp4");
                }

                if (extensionsList.Contains(fileExtension) && fileExtension != ".pdf" && fileExtension != ".mp4" && fileExtension != ".mp3")
                {
                    fileExtension = ".webp";
                }

                return fileExtension;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [FileValidation] Error: {ex.Message}");
                throw;
            }
        }


        public static async Task SaveFile(string imageFolderPath, string imagePath, IFormFile file)
        {
            try
            {
                Console.WriteLine($"🟢 [SaveFile] Checking if folder exists: {imageFolderPath}");

                if (!Directory.Exists(imageFolderPath))
                {
                    Directory.CreateDirectory(imageFolderPath);
                    Console.WriteLine($"✅ [SaveFile] Directory created: {imageFolderPath}");
                }

                Console.WriteLine($"🟢 [SaveFile] Saving file to: {imagePath}");

                using (var stream = file.OpenReadStream())
                using (var image = Image.Load(stream))
                {
                    var encoder = new WebpEncoder { Quality = 80 };
                    await image.SaveAsync(imagePath, encoder);
                }

                Console.WriteLine($"✅ [SaveFile] Image saved successfully at {imagePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SaveFile] Error saving file: {ex}");
                throw;
            }
        }

        public static async Task SavePDF(string folderPath, string pdfPath, IFormFile file)
        {
            try
            {
                Console.WriteLine($"🟢 [SaveFile] Checking if folder exists: {folderPath}");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                    Console.WriteLine($"✅ [SaveFile] Directory created: {folderPath}");
                }

                Console.WriteLine($"🟢 [SaveFile] Saving file to: {pdfPath}");
                using (var stream = new FileStream(pdfPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [SaveFile] Error saving file: {ex}");
                throw;
            }
        }

        public static async Task SaveAudio(string folderPath, string audioPath, IFormFile file)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                using (var stream = new FileStream(audioPath, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public static void MoveFile(string newPath, string oldPath)
        {

            string[] files = Directory.GetFiles(oldPath);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            foreach (var file in files)
            {
                string fileName = Path.GetFileName(file); // Extract only the file name
                File.Move(file, newPath + "/" + fileName);
            }
            Directory.Delete(oldPath);
        }

        public static async Task DeleteFile(string webRootPath, string imageUrl)
        {
            if (File.Exists(webRootPath + imageUrl))
            {
                File.Delete(webRootPath + imageUrl);
            }
        }

        public static async Task<FileContentResult> CreateSaveQRCode(string url, string path)
        {
            var qrCodeGenerator = new QRCodeGenerator();
            var qrCodeData = qrCodeGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            var svgQRCode = new SvgQRCode(qrCodeData);
            var qrCodeImage = svgQRCode.GetGraphic(20);
            try
            {
                var fileBytes = Encoding.UTF8.GetBytes(qrCodeImage);    // Convert string to byte[] for sending the file or saving to disk
                                                                        // Files are always transmitted over HTTP as byte[], not as string.
                var fileName = "qrcode.svg";

                return new FileContentResult(fileBytes, "image/svg+xml")
                {
                    FileDownloadName = fileName
                };
            }
            catch
            {
                throw new InvalidOperationException("QRCode couldn't create");
            }
        }


    }
}
