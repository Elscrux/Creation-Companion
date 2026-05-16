using System.Xml.Linq;
namespace HKX2
{
    // hkbEvent Signatire: 0x3e0fd810 size: 24 flags: FLAGS_NONE

    // m_sender m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 16 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbEvent : hkbEventBase, IEquatable<hkbEvent?>
    {
        private object? m_sender { set; get; }

        public override uint Signature { set; get; } = 0x3e0fd810;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            des.ReadEmptyPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteVoidPointer(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_sender));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEvent);
        }

        public bool Equals(hkbEvent? other)
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

