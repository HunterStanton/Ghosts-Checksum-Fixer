# Ghosts Checksum Fixer
A command-line checksum fixer for Call of Duty: Ghosts Savegame.svg files. Works on PC, Xbox 360, PS3, WiiU, and (theoretically) Xbox One and PS4.

Please be aware that this does not work for Call of Duty: Modern Warfare 2 or Call of Duty: Modern Warfare 3. The save header is a slightly different size. I will be making a separate tool for those games. 

I have no idea about other Call of Duty games (such as Modern Warfare Remastered, Advanced Warfare, or Black Ops 1/2). Call of Duty 4 and 5 do not use checksummed saves.

## Usage
GhostsChecksumFixer.exe savegame.svg endian (big/little)

PS3, Xbox 360, and WiiU use BIG endian. PC, PS4, and XBox One use LITTLE endian.

After fixing, both the original and new checksums will be displayed for your reference.
