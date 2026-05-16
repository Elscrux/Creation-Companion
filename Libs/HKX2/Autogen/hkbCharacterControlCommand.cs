using System.Xml.Linq;
namespace HKX2
{
    // hkbCharacterControlCommand Signatire: 0x7a195d1d size: 32 flags: FLAGS_NONE

    // m_characterId m_class:  Type.TYPE_UINT64 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_command m_class:  Type.TYPE_ENUM Type.TYPE_UINT8 arrSize: 0 offset: 24 flags: FLAGS_NONE enum: CharacterControlCommand
    // m_padding m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 28 flags: FLAGS_NONE enum: 
    public partial class hkbCharacterControlCommand : hkReferencedObject, IEquatable<hkbCharacterControlCommand?>
    {
        public ulong m_characterId { set; get; }
        public byte m_command { set; get; }
        public int m_padding { set; get; }

        public override uint Signature { set; get; } = 0x7a195d1d;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterId = br.ReadUInt64();
            m_command = br.ReadByte();
            br.Position += 3;
            m_padding = br.ReadInt32();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteUInt64(m_characterId);
            bw.WriteByte(m_command);
            bw.Position += 3;
            bw.WriteInt32(m_padding);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_characterId = xd.ReadUInt64(xe, nameof(m_characterId));
            m_command = xd.ReadFlag<CharacterControlCommand, byte>(xe, nameof(m_command));
            m_padding = xd.ReadInt32(xe, nameof(m_padding));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumber(xe, nameof(m_characterId), m_characterId);
            xs.WriteEnum<CharacterControlCommand, byte>(xe, nameof(m_command), m_command);
            xs.WriteNumber(xe, nameof(m_padding), m_padding);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbCharacterControlCommand);
        }

        public bool Equals(hkbCharacterControlCommand? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_characterId.Equals(other.m_characterId) &&
                   m_command.Equals(other.m_command) &&
                   m_padding.Equals(other.m_padding) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_characterId);
            hashcode.Add(m_command);
            hashcode.Add(m_padding);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

