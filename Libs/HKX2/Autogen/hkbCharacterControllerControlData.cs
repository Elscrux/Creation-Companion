using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbCharacterControllerControlData Signatire: 0x5b6c03d9 size: 32 flags: FLAGS_NONE

    // m_desiredVelocity m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_verticalGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_horizontalCatchUpGain m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 20 flags: FLAGS_NONE enum: 
    // m_maxVerticalSeparation m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_maxHorizontalSeparation m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 28 flags: FLAGS_NONE enum: 
    public partial class hkbCharacterControllerControlData : IHavokObject, IEquatable<hkbCharacterControllerControlData?>
    {
        public Vector4 m_desiredVelocity { set; get; }
        public float m_verticalGain { set; get; }
        public float m_horizontalCatchUpGain { set; get; }
        public float m_maxVerticalSeparation { set; get; }
        public float m_maxHorizontalSeparation { set; get; }

        public virtual uint Signature { set; get; } = 0x5b6c03d9;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_desiredVelocity = br.ReadVector4();
            m_verticalGain = br.ReadSingle();
            m_horizontalCatchUpGain = br.ReadSingle();
            m_maxVerticalSeparation = br.ReadSingle();
            m_maxHorizontalSeparation = br.ReadSingle();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_desiredVelocity);
            bw.WriteSingle(m_verticalGain);
            bw.WriteSingle(m_horizontalCatchUpGain);
            bw.WriteSingle(m_maxVerticalSeparation);
            bw.WriteSingle(m_maxHorizontalSeparation);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_desiredVelocity = xd.ReadVector4(xe, nameof(m_desiredVelocity));
            m_verticalGain = xd.ReadSingle(xe, nameof(m_verticalGain));
            m_horizontalCatchUpGain = xd.ReadSingle(xe, nameof(m_horizontalCatchUpGain));
            m_maxVerticalSeparation = xd.ReadSingle(xe, nameof(m_maxVerticalSeparation));
            m_maxHorizontalSeparation = xd.ReadSingle(xe, nameof(m_maxHorizontalSeparation));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_desiredVelocity), m_desiredVelocity);
            xs.WriteFloat(xe, nameof(m_verticalGain), m_verticalGain);
            xs.WriteFloat(xe, nameof(m_horizontalCatchUpGain), m_horizontalCatchUpGain);
            xs.WriteFloat(xe, nameof(m_maxVerticalSeparation), m_maxVerticalSeparation);
            xs.WriteFloat(xe, nameof(m_maxHorizontalSeparation), m_maxHorizontalSeparation);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbCharacterControllerControlData);
        }

        public bool Equals(hkbCharacterControllerControlData? other)
        {
            return other is not null &&
                   m_desiredVelocity.Equals(other.m_desiredVelocity) &&
                   m_verticalGain.Equals(other.m_verticalGain) &&
                   m_horizontalCatchUpGain.Equals(other.m_horizontalCatchUpGain) &&
                   m_maxVerticalSeparation.Equals(other.m_maxVerticalSeparation) &&
                   m_maxHorizontalSeparation.Equals(other.m_maxHorizontalSeparation) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_desiredVelocity);
            hashcode.Add(m_verticalGain);
            hashcode.Add(m_horizontalCatchUpGain);
            hashcode.Add(m_maxVerticalSeparation);
            hashcode.Add(m_maxHorizontalSeparation);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

