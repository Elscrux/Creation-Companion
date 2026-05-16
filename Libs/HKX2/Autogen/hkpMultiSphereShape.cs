using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpMultiSphereShape Signatire: 0x61a590fc size: 176 flags: FLAGS_NONE

    // m_numSpheres m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_spheres m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 8 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpMultiSphereShape : hkpSphereRepShape, IEquatable<hkpMultiSphereShape?>
    {
        public int m_numSpheres { set; get; }
        public Vector4[] m_spheres = new Vector4[8];

        public override uint Signature { set; get; } = 0x61a590fc;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_numSpheres = br.ReadInt32();
            br.Position += 12;
            m_spheres = des.ReadVector4CStyleArray(br, 8);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteInt32(m_numSpheres);
            bw.Position += 12;
            s.WriteVector4CStyleArray(bw, m_spheres);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_numSpheres = xd.ReadInt32(xe, nameof(m_numSpheres));
            m_spheres = xd.ReadVector4CStyleArray(xe, nameof(m_spheres), 8);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_numSpheres), m_numSpheres);
            xs.WriteVector4Array(xe, nameof(m_spheres), m_spheres);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMultiSphereShape);
        }

        public bool Equals(hkpMultiSphereShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_numSpheres.Equals(other.m_numSpheres) &&
                   m_spheres.SequenceEqual(other.m_spheres) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_numSpheres);
            hashcode.Add(m_spheres.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

