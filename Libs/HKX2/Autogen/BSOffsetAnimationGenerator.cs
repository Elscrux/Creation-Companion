using System.Xml.Linq;
namespace HKX2
{
    // BSOffsetAnimationGenerator Signatire: 0xb8571122 size: 176 flags: FLAGS_NONE

    // m_pDefaultGenerator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_pOffsetClipGenerator m_class: hkbGenerator Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 96 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_fOffsetVariable m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_fOffsetRangeStart m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 108 flags: FLAGS_NONE enum: 
    // m_fOffsetRangeEnd m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_BoneOffsetA m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 120 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_BoneIndexA m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 136 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_fCurrentPercentage m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 152 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_iCurrentFrame m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 156 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_bZeroOffset m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 160 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_bOffsetValid m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 161 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class BSOffsetAnimationGenerator : hkbGenerator, IEquatable<BSOffsetAnimationGenerator?>
    {
        public hkbGenerator? m_pDefaultGenerator { set; get; }
        public hkbGenerator? m_pOffsetClipGenerator { set; get; }
        public float m_fOffsetVariable { set; get; }
        public float m_fOffsetRangeStart { set; get; }
        public float m_fOffsetRangeEnd { set; get; }
        public IList<object> m_BoneOffsetA { set; get; } = Array.Empty<object>();
        public IList<object> m_BoneIndexA { set; get; } = Array.Empty<object>();
        private float m_fCurrentPercentage { set; get; }
        private uint m_iCurrentFrame { set; get; }
        private bool m_bZeroOffset { set; get; }
        private bool m_bOffsetValid { set; get; }

        public override uint Signature { set; get; } = 0xb8571122;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_pDefaultGenerator = des.ReadClassPointer<hkbGenerator>(br);
            br.Position += 8;
            m_pOffsetClipGenerator = des.ReadClassPointer<hkbGenerator>(br);
            m_fOffsetVariable = br.ReadSingle();
            m_fOffsetRangeStart = br.ReadSingle();
            m_fOffsetRangeEnd = br.ReadSingle();
            br.Position += 4;
            des.ReadEmptyArray(br);
            des.ReadEmptyArray(br);
            m_fCurrentPercentage = br.ReadSingle();
            m_iCurrentFrame = br.ReadUInt32();
            m_bZeroOffset = br.ReadBoolean();
            m_bOffsetValid = br.ReadBoolean();
            br.Position += 14;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            s.WriteClassPointer(bw, m_pDefaultGenerator);
            bw.Position += 8;
            s.WriteClassPointer(bw, m_pOffsetClipGenerator);
            bw.WriteSingle(m_fOffsetVariable);
            bw.WriteSingle(m_fOffsetRangeStart);
            bw.WriteSingle(m_fOffsetRangeEnd);
            bw.Position += 4;
            s.WriteVoidArray(bw);
            s.WriteVoidArray(bw);
            bw.WriteSingle(m_fCurrentPercentage);
            bw.WriteUInt32(m_iCurrentFrame);
            bw.WriteBoolean(m_bZeroOffset);
            bw.WriteBoolean(m_bOffsetValid);
            bw.Position += 14;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_pDefaultGenerator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_pDefaultGenerator));
            m_pOffsetClipGenerator = xd.ReadClassPointer<hkbGenerator>(xe, nameof(m_pOffsetClipGenerator));
            m_fOffsetVariable = xd.ReadSingle(xe, nameof(m_fOffsetVariable));
            m_fOffsetRangeStart = xd.ReadSingle(xe, nameof(m_fOffsetRangeStart));
            m_fOffsetRangeEnd = xd.ReadSingle(xe, nameof(m_fOffsetRangeEnd));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_pDefaultGenerator), m_pDefaultGenerator);
            xs.WriteClassPointer(xe, nameof(m_pOffsetClipGenerator), m_pOffsetClipGenerator);
            xs.WriteFloat(xe, nameof(m_fOffsetVariable), m_fOffsetVariable);
            xs.WriteFloat(xe, nameof(m_fOffsetRangeStart), m_fOffsetRangeStart);
            xs.WriteFloat(xe, nameof(m_fOffsetRangeEnd), m_fOffsetRangeEnd);
            xs.WriteSerializeIgnored(xe, nameof(m_BoneOffsetA));
            xs.WriteSerializeIgnored(xe, nameof(m_BoneIndexA));
            xs.WriteSerializeIgnored(xe, nameof(m_fCurrentPercentage));
            xs.WriteSerializeIgnored(xe, nameof(m_iCurrentFrame));
            xs.WriteSerializeIgnored(xe, nameof(m_bZeroOffset));
            xs.WriteSerializeIgnored(xe, nameof(m_bOffsetValid));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSOffsetAnimationGenerator);
        }

        public bool Equals(BSOffsetAnimationGenerator? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_pDefaultGenerator is null && other.m_pDefaultGenerator is null) || (m_pDefaultGenerator is not null && other.m_pDefaultGenerator is not null && m_pDefaultGenerator.Equals((IHavokObject)other.m_pDefaultGenerator))) &&
                   ((m_pOffsetClipGenerator is null && other.m_pOffsetClipGenerator is null) || (m_pOffsetClipGenerator is not null && other.m_pOffsetClipGenerator is not null && m_pOffsetClipGenerator.Equals((IHavokObject)other.m_pOffsetClipGenerator))) &&
                   m_fOffsetVariable.Equals(other.m_fOffsetVariable) &&
                   m_fOffsetRangeStart.Equals(other.m_fOffsetRangeStart) &&
                   m_fOffsetRangeEnd.Equals(other.m_fOffsetRangeEnd) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_pDefaultGenerator);
            hashcode.Add(m_pOffsetClipGenerator);
            hashcode.Add(m_fOffsetVariable);
            hashcode.Add(m_fOffsetRangeStart);
            hashcode.Add(m_fOffsetRangeEnd);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

