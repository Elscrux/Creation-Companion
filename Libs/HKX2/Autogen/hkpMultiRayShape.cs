using System.Xml.Linq;
namespace HKX2
{
    // hkpMultiRayShape Signatire: 0xea2e7ec9 size: 56 flags: FLAGS_NONE

    // m_rays m_class: hkpMultiRayShapeRay Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_rayPenetrationDistance m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpMultiRayShape : hkpShape, IEquatable<hkpMultiRayShape?>
    {
        public IList<hkpMultiRayShapeRay> m_rays { set; get; } = Array.Empty<hkpMultiRayShapeRay>();
        public float m_rayPenetrationDistance { set; get; }

        public override uint Signature { set; get; } = 0xea2e7ec9;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_rays = des.ReadClassArray<hkpMultiRayShapeRay>(br);
            m_rayPenetrationDistance = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_rays);
            bw.WriteSingle(m_rayPenetrationDistance);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_rays = xd.ReadClassArray<hkpMultiRayShapeRay>(xe, nameof(m_rays));
            m_rayPenetrationDistance = xd.ReadSingle(xe, nameof(m_rayPenetrationDistance));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_rays), m_rays);
            xs.WriteFloat(xe, nameof(m_rayPenetrationDistance), m_rayPenetrationDistance);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMultiRayShape);
        }

        public bool Equals(hkpMultiRayShape? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_rays.SequenceEqual(other.m_rays) &&
                   m_rayPenetrationDistance.Equals(other.m_rayPenetrationDistance) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_rays.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_rayPenetrationDistance);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

