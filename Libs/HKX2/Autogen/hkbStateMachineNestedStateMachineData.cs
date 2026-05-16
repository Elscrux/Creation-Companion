using System.Xml.Linq;
namespace HKX2
{
    // hkbStateMachineNestedStateMachineData Signatire: 0x7358f5da size: 16 flags: FLAGS_NONE

    // m_nestedStateMachine m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 0 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_eventIdMap m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 8 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbStateMachineNestedStateMachineData : IHavokObject, IEquatable<hkbStateMachineNestedStateMachineData?>
    {
        private object? m_nestedStateMachine { set; get; }
        private object? m_eventIdMap { set; get; }

        public virtual uint Signature { set; get; } = 0x7358f5da;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            des.ReadEmptyPointer(br);
            des.ReadEmptyPointer(br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteVoidPointer(bw);
            s.WriteVoidPointer(bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {

        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteSerializeIgnored(xe, nameof(m_nestedStateMachine));
            xs.WriteSerializeIgnored(xe, nameof(m_eventIdMap));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbStateMachineNestedStateMachineData);
        }

        public bool Equals(hkbStateMachineNestedStateMachineData? other)
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

