using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpSetLocalRotationsConstraintAtom Signatire: 0xf81db8e size: 112 flags: FLAGS_NONE

    // m_rotationA m_class:  Type.TYPE_ROTATION Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_rotationB m_class:  Type.TYPE_ROTATION Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    public partial class hkpSetLocalRotationsConstraintAtom : hkpConstraintAtom, IEquatable<hkpSetLocalRotationsConstraintAtom?>
    {
        public Matrix4x4 m_rotationA { set; get; }
        public Matrix4x4 m_rotationB { set; get; }

        public override uint Signature { set; get; } = 0xf81db8e;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 14;
            m_rotationA = des.ReadMatrix3(br); //TYPE_ROTATION
            m_rotationB = des.ReadMatrix3(br); //TYPE_ROTATION
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 14;
            s.WriteMatrix3(bw, m_rotationA);
            s.WriteMatrix3(bw, m_rotationB);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rotationA = xd.ReadRotation(xe, nameof(m_rotationA));
            m_rotationB = xd.ReadRotation(xe, nameof(m_rotationB));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteRotation(xe, nameof(m_rotationA), m_rotationA);
            xs.WriteRotation(xe, nameof(m_rotationB), m_rotationB);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSetLocalRotationsConstraintAtom);
        }

        public bool Equals(hkpSetLocalRotationsConstraintAtom? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_rotationA.Equals(other.m_rotationA) &&
                   m_rotationB.Equals(other.m_rotationB) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rotationA);
            hashcode.Add(m_rotationB);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

