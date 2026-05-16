using System.Xml.Linq;
namespace HKX2
{
    // hkbBlenderGeneratorInternalState Signatire: 0x84717488 size: 64 flags: FLAGS_NONE

    // m_childrenInternalStates m_class: hkbBlenderGeneratorChildInternalState Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_sortedChildren m_class:  Type.TYPE_ARRAY Type.TYPE_INT16 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_endIntervalWeight m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_numActiveChildren m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 52 flags: FLAGS_NONE enum: 
    // m_beginIntervalIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 56 flags: FLAGS_NONE enum: 
    // m_endIntervalIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 58 flags: FLAGS_NONE enum: 
    // m_initSync m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 60 flags: FLAGS_NONE enum: 
    // m_doSubtractiveBlend m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 61 flags: FLAGS_NONE enum: 
    public partial class hkbBlenderGeneratorInternalState : hkReferencedObject, IEquatable<hkbBlenderGeneratorInternalState?>
    {
        public IList<hkbBlenderGeneratorChildInternalState> m_childrenInternalStates { set; get; } = Array.Empty<hkbBlenderGeneratorChildInternalState>();
        public IList<short> m_sortedChildren { set; get; } = Array.Empty<short>();
        public float m_endIntervalWeight { set; get; }
        public int m_numActiveChildren { set; get; }
        public short m_beginIntervalIndex { set; get; }
        public short m_endIntervalIndex { set; get; }
        public bool m_initSync { set; get; }
        public bool m_doSubtractiveBlend { set; get; }

        public override uint Signature { set; get; } = 0x84717488;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_childrenInternalStates = des.ReadClassArray<hkbBlenderGeneratorChildInternalState>(br);
            m_sortedChildren = des.ReadInt16Array(br);
            m_endIntervalWeight = br.ReadSingle();
            m_numActiveChildren = br.ReadInt32();
            m_beginIntervalIndex = br.ReadInt16();
            m_endIntervalIndex = br.ReadInt16();
            m_initSync = br.ReadBoolean();
            m_doSubtractiveBlend = br.ReadBoolean();
            br.Position += 2;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassArray(bw, m_childrenInternalStates);
            s.WriteInt16Array(bw, m_sortedChildren);
            bw.WriteSingle(m_endIntervalWeight);
            bw.WriteInt32(m_numActiveChildren);
            bw.WriteInt16(m_beginIntervalIndex);
            bw.WriteInt16(m_endIntervalIndex);
            bw.WriteBoolean(m_initSync);
            bw.WriteBoolean(m_doSubtractiveBlend);
            bw.Position += 2;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_childrenInternalStates = xd.ReadClassArray<hkbBlenderGeneratorChildInternalState>(xe, nameof(m_childrenInternalStates));
            m_sortedChildren = xd.ReadInt16Array(xe, nameof(m_sortedChildren));
            m_endIntervalWeight = xd.ReadSingle(xe, nameof(m_endIntervalWeight));
            m_numActiveChildren = xd.ReadInt32(xe, nameof(m_numActiveChildren));
            m_beginIntervalIndex = xd.ReadInt16(xe, nameof(m_beginIntervalIndex));
            m_endIntervalIndex = xd.ReadInt16(xe, nameof(m_endIntervalIndex));
            m_initSync = xd.ReadBoolean(xe, nameof(m_initSync));
            m_doSubtractiveBlend = xd.ReadBoolean(xe, nameof(m_doSubtractiveBlend));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassArray(xe, nameof(m_childrenInternalStates), m_childrenInternalStates);
            xs.WriteNumberArray(xe, nameof(m_sortedChildren), m_sortedChildren);
            xs.WriteFloat(xe, nameof(m_endIntervalWeight), m_endIntervalWeight);
            xs.WriteNumber(xe, nameof(m_numActiveChildren), m_numActiveChildren);
            xs.WriteNumber(xe, nameof(m_beginIntervalIndex), m_beginIntervalIndex);
            xs.WriteNumber(xe, nameof(m_endIntervalIndex), m_endIntervalIndex);
            xs.WriteBoolean(xe, nameof(m_initSync), m_initSync);
            xs.WriteBoolean(xe, nameof(m_doSubtractiveBlend), m_doSubtractiveBlend);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBlenderGeneratorInternalState);
        }

        public bool Equals(hkbBlenderGeneratorInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_childrenInternalStates.SequenceEqual(other.m_childrenInternalStates) &&
                   m_sortedChildren.SequenceEqual(other.m_sortedChildren) &&
                   m_endIntervalWeight.Equals(other.m_endIntervalWeight) &&
                   m_numActiveChildren.Equals(other.m_numActiveChildren) &&
                   m_beginIntervalIndex.Equals(other.m_beginIntervalIndex) &&
                   m_endIntervalIndex.Equals(other.m_endIntervalIndex) &&
                   m_initSync.Equals(other.m_initSync) &&
                   m_doSubtractiveBlend.Equals(other.m_doSubtractiveBlend) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_childrenInternalStates.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_sortedChildren.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_endIntervalWeight);
            hashcode.Add(m_numActiveChildren);
            hashcode.Add(m_beginIntervalIndex);
            hashcode.Add(m_endIntervalIndex);
            hashcode.Add(m_initSync);
            hashcode.Add(m_doSubtractiveBlend);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

