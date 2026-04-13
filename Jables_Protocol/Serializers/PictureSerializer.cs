using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Diagnostics.CodeAnalysis;

namespace Jables_Protocol.Serializers
{
    /// <summary>
    /// Provides helper methods for serializing and deserializing result images
    /// associated with a game outcome.
    /// </summary>
    /// <remarks>
    /// This class differs from the other serializers in the project because it works with
    /// both binary network data and file system operations. It serializes a game result name
    /// together with the contents of a corresponding image file, and can reconstruct that image
    /// on disk during deserialization.
    /// </remarks>
    /// 

    [ExcludeFromCodeCoverage]
    public class PictureSerializer
    {
        /// <summary>
        /// Serializes a game result identifier and its corresponding image file into a byte array.
        /// </summary>
        /// <param name="gameResult">
        /// The game result name used to identify the image file.
        /// The method expects a file named <c>{gameResult}.jpg</c> inside the <c>Assets</c> folder.
        /// </param>
        /// <returns>
        /// A byte array containing the serialized game result string followed by the raw image bytes.
        /// Returns an empty array if the file cannot be read.
        /// </returns>
        /// <remarks>
        /// The serialized format is written in the following order:
        /// <list type="number">
        /// <item><description>Game result string, written using <see cref="BinaryWriter.Write(string)"/></description></item>
        /// <item><description>Raw JPG image bytes (variable length)</description></item>
        /// </list>
        /// The image file is read from:
        /// <c>AppDomain.CurrentDomain.BaseDirectory/Assets/{gameResult}.jpg</c>.
        /// </remarks>
        public static byte[] SerializePic(string gameResult)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", gameResult + ".jpg");

            try
            {
                using var ms = new MemoryStream();
                using var bw = new BinaryWriter(ms);

                bw.Write(gameResult);

                // Read the entire file content into a byte array
                byte[] jpgBytes = File.ReadAllBytes(filePath);
                bw.Write(jpgBytes);

                Console.WriteLine($"Image successfully read. Total bytes: {jpgBytes.Length}");

                return ms.ToArray();
            }
            catch (IOException ex)
            {
                Console.WriteLine($"An error occurred while reading the file: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        /// <summary>
        /// Deserializes a byte array containing a game result string and image data,
        /// then writes the image file to disk.
        /// </summary>
        /// <param name="data">
        /// The byte array containing serialized picture data.
        /// The data must follow the format produced by <see cref="SerializePic(string)"/>.
        /// </param>
        /// <returns>
        /// The deserialized game result string if the image is successfully written;
        /// otherwise, <c>"NA"</c>.
        /// </returns>
        /// <remarks>
        /// This method reads:
        /// <list type="number">
        /// <item><description>A string written using <see cref="BinaryWriter.Write(string)"/></description></item>
        /// <item><description>All remaining bytes as image data</description></item>
        /// </list>
        /// The image is written to:
        /// <c>Assets/{gameResult}.jpg</c>.
        /// If the <c>Assets</c> directory does not exist, it is created automatically.
        /// </remarks>
        public static string DeserializePic(byte[] data)
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