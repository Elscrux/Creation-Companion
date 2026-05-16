using System.Xml.Linq;
namespace HKX2
{
    // hkpEntityExtendedListeners Signatire: 0xf557023c size: 32 flags: FLAGS_NONE

    // m_activationListeners m_class: hkpEntitySmallArraySerializeOverrideType Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 0 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_entityListeners m_class: hkpEntitySmallArraySerializeOverrideType Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 16 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpEntityExtendedListeners : IHavokObject, IEquatable<hkpEntityExtendedListeners?>
    {
        public hkpEntitySmallArraySerializeOverrideType m_activationListeners { set; get; } = new();
        public hkpEntitySmallArraySerializeOverrideType m_entityListeners { set; get; } = new();

        public virtual uint Signature { set; get; } = 0xf557023c;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_activationListeners.Read(des, br);
            m_entityListeners.Read(des, br);
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            m_activationListeners.Write(s, bw);
            m_entityListeners.Write(s, bw);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {

        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteSerializeIgnored(xe, nameof(m_activationListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_entityListeners));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpEntityExtendedListeners);
        }

        public bool Equals(hkpEntityExtendedListeners? other)
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

