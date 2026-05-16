using System.Xml.Linq;
namespace HKX2
{
    // hkpSimpleContactConstraintDataInfo Signatire: 0xb59d1734 size: 32 flags: FLAGS_NONE

    // m_flags m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 0 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_index m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_internalData0 m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_rollingFrictionMultiplier m_class:  Type.TYPE_HALF Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_internalData1 m_class:  Type.TYPE_HALF Type.TYPE_VOID arrSize: 0 offset: 10 flags: FLAGS_NONE enum: 
    // m_data m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 5 offset: 12 flags: FLAGS_NONE enum: 
    public partial class hkpSimpleContactConstraintDataInfo : IHavokObject, IEquatable<hkpSimpleContactConstraintDataInfo?>
    {
        public ushort m_flags { set; get; }
        public ushort m_index { set; get; }
        public float m_internalData0 { set; get; }
        public Half m_rollingFrictionMultiplier { set; get; }
        public Half m_internalData1 { set; get; }
        public uint[] m_data = new uint[5];

        public virtual uint Signature { set; get; } = 0xb59d1734;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_flags = br.ReadUInt16();
            m_index = br.ReadUInt16();
            m_internalData0 = br.ReadSingle();
            m_rollingFrictionMultiplier = br.ReadHalf();
            m_internalData1 = br.ReadHalf();
            m_data = des.ReadUInt32CStyleArray(br, 5);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt16(m_flags);
            bw.WriteUInt16(m_index);
            bw.WriteSingle(m_internalData0);
            bw.WriteHalf(m_rollingFrictionMultiplier);
            bw.WriteHalf(m_internalData1);
            s.WriteUInt32CStyleArray(bw, m_data);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_flags = xd.ReadUInt16(xe, nameof(m_flags));
            m_index = xd.ReadUInt16(xe, nameof(m_index));
            m_internalData0 = xd.ReadSingle(xe, nameof(m_internalData0));
            m_rollingFrictionMultiplier = xd.ReadHalf(xe, nameof(m_rollingFrictionMultiplier));
            m_internalData1 = xd.ReadHalf(xe, nameof(m_internalData1));
            m_data = xd.ReadUInt32CStyleArray(xe, nameof(m_data), 5);
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_flags), m_flags);
            xs.WriteNumber(xe, nameof(m_index), m_index);
            xs.WriteFloat(xe, nameof(m_internalData0), m_internalData0);
            xs.WriteFloat(xe, nameof(m_rollingFrictionMultiplier), m_rollingFrictionMultiplier);
            xs.WriteFloat(xe, nameof(m_internalData1), m_internalData1);
            xs.WriteNumberArray(xe, nameof(m_data), m_data);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSimpleContactConstraintDataInfo);
        }

        public bool Equals(hkpSimpleContactConstraintDataInfo? other)
        {
            return other is not null &&
                   m_flags.Equals(other.m_flags) &&
                   m_index.Equals(other.m_index) &&
                   m_internalData0.Equals(other.m_internalData0) &&
                   m_rollingFrictionMultiplier.Equals(other.m_rollingFrictionMultiplier) &&
                   m_internalData1.Equals(other.m_internalData1) &&
                   m_data.SequenceEqual(other.m_data) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_flags);
            hashcode.Add(m_index);
            hashcode.Add(m_internalData0);
            hashcode.Add(m_rollingFrictionMultiplier);
            hashcode.Add(m_internalData1);
            hashcode.Add(m_data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

