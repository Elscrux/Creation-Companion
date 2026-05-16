using System.Xml.Linq;
namespace HKX2
{
    // hkpMoppCode Signatire: 0x924c2661 size: 64 flags: FLAGS_NONE

    // m_info m_class: hkpMoppCodeCodeInfo Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_data m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_buildType m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: BuildType
    public partial class hkpMoppCode : hkReferencedObject, IEquatable<hkpMoppCode?>
    {
        public hkpMoppCodeCodeInfo m_info { set; get; } = new();
        public IList<byte> m_data { set; get; } = Array.Empty<byte>();
        public sbyte m_buildType { set; get; }

        public override uint Signature { set; get; } = 0x924c2661;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_info.Read(des, br);
            m_data = des.ReadByteArray(br);
            m_buildType = br.ReadSByte();
            br.Position += 15;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_info.Write(s, bw);
            s.WriteByteArray(bw, m_data);
            bw.WriteSByte(m_buildType);
            bw.Position += 15;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_info = xd.ReadClass<hkpMoppCodeCodeInfo>(xe, nameof(m_info));
            m_data = xd.ReadByteArray(xe, nameof(m_data));
            m_buildType = xd.ReadFlag<BuildType, sbyte>(xe, nameof(m_buildType));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkpMoppCodeCodeInfo>(xe, nameof(m_info), m_info);
            xs.WriteNumberArray(xe, nameof(m_data), m_data);
            xs.WriteEnum<BuildType, sbyte>(xe, nameof(m_buildType), m_buildType);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMoppCode);
        }

        public bool Equals(hkpMoppCode? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_info is null && other.m_info is null) || (m_info is not null && other.m_info is not null && m_info.Equals((IHavokObject)other.m_info))) &&
                   m_data.SequenceEqual(other.m_data) &&
                   m_buildType.Equals(other.m_buildType) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_info);
            hashcode.Add(m_data.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_buildType);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

