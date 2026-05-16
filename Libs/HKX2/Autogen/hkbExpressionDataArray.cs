using System.Xml.Linq;
namespace HKX2
{
    // hkbExpressionDataArray Signatire: 0x4b9ee1a2 size: 32 flags: FLAGS_NONE

    // m_expressionsData m_class: hkbExpressionData Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbExpressionDataArray : hkReferencedObject, IEquatable<hkbExpressionDataArray?>
    {
        public IList<hkbExpressionData> m_expressionsData { set; get; } = Array.Empty<hkbExpressionData>();

        public override uint Signature { set; get; } = 0x4b9ee1a2;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_expressionsData = des.ReadClassArray<hkbExpressionData>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_expressionsData);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_expressionsData = xd.ReadClassArray<hkbExpressionData>(xe, nameof(m_expressionsData));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_expressionsData), m_expressionsData);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbExpressionDataArray);
        }

        public bool Equals(hkbExpressionDataArray? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_expressionsData.SequenceEqual(other.m_expressionsData) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_expressionsData.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

