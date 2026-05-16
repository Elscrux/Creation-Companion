using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkxAnimatedQuaternion Signatire: 0xb4f01baa size: 32 flags: FLAGS_NONE

    // m_quaternions m_class:  Type.TYPE_ARRAY Type.TYPE_QUATERNION arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkxAnimatedQuaternion : hkReferencedObject, IEquatable<hkxAnimatedQuaternion?>
    {
        public IList<Quaternion> m_quaternions { set; get; } = Array.Empty<Quaternion>();

        public override uint Signature { set; get; } = 0xb4f01baa;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_quaternions = des.ReadQuaternionArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteQuaternionArray(bw, m_quaternions);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_quaternions = xd.ReadQuaternionArray(xe, nameof(m_quaternions));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteQuaternionArray(xe, nameof(m_quaternions), m_quaternions);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxAnimatedQuaternion);
        }

        public bool Equals(hkxAnimatedQuaternion? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_quaternions.SequenceEqual(other.m_quaternions) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_quaternions.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

