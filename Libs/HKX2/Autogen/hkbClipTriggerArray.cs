using System.Xml.Linq;
namespace HKX2
{
    // hkbClipTriggerArray Signatire: 0x59c23a0f size: 32 flags: FLAGS_NONE

    // m_triggers m_class: hkbClipTrigger Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbClipTriggerArray : hkReferencedObject, IEquatable<hkbClipTriggerArray?>
    {
        public IList<hkbClipTrigger> m_triggers { set; get; } = Array.Empty<hkbClipTrigger>();

        public override uint Signature { set; get; } = 0x59c23a0f;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_triggers = des.ReadClassArray<hkbClipTrigger>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_triggers);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_triggers = xd.ReadClassArray<hkbClipTrigger>(xe, nameof(m_triggers));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_triggers), m_triggers);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbClipTriggerArray);
        }

        public bool Equals(hkbClipTriggerArray? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_triggers.SequenceEqual(other.m_triggers) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_triggers.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

