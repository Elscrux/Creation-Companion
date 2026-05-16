using System.Xml.Linq;
namespace HKX2
{
    // hkaSplineCompressedAnimation Signatire: 0x792ee0bb size: 176 flags: FLAGS_NONE

    // m_numFrames m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_numBlocks m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_maxFramesPerBlock m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_maskAndQuantizationSize m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    // m_blockDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_blockInverseDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 76 flags: FLAGS_NONE enum: 
    // m_frameDuration m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_blockOffsets m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_floatBlockOffsets m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_transformOffsets m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    // m_floatOffsets m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 136 flags: FLAGS_NONE enum: 
    // m_data m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    // m_endian m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    public partial class hkaSplineCompressedAnimation : hkaAnimation, IEquatable<hkaSplineCompressedAnimation?>
    {
        public int m_numFrames { set; get; }
        public int m_numBlocks { set; get; }
        public int m_maxFramesPerBlock { set; get; }
        public int m_maskAndQuantizationSize { set; get; }
        public float m_blockDuration { set; get; }
        public float m_blockInverseDuration { set; get; }
        public float m_frameDuration { set; get; }
        public IList<uint> m_blockOffsets { set; get; } = Array.Empty<uint>();
        public IList<uint> m_floatBlockOffsets { set; get; } = Array.Empty<uint>();
        public IList<uint> m_transformOffsets { set; get; } = Array.Empty<uint>();
        public IList<uint> m_floatOffsets { set; get; } = Array.Empty<uint>();
        public IList<byte> m_data { set; get; } = Array.Empty<byte>();
        public int m_endian { set; get; }

        public override uint Signature { set; get; } = 0x792ee0bb;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_numFrames = br.ReadInt32();
            m_numBlocks = br.ReadInt32();
            m_maxFramesPerBlock = br.ReadInt32();
            m_maskAndQuantizationSize = br.ReadInt32();
            m_blockDuration = br.ReadSingle();
            m_blockInverseDuration = br.ReadSingle();
            m_frameDuration = br.ReadSingle();
            br.Position += 4;
            m_blockOffsets = des.ReadUInt32Array(br);
            m_floatBlockOffsets = des.ReadUInt32Array(br);
            m_transformOffsets = des.ReadUInt32Array(br);
            m_floatOffsets = des.ReadUInt32Array(br);
            m_data = des.ReadByteArray(br);
            m_endian = br.ReadInt32();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_numFrames);
            bw.WriteInt32(m_numBlocks);
            bw.WriteInt32(m_maxFramesPerBlock);
            bw.WriteInt32(m_maskAndQuantizationSize);
            bw.WriteSingle(m_blockDuration);
            bw.WriteSingle(m_blockInverseDuration);
            bw.WriteSingle(m_frameDuration);
            bw.Position += 4;
            s.WriteUInt32Array(bw, m_blockOffsets);
            s.WriteUInt32Array(bw, m_floatBlockOffsets);
            s.WriteUInt32Array(bw, m_transformOffsets);
            s.WriteUInt32Array(bw, m_floatOffsets);
            s.WriteByteArray(bw, m_data);
            bw.WriteInt32(m_endian);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_numFrames = xd.ReadInt32(xe, nameof(m_numFrames));
            m_numBlocks = xd.ReadInt32(xe, nameof(m_numBlocks));
            m_maxFramesPerBlock = xd.ReadInt32(xe, nameof(m_maxFramesPerBlock));
            m_maskAndQuantizationSize = xd.ReadInt32(xe, nameof(m_maskAndQuantizationSize));
            m_blockDuration = xd.ReadSingle(xe, nameof(m_blockDuration));
            m_blockInverseDuration = xd.ReadSingle(xe, nameof(m_blockInverseDuration));
            m_frameDuration = xd.ReadSingle(xe, nameof(m_frameDuration));
            m_blockOffsets = xd.ReadUInt32Array(xe, nameof(m_blockOffsets));
            m_floatBlockOffsets = xd.ReadUInt32Array(xe, nameof(m_floatBlockOffsets));
            m_transformOffsets = xd.ReadUInt32Array(xe, nameof(m_transformOffsets));
            m_floatOffsets = xd.ReadUInt32Array(xe, nameof(m_floatOffsets));
            m_data = xd.ReadByteArray(xe, nameof(m_data));
            m_endian = xd.ReadInt32(xe, nameof(m_endian));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_numFrames), m_numFrames);
            xs.WriteNumber(xe, nameof(m_numBlocks), m_numBlocks);
            xs.WriteNumber(xe, nameof(m_maxFramesPerBlock), m_maxFramesPerBlock);
            xs.WriteNumber(xe, nameof(m_maskAndQuantizationSize), m_maskAndQuantizationSize);
            xs.WriteFloat(xe, nameof(m_blockDuration), m_blockDuration);
            xs.WriteFloat(xe, nameof(m_blockInverseDuration), m_blockInverseDuration);
            xs.WriteFloat(xe, nameof(m_frameDuration), m_frameDuration);
            xs.WriteNumberArray(xe, nameof(m_blockOffsets), m_blockOffsets);
            xs.WriteNumberArray(xe, nameof(m_floatBlockOffsets), m_floatBlockOffsets);
            xs.WriteNumberArray(xe, nameof(m_transformOffsets), m_transformOffsets);
            xs.WriteNumberArray(xe, nameof(m_floatOffsets), m_floatOffsets);
            xs.WriteNumberArray(xe, nameof(m_data), m_data);
            xs.WriteNumber(xe, nameof(m_endian), m_endian);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkaSplineCompressedAnimation);
        }

        public bool Equals(hkaSplineCompressedAnimation? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_numFrames.Equals(other.m_numFrames) &&
                   m_numBlocks.Equals(other.m_numBlocks) &&
                   m_maxFramesPerBlock.Equals(other.m_maxFramesPerBlock) &&
                   m_maskAndQuantizationSize.Equals(other.m_maskAndQuantizationSize) &&
                   m_blockDuration.Equals(other.m_blockDuration) &&
                   m_blockInverseDuration.Equals(other.m_blockInverseDuration) &&
                   m_frameDuration.Equals(other.m_frameDuration) &&
                   m_blockOffsets.SequenceEqual(other.m_blockOffsets) &&
                   m_floatBlockOffsets.SequenceEqual(other.m_floatBlockOffsets) &&
                   m_transformOffsets.SequenceEqual(other.m_transformOffsets) &&
                   m_floatOffsets.SequenceEqual(other.m_floatOffsets) &&
                   m_data.SequenceEqual(other.m_data) &&
                   m_endian.Equals(other.m_endian) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_numFrames);
            hashcode.Add(m_numBlocks);
            hashcode.Add(m_maxFramesPerBlock);
            hashcode.Add(m_maskAndQuantizationSize);
            hashcode.Add(m_blockDuration);
            hashcode.Add(m_blockInverseDuration);
            hashcode.Add(m_frameDuration);
            hashcode.Add(m_blockOffsets.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_floatBlockOffsets.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_transformOffsets.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_floatOffsets.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_endian);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

