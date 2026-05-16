using System.Xml.Linq;
namespace HKX2
{
    // hkpSerializedDisplayMarkerList Signatire: 0x54785c77 size: 32 flags: FLAGS_NONE

    // m_markers m_class: hkpSerializedDisplayMarker Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    public partial class hkpSerializedDisplayMarkerList : hkReferencedObject, IEquatable<hkpSerializedDisplayMarkerList?>
    {
        public IList<hkpSerializedDisplayMarker> m_markers { set; get; } = Array.Empty<hkpSerializedDisplayMarker>();

        public override uint Signature { set; get; } = 0x54785c77;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_markers = des.ReadClassPointerArray<hkpSerializedDisplayMarker>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteClassPointerArray(bw, m_markers);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_markers = xd.ReadClassPointerArray<hkpSerializedDisplayMarker>(xe, nameof(m_markers));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClassPointerArray(xe, nameof(m_markers), m_markers);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpSerializedDisplayMarkerList);
        }

        public bool Equals(hkpSerializedDisplayMarkerList? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_markers.SequenceEqual(other.m_markers) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_markers.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

