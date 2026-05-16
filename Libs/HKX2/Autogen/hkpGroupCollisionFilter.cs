using System.Xml.Linq;
namespace HKX2
{
    // hkpGroupCollisionFilter Signatire: 0x5cc01561 size: 208 flags: FLAGS_NONE

    // m_noGroupCollisionEnabled m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_collisionGroups m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 32 offset: 76 flags: FLAGS_NONE enum: 
    public partial class hkpGroupCollisionFilter : hkpCollisionFilter, IEquatable<hkpGroupCollisionFilter?>
    {
        public bool m_noGroupCollisionEnabled { set; get; }
        public uint[] m_collisionGroups = new uint[32];

        public override uint Signature { set; get; } = 0x5cc01561;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_noGroupCollisionEnabled = br.ReadBoolean();
            br.Position += 3;
            m_collisionGroups = des.ReadUInt32CStyleArray(br, 32);
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteBoolean(m_noGroupCollisionEnabled);
            bw.Position += 3;
            s.WriteUInt32CStyleArray(bw, m_collisionGroups);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_noGroupCollisionEnabled = xd.ReadBoolean(xe, nameof(m_noGroupCollisionEnabled));
            m_collisionGroups = xd.ReadUInt32CStyleArray(xe, nameof(m_collisionGroups), 32);
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBoolean(xe, nameof(m_noGroupCollisionEnabled), m_noGroupCollisionEnabled);
            xs.WriteNumberArray(xe, nameof(m_collisionGroups), m_collisionGroups);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpGroupCollisionFilter);
        }

        public bool Equals(hkpGroupCollisionFilter? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_noGroupCollisionEnabled.Equals(other.m_noGroupCollisionEnabled) &&
                   m_collisionGroups.SequenceEqual(other.m_collisionGroups) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_noGroupCollisionEnabled);
            hashcode.Add(m_collisionGroups.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

