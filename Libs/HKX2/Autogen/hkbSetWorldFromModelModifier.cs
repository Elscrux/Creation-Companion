using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbSetWorldFromModelModifier Signatire: 0xafcfa211 size: 128 flags: FLAGS_NONE

    // m_translation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_rotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_setTranslation m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_setRotation m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 113 flags: FLAGS_NONE enum: 
    public partial class hkbSetWorldFromModelModifier : hkbModifier, IEquatable<hkbSetWorldFromModelModifier?>
    {
        public Vector4 m_translation { set; get; }
        public Quaternion m_rotation { set; get; }
        public bool m_setTranslation { set; get; }
        public bool m_setRotation { set; get; }

        public override uint Signature { set; get; } = 0xafcfa211;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_translation = br.ReadVector4();
            m_rotation = des.ReadQuaternion(br);
            m_setTranslation = br.ReadBoolean();
            m_setRotation = br.ReadBoolean();
            br.Position += 14;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_translation);
            s.WriteQuaternion(bw, m_rotation);
            bw.WriteBoolean(m_setTranslation);
            bw.WriteBoolean(m_setRotation);
            bw.Position += 14;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_translation = xd.ReadVector4(xe, nameof(m_translation));
            m_rotation = xd.ReadQuaternion(xe, nameof(m_rotation));
            m_setTranslation = xd.ReadBoolean(xe, nameof(m_setTranslation));
            m_setRotation = xd.ReadBoolean(xe, nameof(m_setRotation));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_translation), m_translation);
            xs.WriteQuaternion(xe, nameof(m_rotation), m_rotation);
            xs.WriteBoolean(xe, nameof(m_setTranslation), m_setTranslation);
            xs.WriteBoolean(xe, nameof(m_setRotation), m_setRotation);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbSetWorldFromModelModifier);
        }

        public bool Equals(hkbSetWorldFromModelModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_translation.Equals(other.m_translation) &&
                   m_rotation.Equals(other.m_rotation) &&
                   m_setTranslation.Equals(other.m_setTranslation) &&
                   m_setRotation.Equals(other.m_setRotation) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_translation);
            hashcode.Add(m_rotation);
            hashcode.Add(m_setTranslation);
            hashcode.Add(m_setRotation);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

