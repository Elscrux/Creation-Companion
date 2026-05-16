using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbMirroredSkeletonInfo Signatire: 0xc6c2da4f size: 48 flags: FLAGS_NONE

    // m_mirrorAxis m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_bonePairMap m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkbMirroredSkeletonInfo : hkReferencedObject, IEquatable<hkbMirroredSkeletonInfo?>
    {
        public Vector4 m_mirrorAxis { set; get; }
        public IList<short> m_bonePairMap { set; get; } = Array.Empty<short>();

        public override uint Signature { set; get; } = 0xc6c2da4f;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_mirrorAxis = br.ReadVector4();
            m_bonePairMap = des.ReadInt16Array(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_mirrorAxis);
            s.WriteInt16Array(bw, m_bonePairMap);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_mirrorAxis = xd.ReadVector4(xe, nameof(m_mirrorAxis));
            m_bonePairMap = xd.ReadInt16Array(xe, nameof(m_bonePairMap));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_mirrorAxis), m_mirrorAxis);
            xs.WriteNumberArray(xe, nameof(m_bonePairMap), m_bonePairMap);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbMirroredSkeletonInfo);
        }

        public bool Equals(hkbMirroredSkeletonInfo? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_mirrorAxis.Equals(other.m_mirrorAxis) &&
                   m_bonePairMap.SequenceEqual(other.m_bonePairMap) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_mirrorAxis);
            hashcode.Add(m_bonePairMap.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

