using System.Xml.Linq;
namespace HKX2
{
    // hkTrackerSerializableScanSnapshot Signatire: 0x875af1d9 size: 128 flags: FLAGS_NONE

    // m_allocations m_class: hkTrackerSerializableScanSnapshotAllocation Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_blocks m_class: hkTrackerSerializableScanSnapshotBlock Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_refs m_class:  Type.TYPE_ARRAY Type.TYPE_INT32 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_typeNames m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_traceText m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_traceAddrs m_class:  Type.TYPE_ARRAY Type.TYPE_UINT64 arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_traceParents m_class:  Type.TYPE_ARRAY Type.TYPE_INT32 arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkTrackerSerializableScanSnapshot : hkReferencedObject, IEquatable<hkTrackerSerializableScanSnapshot?>
    {
        public IList<hkTrackerSerializableScanSnapshotAllocation> m_allocations { set; get; } = Array.Empty<hkTrackerSerializableScanSnapshotAllocation>();
        public IList<hkTrackerSerializableScanSnapshotBlock> m_blocks { set; get; } = Array.Empty<hkTrackerSerializableScanSnapshotBlock>();
        public IList<int> m_refs { set; get; } = Array.Empty<int>();
        public IList<byte> m_typeNames { set; get; } = Array.Empty<byte>();
        public IList<byte> m_traceText { set; get; } = Array.Empty<byte>();
        public IList<ulong> m_traceAddrs { set; get; } = Array.Empty<ulong>();
        public IList<int> m_traceParents { set; get; } = Array.Empty<int>();

        public override uint Signature { set; get; } = 0x875af1d9;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_allocations = des.ReadClassArray<hkTrackerSerializableScanSnapshotAllocation>(br);
            m_blocks = des.ReadClassArray<hkTrackerSerializableScanSnapshotBlock>(br);
            m_refs = des.ReadInt32Array(br);
            m_typeNames = des.ReadByteArray(br);
            m_traceText = des.ReadByteArray(br);
            m_traceAddrs = des.ReadUInt64Array(br);
            m_traceParents = des.ReadInt32Array(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_allocations);
            s.WriteClassArray(bw, m_blocks);
            s.WriteInt32Array(bw, m_refs);
            s.WriteByteArray(bw, m_typeNames);
            s.WriteByteArray(bw, m_traceText);
            s.WriteUInt64Array(bw, m_traceAddrs);
            s.WriteInt32Array(bw, m_traceParents);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_allocations = xd.ReadClassArray<hkTrackerSerializableScanSnapshotAllocation>(xe, nameof(m_allocations));
            m_blocks = xd.ReadClassArray<hkTrackerSerializableScanSnapshotBlock>(xe, nameof(m_blocks));
            m_refs = xd.ReadInt32Array(xe, nameof(m_refs));
            m_typeNames = xd.ReadByteArray(xe, nameof(m_typeNames));
            m_traceText = xd.ReadByteArray(xe, nameof(m_traceText));
            m_traceAddrs = xd.ReadUInt64Array(xe, nameof(m_traceAddrs));
            m_traceParents = xd.ReadInt32Array(xe, nameof(m_traceParents));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_allocations), m_allocations);
            xs.WriteClassArray(xe, nameof(m_blocks), m_blocks);
            xs.WriteNumberArray(xe, nameof(m_refs), m_refs);
            xs.WriteNumberArray(xe, nameof(m_typeNames), m_typeNames);
            xs.WriteNumberArray(xe, nameof(m_traceText), m_traceText);
            xs.WriteNumberArray(xe, nameof(m_traceAddrs), m_traceAddrs);
            xs.WriteNumberArray(xe, nameof(m_traceParents), m_traceParents);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkTrackerSerializableScanSnapshot);
        }

        public bool Equals(hkTrackerSerializableScanSnapshot? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_allocations.SequenceEqual(other.m_allocations) &&
                   m_blocks.SequenceEqual(other.m_blocks) &&
                   m_refs.SequenceEqual(other.m_refs) &&
                   m_typeNames.SequenceEqual(other.m_typeNames) &&
                   m_traceText.SequenceEqual(other.m_traceText) &&
                   m_traceAddrs.SequenceEqual(other.m_traceAddrs) &&
                   m_traceParents.SequenceEqual(other.m_traceParents) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_allocations.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_blocks.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_refs.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_typeNames.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_traceText.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_traceAddrs.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_traceParents.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

