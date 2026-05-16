using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkMoppBvTreeShapeBase Signatire: 0x7c338c66 size: 80 flags: FLAGS_NONE

    // m_code m_class: hkpMoppCode Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_moppData m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 48 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_moppDataSize m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 56 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_codeInfoCopy m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkMoppBvTreeShapeBase : hkpBvTreeShape, IEquatable<hkMoppBvTreeShapeBase?>
    {
        public hkpMoppCode? m_code { set; get; }
        private object? m_moppData { set; get; }
        private uint m_moppDataSize { set; get; }
        private Vector4 m_codeInfoCopy { set; get; }

        public override uint Signature { set; get; } = 0x7c338c66;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_code = des.ReadClassPointer<hkpMoppCode>(br);
            des.ReadEmptyPointer(br);
            m_moppDataSize = br.ReadUInt32();
            br.Position += 4;
            m_codeInfoCopy = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointer(bw, m_code);
            s.WriteVoidPointer(bw);
            bw.WriteUInt32(m_moppDataSize);
            bw.Position += 4;
            bw.WriteVector4(m_codeInfoCopy);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_code = xd.ReadClassPointer<hkpMoppCode>(xe, nameof(m_code));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointer(xe, nameof(m_code), m_code);
            xs.WriteSerializeIgnored(xe, nameof(m_moppData));
            xs.WriteSerializeIgnored(xe, nameof(m_moppDataSize));
            xs.WriteSerializeIgnored(xe, nameof(m_codeInfoCopy));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkMoppBvTreeShapeBase);
        }

        public bool Equals(hkMoppBvTreeShapeBase? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_code is null && other.m_code is null) || (m_code is not null && other.m_code is not null && m_code.Equals((IHavokObject)other.m_code))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_code);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

