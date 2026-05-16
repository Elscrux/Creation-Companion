using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbKeyframeBonesModifierKeyframeInfo Signatire: 0x72deb7a6 size: 48 flags: FLAGS_NONE

    // m_keyframedPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_keyframedRotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_boneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_isValid m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 34 flags: FLAGS_NONE enum: 
    public partial class hkbKeyframeBonesModifierKeyframeInfo : IHavokObject, IEquatable<hkbKeyframeBonesModifierKeyframeInfo?>
    {
        public Vector4 m_keyframedPosition { set; get; }
        public Quaternion m_keyframedRotation { set; get; }
        public short m_boneIndex { set; get; }
        public bool m_isValid { set; get; }

        public virtual uint Signature { set; get; } = 0x72deb7a6;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_keyframedPosition = br.ReadVector4();
            m_keyframedRotation = des.ReadQuaternion(br);
            m_boneIndex = br.ReadInt16();
            m_isValid = br.ReadBoolean();
            br.Position += 13;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_keyframedPosition);
            s.WriteQuaternion(bw, m_keyframedRotation);
            bw.WriteInt16(m_boneIndex);
            bw.WriteBoolean(m_isValid);
            bw.Position += 13;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_keyframedPosition = xd.ReadVector4(xe, nameof(m_keyframedPosition));
            m_keyframedRotation = xd.ReadQuaternion(xe, nameof(m_keyframedRotation));
            m_boneIndex = xd.ReadInt16(xe, nameof(m_boneIndex));
            m_isValid = xd.ReadBoolean(xe, nameof(m_isValid));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_keyframedPosition), m_keyframedPosition);
            xs.WriteQuaternion(xe, nameof(m_keyframedRotation), m_keyframedRotation);
            xs.WriteNumber(xe, nameof(m_boneIndex), m_boneIndex);
            xs.WriteBoolean(xe, nameof(m_isValid), m_isValid);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbKeyframeBonesModifierKeyframeInfo);
        }

        public bool Equals(hkbKeyframeBonesModifierKeyframeInfo? other)
        {
            return other is not null &&
                   m_keyframedPosition.Equals(other.m_keyframedPosition) &&
                   m_keyframedRotation.Equals(other.m_keyframedRotation) &&
                   m_boneIndex.Equals(other.m_boneIndex) &&
                   m_isValid.Equals(other.m_isValid) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_keyframedPosition);
            hashcode.Add(m_keyframedRotation);
            hashcode.Add(m_boneIndex);
            hashcode.Add(m_isValid);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

