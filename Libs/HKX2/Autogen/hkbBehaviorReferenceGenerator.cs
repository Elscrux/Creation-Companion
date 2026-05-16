using System.Xml.Linq;
namespace HKX2
{
    // hkbBehaviorReferenceGenerator Signatire: 0xfcb5423 size: 88 flags: FLAGS_NONE

    // m_behaviorName m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_behavior m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 80 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbBehaviorReferenceGenerator : hkbGenerator, IEquatable<hkbBehaviorReferenceGenerator?>
    {
        public string m_behaviorName { set; get; } = "";
        private object? m_behavior { set; get; }

        public override uint Signature { set; get; } = 0xfcb5423;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_behaviorName = des.ReadStringPointer(br);
            des.ReadEmptyPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_behaviorName);
            s.WriteVoidPointer(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_behaviorName = xd.ReadString(xe, nameof(m_behaviorName));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_behaviorName), m_behaviorName);
            xs.WriteSerializeIgnored(xe, nameof(m_behavior));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBehaviorReferenceGenerator);
        }

        public bool Equals(hkbBehaviorReferenceGenerator? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   (m_behaviorName is null && other.m_behaviorName is null || m_behaviorName == other.m_behaviorName || m_behaviorName is null && other.m_behaviorName == "" || m_behaviorName == "" && other.m_behaviorName is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_behaviorName);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

