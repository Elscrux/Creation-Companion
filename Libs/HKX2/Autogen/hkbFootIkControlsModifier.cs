using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbFootIkControlsModifier Signatire: 0xe5b6f544 size: 176 flags: FLAGS_NONE

    // m_controlData m_class: hkbFootIkControlData Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_legs m_class: hkbFootIkControlsModifierLeg Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_errorOutTranslation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_alignWithGroundRotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    public partial class hkbFootIkControlsModifier : hkbModifier, IEquatable<hkbFootIkControlsModifier?>
    {
        public hkbFootIkControlData m_controlData { set; get; } = new();
        public IList<hkbFootIkControlsModifierLeg> m_legs { set; get; } = Array.Empty<hkbFootIkControlsModifierLeg>();
        public Vector4 m_errorOutTranslation { set; get; }
        public Quaternion m_alignWithGroundRotation { set; get; }

        public override uint Signature { set; get; } = 0xe5b6f544;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_controlData.Read(des, br);
            m_legs = des.ReadClassArray<hkbFootIkControlsModifierLeg>(br);
            m_errorOutTranslation = br.ReadVector4();
            m_alignWithGroundRotation = des.ReadQuaternion(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_controlData.Write(s, bw);
            s.WriteClassArray(bw, m_legs);
            bw.WriteVector4(m_errorOutTranslation);
            s.WriteQuaternion(bw, m_alignWithGroundRotation);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_controlData = xd.ReadClass<hkbFootIkControlData>(xe, nameof(m_controlData));
            m_legs = xd.ReadClassArray<hkbFootIkControlsModifierLeg>(xe, nameof(m_legs));
            m_errorOutTranslation = xd.ReadVector4(xe, nameof(m_errorOutTranslation));
            m_alignWithGroundRotation = xd.ReadQuaternion(xe, nameof(m_alignWithGroundRotation));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbFootIkControlData>(xe, nameof(m_controlData), m_controlData);
            xs.WriteClassArray(xe, nameof(m_legs), m_legs);
            xs.WriteVector4(xe, nameof(m_errorOutTranslation), m_errorOutTranslation);
            xs.WriteQuaternion(xe, nameof(m_alignWithGroundRotation), m_alignWithGroundRotation);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbFootIkControlsModifier);
        }

        public bool Equals(hkbFootIkControlsModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_controlData is null && other.m_controlData is null) || (m_controlData is not null && other.m_controlData is not null && m_controlData.Equals((IHavokObject)other.m_controlData))) &&
                   m_legs.SequenceEqual(other.m_legs) &&
                   m_errorOutTranslation.Equals(other.m_errorOutTranslation) &&
                   m_alignWithGroundRotation.Equals(other.m_alignWithGroundRotation) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_controlData);
            hashcode.Add(m_legs.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_errorOutTranslation);
            hashcode.Add(m_alignWithGroundRotation);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

