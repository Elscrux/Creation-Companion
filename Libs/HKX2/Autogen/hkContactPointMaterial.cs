using System.Xml.Linq;
namespace HKX2
{
    // hkContactPointMaterial Signatire: 0x4e32287c size: 16 flags: FLAGS_NONE

    // m_userData m_class:  Type.TYPE_ULONG Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_friction m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 8 flags: FLAGS_NONE enum: 
    // m_restitution m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 9 flags: FLAGS_NONE enum: 
    // m_maxImpulse m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 10 flags: FLAGS_NONE enum: 
    // m_flags m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 11 flags: FLAGS_NONE enum: 
    public partial class hkContactPointMaterial : IHavokObject, IEquatable<hkContactPointMaterial?>
    {
        public ulong m_userData { set; get; }
        public byte m_friction { set; get; }
        public byte m_restitution { set; get; }
        public byte m_maxImpulse { set; get; }
        public byte m_flags { set; get; }

        public virtual uint Signature { set; get; } = 0x4e32287c;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_userData = br.ReadUInt64();
            m_friction = br.ReadByte();
            m_restitution = br.ReadByte();
            m_maxImpulse = br.ReadByte();
            m_flags = br.ReadByte();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt64(m_userData);
            bw.WriteByte(m_friction);
            bw.WriteByte(m_restitution);
            bw.WriteByte(m_maxImpulse);
            bw.WriteByte(m_flags);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_userData = xd.ReadUInt64(xe, nameof(m_userData));
            m_friction = xd.ReadByte(xe, nameof(m_friction));
            m_restitution = xd.ReadByte(xe, nameof(m_restitution));
            m_maxImpulse = xd.ReadByte(xe, nameof(m_maxImpulse));
            m_flags = xd.ReadByte(xe, nameof(m_flags));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_userData), m_userData);
            xs.WriteNumber(xe, nameof(m_friction), m_friction);
            xs.WriteNumber(xe, nameof(m_restitution), m_restitution);
            xs.WriteNumber(xe, nameof(m_maxImpulse), m_maxImpulse);
            xs.WriteNumber(xe, nameof(m_flags), m_flags);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkContactPointMaterial);
        }

        public bool Equals(hkContactPointMaterial? other)
        {
            return other is not null &&
                   m_userData.Equals(other.m_userData) &&
                   m_friction.Equals(other.m_friction) &&
                   m_restitution.Equals(other.m_restitution) &&
                   m_maxImpulse.Equals(other.m_maxImpulse) &&
                   m_flags.Equals(other.m_flags) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_userData);
            hashcode.Add(m_friction);
            hashcode.Add(m_restitution);
            hashcode.Add(m_maxImpulse);
            hashcode.Add(m_flags);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

