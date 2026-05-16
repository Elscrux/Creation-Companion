using System.Xml.Linq;
namespace HKX2
{
    // hkbBehaviorGraphStringData Signatire: 0xc713064e size: 80 flags: FLAGS_NONE

    // m_eventNames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_attributeNames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_variableNames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_characterPropertyNames m_class:  Type.TYPE_ARRAY Type.TYPE_STRINGPTR arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkbBehaviorGraphStringData : hkReferencedObject, IEquatable<hkbBehaviorGraphStringData?>
    {
        public IList<string> m_eventNames { set; get; } = Array.Empty<string>();
        public IList<string> m_attributeNames { set; get; } = Array.Empty<string>();
        public IList<string> m_variableNames { set; get; } = Array.Empty<string>();
        public IList<string> m_characterPropertyNames { set; get; } = Array.Empty<string>();

        public override uint Signature { set; get; } = 0xc713064e;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_eventNames = des.ReadStringPointerArray(br);
            m_attributeNames = des.ReadStringPointerArray(br);
            m_variableNames = des.ReadStringPointerArray(br);
            m_characterPropertyNames = des.ReadStringPointerArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointerArray(bw, m_eventNames);
            s.WriteStringPointerArray(bw, m_attributeNames);
            s.WriteStringPointerArray(bw, m_variableNames);
            s.WriteStringPointerArray(bw, m_characterPropertyNames);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_eventNames = xd.ReadStringArray(xe, nameof(m_eventNames));
            m_attributeNames = xd.ReadStringArray(xe, nameof(m_attributeNames));
            m_variableNames = xd.ReadStringArray(xe, nameof(m_variableNames));
            m_characterPropertyNames = xd.ReadStringArray(xe, nameof(m_characterPropertyNames));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteStringArray(xe, nameof(m_eventNames), m_eventNames);
            xs.WriteStringArray(xe, nameof(m_attributeNames), m_attributeNames);
            xs.WriteStringArray(xe, nameof(m_variableNames), m_variableNames);
            xs.WriteStringArray(xe, nameof(m_characterPropertyNames), m_characterPropertyNames);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBehaviorGraphStringData);
        }

        public bool Equals(hkbBehaviorGraphStringData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_eventNames.SequenceEqual(other.m_eventNames) &&
                   m_attributeNames.SequenceEqual(other.m_attributeNames) &&
                   m_variableNames.SequenceEqual(other.m_variableNames) &&
                   m_characterPropertyNames.SequenceEqual(other.m_characterPropertyNames) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_eventNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_attributeNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_variableNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_characterPropertyNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

