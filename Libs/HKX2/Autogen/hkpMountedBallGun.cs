using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpMountedBallGun Signatire: 0x6791ffce size: 128 flags: FLAGS_NONE

    // m_position m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkpMountedBallGun : hkpBallGun, IEquatable<hkpMountedBallGun?>
    {
        public Vector4 m_position { set; get; }

        public override uint Signature { set; get; } = 0x6791ffce;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_position = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_position);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_position = xd.ReadVector4(xe, nameof(m_position));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_position), m_position);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMountedBallGun);
        }

        public bool Equals(hkpMountedBallGun? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_position.Equals(other.m_position) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_position);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

