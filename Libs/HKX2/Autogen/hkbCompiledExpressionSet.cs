using System.Xml.Linq;
namespace HKX2
{
    // hkbCompiledExpressionSet Signatire: 0x3a7d76cc size: 56 flags: FLAGS_NONE

    // m_rpn m_class: hkbCompiledExpressionSetToken Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_expressionToRpnIndex m_class:  Type.TYPE_ARRAY Type.TYPE_INT32 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_numExpressions m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkbCompiledExpressionSet : hkReferencedObject, IEquatable<hkbCompiledExpressionSet?>
    {
        public IList<hkbCompiledExpressionSetToken> m_rpn { set; get; } = Array.Empty<hkbCompiledExpressionSetToken>();
        public IList<int> m_expressionToRpnIndex { set; get; } = Array.Empty<int>();
        public sbyte m_numExpressions { set; get; }

        public override uint Signature { set; get; } = 0x3a7d76cc;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rpn = des.ReadClassArray<hkbCompiledExpressionSetToken>(br);
            m_expressionToRpnIndex = des.ReadInt32Array(br);
            m_numExpressions = br.ReadSByte();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_rpn);
            s.WriteInt32Array(bw, m_expressionToRpnIndex);
            bw.WriteSByte(m_numExpressions);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rpn = xd.ReadClassArray<hkbCompiledExpressionSetToken>(xe, nameof(m_rpn));
            m_expressionToRpnIndex = xd.ReadInt32Array(xe, nameof(m_expressionToRpnIndex));
            m_numExpressions = xd.ReadSByte(xe, nameof(m_numExpressions));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_rpn), m_rpn);
            xs.WriteNumberArray(xe, nameof(m_expressionToRpnIndex), m_expressionToRpnIndex);
            xs.WriteNumber(xe, nameof(m_numExpressions), m_numExpressions);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbCompiledExpressionSet);
        }

        public bool Equals(hkbCompiledExpressionSet? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_rpn.SequenceEqual(other.m_rpn) &&
                   m_expressionToRpnIndex.SequenceEqual(other.m_expressionToRpnIndex) &&
                   m_numExpressions.Equals(other.m_numExpressions) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rpn.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_expressionToRpnIndex.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_numExpressions);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

