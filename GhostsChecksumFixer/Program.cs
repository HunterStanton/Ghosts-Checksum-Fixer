﻿using System;
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
            if (args.Length != 2)
            {
                System.Console.WriteLine("GhostsChecksumFixer\nA tool for fixing the checksum in Ghosts savegames, so that they can be modified.\nWorks on PC, Xbox 360, PS3, WiiU, and (theoretically) Xbox One and PS4.\nUsage: GhostsChecksumFixer <savegame.svg filename> <endian (big/little)>\nNote: PS3, Xbox 360, and WiiU are BIG endian. PC, Xbox One, and PS4 are LITTLE endian.");
                return;
            }
            else
            {

                // Storage variable for the calculated checksum
                uint sum = 0;

                System.Console.WriteLine("GhostsChecksumFixer");
                // Try to create a new filestream
                try
                {
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
                    if (args[1] == "big")
                    {
                        byte[] lol = new byte[3];
                        byte[] sumStorage = BitConverter.GetBytes((UInt32)adler32.Value);
                        Array.Reverse(sumStorage);
                        sum = BitConverter.ToUInt32(sumStorage, 0);
                        writer.Write(sum);
                    }
                    else if (args[1] == "little")
                    {
                        sum = (uint)adler32.Value;
                        writer.Write(sum);
                    }
                    else
                    {
                        System.Console.WriteLine("Invalid endian \"" + args[1] + "\" entered.");
                        return;
                    }

                    // Flush and close our reader and writer
                    writer.Flush();
                    writer.Close();
                    reader.Close();

                    // Close the memory stream
                    savegameStream.Close();

                    // Print new checksum and original
                    System.Console.WriteLine("Savegame checksum updated!\nOriginal: " + origChecksum + " (" + origChecksum.ToString("X2") + ")" + "\nNew: " + sum + " (" + sum.ToString("X2") + ")");

                    return;
                }

                // Bit of error handling for when the file is either not there or unaccessible
                catch (FileNotFoundException)
                {
                    System.Console.WriteLine("The savegame you are trying to fix cannot be found.");
                    return;
                }
                catch (UnauthorizedAccessException)
                {
                    System.Console.WriteLine("The savegame you are trying to fix could not be fixed because access was denied.");
                }
            }
        }
    }
}
