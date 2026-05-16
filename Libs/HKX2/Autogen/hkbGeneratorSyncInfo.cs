using System.Xml.Linq;
namespace HKX2
{
    // hkbGeneratorSyncInfo Signatire: 0xa3c341f8 size: 80 flags: FLAGS_NONE

    // m_syncPoints m_class: hkbGeneratorSyncInfoSyncPoint Type.TYPE_STRUCT Type.TYPE_VOID arrSize: 8 offset: 0 flags: FLAGS_NONE enum: 
    // m_baseFrequency m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_localTime m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    // m_playbackSpeed m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_numSyncPoints m_class:  Type.TYPE_INT8 Type.TYPE_VOID arrSize: 0 offset: 76 flags: FLAGS_NONE enum: 
    // m_isCyclic m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 77 flags: FLAGS_NONE enum: 
    // m_isMirrored m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 78 flags: FLAGS_NONE enum: 
    // m_isAdditive m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 79 flags: FLAGS_NONE enum: 
    public partial class hkbGeneratorSyncInfo : IHavokObject, IEquatable<hkbGeneratorSyncInfo?>
    {
        public hkbGeneratorSyncInfoSyncPoint[] m_syncPoints = new hkbGeneratorSyncInfoSyncPoint[8];
        public float m_baseFrequency { set; get; }
        public float m_localTime { set; get; }
        public float m_playbackSpeed { set; get; }
        public sbyte m_numSyncPoints { set; get; }
        public bool m_isCyclic { set; get; }
        public bool m_isMirrored { set; get; }
        public bool m_isAdditive { set; get; }

        public virtual uint Signature { set; get; } = 0xa3c341f8;

        public virtual void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            m_syncPoints = des.ReadStructCStyleArray<hkbGeneratorSyncInfoSyncPoint>(br, 8);
            br.Position += 8;
            m_baseFrequency = br.ReadSingle();
            m_localTime = br.ReadSingle();
            m_playbackSpeed = br.ReadSingle();
            m_numSyncPoints = br.ReadSByte();
            m_isCyclic = br.ReadBoolean();
            m_isMirrored = br.ReadBoolean();
            m_isAdditive = br.ReadBoolean();
        }

        public virtual void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            s.WriteStructCStyleArray(bw, m_syncPoints);
            bw.Position += 8;
            bw.WriteSingle(m_baseFrequency);
            bw.WriteSingle(m_localTime);
            bw.WriteSingle(m_playbackSpeed);
            bw.WriteSByte(m_numSyncPoints);
            bw.WriteBoolean(m_isCyclic);
            bw.WriteBoolean(m_isMirrored);
            bw.WriteBoolean(m_isAdditive);
        }

        public virtual void ReadXml(XmlDeserializer xd, XElement xe)
        {
            m_syncPoints = xd.ReadClassCStyleArray<hkbGeneratorSyncInfoSyncPoint>(xe, nameof(m_syncPoints), 8);
            m_baseFrequency = xd.ReadSingle(xe, nameof(m_baseFrequency));
            m_localTime = xd.ReadSingle(xe, nameof(m_localTime));
            m_playbackSpeed = xd.ReadSingle(xe, nameof(m_playbackSpeed));
            m_numSyncPoints = xd.ReadSByte(xe, nameof(m_numSyncPoints));
            m_isCyclic = xd.ReadBoolean(xe, nameof(m_isCyclic));
            m_isMirrored = xd.ReadBoolean(xe, nameof(m_isMirrored));
            m_isAdditive = xd.ReadBoolean(xe, nameof(m_isAdditive));
        }

        public virtual void WriteXml(XmlSerializer xs, XElement xe)
        {
            xs.WriteClassArray<hkbGeneratorSyncInfoSyncPoint>(xe, nameof(m_syncPoints), m_syncPoints);
            xs.WriteFloat(xe, nameof(m_baseFrequency), m_baseFrequency);
            xs.WriteFloat(xe, nameof(m_localTime), m_localTime);
            xs.WriteFloat(xe, nameof(m_playbackSpeed), m_playbackSpeed);
            xs.WriteNumber(xe, nameof(m_numSyncPoints), m_numSyncPoints);
            xs.WriteBoolean(xe, nameof(m_isCyclic), m_isCyclic);
            xs.WriteBoolean(xe, nameof(m_isMirrored), m_isMirrored);
            xs.WriteBoolean(xe, nameof(m_isAdditive), m_isAdditive);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbGeneratorSyncInfo);
        }

        public bool Equals(hkbGeneratorSyncInfo? other)
        {
            return other is not null &&
                   m_syncPoints.SequenceEqual(other.m_syncPoints) &&
                   m_baseFrequency.Equals(other.m_baseFrequency) &&
                   m_localTime.Equals(other.m_localTime) &&
                   m_playbackSpeed.Equals(other.m_playbackSpeed) &&
                   m_numSyncPoints.Equals(other.m_numSyncPoints) &&
                   m_isCyclic.Equals(other.m_isCyclic) &&
                   m_isMirrored.Equals(other.m_isMirrored) &&
                   m_isAdditive.Equals(other.m_isAdditive) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(m_syncPoints.Aggregate(0, (x, y) => x ^ y?.GetHashCode() ?? 0));
            hashcode.Add(m_baseFrequency);
            hashcode.Add(m_localTime);
            hashcode.Add(m_playbackSpeed);
            hashcode.Add(m_numSyncPoints);
            hashcode.Add(m_isCyclic);
            hashcode.Add(m_isMirrored);
            hashcode.Add(m_isAdditive);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

