using System.Xml.Linq;
namespace HKX2
{
    // BSModifyOnceModifier Signatire: 0x1e20a97a size: 112 flags: FLAGS_NONE

    // m_pOnActivateModifier m_class: hkbModifier Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: ALIGN_16|FLAGS_NONE enum: 
    // m_pOnDeactivateModifier m_class: hkbModifier Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 96 flags: ALIGN_16|FLAGS_NONE enum: 
    public partial class BSModifyOnceModifier : hkbModifier, IEquatable<BSModifyOnceModifier?>
    {
        public hkbModifier? m_pOnActivateModifier { set; get; }
        public hkbModifier? m_pOnDeactivateModifier { set; get; }

        public override uint Signature { set; get; } = 0x1e20a97a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_pOnActivateModifier = des.ReadClassPointer<hkbModifier>(br);
            br.Position += 8;
            m_pOnDeactivateModifier = des.ReadClassPointer<hkbModifier>(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_pOnActivateModifier);
            bw.Position += 8;
            s.WriteClassPointer(bw, m_pOnDeactivateModifier);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_pOnActivateModifier = xd.ReadClassPointer<hkbModifier>(xe, nameof(m_pOnActivateModifier));
            m_pOnDeactivateModifier = xd.ReadClassPointer<hkbModifier>(xe, nameof(m_pOnDeactivateModifier));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_pOnActivateModifier), m_pOnActivateModifier);
            xs.WriteClassPointer(xe, nameof(m_pOnDeactivateModifier), m_pOnDeactivateModifier);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as BSModifyOnceModifier);
        }

        public bool Equals(BSModifyOnceModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_pOnActivateModifier is null && other.m_pOnActivateModifier is null) || (m_pOnActivateModifier is not null && other.m_pOnActivateModifier is not null && m_pOnActivateModifier.Equals((IHavokObject)other.m_pOnActivateModifier))) &&
                   ((m_pOnDeactivateModifier is null && other.m_pOnDeactivateModifier is null) || (m_pOnDeactivateModifier is not null && other.m_pOnDeactivateModifier is not null && m_pOnDeactivateModifier.Equals((IHavokObject)other.m_pOnDeactivateModifier))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_pOnActivateModifier);
            hashcode.Add(m_pOnDeactivateModifier);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

