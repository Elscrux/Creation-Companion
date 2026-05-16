using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkpMouseSpringAction Signatire: 0x6e087fd6 size: 144 flags: FLAGS_NONE

    // m_positionInRbLocal m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_mousePositionInWorld m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 80 flags: FLAGS_NONE enum: 
    // m_springDamping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 96 flags: FLAGS_NONE enum: 
    // m_springElasticity m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 100 flags: FLAGS_NONE enum: 
    // m_maxRelativeForce m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 104 flags: FLAGS_NONE enum: 
    // m_objectDamping m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 108 flags: FLAGS_NONE enum: 
    // m_shapeKey m_class:  Type.TYPE_UINT32 Type.TYPE_VOID arrSize: 0 offset: 112 flags: FLAGS_NONE enum: 
    // m_applyCallbacks m_class:  Type.TYPE_ARRAY Type.TYPE_POINTER arrSize: 0 offset: 120 flags: SERIALIZE_IGNORED|FLAGS_NONE enum: 
    public partial class hkpMouseSpringAction : hkpUnaryAction, IEquatable<hkpMouseSpringAction?>
    {
        public Vector4 m_positionInRbLocal { set; get; }
        public Vector4 m_mousePositionInWorld { set; get; }
        public float m_springDamping { set; get; }
        public float m_springElasticity { set; get; }
        public float m_maxRelativeForce { set; get; }
        public float m_objectDamping { set; get; }
        public uint m_shapeKey { set; get; }
        public IList<object> m_applyCallbacks { set; get; } = Array.Empty<object>();

        public override uint Signature { set; get; } = 0x6e087fd6;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            br.Position += 8;
            m_positionInRbLocal = br.ReadVector4();
            m_mousePositionInWorld = br.ReadVector4();
            m_springDamping = br.ReadSingle();
            m_springElasticity = br.ReadSingle();
            m_maxRelativeForce = br.ReadSingle();
            m_objectDamping = br.ReadSingle();
            m_shapeKey = br.ReadUInt32();
            br.Position += 4;
            des.ReadEmptyArray(br);
            br.Position += 8;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.Position += 8;
            bw.WriteVector4(m_positionInRbLocal);
            bw.WriteVector4(m_mousePositionInWorld);
            bw.WriteSingle(m_springDamping);
            bw.WriteSingle(m_springElasticity);
            bw.WriteSingle(m_maxRelativeForce);
            bw.WriteSingle(m_objectDamping);
            bw.WriteUInt32(m_shapeKey);
            bw.Position += 4;
            s.WriteVoidArray(bw);
            bw.Position += 8;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_positionInRbLocal = xd.ReadVector4(xe, nameof(m_positionInRbLocal));
            m_mousePositionInWorld = xd.ReadVector4(xe, nameof(m_mousePositionInWorld));
            m_springDamping = xd.ReadSingle(xe, nameof(m_springDamping));
            m_springElasticity = xd.ReadSingle(xe, nameof(m_springElasticity));
            m_maxRelativeForce = xd.ReadSingle(xe, nameof(m_maxRelativeForce));
            m_objectDamping = xd.ReadSingle(xe, nameof(m_objectDamping));
            m_shapeKey = xd.ReadUInt32(xe, nameof(m_shapeKey));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_positionInRbLocal), m_positionInRbLocal);
            xs.WriteVector4(xe, nameof(m_mousePositionInWorld), m_mousePositionInWorld);
            xs.WriteFloat(xe, nameof(m_springDamping), m_springDamping);
            xs.WriteFloat(xe, nameof(m_springElasticity), m_springElasticity);
            xs.WriteFloat(xe, nameof(m_maxRelativeForce), m_maxRelativeForce);
            xs.WriteFloat(xe, nameof(m_objectDamping), m_objectDamping);
            xs.WriteNumber(xe, nameof(m_shapeKey), m_shapeKey);
            xs.WriteSerializeIgnored(xe, nameof(m_applyCallbacks));
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkpMouseSpringAction);
        }

        public bool Equals(hkpMouseSpringAction? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_positionInRbLocal.Equals(other.m_positionInRbLocal) &&
                   m_mousePositionInWorld.Equals(other.m_mousePositionInWorld) &&
                   m_springDamping.Equals(other.m_springDamping) &&
                   m_springElasticity.Equals(other.m_springElasticity) &&
                   m_maxRelativeForce.Equals(other.m_maxRelativeForce) &&
                   m_objectDamping.Equals(other.m_objectDamping) &&
                   m_shapeKey.Equals(other.m_shapeKey) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_positionInRbLocal);
            hashcode.Add(m_mousePositionInWorld);
            hashcode.Add(m_springDamping);
            hashcode.Add(m_springElasticity);
            hashcode.Add(m_maxRelativeForce);
            hashcode.Add(m_objectDamping);
            hashcode.Add(m_shapeKey);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

