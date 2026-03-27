using System;
using System.Collections.Generic;
using System.Text;

namespace Jables_Protocol.Serializers
{
    class PictureSerializer    // don't think I need to implement the serializer interface
    {

        static public byte[] SerializePic(string gameResult)
        {
            string filePath = "Assets/" + gameResult + ".jpg";
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
            string filePath = "Assets/" + gameResult + ".jpg";
            try
            {
                File.WriteAllBytes(filePath, data);
                Console.WriteLine($"{gameResult} picture written to Assets folder successfully!");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An error occurred while writing the file: {ex.Message}");
            }
        }
    }
}
