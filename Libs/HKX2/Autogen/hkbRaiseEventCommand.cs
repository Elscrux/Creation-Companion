using System.Xml.Linq;
namespace HKX2
{
    // hkbRaiseEventCommand Signatire: 0xa0a7bf9c size: 32 flags: FLAGS_NONE

    // m_characterId m_class:  Type.TYPE_UINT64 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_global m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_externalId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 28 flags: FLAGS_NONE enum: 
    public partial class hkbRaiseEventCommand : hkReferencedObject, IEquatable<hkbRaiseEventCommand?>
    {
        public ulong m_characterId { set; get; }
        public bool m_global { set; get; }
        public int m_externalId { set; get; }

        public override uint Signature { set; get; } = 0xa0a7bf9c;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_global = br.ReadBoolean();
            br.Position += 3;
            m_externalId = br.ReadInt32();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(m_characterId);
            bw.WriteBoolean(m_global);
            bw.Position += 3;
            bw.WriteInt32(m_externalId);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_characterId = xd.ReadUInt64(xe, nameof(m_characterId));
            m_global = xd.ReadBoolean(xe, nameof(m_global));
            m_externalId = xd.ReadInt32(xe, nameof(m_externalId));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_characterId), m_characterId);
            xs.WriteBoolean(xe, nameof(m_global), m_global);
            xs.WriteNumber(xe, nameof(m_externalId), m_externalId);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbRaiseEventCommand);
        }

        public bool Equals(hkbRaiseEventCommand? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_characterId.Equals(other.m_characterId) &&
                   m_global.Equals(other.m_global) &&
                   m_externalId.Equals(other.m_externalId) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_characterId);
            hashcode.Add(m_global);
            hashcode.Add(m_externalId);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

