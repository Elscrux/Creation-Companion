using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbDampingModifier Signatire: 0x9a040f03 size: 192 flags: FLAGS_NONE

    // m_kP m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_kI m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_kD m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_enableScalarDamping m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 92 flags: FLAGS_NONE enum: 
    // m_enableVectorDamping m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 93 flags: FLAGS_NONE enum: 
    // m_rawValue m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_dampedValue m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_rawVector m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_dampedVector m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 128 flags: FLAGS_NONE enum: 
    // m_vecErrorSum m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 144 flags: FLAGS_NONE enum: 
    // m_vecPreviousError m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 160 flags: FLAGS_NONE enum: 
    // m_errorSum m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 176 flags: FLAGS_NONE enum: 
    // m_previousError m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 180 flags: FLAGS_NONE enum: 
    public partial class hkbDampingModifier : hkbModifier, IEquatable<hkbDampingModifier?>
    {
        public float m_kP { set; get; }
        public float m_kI { set; get; }
        public float m_kD { set; get; }
        public bool m_enableScalarDamping { set; get; }
        public bool m_enableVectorDamping { set; get; }
        public float m_rawValue { set; get; }
        public float m_dampedValue { set; get; }
        public Vector4 m_rawVector { set; get; }
        public Vector4 m_dampedVector { set; get; }
        public Vector4 m_vecErrorSum { set; get; }
        public Vector4 m_vecPreviousError { set; get; }
        public float m_errorSum { set; get; }
        public float m_previousError { set; get; }

        public override uint Signature { set; get; } = 0x9a040f03;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_kP = br.ReadSingle();
            m_kI = br.ReadSingle();
            m_kD = br.ReadSingle();
            m_enableScalarDamping = br.ReadBoolean();
            m_enableVectorDamping = br.ReadBoolean();
            br.Position += 2;
            m_rawValue = br.ReadSingle();
            m_dampedValue = br.ReadSingle();
            br.Position += 8;
            m_rawVector = br.ReadVector4();
            m_dampedVector = br.ReadVector4();
            m_vecErrorSum = br.ReadVector4();
            m_vecPreviousError = br.ReadVector4();
            m_errorSum = br.ReadSingle();
            m_previousError = br.ReadSingle();
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteSingle(m_kP);
            bw.WriteSingle(m_kI);
            bw.WriteSingle(m_kD);
            bw.WriteBoolean(m_enableScalarDamping);
            bw.WriteBoolean(m_enableVectorDamping);
            bw.Position += 2;
            bw.WriteSingle(m_rawValue);
            bw.WriteSingle(m_dampedValue);
            bw.Position += 8;
            bw.WriteVector4(m_rawVector);
            bw.WriteVector4(m_dampedVector);
            bw.WriteVector4(m_vecErrorSum);
            bw.WriteVector4(m_vecPreviousError);
            bw.WriteSingle(m_errorSum);
            bw.WriteSingle(m_previousError);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_kP = xd.ReadSingle(xe, nameof(m_kP));
            m_kI = xd.ReadSingle(xe, nameof(m_kI));
            m_kD = xd.ReadSingle(xe, nameof(m_kD));
            m_enableScalarDamping = xd.ReadBoolean(xe, nameof(m_enableScalarDamping));
            m_enableVectorDamping = xd.ReadBoolean(xe, nameof(m_enableVectorDamping));
            m_rawValue = xd.ReadSingle(xe, nameof(m_rawValue));
            m_dampedValue = xd.ReadSingle(xe, nameof(m_dampedValue));
            m_rawVector = xd.ReadVector4(xe, nameof(m_rawVector));
            m_dampedVector = xd.ReadVector4(xe, nameof(m_dampedVector));
            m_vecErrorSum = xd.ReadVector4(xe, nameof(m_vecErrorSum));
            m_vecPreviousError = xd.ReadVector4(xe, nameof(m_vecPreviousError));
            m_errorSum = xd.ReadSingle(xe, nameof(m_errorSum));
            m_previousError = xd.ReadSingle(xe, nameof(m_previousError));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteFloat(xe, nameof(m_kP), m_kP);
            xs.WriteFloat(xe, nameof(m_kI), m_kI);
            xs.WriteFloat(xe, nameof(m_kD), m_kD);
            xs.WriteBoolean(xe, nameof(m_enableScalarDamping), m_enableScalarDamping);
            xs.WriteBoolean(xe, nameof(m_enableVectorDamping), m_enableVectorDamping);
            xs.WriteFloat(xe, nameof(m_rawValue), m_rawValue);
            xs.WriteFloat(xe, nameof(m_dampedValue), m_dampedValue);
            xs.WriteVector4(xe, nameof(m_rawVector), m_rawVector);
            xs.WriteVector4(xe, nameof(m_dampedVector), m_dampedVector);
            xs.WriteVector4(xe, nameof(m_vecErrorSum), m_vecErrorSum);
            xs.WriteVector4(xe, nameof(m_vecPreviousError), m_vecPreviousError);
            xs.WriteFloat(xe, nameof(m_errorSum), m_errorSum);
            xs.WriteFloat(xe, nameof(m_previousError), m_previousError);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbDampingModifier);
        }

        public bool Equals(hkbDampingModifier? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_kP.Equals(other.m_kP) &&
                   m_kI.Equals(other.m_kI) &&
                   m_kD.Equals(other.m_kD) &&
                   m_enableScalarDamping.Equals(other.m_enableScalarDamping) &&
                   m_enableVectorDamping.Equals(other.m_enableVectorDamping) &&
                   m_rawValue.Equals(other.m_rawValue) &&
                   m_dampedValue.Equals(other.m_dampedValue) &&
                   m_rawVector.Equals(other.m_rawVector) &&
                   m_dampedVector.Equals(other.m_dampedVector) &&
                   m_vecErrorSum.Equals(other.m_vecErrorSum) &&
                   m_vecPreviousError.Equals(other.m_vecPreviousError) &&
                   m_errorSum.Equals(other.m_errorSum) &&
                   m_previousError.Equals(other.m_previousError) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_kP);
            hashcode.Add(m_kI);
            hashcode.Add(m_kD);
            hashcode.Add(m_enableScalarDamping);
            hashcode.Add(m_enableVectorDamping);
            hashcode.Add(m_rawValue);
            hashcode.Add(m_dampedValue);
            hashcode.Add(m_rawVector);
            hashcode.Add(m_dampedVector);
            hashcode.Add(m_vecErrorSum);
            hashcode.Add(m_vecPreviousError);
            hashcode.Add(m_errorSum);
            hashcode.Add(m_previousError);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

