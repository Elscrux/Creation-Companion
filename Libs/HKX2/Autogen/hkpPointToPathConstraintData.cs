using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpPointToPathConstraintData Signatire: 0x8e7cb5da size: 192 flags: FLAGS_NONE

    // m_atoms m_class: hkpBridgeAtoms Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_path m_class: hkpParametricCurve Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_maxFrictionForce m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_angularConstrainedDOF m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 60 flags: FLAGS_NONE enum: OrientationConstraintType
    // m_transform_OS_KS m_class:  Type.TYPE_TRANSFORM Type.TYPE_VOID arrSize: 2 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpPointToPathConstraintData : hkpConstraintData, IEquatable<hkpPointToPathConstraintData?>
    {
        public hkpBridgeAtoms m_atoms { set; get; } = new();
        public hkpParametricCurve? m_path { set; get; }
        public float m_maxFrictionForce { set; get; }
        public sbyte m_angularConstrainedDOF { set; get; }
        public Matrix4x4[] m_transform_OS_KS = new Matrix4x4[2];

        public override uint Signature { set; get; } = 0x8e7cb5da;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_atoms.Read(des, br);
            m_path = des.ReadClassPointer<hkpParametricCurve>(br);
            m_maxFrictionForce = br.ReadSingle();
            m_angularConstrainedDOF = br.ReadSByte();
            br.Position += 3;
            m_transform_OS_KS = des.ReadTransformCStyleArray(br, 2);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_atoms.Write(s, bw);
            s.WriteClassPointer(bw, m_path);
            bw.WriteSingle(m_maxFrictionForce);
            bw.WriteSByte(m_angularConstrainedDOF);
            bw.Position += 3;
            s.WriteTransformCStyleArray(bw, m_transform_OS_KS);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_atoms = xd.ReadClass<hkpBridgeAtoms>(xe, nameof(m_atoms));
            m_path = xd.ReadClassPointer<hkpParametricCurve>(xe, nameof(m_path));
            m_maxFrictionForce = xd.ReadSingle(xe, nameof(m_maxFrictionForce));
            m_angularConstrainedDOF = xd.ReadFlag<OrientationConstraintType, sbyte>(xe, nameof(m_angularConstrainedDOF));
            m_transform_OS_KS = xd.ReadTransformCStyleArray(xe, nameof(m_transform_OS_KS), 2);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkpBridgeAtoms>(xe, nameof(m_atoms), m_atoms);
            xs.WriteClassPointer(xe, nameof(m_path), m_path);
            xs.WriteFloat(xe, nameof(m_maxFrictionForce), m_maxFrictionForce);
            xs.WriteEnum<OrientationConstraintType, sbyte>(xe, nameof(m_angularConstrainedDOF), m_angularConstrainedDOF);
            xs.WriteTransformArray(xe, nameof(m_transform_OS_KS), m_transform_OS_KS);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpPointToPathConstraintData);
        }

        public bool Equals(hkpPointToPathConstraintData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_atoms is null && other.m_atoms is null) || (m_atoms is not null && other.m_atoms is not null && m_atoms.Equals((IHavokObject)other.m_atoms))) &&
                   ((m_path is null && other.m_path is null) || (m_path is not null && other.m_path is not null && m_path.Equals((IHavokObject)other.m_path))) &&
                   m_maxFrictionForce.Equals(other.m_maxFrictionForce) &&
                   m_angularConstrainedDOF.Equals(other.m_angularConstrainedDOF) &&
                   m_transform_OS_KS.SequenceEqual(other.m_transform_OS_KS) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_atoms);
            hashcode.Add(m_path);
            hashcode.Add(m_maxFrictionForce);
            hashcode.Add(m_angularConstrainedDOF);
            hashcode.Add(m_transform_OS_KS.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

