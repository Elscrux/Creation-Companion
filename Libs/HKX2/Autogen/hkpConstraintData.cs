using System.Xml.Linq;
namespace HKX2
{
    // hkpConstraintData Signatire: 0x80559a4e size: 24 flags: FLAGS_NONE

    // m_userData m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpConstraintData : hkReferencedObject, IEquatable<hkpConstraintData?>
    {
        public ulong m_userData { set; get; }

        public override uint Signature { set; get; } = 0x80559a4e;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_userData = br.ReadUInt64();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(m_userData);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_userData = xd.ReadUInt64(xe, nameof(m_userData));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_userData), m_userData);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConstraintData);
        }

        public bool Equals(hkpConstraintData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_userData.Equals(other.m_userData) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_userData);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

