using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.Serializers
{
    class PictureSerializer    // don't think I need to implement the serializer interface
    {

        static public byte[] SerializePic(string gameResult)
        {
            //string filePath = "Assets/" + gameResult + ".jpg";
            // Path.Combine constructs the file path in a platform-independent way
            //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", gameResult + ".jpg");
            // uses the current working directory, which should be the project root, so we can just do Assets/filename.jpg
            string filePath = Path.Combine("Assets", gameResult + ".jpg");
            try
            {
                // Read the entire file content into a byte array
                byte[] jpgBytes = File.ReadAllBytes(filePath);

                // You can now use the byte array as needed (e.g., store in a database, send over a network)
                Console.WriteLine($"Image successfully read. Total bytes: {jpgBytes.Length}");
                
                return jpgBytes;
            }
            catch (IOException ex)
            {
                // Handle file-related exceptions
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        static public void DeserializePic(byte[] data, string gameResult)
        {

            string assetsFolder = "Assets";
            string filePath = Path.Combine(assetsFolder, gameResult + ".jpg");

            try
            {
                Directory.CreateDirectory(assetsFolder);

                Console.WriteLine("Current directory: " + Directory.GetCurrentDirectory());
                Console.WriteLine("Writing to: " + Path.GetFullPath(filePath));

                File.WriteAllBytes(filePath, data);
                Console.WriteLine($"{gameResult} picture written successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing file: {ex.Message}");
            }

            //string filePath = "Assets/" + gameResult + ".jpg";
            //try
            //{
            //    File.WriteAllBytes(filePath, data);
            //    Console.WriteLine($"{gameResult} picture written to Assets folder successfully!");
            //}
            //catch (IOException ex)
            //{
            //    Console.WriteLine($"An error occurred while writing the file: {ex.Message}");
            //}
        }
    }
}
