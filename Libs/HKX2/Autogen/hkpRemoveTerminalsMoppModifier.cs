using System.Xml.Linq;
namespace HKX2
{
    // hkpRemoveTerminalsMoppModifier Signatire: 0x91367f03 size: 48 flags: FLAGS_NONE

    // m_removeInfo m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 24 flags: FLAGS_NONE enum: 
    // m_tempShapesToRemove m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 40 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpRemoveTerminalsMoppModifier : hkReferencedObject, IEquatable<hkpRemoveTerminalsMoppModifier?>
    {
        public IList<uint> m_removeInfo { set; get; } = Array.Empty<uint>();
        private object? m_tempShapesToRemove { set; get; }

        public override uint Signature { set; get; } = 0x91367f03;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_removeInfo = des.ReadUInt32Array(br);
            des.ReadEmptyPointer(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            s.WriteUInt32Array(bw, m_removeInfo);
            s.WriteVoidPointer(bw);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_removeInfo = xd.ReadUInt32Array(xe, nameof(m_removeInfo));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumberArray(xe, nameof(m_removeInfo), m_removeInfo);
            xs.WriteSerializeIgnored(xe, nameof(m_tempShapesToRemove));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpRemoveTerminalsMoppModifier);
        }

        public bool Equals(hkpRemoveTerminalsMoppModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_removeInfo.SequenceEqual(other.m_removeInfo) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_removeInfo.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

