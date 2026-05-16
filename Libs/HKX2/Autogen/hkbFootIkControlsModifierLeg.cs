using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbFootIkControlsModifierLeg Signatire: 0x9e17091a size: 48 flags: FLAGS_NONE

    // m_groundPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_ungroundedEvent m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_verticalError m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_hitSomething m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_isPlantedMS m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 37 flags: FLAGS_NONE enum: 
    public partial class hkbFootIkControlsModifierLeg : IHavokObject, IEquatable<hkbFootIkControlsModifierLeg?>
    {
        public Vector4 m_groundPosition { set; get; }
        public hkbEventProperty m_ungroundedEvent { set; get; } = new();
        public float m_verticalError { set; get; }
        public bool m_hitSomething { set; get; }
        public bool m_isPlantedMS { set; get; }

        public virtual uint Signature { set; get; } = 0x9e17091a;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_groundPosition = br.ReadVector4();
            m_ungroundedEvent.Read(des, br);
            m_verticalError = br.ReadSingle();
            m_hitSomething = br.ReadBoolean();
            m_isPlantedMS = br.ReadBoolean();
            br.Position += 10;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteVector4(m_groundPosition);
            m_ungroundedEvent.Write(s, bw);
            bw.WriteSingle(m_verticalError);
            bw.WriteBoolean(m_hitSomething);
            bw.WriteBoolean(m_isPlantedMS);
            bw.Position += 10;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_groundPosition = xd.ReadVector4(xe, nameof(m_groundPosition));
            m_ungroundedEvent = xd.ReadClass<hkbEventProperty>(xe, nameof(m_ungroundedEvent));
            m_verticalError = xd.ReadSingle(xe, nameof(m_verticalError));
            m_hitSomething = xd.ReadBoolean(xe, nameof(m_hitSomething));
            m_isPlantedMS = xd.ReadBoolean(xe, nameof(m_isPlantedMS));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteVector4(xe, nameof(m_groundPosition), m_groundPosition);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_ungroundedEvent), m_ungroundedEvent);
            xs.WriteFloat(xe, nameof(m_verticalError), m_verticalError);
            xs.WriteBoolean(xe, nameof(m_hitSomething), m_hitSomething);
            xs.WriteBoolean(xe, nameof(m_isPlantedMS), m_isPlantedMS);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbFootIkControlsModifierLeg);
        }

        public bool Equals(hkbFootIkControlsModifierLeg? other)
        {
            return other is not null &&
                   m_groundPosition.Equals(other.m_groundPosition) &&
                   ((m_ungroundedEvent is null && other.m_ungroundedEvent is null) || (m_ungroundedEvent is not null && other.m_ungroundedEvent is not null && m_ungroundedEvent.Equals((IHavokObject)other.m_ungroundedEvent))) &&
                   m_verticalError.Equals(other.m_verticalError) &&
                   m_hitSomething.Equals(other.m_hitSomething) &&
                   m_isPlantedMS.Equals(other.m_isPlantedMS) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_groundPosition);
            hashcode.Add(m_ungroundedEvent);
            hashcode.Add(m_verticalError);
            hashcode.Add(m_hitSomething);
            hashcode.Add(m_isPlantedMS);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

