using System.Xml.Linq;
namespace HKX2
{
    // hkpBroadPhaseHandle Signatire: 0x940569dc size: 4 flags: FLAGS_NONE

    // m_id m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 0 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpBroadPhaseHandle : IHavokObject, IEquatable<hkpBroadPhaseHandle?>
    {
        private uint m_id { set; get; }

        public virtual uint Signature { set; get; } = 0x940569dc;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_id = br.ReadUInt32();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteUInt32(m_id);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {

        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteSerializeIgnored(xe, nameof(m_id));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpBroadPhaseHandle);
        }

        public bool Equals(hkpBroadPhaseHandle? other)
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

