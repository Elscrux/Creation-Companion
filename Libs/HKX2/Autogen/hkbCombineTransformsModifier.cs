using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbCombineTransformsModifier Signatire: 0xfd1f0b79 size: 192 flags: FLAGS_NONE

    // m_translationOut m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_rotationOut m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_leftTranslation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_leftRotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_rightTranslation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_rightRotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_invertLeftTransform m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_invertRightTransform m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 177 flags: FLAGS_NONE enum: 
    // m_invertResult m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 178 flags: FLAGS_NONE enum: 
    public partial class hkbCombineTransformsModifier : hkbModifier, IEquatable<hkbCombineTransformsModifier?>
    {
        public Vector4 m_translationOut { set; get; }
        public Quaternion m_rotationOut { set; get; }
        public Vector4 m_leftTranslation { set; get; }
        public Quaternion m_leftRotation { set; get; }
        public Vector4 m_rightTranslation { set; get; }
        public Quaternion m_rightRotation { set; get; }
        public bool m_invertLeftTransform { set; get; }
        public bool m_invertRightTransform { set; get; }
        public bool m_invertResult { set; get; }

        public override uint Signature { set; get; } = 0xfd1f0b79;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_translationOut = br.ReadVector4();
            m_rotationOut = des.ReadQuaternion(br);
            m_leftTranslation = br.ReadVector4();
            m_leftRotation = des.ReadQuaternion(br);
            m_rightTranslation = br.ReadVector4();
            m_rightRotation = des.ReadQuaternion(br);
            m_invertLeftTransform = br.ReadBoolean();
            m_invertRightTransform = br.ReadBoolean();
            m_invertResult = br.ReadBoolean();
            br.Position += 13;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_translationOut);
            s.WriteQuaternion(bw, m_rotationOut);
            bw.WriteVector4(m_leftTranslation);
            s.WriteQuaternion(bw, m_leftRotation);
            bw.WriteVector4(m_rightTranslation);
            s.WriteQuaternion(bw, m_rightRotation);
            bw.WriteBoolean(m_invertLeftTransform);
            bw.WriteBoolean(m_invertRightTransform);
            bw.WriteBoolean(m_invertResult);
            bw.Position += 13;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_translationOut = xd.ReadVector4(xe, nameof(m_translationOut));
            m_rotationOut = xd.ReadQuaternion(xe, nameof(m_rotationOut));
            m_leftTranslation = xd.ReadVector4(xe, nameof(m_leftTranslation));
            m_leftRotation = xd.ReadQuaternion(xe, nameof(m_leftRotation));
            m_rightTranslation = xd.ReadVector4(xe, nameof(m_rightTranslation));
            m_rightRotation = xd.ReadQuaternion(xe, nameof(m_rightRotation));
            m_invertLeftTransform = xd.ReadBoolean(xe, nameof(m_invertLeftTransform));
            m_invertRightTransform = xd.ReadBoolean(xe, nameof(m_invertRightTransform));
            m_invertResult = xd.ReadBoolean(xe, nameof(m_invertResult));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_translationOut), m_translationOut);
            xs.WriteQuaternion(xe, nameof(m_rotationOut), m_rotationOut);
            xs.WriteVector4(xe, nameof(m_leftTranslation), m_leftTranslation);
            xs.WriteQuaternion(xe, nameof(m_leftRotation), m_leftRotation);
            xs.WriteVector4(xe, nameof(m_rightTranslation), m_rightTranslation);
            xs.WriteQuaternion(xe, nameof(m_rightRotation), m_rightRotation);
            xs.WriteBoolean(xe, nameof(m_invertLeftTransform), m_invertLeftTransform);
            xs.WriteBoolean(xe, nameof(m_invertRightTransform), m_invertRightTransform);
            xs.WriteBoolean(xe, nameof(m_invertResult), m_invertResult);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbCombineTransformsModifier);
        }

        public bool Equals(hkbCombineTransformsModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_translationOut.Equals(other.m_translationOut) &&
                   m_rotationOut.Equals(other.m_rotationOut) &&
                   m_leftTranslation.Equals(other.m_leftTranslation) &&
                   m_leftRotation.Equals(other.m_leftRotation) &&
                   m_rightTranslation.Equals(other.m_rightTranslation) &&
                   m_rightRotation.Equals(other.m_rightRotation) &&
                   m_invertLeftTransform.Equals(other.m_invertLeftTransform) &&
                   m_invertRightTransform.Equals(other.m_invertRightTransform) &&
                   m_invertResult.Equals(other.m_invertResult) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_translationOut);
            hashcode.Add(m_rotationOut);
            hashcode.Add(m_leftTranslation);
            hashcode.Add(m_leftRotation);
            hashcode.Add(m_rightTranslation);
            hashcode.Add(m_rightRotation);
            hashcode.Add(m_invertLeftTransform);
            hashcode.Add(m_invertRightTransform);
            hashcode.Add(m_invertResult);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

