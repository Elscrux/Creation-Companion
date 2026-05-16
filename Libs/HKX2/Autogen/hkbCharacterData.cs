using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbCharacterData Signatire: 0x300d6808 size: 176 flags: FLAGS_NONE

    // m_characterControllerInfo m_class: hkbCharacterDataCharacterControllerInfo Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_modelUpMS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_modelForwardMS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_modelRightMS m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_characterPropertyInfos m_class: hkbVariableInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_numBonesPerLod m_class:  Type.TYPE_ARRAY Type.TYPE_INT32 arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_characterPropertyValues m_class: hkbVariableValueSet Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_footIkDriverInfo m_class: hkbFootIkDriverInfo Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 136 flags: FLAGS_NONE enum: 
    // m_handIkDriverInfo m_class: hkbHandIkDriverInfo Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_stringData m_class: hkbCharacterStringData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 152 flags: FLAGS_NONE enum: 
    // m_mirroredSkeletonInfo m_class: hkbMirroredSkeletonInfo Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_scale m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 168 flags: FLAGS_NONE enum: 
    // m_numHands m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 172 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_numFloatSlots m_class:  Type.TYPE_INT16 Type.TYPE_VOID arrSize: 0 offset: 174 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkbCharacterData : hkReferencedObject, IEquatable<hkbCharacterData?>
    {
        public hkbCharacterDataCharacterControllerInfo m_characterControllerInfo { set; get; } = new();
        public Vector4 m_modelUpMS { set; get; }
        public Vector4 m_modelForwardMS { set; get; }
        public Vector4 m_modelRightMS { set; get; }
        public IList<hkbVariableInfo> m_characterPropertyInfos { set; get; } = Array.Empty<hkbVariableInfo>();
        public IList<int> m_numBonesPerLod { set; get; } = Array.Empty<int>();
        public hkbVariableValueSet? m_characterPropertyValues { set; get; }
        public hkbFootIkDriverInfo? m_footIkDriverInfo { set; get; }
        public hkbHandIkDriverInfo? m_handIkDriverInfo { set; get; }
        public hkbCharacterStringData? m_stringData { set; get; }
        public hkbMirroredSkeletonInfo? m_mirroredSkeletonInfo { set; get; }
        public float m_scale { set; get; }
        private short m_numHands { set; get; }
        private short m_numFloatSlots { set; get; }

        public override uint Signature { set; get; } = 0x300d6808;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_characterControllerInfo.Read(des, br);
            br.Position += 8;
            m_modelUpMS = br.ReadVector4();
            m_modelForwardMS = br.ReadVector4();
            m_modelRightMS = br.ReadVector4();
            m_characterPropertyInfos = des.ReadClassArray<hkbVariableInfo>(br);
            m_numBonesPerLod = des.ReadInt32Array(br);
            m_characterPropertyValues = des.ReadClassPointer<hkbVariableValueSet>(br);
            m_footIkDriverInfo = des.ReadClassPointer<hkbFootIkDriverInfo>(br);
            m_handIkDriverInfo = des.ReadClassPointer<hkbHandIkDriverInfo>(br);
            m_stringData = des.ReadClassPointer<hkbCharacterStringData>(br);
            m_mirroredSkeletonInfo = des.ReadClassPointer<hkbMirroredSkeletonInfo>(br);
            m_scale = br.ReadSingle();
            m_numHands = br.ReadInt16();
            m_numFloatSlots = br.ReadInt16();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            m_characterControllerInfo.Write(s, bw);
            bw.Position += 8;
            bw.WriteVector4(m_modelUpMS);
            bw.WriteVector4(m_modelForwardMS);
            bw.WriteVector4(m_modelRightMS);
            s.WriteClassArray(bw, m_characterPropertyInfos);
            s.WriteInt32Array(bw, m_numBonesPerLod);
            s.WriteClassPointer(bw, m_characterPropertyValues);
            s.WriteClassPointer(bw, m_footIkDriverInfo);
            s.WriteClassPointer(bw, m_handIkDriverInfo);
            s.WriteClassPointer(bw, m_stringData);
            s.WriteClassPointer(bw, m_mirroredSkeletonInfo);
            bw.WriteSingle(m_scale);
            bw.WriteInt16(m_numHands);
            bw.WriteInt16(m_numFloatSlots);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_characterControllerInfo = xd.ReadClass<hkbCharacterDataCharacterControllerInfo>(xe, nameof(m_characterControllerInfo));
            m_modelUpMS = xd.ReadVector4(xe, nameof(m_modelUpMS));
            m_modelForwardMS = xd.ReadVector4(xe, nameof(m_modelForwardMS));
            m_modelRightMS = xd.ReadVector4(xe, nameof(m_modelRightMS));
            m_characterPropertyInfos = xd.ReadClassArray<hkbVariableInfo>(xe, nameof(m_characterPropertyInfos));
            m_numBonesPerLod = xd.ReadInt32Array(xe, nameof(m_numBonesPerLod));
            m_characterPropertyValues = xd.ReadClassPointer<hkbVariableValueSet>(xe, nameof(m_characterPropertyValues));
            m_footIkDriverInfo = xd.ReadClassPointer<hkbFootIkDriverInfo>(xe, nameof(m_footIkDriverInfo));
            m_handIkDriverInfo = xd.ReadClassPointer<hkbHandIkDriverInfo>(xe, nameof(m_handIkDriverInfo));
            m_stringData = xd.ReadClassPointer<hkbCharacterStringData>(xe, nameof(m_stringData));
            m_mirroredSkeletonInfo = xd.ReadClassPointer<hkbMirroredSkeletonInfo>(xe, nameof(m_mirroredSkeletonInfo));
            m_scale = xd.ReadSingle(xe, nameof(m_scale));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteClass<hkbCharacterDataCharacterControllerInfo>(xe, nameof(m_characterControllerInfo), m_characterControllerInfo);
            xs.WriteVector4(xe, nameof(m_modelUpMS), m_modelUpMS);
            xs.WriteVector4(xe, nameof(m_modelForwardMS), m_modelForwardMS);
            xs.WriteVector4(xe, nameof(m_modelRightMS), m_modelRightMS);
            xs.WriteClassArray(xe, nameof(m_characterPropertyInfos), m_characterPropertyInfos);
            xs.WriteNumberArray(xe, nameof(m_numBonesPerLod), m_numBonesPerLod);
            xs.WriteClassPointer(xe, nameof(m_characterPropertyValues), m_characterPropertyValues);
            xs.WriteClassPointer(xe, nameof(m_footIkDriverInfo), m_footIkDriverInfo);
            xs.WriteClassPointer(xe, nameof(m_handIkDriverInfo), m_handIkDriverInfo);
            xs.WriteClassPointer(xe, nameof(m_stringData), m_stringData);
            xs.WriteClassPointer(xe, nameof(m_mirroredSkeletonInfo), m_mirroredSkeletonInfo);
            xs.WriteFloat(xe, nameof(m_scale), m_scale);
            xs.WriteSerializeIgnored(xe, nameof(m_numHands));
            xs.WriteSerializeIgnored(xe, nameof(m_numFloatSlots));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbCharacterData);
        }

        public bool Equals(hkbCharacterData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   ((m_characterControllerInfo is null && other.m_characterControllerInfo is null) || (m_characterControllerInfo is not null && other.m_characterControllerInfo is not null && m_characterControllerInfo.Equals((IHavokObject)other.m_characterControllerInfo))) &&
                   m_modelUpMS.Equals(other.m_modelUpMS) &&
                   m_modelForwardMS.Equals(other.m_modelForwardMS) &&
                   m_modelRightMS.Equals(other.m_modelRightMS) &&
                   m_characterPropertyInfos.SequenceEqual(other.m_characterPropertyInfos) &&
                   m_numBonesPerLod.SequenceEqual(other.m_numBonesPerLod) &&
                   ((m_characterPropertyValues is null && other.m_characterPropertyValues is null) || (m_characterPropertyValues is not null && other.m_characterPropertyValues is not null && m_characterPropertyValues.Equals((IHavokObject)other.m_characterPropertyValues))) &&
                   ((m_footIkDriverInfo is null && other.m_footIkDriverInfo is null) || (m_footIkDriverInfo is not null && other.m_footIkDriverInfo is not null && m_footIkDriverInfo.Equals((IHavokObject)other.m_footIkDriverInfo))) &&
                   ((m_handIkDriverInfo is null && other.m_handIkDriverInfo is null) || (m_handIkDriverInfo is not null && other.m_handIkDriverInfo is not null && m_handIkDriverInfo.Equals((IHavokObject)other.m_handIkDriverInfo))) &&
                   ((m_stringData is null && other.m_stringData is null) || (m_stringData is not null && other.m_stringData is not null && m_stringData.Equals((IHavokObject)other.m_stringData))) &&
                   ((m_mirroredSkeletonInfo is null && other.m_mirroredSkeletonInfo is null) || (m_mirroredSkeletonInfo is not null && other.m_mirroredSkeletonInfo is not null && m_mirroredSkeletonInfo.Equals((IHavokObject)other.m_mirroredSkeletonInfo))) &&
                   m_scale.Equals(other.m_scale) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_characterControllerInfo);
            hashcode.Add(m_modelUpMS);
            hashcode.Add(m_modelForwardMS);
            hashcode.Add(m_modelRightMS);
            hashcode.Add(m_characterPropertyInfos.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_numBonesPerLod.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_characterPropertyValues);
            hashcode.Add(m_footIkDriverInfo);
            hashcode.Add(m_handIkDriverInfo);
            hashcode.Add(m_stringData);
            hashcode.Add(m_mirroredSkeletonInfo);
            hashcode.Add(m_scale);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

