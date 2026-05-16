using System.Xml.Linq;
namespace HKX2
{
    // hkbEventsFromRangeModifierInternalState Signatire: 0xcc47b48d size: 32 flags: FLAGS_NONE

    // m_wasActiveInPreviousFrame m_class:  Type.TYPE_ARRAY Type.TYPE_BOOL arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkbEventsFromRangeModifierInternalState : hkReferencedObject, IEquatable<hkbEventsFromRangeModifierInternalState?>
    {
        public IList<bool> m_wasActiveInPreviousFrame { set; get; } = Array.Empty<bool>();

        public override uint Signature { set; get; } = 0xcc47b48d;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_wasActiveInPreviousFrame = des.ReadBooleanArray(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteBooleanArray(bw, m_wasActiveInPreviousFrame);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_wasActiveInPreviousFrame = xd.ReadBooleanArray(xe, nameof(m_wasActiveInPreviousFrame));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteBooleanArray(xe, nameof(m_wasActiveInPreviousFrame), m_wasActiveInPreviousFrame);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbEventsFromRangeModifierInternalState);
        }

        public bool Equals(hkbEventsFromRangeModifierInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_wasActiveInPreviousFrame.SequenceEqual(other.m_wasActiveInPreviousFrame) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_wasActiveInPreviousFrame.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

