using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Jables_Protocol.Serializers
{
    public class PictureSerializer    // don't think I need to implement the serializer interface
    {

        static public byte[] SerializePic(string gameResult)
        {
            //string filePath = "Assets/" + gameResult + ".jpg";
            
            // uses the current working directory, which should be the project root, so we can just do Assets/filename.jpg
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"Assets", gameResult + ".jpg");


            // Path.Combine constructs the file path in a platform-independent way
            //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", gameResult + ".jpg");

            try
            {
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms);
                //byte[] resultBytes = Encoding.UTF8.GetBytes(gameResult ?? string.Empty);
                //bw.Write(resultBytes);
                bw.Write(gameResult);

                // Read the entire file content into a byte array
                byte[] jpgBytes = File.ReadAllBytes(filePath);
                bw.Write(jpgBytes);

                // You can now use the byte array as needed (e.g., store in a database, send over a network)
                Console.WriteLine($"Image successfully read. Total bytes: {jpgBytes.Length}");
                
                return ms.ToArray();
            }
            catch (IOException ex)
            {
                // Handle file-related exceptions
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        static public string DeserializePic(byte[] data)
        {
            using var ms = new MemoryStream(data);
            using var br = new BinaryReader(ms);

            try
            {
                string gameResult = br.ReadString();

                byte[] imageBytes = br.ReadBytes((int)(ms.Length - ms.Position));

                string assetsFolder = "Assets";
                string filePath = Path.Combine(assetsFolder, gameResult + ".jpg");

                Directory.CreateDirectory(assetsFolder);

                Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
                Console.WriteLine("Writing to: " + Path.GetFullPath(filePath));

                File.WriteAllBytes(filePath, imageBytes);
                Console.WriteLine($"{gameResult} picture written successfully.");

                return gameResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing file: {ex.Message}");
                return "NA";
            }
        }
    }
}
