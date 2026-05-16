using System.Xml.Linq;
namespace HKX2
{
    // hkbMirrorModifier Signatire: 0xa9a271ea size: 88 flags: FLAGS_NONE

    // m_isAdditive m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    public partial class hkbMirrorModifier : hkbModifier, IEquatable<hkbMirrorModifier?>
    {
        public bool m_isAdditive { set; get; }

        public override uint Signature { set; get; } = 0xa9a271ea;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_isAdditive = br.ReadBoolean();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_isAdditive);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_isAdditive = xd.ReadBoolean(xe, nameof(m_isAdditive));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_isAdditive), m_isAdditive);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbMirrorModifier);
        }

        public bool Equals(hkbMirrorModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_isAdditive.Equals(other.m_isAdditive) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_isAdditive);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

