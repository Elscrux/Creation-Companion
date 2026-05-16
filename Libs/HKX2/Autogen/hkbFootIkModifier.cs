using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbFootIkModifier Signatire: 0xed8966c0 size: 256 flags: FLAGS_NONE

    // m_gains m_class: hkbFootIkGains Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_legs m_class: hkbFootIkModifierLeg Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_raycastDistanceUp m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_raycastDistanceDown m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 148 flags: FLAGS_NONE enum: 
    // m_originalGroundHeightMS m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    // m_errorOut m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 156 flags: FLAGS_NONE enum: 
    // m_errorOutTranslation m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_alignWithGroundRotation m_class:  Type.TYPE_QUATERNION Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_verticalOffset m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 192 flags: FLAGS_NONE enum: 
    // m_collisionFilterInfo m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 196 flags: FLAGS_NONE enum: 
    // m_forwardAlignFraction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 200 flags: FLAGS_NONE enum: 
    // m_sidewaysAlignFraction m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 204 flags: FLAGS_NONE enum: 
    // m_sidewaysSampleWidth m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 208 flags: FLAGS_NONE enum: 
    // m_useTrackData m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 212 flags: FLAGS_NONE enum: 
    // m_lockFeetWhenPlanted m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 213 flags: FLAGS_NONE enum: 
    // m_useCharacterUpVector m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 214 flags: FLAGS_NONE enum: 
    // m_alignMode m_class:  Type.TYPE_ENUM Type.TYPE_INT8 arrSize: 0 offset: 215 flags: FLAGS_NONE enum: AlignMode
    // m_internalLegData m_class: hkbFootIkModifierInternalLegData Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 216 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_prevIsFootIkEnabled m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 232 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_isSetUp m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 236 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_isGroundPositionValid m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 237 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_timeStep m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 240 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbFootIkModifier : hkbModifier, IEquatable<hkbFootIkModifier?>
    {
        public hkbFootIkGains m_gains { set; get; } = new();
        public IList<hkbFootIkModifierLeg> m_legs { set; get; } = Array.Empty<hkbFootIkModifierLeg>();
        public float m_raycastDistanceUp { set; get; }
        public float m_raycastDistanceDown { set; get; }
        public float m_originalGroundHeightMS { set; get; }
        public float m_errorOut { set; get; }
        public Vector4 m_errorOutTranslation { set; get; }
        public Quaternion m_alignWithGroundRotation { set; get; }
        public float m_verticalOffset { set; get; }
        public uint m_collisionFilterInfo { set; get; }
        public float m_forwardAlignFraction { set; get; }
        public float m_sidewaysAlignFraction { set; get; }
        public float m_sidewaysSampleWidth { set; get; }
        public bool m_useTrackData { set; get; }
        public bool m_lockFeetWhenPlanted { set; get; }
        public bool m_useCharacterUpVector { set; get; }
        public sbyte m_alignMode { set; get; }
        public IList<hkbFootIkModifierInternalLegData> m_internalLegData { set; get; } = Array.Empty<hkbFootIkModifierInternalLegData>();
        private float m_prevIsFootIkEnabled { set; get; }
        private bool m_isSetUp { set; get; }
        private bool m_isGroundPositionValid { set; get; }
        private float m_timeStep { set; get; }

        public override uint Signature { set; get; } = 0xed8966c0;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_gains.Read(des, br);
            m_legs = des.ReadClassArray<hkbFootIkModifierLeg>(br);
            m_raycastDistanceUp = br.ReadSingle();
            m_raycastDistanceDown = br.ReadSingle();
            m_originalGroundHeightMS = br.ReadSingle();
            m_errorOut = br.ReadSingle();
            m_errorOutTranslation = br.ReadVector4();
            m_alignWithGroundRotation = des.ReadQuaternion(br);
            m_verticalOffset = br.ReadSingle();
            m_collisionFilterInfo = br.ReadUInt32();
            m_forwardAlignFraction = br.ReadSingle();
            m_sidewaysAlignFraction = br.ReadSingle();
            m_sidewaysSampleWidth = br.ReadSingle();
            m_useTrackData = br.ReadBoolean();
            m_lockFeetWhenPlanted = br.ReadBoolean();
            m_useCharacterUpVector = br.ReadBoolean();
            m_alignMode = br.ReadSByte();
            m_internalLegData = des.ReadClassArray<hkbFootIkModifierInternalLegData>(br);
            m_prevIsFootIkEnabled = br.ReadSingle();
            m_isSetUp = br.ReadBoolean();
            m_isGroundPositionValid = br.ReadBoolean();
            br.Position += 2;
            m_timeStep = br.ReadSingle();
            br.Position += 12;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_gains.Write(s, bw);
            s.WriteClassArray(bw, m_legs);
            bw.WriteSingle(m_raycastDistanceUp);
            bw.WriteSingle(m_raycastDistanceDown);
            bw.WriteSingle(m_originalGroundHeightMS);
            bw.WriteSingle(m_errorOut);
            bw.WriteVector4(m_errorOutTranslation);
            s.WriteQuaternion(bw, m_alignWithGroundRotation);
            bw.WriteSingle(m_verticalOffset);
            bw.WriteUInt32(m_collisionFilterInfo);
            bw.WriteSingle(m_forwardAlignFraction);
            bw.WriteSingle(m_sidewaysAlignFraction);
            bw.WriteSingle(m_sidewaysSampleWidth);
            bw.WriteBoolean(m_useTrackData);
            bw.WriteBoolean(m_lockFeetWhenPlanted);
            bw.WriteBoolean(m_useCharacterUpVector);
            bw.WriteSByte(m_alignMode);
            s.WriteClassArray(bw, m_internalLegData);
            bw.WriteSingle(m_prevIsFootIkEnabled);
            bw.WriteBoolean(m_isSetUp);
            bw.WriteBoolean(m_isGroundPositionValid);
            bw.Position += 2;
            bw.WriteSingle(m_timeStep);
            bw.Position += 12;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_gains = xd.ReadClass<hkbFootIkGains>(xe, nameof(m_gains));
            m_legs = xd.ReadClassArray<hkbFootIkModifierLeg>(xe, nameof(m_legs));
            m_raycastDistanceUp = xd.ReadSingle(xe, nameof(m_raycastDistanceUp));
            m_raycastDistanceDown = xd.ReadSingle(xe, nameof(m_raycastDistanceDown));
            m_originalGroundHeightMS = xd.ReadSingle(xe, nameof(m_originalGroundHeightMS));
            m_errorOut = xd.ReadSingle(xe, nameof(m_errorOut));
            m_errorOutTranslation = xd.ReadVector4(xe, nameof(m_errorOutTranslation));
            m_alignWithGroundRotation = xd.ReadQuaternion(xe, nameof(m_alignWithGroundRotation));
            m_verticalOffset = xd.ReadSingle(xe, nameof(m_verticalOffset));
            m_collisionFilterInfo = xd.ReadUInt32(xe, nameof(m_collisionFilterInfo));
            m_forwardAlignFraction = xd.ReadSingle(xe, nameof(m_forwardAlignFraction));
            m_sidewaysAlignFraction = xd.ReadSingle(xe, nameof(m_sidewaysAlignFraction));
            m_sidewaysSampleWidth = xd.ReadSingle(xe, nameof(m_sidewaysSampleWidth));
            m_useTrackData = xd.ReadBoolean(xe, nameof(m_useTrackData));
            m_lockFeetWhenPlanted = xd.ReadBoolean(xe, nameof(m_lockFeetWhenPlanted));
            m_useCharacterUpVector = xd.ReadBoolean(xe, nameof(m_useCharacterUpVector));
            m_alignMode = xd.ReadFlag<AlignMode, sbyte>(xe, nameof(m_alignMode));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbFootIkGains>(xe, nameof(m_gains), m_gains);
            xs.WriteClassArray(xe, nameof(m_legs), m_legs);
            xs.WriteFloat(xe, nameof(m_raycastDistanceUp), m_raycastDistanceUp);
            xs.WriteFloat(xe, nameof(m_raycastDistanceDown), m_raycastDistanceDown);
            xs.WriteFloat(xe, nameof(m_originalGroundHeightMS), m_originalGroundHeightMS);
            xs.WriteFloat(xe, nameof(m_errorOut), m_errorOut);
            xs.WriteVector4(xe, nameof(m_errorOutTranslation), m_errorOutTranslation);
            xs.WriteQuaternion(xe, nameof(m_alignWithGroundRotation), m_alignWithGroundRotation);
            xs.WriteFloat(xe, nameof(m_verticalOffset), m_verticalOffset);
            xs.WriteNumber(xe, nameof(m_collisionFilterInfo), m_collisionFilterInfo);
            xs.WriteFloat(xe, nameof(m_forwardAlignFraction), m_forwardAlignFraction);
            xs.WriteFloat(xe, nameof(m_sidewaysAlignFraction), m_sidewaysAlignFraction);
            xs.WriteFloat(xe, nameof(m_sidewaysSampleWidth), m_sidewaysSampleWidth);
            xs.WriteBoolean(xe, nameof(m_useTrackData), m_useTrackData);
            xs.WriteBoolean(xe, nameof(m_lockFeetWhenPlanted), m_lockFeetWhenPlanted);
            xs.WriteBoolean(xe, nameof(m_useCharacterUpVector), m_useCharacterUpVector);
            xs.WriteEnum<AlignMode, sbyte>(xe, nameof(m_alignMode), m_alignMode);
            xs.WriteSerializeIgnored(xe, nameof(m_internalLegData));
            xs.WriteSerializeIgnored(xe, nameof(m_prevIsFootIkEnabled));
            xs.WriteSerializeIgnored(xe, nameof(m_isSetUp));
            xs.WriteSerializeIgnored(xe, nameof(m_isGroundPositionValid));
            xs.WriteSerializeIgnored(xe, nameof(m_timeStep));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbFootIkModifier);
        }

        public bool Equals(hkbFootIkModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_gains is null && other.m_gains is null) || (m_gains is not null && other.m_gains is not null && m_gains.Equals((IHavokObject)other.m_gains))) &&
                   m_legs.SequenceEqual(other.m_legs) &&
                   m_raycastDistanceUp.Equals(other.m_raycastDistanceUp) &&
                   m_raycastDistanceDown.Equals(other.m_raycastDistanceDown) &&
                   m_originalGroundHeightMS.Equals(other.m_originalGroundHeightMS) &&
                   m_errorOut.Equals(other.m_errorOut) &&
                   m_errorOutTranslation.Equals(other.m_errorOutTranslation) &&
                   m_alignWithGroundRotation.Equals(other.m_alignWithGroundRotation) &&
                   m_verticalOffset.Equals(other.m_verticalOffset) &&
                   m_collisionFilterInfo.Equals(other.m_collisionFilterInfo) &&
                   m_forwardAlignFraction.Equals(other.m_forwardAlignFraction) &&
                   m_sidewaysAlignFraction.Equals(other.m_sidewaysAlignFraction) &&
                   m_sidewaysSampleWidth.Equals(other.m_sidewaysSampleWidth) &&
                   m_useTrackData.Equals(other.m_useTrackData) &&
                   m_lockFeetWhenPlanted.Equals(other.m_lockFeetWhenPlanted) &&
                   m_useCharacterUpVector.Equals(other.m_useCharacterUpVector) &&
                   m_alignMode.Equals(other.m_alignMode) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_gains);
            hashcode.Add(m_legs.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_raycastDistanceUp);
            hashcode.Add(m_raycastDistanceDown);
            hashcode.Add(m_originalGroundHeightMS);
            hashcode.Add(m_errorOut);
            hashcode.Add(m_errorOutTranslation);
            hashcode.Add(m_alignWithGroundRotation);
            hashcode.Add(m_verticalOffset);
            hashcode.Add(m_collisionFilterInfo);
            hashcode.Add(m_forwardAlignFraction);
            hashcode.Add(m_sidewaysAlignFraction);
            hashcode.Add(m_sidewaysSampleWidth);
            hashcode.Add(m_useTrackData);
            hashcode.Add(m_lockFeetWhenPlanted);
            hashcode.Add(m_useCharacterUpVector);
            hashcode.Add(m_alignMode);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

