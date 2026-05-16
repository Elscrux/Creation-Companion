using System.Xml.Linq;
namespace HKX2
{
    // hkpSimpleShapePhantom Signatire: 0x32a2a8a8 size: 448 flags: FLAGS_NONE

    // m_collisionDetails m_class: hkpSimpleShapePhantomCollisionDetail Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 416 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_orderDirty m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 432 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpSimpleShapePhantom : hkpShapePhantom, IEquatable<hkpSimpleShapePhantom?>
    {
        public IList<hkpSimpleShapePhantomCollisionDetail> m_collisionDetails { set; get; } = Array.Empty<hkpSimpleShapePhantomCollisionDetail>();
        private bool m_orderDirty { set; get; }

        public override uint Signature { set; get; } = 0x32a2a8a8;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_collisionDetails = des.ReadClassArray<hkpSimpleShapePhantomCollisionDetail>(br);
            m_orderDirty = br.ReadBoolean();
            br.Position += 15;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_collisionDetails);
            bw.WriteBoolean(m_orderDirty);
            bw.Position += 15;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_collisionDetails));
            xs.WriteSerializeIgnored(xe, nameof(m_orderDirty));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSimpleShapePhantom);
        }

        public bool Equals(hkpSimpleShapePhantom? other)
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

