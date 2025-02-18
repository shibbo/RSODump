using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSODump
{
    /// <summary>
    /// C# BinaryReader is little endian by default
    /// don't expect anything crazy from this reader
    /// </summary>
    public class EndianBinaryReader : BinaryReader
    {
        public EndianBinaryReader(Stream input) : base(input)
        {

        }

        public override uint ReadUInt32()
        {
            uint val = base.ReadUInt32();
            return (val >> 24) | ((val >> 8) & 0x0000FF00) | ((val << 8) & 0x00FF0000) | (val << 24);
        }

        public override ushort ReadUInt16()
        {
            ushort val = base.ReadUInt16();
            return (ushort)((val >> 8) | (val << 8));
        }

        public override int ReadInt32()
        {
            int val = base.ReadInt32();
            return (val >> 24) | ((val >> 8) & 0x0000FF00) | ((val << 8) & 0x00FF0000) | (val << 24);
        }

        public override short ReadInt16()
        {
            short val = base.ReadInt16();
            return (short)((val >> 8) | (val << 8));
        }
    }
}
