using System.Xml.Linq;
namespace HKX2
{
    // hkbExpressionCondition Signatire: 0x1c3c1045 size: 32 flags: FLAGS_NONE

    // m_expression m_class:  Type.TYPE_STRINGPTR Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_compiledExpressionSet m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 24 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbExpressionCondition : hkbCondition, IEquatable<hkbExpressionCondition?>
    {
        public string m_expression { set; get; } = "";
        private object? m_compiledExpressionSet { set; get; }

        public override uint Signature { set; get; } = 0x1c3c1045;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_expression = des.ReadStringPointer(br);
            des.ReadEmptyPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteStringPointer(bw, m_expression);
            s.WriteVoidPointer(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_expression = xd.ReadString(xe, nameof(m_expression));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteString(xe, nameof(m_expression), m_expression);
            xs.WriteSerializeIgnored(xe, nameof(m_compiledExpressionSet));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbExpressionCondition);
        }

        public bool Equals(hkbExpressionCondition? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   (m_expression is null && other.m_expression is null || m_expression == other.m_expression || m_expression is null && other.m_expression == "" || m_expression == "" && other.m_expression is null) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_expression);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

