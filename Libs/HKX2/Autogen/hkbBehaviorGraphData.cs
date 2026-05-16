using System.Xml.Linq;
namespace HKX2
{
    // hkbBehaviorGraphData Signatire: 0x95aca5d size: 128 flags: FLAGS_NONE

    // m_attributeDefaults m_class:  Type.TYPE_ARRAY Type.TYPE_REAL arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_variableInfos m_class: hkbVariableInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_characterPropertyInfos m_class: hkbVariableInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_eventInfos m_class: hkbEventInfo Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_wordMinVariableValues m_class: hkbVariableValue Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_wordMaxVariableValues m_class: hkbVariableValue Type.TYPE_ARRAY Type.TYPE_STRUCT arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_variableInitialValues m_class: hkbVariableValueSet Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_stringData m_class: hkbBehaviorGraphStringData Type.TYPE_POINTER Type.TYPE_STRUCT arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    public partial class hkbBehaviorGraphData : hkReferencedObject, IEquatable<hkbBehaviorGraphData?>
    {
        public IList<float> m_attributeDefaults { set; get; } = Array.Empty<float>();
        public IList<hkbVariableInfo> m_variableInfos { set; get; } = Array.Empty<hkbVariableInfo>();
        public IList<hkbVariableInfo> m_characterPropertyInfos { set; get; } = Array.Empty<hkbVariableInfo>();
        public IList<hkbEventInfo> m_eventInfos { set; get; } = Array.Empty<hkbEventInfo>();
        public IList<hkbVariableValue> m_wordMinVariableValues { set; get; } = Array.Empty<hkbVariableValue>();
        public IList<hkbVariableValue> m_wordMaxVariableValues { set; get; } = Array.Empty<hkbVariableValue>();
        public hkbVariableValueSet? m_variableInitialValues { set; get; }
        public hkbBehaviorGraphStringData? m_stringData { set; get; }

        public override uint Signature { set; get; } = 0x95aca5d;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_attributeDefaults = des.ReadSingleArray(br);
            m_variableInfos = des.ReadClassArray<hkbVariableInfo>(br);
            m_characterPropertyInfos = des.ReadClassArray<hkbVariableInfo>(br);
            m_eventInfos = des.ReadClassArray<hkbEventInfo>(br);
            m_wordMinVariableValues = des.ReadClassArray<hkbVariableValue>(br);
            m_wordMaxVariableValues = des.ReadClassArray<hkbVariableValue>(br);
            m_variableInitialValues = des.ReadClassPointer<hkbVariableValueSet>(br);
            m_stringData = des.ReadClassPointer<hkbBehaviorGraphStringData>(br);
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteSingleArray(bw, m_attributeDefaults);
            s.WriteClassArray(bw, m_variableInfos);
            s.WriteClassArray(bw, m_characterPropertyInfos);
            s.WriteClassArray(bw, m_eventInfos);
            s.WriteClassArray(bw, m_wordMinVariableValues);
            s.WriteClassArray(bw, m_wordMaxVariableValues);
            s.WriteClassPointer(bw, m_variableInitialValues);
            s.WriteClassPointer(bw, m_stringData);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_attributeDefaults = xd.ReadSingleArray(xe, nameof(m_attributeDefaults));
            m_variableInfos = xd.ReadClassArray<hkbVariableInfo>(xe, nameof(m_variableInfos));
            m_characterPropertyInfos = xd.ReadClassArray<hkbVariableInfo>(xe, nameof(m_characterPropertyInfos));
            m_eventInfos = xd.ReadClassArray<hkbEventInfo>(xe, nameof(m_eventInfos));
            m_wordMinVariableValues = xd.ReadClassArray<hkbVariableValue>(xe, nameof(m_wordMinVariableValues));
            m_wordMaxVariableValues = xd.ReadClassArray<hkbVariableValue>(xe, nameof(m_wordMaxVariableValues));
            m_variableInitialValues = xd.ReadClassPointer<hkbVariableValueSet>(xe, nameof(m_variableInitialValues));
            m_stringData = xd.ReadClassPointer<hkbBehaviorGraphStringData>(xe, nameof(m_stringData));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloatArray(xe, nameof(m_attributeDefaults), m_attributeDefaults);
            xs.WriteClassArray(xe, nameof(m_variableInfos), m_variableInfos);
            xs.WriteClassArray(xe, nameof(m_characterPropertyInfos), m_characterPropertyInfos);
            xs.WriteClassArray(xe, nameof(m_eventInfos), m_eventInfos);
            xs.WriteClassArray(xe, nameof(m_wordMinVariableValues), m_wordMinVariableValues);
            xs.WriteClassArray(xe, nameof(m_wordMaxVariableValues), m_wordMaxVariableValues);
            xs.WriteClassPointer(xe, nameof(m_variableInitialValues), m_variableInitialValues);
            xs.WriteClassPointer(xe, nameof(m_stringData), m_stringData);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbBehaviorGraphData);
        }

        public bool Equals(hkbBehaviorGraphData? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_attributeDefaults.SequenceEqual(other.m_attributeDefaults) &&
                   m_variableInfos.SequenceEqual(other.m_variableInfos) &&
                   m_characterPropertyInfos.SequenceEqual(other.m_characterPropertyInfos) &&
                   m_eventInfos.SequenceEqual(other.m_eventInfos) &&
                   m_wordMinVariableValues.SequenceEqual(other.m_wordMinVariableValues) &&
                   m_wordMaxVariableValues.SequenceEqual(other.m_wordMaxVariableValues) &&
                   ((m_variableInitialValues is null && other.m_variableInitialValues is null) || (m_variableInitialValues is not null && other.m_variableInitialValues is not null && m_variableInitialValues.Equals((IHavokObject)other.m_variableInitialValues))) &&
                   ((m_stringData is null && other.m_stringData is null) || (m_stringData is not null && other.m_stringData is not null && m_stringData.Equals((IHavokObject)other.m_stringData))) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_attributeDefaults.Aggregate(0, (x, y) => x ^ y.GetHashCode()));
            hashcode.Add(m_variableInfos.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_characterPropertyInfos.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_eventInfos.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_wordMinVariableValues.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_wordMaxVariableValues.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_variableInitialValues);
            hashcode.Add(m_stringData);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

