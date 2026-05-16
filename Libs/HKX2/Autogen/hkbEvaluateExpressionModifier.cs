using System.Xml.Linq;
namespace HKX2
{
    // hkbEvaluateExpressionModifier Signatire: 0xf900f6be size: 112 flags: FLAGS_NONE

    // m_expressions m_class: hkbExpressionDataArray Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_compiledExpressionSet m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 88 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_internalExpressionsData m_class:  Type.TYPE_ARRAY Type.TYPE_VOID arrSize: 0 offset: 96 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbEvaluateExpressionModifier : hkbModifier, IEquatable<hkbEvaluateExpressionModifier?>
    {
        public hkbExpressionDataArray? m_expressions { set; get; }
        private object? m_compiledExpressionSet { set; get; }
        public IList<object> m_internalExpressionsData { set; get; } = Array.Empty<object>();

        public override uint Signature { set; get; } = 0xf900f6be;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_expressions = des.ReadClassPointer<hkbExpressionDataArray>(br);
            des.ReadEmptyPointer(br);
            des.ReadEmptyArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_expressions);
            s.WriteVoidPointer(bw);
            s.WriteVoidArray(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_expressions = xd.ReadClassPointer<hkbExpressionDataArray>(xe, nameof(m_expressions));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_expressions), m_expressions);
            xs.WriteSerializeIgnored(xe, nameof(m_compiledExpressionSet));
            xs.WriteSerializeIgnored(xe, nameof(m_internalExpressionsData));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEvaluateExpressionModifier);
        }

        public bool Equals(hkbEvaluateExpressionModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_expressions is null && other.m_expressions is null) || (m_expressions is not null && other.m_expressions is not null && m_expressions.Equals((IHavokObject)other.m_expressions))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_expressions);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

