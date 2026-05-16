using System.Xml.Linq;
namespace HKX2
{
    // hkbCharacterDataCharacterControllerInfo Signatire: 0xa0f415bf size: 24 flags: FLAGS_NONE

    // m_capsuleHeight m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_capsuleRadius m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_collisionFilterInfo m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_characterControllerCinfo m_class: hkpCharacterControllerCinfo Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbCharacterDataCharacterControllerInfo : IHavokObject, IEquatable<hkbCharacterDataCharacterControllerInfo?>
    {
        public float m_capsuleHeight { set; get; }
        public float m_capsuleRadius { set; get; }
        public uint m_collisionFilterInfo { set; get; }
        public hkpCharacterControllerCinfo? m_characterControllerCinfo { set; get; }

        public virtual uint Signature { set; get; } = 0xa0f415bf;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_capsuleHeight = br.ReadSingle();
            m_capsuleRadius = br.ReadSingle();
            m_collisionFilterInfo = br.ReadUInt32();
            br.Position += 4;
            m_characterControllerCinfo = des.ReadClassPointer<hkpCharacterControllerCinfo>(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteSingle(m_capsuleHeight);
            bw.WriteSingle(m_capsuleRadius);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.Position += 4;
            s.WriteClassPointer(bw, m_characterControllerCinfo);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_capsuleHeight = xd.ReadSingle(xe, nameof(m_capsuleHeight));
            m_capsuleRadius = xd.ReadSingle(xe, nameof(m_capsuleRadius));
            m_collisionFilterInfo = xd.ReadUInt32(xe, nameof(m_collisionFilterInfo));
            m_characterControllerCinfo = xd.ReadClassPointer<hkpCharacterControllerCinfo>(xe, nameof(m_characterControllerCinfo));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteFloat(xe, nameof(m_capsuleHeight), m_capsuleHeight);
            xs.WriteFloat(xe, nameof(m_capsuleRadius), m_capsuleRadius);
            xs.WriteNumber(xe, nameof(m_collisionFilterInfo), m_collisionFilterInfo);
            xs.WriteClassPointer(xe, nameof(m_characterControllerCinfo), m_characterControllerCinfo);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbCharacterDataCharacterControllerInfo);
        }

        public bool Equals(hkbCharacterDataCharacterControllerInfo? other)
        {
            return other is not null &&
                   m_capsuleHeight.Equals(other.m_capsuleHeight) &&
                   m_capsuleRadius.Equals(other.m_capsuleRadius) &&
                   m_collisionFilterInfo.Equals(other.m_collisionFilterInfo) &&
                   ((m_characterControllerCinfo is null && other.m_characterControllerCinfo is null) || (m_characterControllerCinfo is not null && other.m_characterControllerCinfo is not null && m_characterControllerCinfo.Equals((IHavokObject)other.m_characterControllerCinfo))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_capsuleHeight);
            hashcode.Add(m_capsuleRadius);
            hashcode.Add(m_collisionFilterInfo);
            hashcode.Add(m_characterControllerCinfo);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

