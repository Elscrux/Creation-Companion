using System.Xml.Linq;
namespace HKX2
{
    // hkMultiThreadCheck Signatire: 0x11e4408b size: 12 flags: FLAGS_NONE

    // m_threadId m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_stackTraceId m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 4 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_markCount m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 8 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_markBitStack m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 10 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkMultiThreadCheck : IHavokObject, IEquatable<hkMultiThreadCheck?>
    {
        private uint m_threadId { set; get; }
        private int m_stackTraceId { set; get; }
        private ushort m_markCount { set; get; }
        private ushort m_markBitStack { set; get; }

        public virtual uint Signature { set; get; } = 0x11e4408b;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_threadId = br.ReadUInt32();
            m_stackTraceId = br.ReadInt32();
            m_markCount = br.ReadUInt16();
            m_markBitStack = br.ReadUInt16();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_threadId);
            bw.WriteInt32(m_stackTraceId);
            bw.WriteUInt16(m_markCount);
            bw.WriteUInt16(m_markBitStack);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {

        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteSerializeIgnored(xe, nameof(m_threadId));
            xs.WriteSerializeIgnored(xe, nameof(m_stackTraceId));
            xs.WriteSerializeIgnored(xe, nameof(m_markCount));
            xs.WriteSerializeIgnored(xe, nameof(m_markBitStack));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkMultiThreadCheck);
        }

        public bool Equals(hkMultiThreadCheck? other)
        {
            return other is not null &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();

            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

