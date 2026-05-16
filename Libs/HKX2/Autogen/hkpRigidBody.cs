using System.Xml.Linq;
namespace HKX2
{
    // hkpRigidBody Signatire: 0x75f8d805 size: 720 flags: FLAGS_NONE


    public partial class hkpRigidBody : hkpEntity, IEquatable<hkpRigidBody?>
    {


        public override uint Signature { set; get; } = 0x75f8d805;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpRigidBody);
        }

        public bool Equals(hkpRigidBody? other)
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

