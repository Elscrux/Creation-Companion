using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbComputeDirectionModifier Signatire: 0xdf358bd3 size: 144 flags: FLAGS_NONE

    // m_pointIn m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_pointOut m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_groundAngleOut m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_upAngleOut m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 116 flags: FLAGS_NONE enum: 
    // m_verticalOffset m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 120 flags: FLAGS_NONE enum: 
    // m_reverseGroundAngle m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 124 flags: FLAGS_NONE enum: 
    // m_reverseUpAngle m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 125 flags: FLAGS_NONE enum: 
    // m_projectPoint m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 126 flags: FLAGS_NONE enum: 
    // m_normalizePoint m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 127 flags: FLAGS_NONE enum: 
    // m_computeOnlyOnce m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_computedOutput m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 129 flags: FLAGS_NONE enum: 
    public partial class hkbComputeDirectionModifier : hkbModifier, IEquatable<hkbComputeDirectionModifier?>
    {
        public Vector4 m_pointIn { set; get; }
        public Vector4 m_pointOut { set; get; }
        public float m_groundAngleOut { set; get; }
        public float m_upAngleOut { set; get; }
        public float m_verticalOffset { set; get; }
        public bool m_reverseGroundAngle { set; get; }
        public bool m_reverseUpAngle { set; get; }
        public bool m_projectPoint { set; get; }
        public bool m_normalizePoint { set; get; }
        public bool m_computeOnlyOnce { set; get; }
        public bool m_computedOutput { set; get; }

        public override uint Signature { set; get; } = 0xdf358bd3;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_pointIn = br.ReadVector4();
            m_pointOut = br.ReadVector4();
            m_groundAngleOut = br.ReadSingle();
            m_upAngleOut = br.ReadSingle();
            m_verticalOffset = br.ReadSingle();
            m_reverseGroundAngle = br.ReadBoolean();
            m_reverseUpAngle = br.ReadBoolean();
            m_projectPoint = br.ReadBoolean();
            m_normalizePoint = br.ReadBoolean();
            m_computeOnlyOnce = br.ReadBoolean();
            m_computedOutput = br.ReadBoolean();
            br.Position += 14;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_pointIn);
            bw.WriteVector4(m_pointOut);
            bw.WriteSingle(m_groundAngleOut);
            bw.WriteSingle(m_upAngleOut);
            bw.WriteSingle(m_verticalOffset);
            bw.WriteBoolean(m_reverseGroundAngle);
            bw.WriteBoolean(m_reverseUpAngle);
            bw.WriteBoolean(m_projectPoint);
            bw.WriteBoolean(m_normalizePoint);
            bw.WriteBoolean(m_computeOnlyOnce);
            bw.WriteBoolean(m_computedOutput);
            bw.Position += 14;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_pointIn = xd.ReadVector4(xe, nameof(m_pointIn));
            m_pointOut = xd.ReadVector4(xe, nameof(m_pointOut));
            m_groundAngleOut = xd.ReadSingle(xe, nameof(m_groundAngleOut));
            m_upAngleOut = xd.ReadSingle(xe, nameof(m_upAngleOut));
            m_verticalOffset = xd.ReadSingle(xe, nameof(m_verticalOffset));
            m_reverseGroundAngle = xd.ReadBoolean(xe, nameof(m_reverseGroundAngle));
            m_reverseUpAngle = xd.ReadBoolean(xe, nameof(m_reverseUpAngle));
            m_projectPoint = xd.ReadBoolean(xe, nameof(m_projectPoint));
            m_normalizePoint = xd.ReadBoolean(xe, nameof(m_normalizePoint));
            m_computeOnlyOnce = xd.ReadBoolean(xe, nameof(m_computeOnlyOnce));
            m_computedOutput = xd.ReadBoolean(xe, nameof(m_computedOutput));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_pointIn), m_pointIn);
            xs.WriteVector4(xe, nameof(m_pointOut), m_pointOut);
            xs.WriteFloat(xe, nameof(m_groundAngleOut), m_groundAngleOut);
            xs.WriteFloat(xe, nameof(m_upAngleOut), m_upAngleOut);
            xs.WriteFloat(xe, nameof(m_verticalOffset), m_verticalOffset);
            xs.WriteBoolean(xe, nameof(m_reverseGroundAngle), m_reverseGroundAngle);
            xs.WriteBoolean(xe, nameof(m_reverseUpAngle), m_reverseUpAngle);
            xs.WriteBoolean(xe, nameof(m_projectPoint), m_projectPoint);
            xs.WriteBoolean(xe, nameof(m_normalizePoint), m_normalizePoint);
            xs.WriteBoolean(xe, nameof(m_computeOnlyOnce), m_computeOnlyOnce);
            xs.WriteBoolean(xe, nameof(m_computedOutput), m_computedOutput);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbComputeDirectionModifier);
        }

        public bool Equals(hkbComputeDirectionModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_pointIn.Equals(other.m_pointIn) &&
                   m_pointOut.Equals(other.m_pointOut) &&
                   m_groundAngleOut.Equals(other.m_groundAngleOut) &&
                   m_upAngleOut.Equals(other.m_upAngleOut) &&
                   m_verticalOffset.Equals(other.m_verticalOffset) &&
                   m_reverseGroundAngle.Equals(other.m_reverseGroundAngle) &&
                   m_reverseUpAngle.Equals(other.m_reverseUpAngle) &&
                   m_projectPoint.Equals(other.m_projectPoint) &&
                   m_normalizePoint.Equals(other.m_normalizePoint) &&
                   m_computeOnlyOnce.Equals(other.m_computeOnlyOnce) &&
                   m_computedOutput.Equals(other.m_computedOutput) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_pointIn);
            hashcode.Add(m_pointOut);
            hashcode.Add(m_groundAngleOut);
            hashcode.Add(m_upAngleOut);
            hashcode.Add(m_verticalOffset);
            hashcode.Add(m_reverseGroundAngle);
            hashcode.Add(m_reverseUpAngle);
            hashcode.Add(m_projectPoint);
            hashcode.Add(m_normalizePoint);
            hashcode.Add(m_computeOnlyOnce);
            hashcode.Add(m_computedOutput);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

