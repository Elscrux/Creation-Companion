using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpListShape Signatire: 0xa1937cbd size: 144 flags: FLAGS_NONE

    // m_childInfo m_class: hkpListShapeChildInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_flags m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_numDisabledChildren m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 66 flags: FLAGS_NONE enum: 
    // m_aabbHalfExtents m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_aabbCenter m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_enabledChildren m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 8 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkpListShape : hkpShapeCollection, IEquatable<hkpListShape?>
    {
        public IList<hkpListShapeChildInfo> m_childInfo { set; get; } = Array.Empty<hkpListShapeChildInfo>();
        public ushort m_flags { set; get; }
        public ushort m_numDisabledChildren { set; get; }
        public Vector4 m_aabbHalfExtents { set; get; }
        public Vector4 m_aabbCenter { set; get; }
        public uint[] m_enabledChildren = new uint[8];

        public override uint Signature { set; get; } = 0xa1937cbd;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_childInfo = des.ReadClassArray<hkpListShapeChildInfo>(br);
            m_flags = br.ReadUInt16();
            m_numDisabledChildren = br.ReadUInt16();
            br.Position += 12;
            m_aabbHalfExtents = br.ReadVector4();
            m_aabbCenter = br.ReadVector4();
            m_enabledChildren = des.ReadUInt32CStyleArray(br, 8);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_childInfo);
            bw.WriteUInt16(m_flags);
            bw.WriteUInt16(m_numDisabledChildren);
            bw.Position += 12;
            bw.WriteVector4(m_aabbHalfExtents);
            bw.WriteVector4(m_aabbCenter);
            s.WriteUInt32CStyleArray(bw, m_enabledChildren);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_childInfo = xd.ReadClassArray<hkpListShapeChildInfo>(xe, nameof(m_childInfo));
            m_flags = xd.ReadUInt16(xe, nameof(m_flags));
            m_numDisabledChildren = xd.ReadUInt16(xe, nameof(m_numDisabledChildren));
            m_aabbHalfExtents = xd.ReadVector4(xe, nameof(m_aabbHalfExtents));
            m_aabbCenter = xd.ReadVector4(xe, nameof(m_aabbCenter));
            m_enabledChildren = xd.ReadUInt32CStyleArray(xe, nameof(m_enabledChildren), 8);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_childInfo), m_childInfo);
            xs.WriteNumber(xe, nameof(m_flags), m_flags);
            xs.WriteNumber(xe, nameof(m_numDisabledChildren), m_numDisabledChildren);
            xs.WriteVector4(xe, nameof(m_aabbHalfExtents), m_aabbHalfExtents);
            xs.WriteVector4(xe, nameof(m_aabbCenter), m_aabbCenter);
            xs.WriteNumberArray(xe, nameof(m_enabledChildren), m_enabledChildren);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpListShape);
        }

        public bool Equals(hkpListShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_childInfo.SequenceEqual(other.m_childInfo) &&
                   m_flags.Equals(other.m_flags) &&
                   m_numDisabledChildren.Equals(other.m_numDisabledChildren) &&
                   m_aabbHalfExtents.Equals(other.m_aabbHalfExtents) &&
                   m_aabbCenter.Equals(other.m_aabbCenter) &&
                   m_enabledChildren.SequenceEqual(other.m_enabledChildren) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_childInfo.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_flags);
            hashcode.Add(m_numDisabledChildren);
            hashcode.Add(m_aabbHalfExtents);
            hashcode.Add(m_aabbCenter);
            hashcode.Add(m_enabledChildren.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

