using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpGravityGun Signatire: 0x5e2754cd size: 128 flags: FLAGS_NONE

    // m_grabbedBodies m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 56 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    // m_maxNumObjectsPicked m_class:  Type.TYPE_INT32 Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_maxMassOfObjectPicked m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 76 flags: FLAGS_NONE enum: 
    // m_maxDistOfObjectPicked m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_impulseAppliedWhenObjectNotPicked m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 84 flags: FLAGS_NONE enum: 
    // m_throwVelocity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 88 flags: FLAGS_NONE enum: 
    // m_capturedObjectPosition m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_capturedObjectsOffset m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    public partial class hkpGravityGun : hkpFirstPersonGun, IEquatable<hkpGravityGun?>
    {
        public IList<object> m_grabbedBodies { set; get; } = Array.Empty<object>();
        public int m_maxNumObjectsPicked { set; get; }
        public float m_maxMassOfObjectPicked { set; get; }
        public float m_maxDistOfObjectPicked { set; get; }
        public float m_impulseAppliedWhenObjectNotPicked { set; get; }
        public float m_throwVelocity { set; get; }
        public Vector4 m_capturedObjectPosition { set; get; }
        public Vector4 m_capturedObjectsOffset { set; get; }

        public override uint Signature { set; get; } = 0x5e2754cd;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            des.ReadEmptyArray(br);
            m_maxNumObjectsPicked = br.ReadInt32();
            m_maxMassOfObjectPicked = br.ReadSingle();
            m_maxDistOfObjectPicked = br.ReadSingle();
            m_impulseAppliedWhenObjectNotPicked = br.ReadSingle();
            m_throwVelocity = br.ReadSingle();
            br.Position += 4;
            m_capturedObjectPosition = br.ReadVector4();
            m_capturedObjectsOffset = br.ReadVector4();
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            s.WriteVoidArray(bw);
            bw.WriteInt32(m_maxNumObjectsPicked);
            bw.WriteSingle(m_maxMassOfObjectPicked);
            bw.WriteSingle(m_maxDistOfObjectPicked);
            bw.WriteSingle(m_impulseAppliedWhenObjectNotPicked);
            bw.WriteSingle(m_throwVelocity);
            bw.Position += 4;
            bw.WriteVector4(m_capturedObjectPosition);
            bw.WriteVector4(m_capturedObjectsOffset);
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_maxNumObjectsPicked = xd.ReadInt32(xe, nameof(m_maxNumObjectsPicked));
            m_maxMassOfObjectPicked = xd.ReadSingle(xe, nameof(m_maxMassOfObjectPicked));
            m_maxDistOfObjectPicked = xd.ReadSingle(xe, nameof(m_maxDistOfObjectPicked));
            m_impulseAppliedWhenObjectNotPicked = xd.ReadSingle(xe, nameof(m_impulseAppliedWhenObjectNotPicked));
            m_throwVelocity = xd.ReadSingle(xe, nameof(m_throwVelocity));
            m_capturedObjectPosition = xd.ReadVector4(xe, nameof(m_capturedObjectPosition));
            m_capturedObjectsOffset = xd.ReadVector4(xe, nameof(m_capturedObjectsOffset));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteSerializeIgnored(xe, nameof(m_grabbedBodies));
            xs.WriteNumber(xe, nameof(m_maxNumObjectsPicked), m_maxNumObjectsPicked);
            xs.WriteFloat(xe, nameof(m_maxMassOfObjectPicked), m_maxMassOfObjectPicked);
            xs.WriteFloat(xe, nameof(m_maxDistOfObjectPicked), m_maxDistOfObjectPicked);
            xs.WriteFloat(xe, nameof(m_impulseAppliedWhenObjectNotPicked), m_impulseAppliedWhenObjectNotPicked);
            xs.WriteFloat(xe, nameof(m_throwVelocity), m_throwVelocity);
            xs.WriteVector4(xe, nameof(m_capturedObjectPosition), m_capturedObjectPosition);
            xs.WriteVector4(xe, nameof(m_capturedObjectsOffset), m_capturedObjectsOffset);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpGravityGun);
        }

        public bool Equals(hkpGravityGun? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_maxNumObjectsPicked.Equals(other.m_maxNumObjectsPicked) &&
                   m_maxMassOfObjectPicked.Equals(other.m_maxMassOfObjectPicked) &&
                   m_maxDistOfObjectPicked.Equals(other.m_maxDistOfObjectPicked) &&
                   m_impulseAppliedWhenObjectNotPicked.Equals(other.m_impulseAppliedWhenObjectNotPicked) &&
                   m_throwVelocity.Equals(other.m_throwVelocity) &&
                   m_capturedObjectPosition.Equals(other.m_capturedObjectPosition) &&
                   m_capturedObjectsOffset.Equals(other.m_capturedObjectsOffset) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_maxNumObjectsPicked);
            hashcode.Add(m_maxMassOfObjectPicked);
            hashcode.Add(m_maxDistOfObjectPicked);
            hashcode.Add(m_impulseAppliedWhenObjectNotPicked);
            hashcode.Add(m_throwVelocity);
            hashcode.Add(m_capturedObjectPosition);
            hashcode.Add(m_capturedObjectsOffset);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

