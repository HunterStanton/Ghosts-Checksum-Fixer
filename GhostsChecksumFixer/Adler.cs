using System;
// Implements Adler32 checksumming, used in calculation of Ghosts savegame checksum

namespace GhostsChecksumFixer
{
    class Adler
    {
        private static uint MOD_ADLER = 65521;
        private uint Checksum;

        public long Value
        {
            get
            {
                return (long)this.Checksum;
            }
        }

        public Adler()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.Checksum = 1U;
        }

        public void Update(byte[] buff)
        {
            this.Update(buff, buff.Length);
        }

        public void Update(byte[] buff, int length)
        {
            int offset = 0;

            if (buff == null)
            {
                // Buffer should never be null, if is throw error
                throw new ArgumentNullException("buf");
            }

            uint check1 = this.Checksum & (uint)ushort.MaxValue;
            uint check2 = this.Checksum >> 16;
            while (length > 0)
            {
                int check3 = 3800;

                if (check3 > length)
                {
                    check3 = length;
                }

                length -= check3;

                while (--check3 >= 0)
                {
                    check1 += (uint)buff[offset++] & (uint)byte.MaxValue;
                    check2 += check1;
                }

                check1 %= Adler.MOD_ADLER;
                check2 %= Adler.MOD_ADLER;
            }
            this.Checksum = check2 << 16 | check1;
        }
    }
}
