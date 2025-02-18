using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gee.External.Capstone;
using Gee.External.Capstone.PowerPc;

namespace RSODump
{
    public class RSO
    {
        enum Segment
        {
            Init = 1,
            Text = 2,
            Ctors = 3,
            Dtors = 4,
            Rodata = 5,
            Data = 6,
            Bss = 7,
            Sdata = 8,
            Sdata2 = 9,
            sdata0 = 10,
            Sbss = 11,
            Sbss2 = 12,
            Sbss0 = 13
        }
        public RSO(EndianBinaryReader file)
        {
            mFile = file;
            /* first 8 bytes are two pointers to other RSOObjectInfo instances */
            mFile.ReadBytes(8);
            mNumSections = mFile.ReadUInt32();
            mSectionInfoOffs = mFile.ReadUInt32();
            mNameOffset = mFile.ReadUInt32();
            mNameSize = mFile.ReadUInt32();
            mVersion = mFile.ReadUInt32();

            mBSS_Size = mFile.ReadUInt32();
            mPrologueSection = mFile.ReadByte();
            mEpilogueSection = mFile.ReadByte();
            mUnresolvedSection = mFile.ReadByte();
            mBSS_Section = mFile.ReadByte();
            mProlog = mFile.ReadUInt32();
            mEpilog = mFile.ReadUInt32();
            mUnresolved = mFile.ReadUInt32();
            mInternalRELOffset = mFile.ReadUInt32();
            mInternalRELSize = mFile.ReadUInt32();
            mExternalRELOffset = mFile.ReadUInt32();
            mExternalRELSize = mFile.ReadUInt32();
            mExportHeader = new(mFile);
            mImportHeader = new(mFile);

            for (uint i = 0; i < mNumSections; i++)
            {
                mSectionInfo.Add((Segment)(i + 1), new(mFile));
            }

            foreach(KeyValuePair<Segment, RSOSectionInfo> kvp in mSectionInfo)
            {
                switch (kvp.Key)
                {
                    case Segment.Text:
                        ParseTextSegment(kvp.Value.GetOffset(), kvp.Value.GetSize());
                        break;
                }
            }
        }

        private void ParseTextSegment(uint offset, uint size)
        {
            mFile.BaseStream.Seek((long)offset, SeekOrigin.Begin);
            byte[] text = mFile.ReadBytes((int)size);

            using (CapstonePowerPcDisassembler dis = CapstoneDisassembler.CreatePowerPcDisassembler(PowerPcDisassembleMode.BigEndian))
            {
                dis.EnableInstructionDetails = true;
                dis.EnableSkipDataMode = true;
                dis.DisassembleSyntax = DisassembleSyntax.Intel;

                PowerPcInstruction[] instrs = dis.Disassemble(text, 0);
                /* do stuff */
            }
        }

        public void PrintInfo()
        {
            Console.WriteLine("================== INFO ==================");
            Console.WriteLine($"Section Count: {mNumSections}");
            Console.WriteLine($"Version: {mVersion}");

            Console.WriteLine("================== SECTIONS ==================");

            foreach (KeyValuePair<Segment, RSOSectionInfo> kvp in mSectionInfo)
            {
                Console.WriteLine($"Segment {kvp.Key}:");
                Console.WriteLine($"Offset: {kvp.Value.GetOffset()}");
                Console.WriteLine($"Size: {kvp.Value.GetSize()}");
                Console.WriteLine("================== END OF SECTION ==================");
            }
        }

        EndianBinaryReader mFile;
        uint mNumSections;
        uint mSectionInfoOffs;
        uint mNameOffset;
        uint mNameSize;
        uint mVersion;

        uint mBSS_Size;
        byte mPrologueSection;
        byte mEpilogueSection;
        byte mUnresolvedSection;
        byte mBSS_Section;
        uint mProlog;
        uint mEpilog;
        uint mUnresolved;
        uint mInternalRELOffset;
        uint mInternalRELSize;
        uint mExternalRELOffset;
        uint mExternalRELSize;
        RSOSymbolHeader mExportHeader;
        RSOSymbolHeader mImportHeader;
        Dictionary<Segment, RSOSectionInfo> mSectionInfo = new();

        Dictionary<Segment, object> mSegments = new();
    }

    public class RSOSymbolHeader
    {
        public RSOSymbolHeader(EndianBinaryReader file)
        {
            mTableOffset = file.ReadUInt32();
            mTableSize = file.ReadUInt32();
            mStringOffset = file.ReadUInt32();
        }

        uint mTableOffset;
        uint mTableSize;
        uint mStringOffset;
    }

    class RSOSectionInfo
    {
        public RSOSectionInfo(EndianBinaryReader file)
        {
            mOffset = file.ReadUInt32();
            mSize = file.ReadUInt32();
        }

        public uint GetOffset()
        {
            return mOffset;
        }

        public uint GetSize()
        {
            return mSize;
        }

        uint mOffset;
        uint mSize;
    }
}
