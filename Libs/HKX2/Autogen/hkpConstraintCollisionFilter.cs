using System.Xml.Linq;
namespace HKX2
{
    // hkpConstraintCollisionFilter Signatire: 0xc3b577b1 size: 104 flags: FLAGS_NONE


    public partial class hkpConstraintCollisionFilter : hkpPairCollisionFilter, IEquatable<hkpConstraintCollisionFilter?>
    {


        public override uint Signature { set; get; } = 0xc3b577b1;

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
            return Equals(obj as hkpConstraintCollisionFilter);
        }

        public bool Equals(hkpConstraintCollisionFilter? other)
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

