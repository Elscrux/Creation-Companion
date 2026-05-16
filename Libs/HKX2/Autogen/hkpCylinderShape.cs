using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpCylinderShape Signatire: 0x3e463c3a size: 112 flags: FLAGS_NONE

    // m_cylRadius m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    // m_cylBaseRadiusFactorForHeightFieldCollisions m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 44 flags: FLAGS_NONE enum: 
    // m_vertexA m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_vertexB m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_perpendicular1 m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_perpendicular2 m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    public partial class hkpCylinderShape : hkpConvexShape, IEquatable<hkpCylinderShape?>
    {
        public float m_cylRadius { set; get; }
        public float m_cylBaseRadiusFactorForHeightFieldCollisions { set; get; }
        public Vector4 m_vertexA { set; get; }
        public Vector4 m_vertexB { set; get; }
        public Vector4 m_perpendicular1 { set; get; }
        public Vector4 m_perpendicular2 { set; get; }

        public override uint Signature { set; get; } = 0x3e463c3a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_cylRadius = br.ReadSingle();
            m_cylBaseRadiusFactorForHeightFieldCollisions = br.ReadSingle();
            m_vertexA = br.ReadVector4();
            m_vertexB = br.ReadVector4();
            m_perpendicular1 = br.ReadVector4();
            m_perpendicular2 = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_cylRadius);
            bw.WriteSingle(m_cylBaseRadiusFactorForHeightFieldCollisions);
            bw.WriteVector4(m_vertexA);
            bw.WriteVector4(m_vertexB);
            bw.WriteVector4(m_perpendicular1);
            bw.WriteVector4(m_perpendicular2);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_cylRadius = xd.ReadSingle(xe, nameof(m_cylRadius));
            m_cylBaseRadiusFactorForHeightFieldCollisions = xd.ReadSingle(xe, nameof(m_cylBaseRadiusFactorForHeightFieldCollisions));
            m_vertexA = xd.ReadVector4(xe, nameof(m_vertexA));
            m_vertexB = xd.ReadVector4(xe, nameof(m_vertexB));
            m_perpendicular1 = xd.ReadVector4(xe, nameof(m_perpendicular1));
            m_perpendicular2 = xd.ReadVector4(xe, nameof(m_perpendicular2));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_cylRadius), m_cylRadius);
            xs.WriteFloat(xe, nameof(m_cylBaseRadiusFactorForHeightFieldCollisions), m_cylBaseRadiusFactorForHeightFieldCollisions);
            xs.WriteVector4(xe, nameof(m_vertexA), m_vertexA);
            xs.WriteVector4(xe, nameof(m_vertexB), m_vertexB);
            xs.WriteVector4(xe, nameof(m_perpendicular1), m_perpendicular1);
            xs.WriteVector4(xe, nameof(m_perpendicular2), m_perpendicular2);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpCylinderShape);
        }

        public bool Equals(hkpCylinderShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_cylRadius.Equals(other.m_cylRadius) &&
                   m_cylBaseRadiusFactorForHeightFieldCollisions.Equals(other.m_cylBaseRadiusFactorForHeightFieldCollisions) &&
                   m_vertexA.Equals(other.m_vertexA) &&
                   m_vertexB.Equals(other.m_vertexB) &&
                   m_perpendicular1.Equals(other.m_perpendicular1) &&
                   m_perpendicular2.Equals(other.m_perpendicular2) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_cylRadius);
            hashcode.Add(m_cylBaseRadiusFactorForHeightFieldCollisions);
            hashcode.Add(m_vertexA);
            hashcode.Add(m_vertexB);
            hashcode.Add(m_perpendicular1);
            hashcode.Add(m_perpendicular2);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

