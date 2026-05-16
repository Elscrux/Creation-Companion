using System.Xml.Linq;
namespace HKX2
{
    // hkpEntity Signatire: 0xa03c774b size: 720 flags: FLAGS_NONE

    // m_material m_class: hkpMaterial Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 208 flags: FLAGS_NONE enum: 
    // m_limitContactImpulseUtilAndFlag m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 224 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_damageMultiplier m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 232 flags: FLAGS_NONE enum: 
    // m_breakableBody m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 240 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_solverData m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 248 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_storageIndex m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 252 flags: FLAGS_NONE enum: 
    // m_contactPointCallbackDelay m_class:  Type.TYPE_UINT16 Type.TYPE_VOID arrSize: 0 offset: 254 flags: FLAGS_NONE enum: 
    // m_constraintsMaster m_class: hkpEntitySmallArraySerializeOverrideType Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 256 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_constraintsSlave m_class: hkpConstraintInstance Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 272 flags: SERIALIZE_IGNORED|NOT_OWNED|FLAGS_NONE enum: 
    // m_constraintRuntime m_class:  Type.TYPE_ARRAY Type.TYPE_UINT8 arrSize: 0 offset: 288 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_simulationIsland m_class:  Type.TYPE_POINTER Type.TYPE_VOID arrSize: 0 offset: 304 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_autoRemoveLevel m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 312 flags: FLAGS_NONE enum: 
    // m_numShapeKeysInContactPointProperties m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 313 flags: FLAGS_NONE enum: 
    // m_responseModifierFlags m_class:  Type.TYPE_UINT8 Type.TYPE_VOID arrSize: 0 offset: 314 flags: FLAGS_NONE enum: 
    // m_uid m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 316 flags: FLAGS_NONE enum: 
    // m_spuCollisionCallback m_class: hkpEntitySpuCollisionCallback Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 320 flags: FLAGS_NONE enum: 
    // m_motion m_class: hkpMaxSizeMotion Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 336 flags: FLAGS_NONE enum: 
    // m_contactListeners m_class: hkpEntitySmallArraySerializeOverrideType Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 656 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_actions m_class: hkpEntitySmallArraySerializeOverrideType Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 672 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_localFrame m_class: hkLocalFrame Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 688 flags: FLAGS_NONE enum: 
    // m_extendedListeners m_class: hkpEntityExtendedListeners Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 696 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_npData m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 704 flags: FLAGS_NONE enum: 
    public partial class hkpEntity : hkpWorldObject, IEquatable<hkpEntity?>
    {
        public hkpMaterial m_material { set; get; } = new();
        private object? m_limitContactImpulseUtilAndFlag { set; get; }
        public float m_damageMultiplier { set; get; }
        private object? m_breakableBody { set; get; }
        private uint m_solverData { set; get; }
        public ushort m_storageIndex { set; get; }
        public ushort m_contactPointCallbackDelay { set; get; }
        public hkpEntitySmallArraySerializeOverrideType m_constraintsMaster { set; get; } = new();
        public IList<hkpConstraintInstance> m_constraintsSlave { set; get; } = new List<hkpConstraintInstance>();
        public IList<byte> m_constraintRuntime { set; get; } = Array.Empty<byte>();
        private object? m_simulationIsland { set; get; }
        public sbyte m_autoRemoveLevel { set; get; }
        public byte m_numShapeKeysInContactPointProperties { set; get; }
        public byte m_responseModifierFlags { set; get; }
        public uint m_uid { set; get; }
        public hkpEntitySpuCollisionCallback m_spuCollisionCallback { set; get; } = new();
        public hkpMaxSizeMotion m_motion { set; get; } = new();
        public hkpEntitySmallArraySerializeOverrideType m_contactListeners { set; get; } = new();
        public hkpEntitySmallArraySerializeOverrideType m_actions { set; get; } = new();
        public hkLocalFrame? m_localFrame { set; get; }
        private hkpEntityExtendedListeners? m_extendedListeners { set; get; }
        public uint m_npData { set; get; }

