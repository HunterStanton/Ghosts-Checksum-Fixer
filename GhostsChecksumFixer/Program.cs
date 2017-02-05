using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GhostsChecksumFixer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                System.Console.WriteLine("GhostsChecksumFixer\nA tool for fixing the checksum in Ghosts savegames, so that they can be modified.\nWorks on PC, and would probably work on Xbox One and PS4.\nDoes NOT work on Xbox 360 or PS3 due to differing Endian.\nUsage: GhostsChecksumFixer <savegame.svg filename>");
                return;
            }
            else
            {
                System.Console.WriteLine("GhostsChecksumFixer");
                // Create a new filestream
                FileStream savegameStream = new FileStream(args[0], FileMode.Open, FileAccess.ReadWrite);

                // Create our binary reader/writer
                BinaryReader reader = new BinaryReader((Stream)savegameStream);
                BinaryWriter writer = new BinaryWriter((Stream)savegameStream);

                // Get the original checksum from the file and store it
                reader.BaseStream.Position = 0x8;
                uint origChecksum = reader.ReadUInt32();

                // Put the entire savegame after 0x500 (which is the data that is checksummed by the game) into a buffer
                reader.BaseStream.Position = 0x500;
                byte[] buffer = reader.ReadBytes((int)reader.BaseStream.Length - 0x500);
                
                // Calculate adler32 checksum of buffer
                Adler adler32 = new Adler();
                adler32.Update(buffer);

                // Overwrite the adler32 sum that is stored in the savegame
                writer.BaseStream.Position = 0x8;
                writer.Write(adler32.Value);

                // Flush and close our reader and writer
                writer.Flush();
                writer.Close();
                reader.Close();

                // Close the memory stream
                savegameStream.Close();

                // Print new checksum and original
                System.Console.WriteLine("Savegame checksum updated!\nOriginal: " + origChecksum + " (" + origChecksum.ToString("X2")+")" + "\nNew: " + adler32.Value + " (" + adler32.Value.ToString("X2") + ")");

                return;
            }
        }
    }
}
