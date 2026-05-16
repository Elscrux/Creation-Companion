using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkbDampingModifierInternalState Signatire: 0x508d3b36 size: 80 flags: FLAGS_NONE

    // m_dampedVector m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_vecErrorSum m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_vecPreviousError m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_dampedValue m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_errorSum m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    // m_previousError m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    public partial class hkbDampingModifierInternalState : hkReferencedObject, IEquatable<hkbDampingModifierInternalState?>
    {
        public Vector4 m_dampedVector { set; get; }
        public Vector4 m_vecErrorSum { set; get; }
        public Vector4 m_vecPreviousError { set; get; }
        public float m_dampedValue { set; get; }
        public float m_errorSum { set; get; }
        public float m_previousError { set; get; }

        public override uint Signature { set; get; } = 0x508d3b36;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_dampedVector = br.ReadVector4();
            m_vecErrorSum = br.ReadVector4();
            m_vecPreviousError = br.ReadVector4();
            m_dampedValue = br.ReadSingle();
            m_errorSum = br.ReadSingle();
            m_previousError = br.ReadSingle();
            br.Position += 4;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_dampedVector);
            bw.WriteVector4(m_vecErrorSum);
            bw.WriteVector4(m_vecPreviousError);
            bw.WriteSingle(m_dampedValue);
            bw.WriteSingle(m_errorSum);
            bw.WriteSingle(m_previousError);
            bw.Position += 4;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_dampedVector = xd.ReadVector4(xe, nameof(m_dampedVector));
            m_vecErrorSum = xd.ReadVector4(xe, nameof(m_vecErrorSum));
            m_vecPreviousError = xd.ReadVector4(xe, nameof(m_vecPreviousError));
            m_dampedValue = xd.ReadSingle(xe, nameof(m_dampedValue));
            m_errorSum = xd.ReadSingle(xe, nameof(m_errorSum));
            m_previousError = xd.ReadSingle(xe, nameof(m_previousError));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_dampedVector), m_dampedVector);
            xs.WriteVector4(xe, nameof(m_vecErrorSum), m_vecErrorSum);
            xs.WriteVector4(xe, nameof(m_vecPreviousError), m_vecPreviousError);
            xs.WriteFloat(xe, nameof(m_dampedValue), m_dampedValue);
            xs.WriteFloat(xe, nameof(m_errorSum), m_errorSum);
            xs.WriteFloat(xe, nameof(m_previousError), m_previousError);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkbDampingModifierInternalState);
        }

        public bool Equals(hkbDampingModifierInternalState? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_dampedVector.Equals(other.m_dampedVector) &&
                   m_vecErrorSum.Equals(other.m_vecErrorSum) &&
                   m_vecPreviousError.Equals(other.m_vecPreviousError) &&
                   m_dampedValue.Equals(other.m_dampedValue) &&
                   m_errorSum.Equals(other.m_errorSum) &&
                   m_previousError.Equals(other.m_previousError) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_dampedVector);
            hashcode.Add(m_vecErrorSum);
            hashcode.Add(m_vecPreviousError);
            hashcode.Add(m_dampedValue);
            hashcode.Add(m_errorSum);
            hashcode.Add(m_previousError);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