        public override uint Signature { set; get; } = 0xa03c774b;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_material.Read(des, br);
            br.Position += 4;
            des.ReadEmptyPointer(br);
            m_damageMultiplier = br.ReadSingle();
            br.Position += 4;
            des.ReadEmptyPointer(br);
            m_solverData = br.ReadUInt32();
            m_storageIndex = br.ReadUInt16();
            m_contactPointCallbackDelay = br.ReadUInt16();
            m_constraintsMaster.Read(des, br);
            des.ReadEmptyArray(br);
            m_constraintRuntime = des.ReadByteArray(br);
            des.ReadEmptyPointer(br);
            m_autoRemoveLevel = br.ReadSByte();
            m_numShapeKeysInContactPointProperties = br.ReadByte();
            m_responseModifierFlags = br.ReadByte();
            br.Position += 1;
            m_uid = br.ReadUInt32();
            m_spuCollisionCallback.Read(des, br);
            m_motion.Read(des, br);
            m_contactListeners.Read(des, br);
            m_actions.Read(des, br);
            m_localFrame = des.ReadClassPointer<hkLocalFrame>(br);
            m_extendedListeners = des.ReadClassPointer<hkpEntityExtendedListeners>(br);
            m_npData = br.ReadUInt32();
            br.Position += 12;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_material.Write(s, bw);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            bw.WriteSingle(m_damageMultiplier);
            bw.Position += 4;
            s.WriteVoidPointer(bw);
            bw.WriteUInt32(m_solverData);
            bw.WriteUInt16(m_storageIndex);
            bw.WriteUInt16(m_contactPointCallbackDelay);
            m_constraintsMaster.Write(s, bw);
            s.WriteVoidArray(bw);
            s.WriteByteArray(bw, m_constraintRuntime);
            s.WriteVoidPointer(bw);
            bw.WriteSByte(m_autoRemoveLevel);
            bw.WriteByte(m_numShapeKeysInContactPointProperties);
            bw.WriteByte(m_responseModifierFlags);
            bw.Position += 1;
            bw.WriteUInt32(m_uid);
            m_spuCollisionCallback.Write(s, bw);
            m_motion.Write(s, bw);
            m_contactListeners.Write(s, bw);
            m_actions.Write(s, bw);
            s.WriteClassPointer(bw, m_localFrame);
            s.WriteClassPointer(bw, m_extendedListeners);
            bw.WriteUInt32(m_npData);
            bw.Position += 12;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_material = xd.ReadClass<hkpMaterial>(xe, nameof(m_material));
            m_damageMultiplier = xd.ReadSingle(xe, nameof(m_damageMultiplier));
            m_storageIndex = xd.ReadUInt16(xe, nameof(m_storageIndex));
            m_contactPointCallbackDelay = xd.ReadUInt16(xe, nameof(m_contactPointCallbackDelay));
            m_autoRemoveLevel = xd.ReadSByte(xe, nameof(m_autoRemoveLevel));
            m_numShapeKeysInContactPointProperties = xd.ReadByte(xe, nameof(m_numShapeKeysInContactPointProperties));
            m_responseModifierFlags = xd.ReadByte(xe, nameof(m_responseModifierFlags));
            m_uid = xd.ReadUInt32(xe, nameof(m_uid));
            m_spuCollisionCallback = xd.ReadClass<hkpEntitySpuCollisionCallback>(xe, nameof(m_spuCollisionCallback));
            m_motion = xd.ReadClass<hkpMaxSizeMotion>(xe, nameof(m_motion));
            m_localFrame = xd.ReadClassPointer<hkLocalFrame>(xe, nameof(m_localFrame));
            m_npData = xd.ReadUInt32(xe, nameof(m_npData));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkpMaterial>(xe, nameof(m_material), m_material);
            xs.WriteSerializeIgnored(xe, nameof(m_limitContactImpulseUtilAndFlag));
            xs.WriteFloat(xe, nameof(m_damageMultiplier), m_damageMultiplier);
            xs.WriteSerializeIgnored(xe, nameof(m_breakableBody));
            xs.WriteSerializeIgnored(xe, nameof(m_solverData));
            xs.WriteNumber(xe, nameof(m_storageIndex), m_storageIndex);
            xs.WriteNumber(xe, nameof(m_contactPointCallbackDelay), m_contactPointCallbackDelay);
            xs.WriteSerializeIgnored(xe, nameof(m_constraintsMaster));
            xs.WriteSerializeIgnored(xe, nameof(m_constraintsSlave));
            xs.WriteSerializeIgnored(xe, nameof(m_constraintRuntime));
            xs.WriteSerializeIgnored(xe, nameof(m_simulationIsland));
            xs.WriteNumber(xe, nameof(m_autoRemoveLevel), m_autoRemoveLevel);
            xs.WriteNumber(xe, nameof(m_numShapeKeysInContactPointProperties), m_numShapeKeysInContactPointProperties);
            xs.WriteNumber(xe, nameof(m_responseModifierFlags), m_responseModifierFlags);
            xs.WriteNumber(xe, nameof(m_uid), m_uid);
            xs.WriteClass(xe, nameof(m_spuCollisionCallback), m_spuCollisionCallback);
            xs.WriteClass<hkpMaxSizeMotion>(xe, nameof(m_motion), m_motion);
            xs.WriteSerializeIgnored(xe, nameof(m_contactListeners));
            xs.WriteSerializeIgnored(xe, nameof(m_actions));
            xs.WriteClassPointer(xe, nameof(m_localFrame), m_localFrame);
            xs.WriteSerializeIgnored(xe, nameof(m_extendedListeners));
            xs.WriteNumber(xe, nameof(m_npData), m_npData);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpEntity);
        }

        public bool Equals(hkpEntity? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_material is null && other.m_material is null) || (m_material is not null && other.m_material is not null && m_material.Equals((IHavokObject)other.m_material))) &&
                   m_damageMultiplier.Equals(other.m_damageMultiplier) &&
                   m_storageIndex.Equals(other.m_storageIndex) &&
                   m_contactPointCallbackDelay.Equals(other.m_contactPointCallbackDelay) &&
                   m_autoRemoveLevel.Equals(other.m_autoRemoveLevel) &&
                   m_numShapeKeysInContactPointProperties.Equals(other.m_numShapeKeysInContactPointProperties) &&
                   m_responseModifierFlags.Equals(other.m_responseModifierFlags) &&
                   m_uid.Equals(other.m_uid) &&
                   ((m_spuCollisionCallback is null && other.m_spuCollisionCallback is null) || (m_spuCollisionCallback is not null && other.m_spuCollisionCallback is not null && m_spuCollisionCallback.Equals((IHavokObject)other.m_spuCollisionCallback))) &&
                   ((m_motion is null && other.m_motion is null) || (m_motion is not null && other.m_motion is not null && m_motion.Equals((IHavokObject)other.m_motion))) &&
                   ((m_localFrame is null && other.m_localFrame is null) || (m_localFrame is not null && other.m_localFrame is not null && m_localFrame.Equals((IHavokObject)other.m_localFrame))) &&
                   m_npData.Equals(other.m_npData) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_material);
            hashcode.Add(m_damageMultiplier);
            hashcode.Add(m_storageIndex);
            hashcode.Add(m_contactPointCallbackDelay);
            hashcode.Add(m_autoRemoveLevel);
            hashcode.Add(m_numShapeKeysInContactPointProperties);
            hashcode.Add(m_responseModifierFlags);
            hashcode.Add(m_uid);
            hashcode.Add(m_spuCollisionCallback);
            hashcode.Add(m_motion);
            hashcode.Add(m_localFrame);
            hashcode.Add(m_npData);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

