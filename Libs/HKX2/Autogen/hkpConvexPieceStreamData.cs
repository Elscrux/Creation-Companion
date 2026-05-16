using System.Xml.Linq;
namespace HKX2
{
    // hkpConvexPieceStreamData Signatire: 0xa5bd1d6e size: 64 flags: FLAGS_NONE

    // m_convexPieceStream m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_convexPieceOffsets m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_convexPieceSingleTriangles m_class:  Type.TYPE_ARRAY Type.TYPE_UINT32 arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    public partial class hkpConvexPieceStreamData : hkReferencedObject, IEquatable<hkpConvexPieceStreamData?>
    {
        public IList<uint> m_convexPieceStream { set; get; } = Array.Empty<uint>();
        public IList<uint> m_convexPieceOffsets { set; get; } = Array.Empty<uint>();
        public IList<uint> m_convexPieceSingleTriangles { set; get; } = Array.Empty<uint>();

        public override uint Signature { set; get; } = 0xa5bd1d6e;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_convexPieceStream = des.ReadUInt32Array(br);
            m_convexPieceOffsets = des.ReadUInt32Array(br);
            m_convexPieceSingleTriangles = des.ReadUInt32Array(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteUInt32Array(bw, m_convexPieceStream);
            s.WriteUInt32Array(bw, m_convexPieceOffsets);
            s.WriteUInt32Array(bw, m_convexPieceSingleTriangles);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_convexPieceStream = xd.ReadUInt32Array(xe, nameof(m_convexPieceStream));
            m_convexPieceOffsets = xd.ReadUInt32Array(xe, nameof(m_convexPieceOffsets));
            m_convexPieceSingleTriangles = xd.ReadUInt32Array(xe, nameof(m_convexPieceSingleTriangles));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteNumberArray(xe, nameof(m_convexPieceStream), m_convexPieceStream);
            xs.WriteNumberArray(xe, nameof(m_convexPieceOffsets), m_convexPieceOffsets);
            xs.WriteNumberArray(xe, nameof(m_convexPieceSingleTriangles), m_convexPieceSingleTriangles);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpConvexPieceStreamData);
        }

        public bool Equals(hkpConvexPieceStreamData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_convexPieceStream.SequenceEqual(other.m_convexPieceStream) &&
                   m_convexPieceOffsets.SequenceEqual(other.m_convexPieceOffsets) &&
                   m_convexPieceSingleTriangles.SequenceEqual(other.m_convexPieceSingleTriangles) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_convexPieceStream.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_convexPieceOffsets.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_convexPieceSingleTriangles.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

