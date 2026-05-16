using System.Numerics;
using System.Xml.Linq;
namespace HKX2
{
    // hkxCamera Signatire: 0xe3597b02 size: 80 flags: FLAGS_NONE

    // m_from m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 16 flags: FLAGS_NONE enum: 
    // m_focus m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 32 flags: FLAGS_NONE enum: 
    // m_up m_class:  Type.TYPE_VECTOR4 Type.TYPE_VOID arrSize: 0 offset: 48 flags: FLAGS_NONE enum: 
    // m_fov m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 64 flags: FLAGS_NONE enum: 
    // m_far m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 68 flags: FLAGS_NONE enum: 
    // m_near m_class:  Type.TYPE_REAL Type.TYPE_VOID arrSize: 0 offset: 72 flags: FLAGS_NONE enum: 
    // m_leftHanded m_class:  Type.TYPE_BOOL Type.TYPE_VOID arrSize: 0 offset: 76 flags: FLAGS_NONE enum: 
    public partial class hkxCamera : hkReferencedObject, IEquatable<hkxCamera?>
    {
        public Vector4 m_from { set; get; }
        public Vector4 m_focus { set; get; }
        public Vector4 m_up { set; get; }
        public float m_fov { set; get; }
        public float m_far { set; get; }
        public float m_near { set; get; }
        public bool m_leftHanded { set; get; }

        public override uint Signature { set; get; } = 0xe3597b02;

        public override void Read(PackFileDeserializer des, BinaryReaderEx br)
        {
            base.Read(des, br);
            m_from = br.ReadVector4();
            m_focus = br.ReadVector4();
            m_up = br.ReadVector4();
            m_fov = br.ReadSingle();
            m_far = br.ReadSingle();
            m_near = br.ReadSingle();
            m_leftHanded = br.ReadBoolean();
            br.Position += 3;
        }

        public override void Write(PackFileSerializer s, BinaryWriterEx bw)
        {
            base.Write(s, bw);
            bw.WriteVector4(m_from);
            bw.WriteVector4(m_focus);
            bw.WriteVector4(m_up);
            bw.WriteSingle(m_fov);
            bw.WriteSingle(m_far);
            bw.WriteSingle(m_near);
            bw.WriteBoolean(m_leftHanded);
            bw.Position += 3;
        }

        public override void ReadXml(XmlDeserializer xd, XElement xe)
        {
            base.ReadXml(xd, xe);
            m_from = xd.ReadVector4(xe, nameof(m_from));
            m_focus = xd.ReadVector4(xe, nameof(m_focus));
            m_up = xd.ReadVector4(xe, nameof(m_up));
            m_fov = xd.ReadSingle(xe, nameof(m_fov));
            m_far = xd.ReadSingle(xe, nameof(m_far));
            m_near = xd.ReadSingle(xe, nameof(m_near));
            m_leftHanded = xd.ReadBoolean(xe, nameof(m_leftHanded));
        }

        public override void WriteXml(XmlSerializer xs, XElement xe)
        {
            base.WriteXml(xs, xe);
            xs.WriteVector4(xe, nameof(m_from), m_from);
            xs.WriteVector4(xe, nameof(m_focus), m_focus);
            xs.WriteVector4(xe, nameof(m_up), m_up);
            xs.WriteFloat(xe, nameof(m_fov), m_fov);
            xs.WriteFloat(xe, nameof(m_far), m_far);
            xs.WriteFloat(xe, nameof(m_near), m_near);
            xs.WriteBoolean(xe, nameof(m_leftHanded), m_leftHanded);
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as hkxCamera);
        }

        public bool Equals(hkxCamera? other)
        {
            return other is not null &&
                   base.Equals(other) &&
                   m_from.Equals(other.m_from) &&
                   m_focus.Equals(other.m_focus) &&
                   m_up.Equals(other.m_up) &&
                   m_fov.Equals(other.m_fov) &&
                   m_far.Equals(other.m_far) &&
                   m_near.Equals(other.m_near) &&
                   m_leftHanded.Equals(other.m_leftHanded) &&
                   Signature == other.Signature;
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();
            hashcode.Add(base.GetHashCode());
            hashcode.Add(m_from);
            hashcode.Add(m_focus);
            hashcode.Add(m_up);
            hashcode.Add(m_fov);
            hashcode.Add(m_far);
            hashcode.Add(m_near);
            hashcode.Add(m_leftHanded);
            hashcode.Add(Signature);
            return hashcode.ToHashCode();
        }
    }
}

