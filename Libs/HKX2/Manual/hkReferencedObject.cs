using System.Xml.Linq;
namespace HKX2
{
    // hkReferencedObject Signatire: 0x3b1c1113 size: 16 flags: FLAGS_NONE

    // m_memSizeAndFlags m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 8 flags: SERIALIZE_IGNORED enum: 
    // m_referenceCount m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 10 flags: SERIALIZE_IGNORED enum: 

    public partial class hkReferencedObject : hkBaseObject, IEquatable<hkReferencedObject?>
    {

        public ushort m_memSizeAndFlags { set; get; } = default;
        public short m_referenceCount { set; get; } = default;

        public override uint Signature { set; get; } = 0x3b1c1113;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {

            base.Read(des, br);
            m_memSizeAndFlags = br.ReadUInt16();
            m_referenceCount = br.ReadInt16();

            if (des._header.PointerSize == 8) br.Pad(8);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {

            base.Write(s, bw);
            bw.WriteUInt16(m_memSizeAndFlags);
            bw.WriteInt16(m_referenceCount);

            if (s._header.PointerSize == 8) bw.Pad(8);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {

        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_memSizeAndFlags));
            xs.WriteSerializeIgnored(xe, nameof(m_referenceCount));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkReferencedObject);
        }

        public bool Equals(hkReferencedObject? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

