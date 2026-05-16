using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbComputeDirectionModifierInternalState Signatire: 0x6ac054d7 size: 48 flags: FLAGS_NONE

    // m_pointOut m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_groundAngleOut m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_upAngleOut m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 36 flags: FLAGS_NONE enum: 
    // m_computedOutput m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 40 flags: FLAGS_NONE enum: 
    public partial class hkbComputeDirectionModifierInternalState : hkReferencedObject, IEquatable<hkbComputeDirectionModifierInternalState?>
    {
        public Vector4 m_pointOut { set; get; }
        public float m_groundAngleOut { set; get; }
        public float m_upAngleOut { set; get; }
        public bool m_computedOutput { set; get; }

        public override uint Signature { set; get; } = 0x6ac054d7;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_pointOut = br.ReadVector4();
            m_groundAngleOut = br.ReadSingle();
            m_upAngleOut = br.ReadSingle();
            m_computedOutput = br.ReadBoolean();
            br.Position += 7;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_pointOut);
            bw.WriteSingle(m_groundAngleOut);
            bw.WriteSingle(m_upAngleOut);
            bw.WriteBoolean(m_computedOutput);
            bw.Position += 7;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_pointOut = xd.ReadVector4(xe, nameof(m_pointOut));
            m_groundAngleOut = xd.ReadSingle(xe, nameof(m_groundAngleOut));
            m_upAngleOut = xd.ReadSingle(xe, nameof(m_upAngleOut));
            m_computedOutput = xd.ReadBoolean(xe, nameof(m_computedOutput));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_pointOut), m_pointOut);
            xs.WriteFloat(xe, nameof(m_groundAngleOut), m_groundAngleOut);
            xs.WriteFloat(xe, nameof(m_upAngleOut), m_upAngleOut);
            xs.WriteBoolean(xe, nameof(m_computedOutput), m_computedOutput);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbComputeDirectionModifierInternalState);
        }

        public bool Equals(hkbComputeDirectionModifierInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_pointOut.Equals(other.m_pointOut) &&
                   m_groundAngleOut.Equals(other.m_groundAngleOut) &&
                   m_upAngleOut.Equals(other.m_upAngleOut) &&
                   m_computedOutput.Equals(other.m_computedOutput) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_pointOut);
            hashcode.Add(m_groundAngleOut);
            hashcode.Add(m_upAngleOut);
            hashcode.Add(m_computedOutput);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

