using System.Xml.Linq;
namespace HKX2
{
    // hkpUnaryAction Signatire: 0x895532c0 size: 56 flags: FLAGS_NONE

    // m_entity m_class: hkpEntity Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpUnaryAction : hkpAction, IEquatable<hkpUnaryAction?>
    {
        public hkpEntity? m_entity { set; get; }

        public override uint Signature { set; get; } = 0x895532c0;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_entity = des.ReadClassPointer<hkpEntity>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_entity);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_entity = xd.ReadClassPointer<hkpEntity>(xe, nameof(m_entity));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_entity), m_entity);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpUnaryAction);
        }

        public bool Equals(hkpUnaryAction? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_entity is null && other.m_entity is null) || (m_entity is not null && other.m_entity is not null && m_entity.Equals((IHavokObject)other.m_entity))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_entity);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

