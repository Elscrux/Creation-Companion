using System.Xml.Linq;
namespace HKX2
{
    // hkpConvexShape Signatire: 0xf8f74f85 size: 40 flags: FLAGS_NONE

    // m_radius m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    public partial class hkpConvexShape : hkpSphereRepShape, IEquatable<hkpConvexShape?>
    {
        public float m_radius { set; get; }

        public override uint Signature { set; get; } = 0xf8f74f85;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_radius = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_radius);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_radius = xd.ReadSingle(xe, nameof(m_radius));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_radius), m_radius);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConvexShape);
        }

        public bool Equals(hkpConvexShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_radius.Equals(other.m_radius) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_radius);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

