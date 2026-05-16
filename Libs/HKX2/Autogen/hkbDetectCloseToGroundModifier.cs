using System.Xml.Linq;
namespace HKX2
{
    // hkbDetectCloseToGroundModifier Signatire: 0x981687b2 size: 120 flags: FLAGS_NONE

    // m_closeToGroundEvent m_class: hkbEventProperty Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_closeToGroundHeight m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_raycastDistanceDown m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_collisionFilterInfo m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_boneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 108 flags: FLAGS_NONE enum: 
    // m_animBoneIndex m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 110 flags: FLAGS_NONE enum: 
    // m_isCloseToGround m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 112 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbDetectCloseToGroundModifier : hkbModifier, IEquatable<hkbDetectCloseToGroundModifier?>
    {
        public hkbEventProperty m_closeToGroundEvent { set; get; } = new();
        public float m_closeToGroundHeight { set; get; }
        public float m_raycastDistanceDown { set; get; }
        public uint m_collisionFilterInfo { set; get; }
        public short m_boneIndex { set; get; }
        public short m_animBoneIndex { set; get; }
        private bool m_isCloseToGround { set; get; }

        public override uint Signature { set; get; } = 0x981687b2;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_closeToGroundEvent.Read(des, br);
            m_closeToGroundHeight = br.ReadSingle();
            m_raycastDistanceDown = br.ReadSingle();
            m_collisionFilterInfo = br.ReadUInt32();
            m_boneIndex = br.ReadInt16();
            m_animBoneIndex = br.ReadInt16();
            m_isCloseToGround = br.ReadBoolean();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_closeToGroundEvent.Write(s, bw);
            bw.WriteSingle(m_closeToGroundHeight);
            bw.WriteSingle(m_raycastDistanceDown);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteInt16(m_boneIndex);
            bw.WriteInt16(m_animBoneIndex);
            bw.WriteBoolean(m_isCloseToGround);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_closeToGroundEvent = xd.ReadClass<hkbEventProperty>(xe, nameof(m_closeToGroundEvent));
            m_closeToGroundHeight = xd.ReadSingle(xe, nameof(m_closeToGroundHeight));
            m_raycastDistanceDown = xd.ReadSingle(xe, nameof(m_raycastDistanceDown));
            m_collisionFilterInfo = xd.ReadUInt32(xe, nameof(m_collisionFilterInfo));
            m_boneIndex = xd.ReadInt16(xe, nameof(m_boneIndex));
            m_animBoneIndex = xd.ReadInt16(xe, nameof(m_animBoneIndex));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbEventProperty>(xe, nameof(m_closeToGroundEvent), m_closeToGroundEvent);
            xs.WriteFloat(xe, nameof(m_closeToGroundHeight), m_closeToGroundHeight);
            xs.WriteFloat(xe, nameof(m_raycastDistanceDown), m_raycastDistanceDown);
            xs.WriteNumber(xe, nameof(m_collisionFilterInfo), m_collisionFilterInfo);
            xs.WriteNumber(xe, nameof(m_boneIndex), m_boneIndex);
            xs.WriteNumber(xe, nameof(m_animBoneIndex), m_animBoneIndex);
            xs.WriteSerializeIgnored(xe, nameof(m_isCloseToGround));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbDetectCloseToGroundModifier);
        }

        public bool Equals(hkbDetectCloseToGroundModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_closeToGroundEvent is null && other.m_closeToGroundEvent is null) || (m_closeToGroundEvent is not null && other.m_closeToGroundEvent is not null && m_closeToGroundEvent.Equals((IHavokObject)other.m_closeToGroundEvent))) &&
                   m_closeToGroundHeight.Equals(other.m_closeToGroundHeight) &&
                   m_raycastDistanceDown.Equals(other.m_raycastDistanceDown) &&
                   m_collisionFilterInfo.Equals(other.m_collisionFilterInfo) &&
                   m_boneIndex.Equals(other.m_boneIndex) &&
                   m_animBoneIndex.Equals(other.m_animBoneIndex) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_closeToGroundEvent);
            hashcode.Add(m_closeToGroundHeight);
            hashcode.Add(m_raycastDistanceDown);
            hashcode.Add(m_collisionFilterInfo);
            hashcode.Add(m_boneIndex);
            hashcode.Add(m_animBoneIndex);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

