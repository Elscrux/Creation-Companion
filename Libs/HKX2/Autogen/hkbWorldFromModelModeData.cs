using System.Xml.Linq;
namespace HKX2
{
    // hkbWorldFromModelModeData Signatire: 0xa3af8783 size: 8 flags: FLAGS_NONE

    // m_poseMatchingBone0 m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 0 flags: FLAGS_NONE enum: 
    // m_poseMatchingBone1 m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 2 flags: FLAGS_NONE enum: 
    // m_poseMatchingBone2 m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 4 flags: FLAGS_NONE enum: 
    // m_mode m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 6 flags: FLAGS_NONE enum: WorldFromModelMode
    public partial class hkbWorldFromModelModeData : IHavokObject, IEquatable<hkbWorldFromModelModeData?>
    {
        public short m_poseMatchingBone0 { set; get; }
        public short m_poseMatchingBone1 { set; get; }
        public short m_poseMatchingBone2 { set; get; }
        public sbyte m_mode { set; get; }

        public virtual uint Signature { set; get; } = 0xa3af8783;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_poseMatchingBone0 = br.ReadInt16();
            m_poseMatchingBone1 = br.ReadInt16();
            m_poseMatchingBone2 = br.ReadInt16();
            m_mode = br.ReadSByte();
            br.Position += 1;
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            bw.WriteInt16(m_poseMatchingBone0);
            bw.WriteInt16(m_poseMatchingBone1);
            bw.WriteInt16(m_poseMatchingBone2);
            bw.WriteSByte(m_mode);
            bw.Position += 1;
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_poseMatchingBone0 = xd.ReadInt16(xe, nameof(m_poseMatchingBone0));
            m_poseMatchingBone1 = xd.ReadInt16(xe, nameof(m_poseMatchingBone1));
            m_poseMatchingBone2 = xd.ReadInt16(xe, nameof(m_poseMatchingBone2));
            m_mode = xd.ReadFlag<WorldFromModelMode, sbyte>(xe, nameof(m_mode));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteNumber(xe, nameof(m_poseMatchingBone0), m_poseMatchingBone0);
            xs.WriteNumber(xe, nameof(m_poseMatchingBone1), m_poseMatchingBone1);
            xs.WriteNumber(xe, nameof(m_poseMatchingBone2), m_poseMatchingBone2);
            xs.WriteEnum<WorldFromModelMode, sbyte>(xe, nameof(m_mode), m_mode);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbWorldFromModelModeData);
        }

        public bool Equals(hkbWorldFromModelModeData? other)
        {
            return other is not null &&
                   m_poseMatchingBone0.Equals(other.m_poseMatchingBone0) &&
                   m_poseMatchingBone1.Equals(other.m_poseMatchingBone1) &&
                   m_poseMatchingBone2.Equals(other.m_poseMatchingBone2) &&
                   m_mode.Equals(other.m_mode) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_poseMatchingBone0);
            hashcode.Add(m_poseMatchingBone1);
            hashcode.Add(m_poseMatchingBone2);
            hashcode.Add(m_mode);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

