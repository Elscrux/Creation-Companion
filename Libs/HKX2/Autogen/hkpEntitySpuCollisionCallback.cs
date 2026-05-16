using System.Xml.Linq;
namespace HKX2
{
    // hkpEntitySpuCollisionCallback Signatire: 0x81147f05 size: 16 flags: FLAGS_NONE

    // m_util m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 0 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_capacity m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 8 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_eventFilter m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 10 flags: FLAGS_NONE enum: 
    // m_userFilter m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 11 flags: FLAGS_NONE enum: 
    public partial class hkpEntitySpuCollisionCallback : IHavokObject, IEquatable<hkpEntitySpuCollisionCallback?>
    {
        private object? m_util { set; get; }
        private ushort m_capacity { set; get; }
        public byte m_eventFilter { set; get; }
        public byte m_userFilter { set; get; }

        public virtual uint Signature { set; get; } = 0x81147f05;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            des.ReadEmptyPointer(br);
            m_capacity = br.ReadUInt16();
            m_eventFilter = br.ReadByte();
            m_userFilter = br.ReadByte();
            br.Position += 4;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteVoidPointer(bw);
            bw.WriteUInt16(m_capacity);
            bw.WriteByte(m_eventFilter);
            bw.WriteByte(m_userFilter);
            bw.Position += 4;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_eventFilter = xd.ReadByte(xe, nameof(m_eventFilter));
            m_userFilter = xd.ReadByte(xe, nameof(m_userFilter));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteSerializeIgnored(xe, nameof(m_util));
            xs.WriteSerializeIgnored(xe, nameof(m_capacity));
            xs.WriteNumber(xe, nameof(m_eventFilter), m_eventFilter);
            xs.WriteNumber(xe, nameof(m_userFilter), m_userFilter);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpEntitySpuCollisionCallback);
        }

        public bool Equals(hkpEntitySpuCollisionCallback? other)
        {
            return other is not null &&
                   m_eventFilter.Equals(other.m_eventFilter) &&
                   m_userFilter.Equals(other.m_userFilter) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_eventFilter);
            hashcode.Add(m_userFilter);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

