using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpReorientAction Signatire: 0x2dc0ec6a size: 112 flags: FLAGS_NONE

    // m_rotationAxis m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_upAxis m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_strength m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_damping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    public partial class hkpReorientAction : hkpUnaryAction, IEquatable<hkpReorientAction?>
    {
        public Vector4 m_rotationAxis { set; get; }
        public Vector4 m_upAxis { set; get; }
        public float m_strength { set; get; }
        public float m_damping { set; get; }

        public override uint Signature { set; get; } = 0x2dc0ec6a;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_rotationAxis = br.ReadVector4();
            m_upAxis = br.ReadVector4();
            m_strength = br.ReadSingle();
            m_damping = br.ReadSingle();
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            bw.WriteVector4(m_rotationAxis);
            bw.WriteVector4(m_upAxis);
            bw.WriteSingle(m_strength);
            bw.WriteSingle(m_damping);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rotationAxis = xd.ReadVector4(xe, nameof(m_rotationAxis));
            m_upAxis = xd.ReadVector4(xe, nameof(m_upAxis));
            m_strength = xd.ReadSingle(xe, nameof(m_strength));
            m_damping = xd.ReadSingle(xe, nameof(m_damping));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_rotationAxis), m_rotationAxis);
            xs.WriteVector4(xe, nameof(m_upAxis), m_upAxis);
            xs.WriteFloat(xe, nameof(m_strength), m_strength);
            xs.WriteFloat(xe, nameof(m_damping), m_damping);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpReorientAction);
        }

        public bool Equals(hkpReorientAction? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_rotationAxis.Equals(other.m_rotationAxis) &&
                   m_upAxis.Equals(other.m_upAxis) &&
                   m_strength.Equals(other.m_strength) &&
                   m_damping.Equals(other.m_damping) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rotationAxis);
            hashcode.Add(m_upAxis);
            hashcode.Add(m_strength);
            hashcode.Add(m_damping);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

