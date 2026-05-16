using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbComputeRotationFromAxisAngleModifier Signatire: 0x9b3f6936 size: 128 flags: FLAGS_NONE

    // m_rotationOut m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_axis m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_angleDegrees m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkbComputeRotationFromAxisAngleModifier : hkbModifier, IEquatable<hkbComputeRotationFromAxisAngleModifier?>
    {
        public Quaternion m_rotationOut { set; get; }
        public Vector4 m_axis { set; get; }
        public float m_angleDegrees { set; get; }

        public override uint Signature { set; get; } = 0x9b3f6936;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rotationOut = des.ReadQuaternion(br);
            m_axis = br.ReadVector4();
            m_angleDegrees = br.ReadSingle();
            br.Position += 12;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteQuaternion(bw, m_rotationOut);
            bw.WriteVector4(m_axis);
            bw.WriteSingle(m_angleDegrees);
            bw.Position += 12;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rotationOut = xd.ReadQuaternion(xe, nameof(m_rotationOut));
            m_axis = xd.ReadVector4(xe, nameof(m_axis));
            m_angleDegrees = xd.ReadSingle(xe, nameof(m_angleDegrees));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteQuaternion(xe, nameof(m_rotationOut), m_rotationOut);
            xs.WriteVector4(xe, nameof(m_axis), m_axis);
            xs.WriteFloat(xe, nameof(m_angleDegrees), m_angleDegrees);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbComputeRotationFromAxisAngleModifier);
        }

        public bool Equals(hkbComputeRotationFromAxisAngleModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_rotationOut.Equals(other.m_rotationOut) &&
                   m_axis.Equals(other.m_axis) &&
                   m_angleDegrees.Equals(other.m_angleDegrees) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rotationOut);
            hashcode.Add(m_axis);
            hashcode.Add(m_angleDegrees);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

