using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using WaterFlowPainter.Model;
namespace WaterFlowPainter.Services;

/// <summary>
/// Simulates water particles flowing along a flowmap and renders them as fading trails into a
/// 64x64 bitmap that can be drawn over the cell in the overview.
/// </summary>
public sealed class FlowParticleSimulator {
    public const int ParticleCount = 512;

    /// <summary>Particle speed in flowmap pixels per second at full flow speed.</summary>
    public const float BaseSpeed = 20f;

    /// <summary>How much the trails fade per tick (0-1, higher = shorter trails).</summary>
    public const float TrailFade = 0.90f;

    public struct Particle {
        public float X;
        public float Y;
        public float Age;
        public float MaxAge;
    }

    public sealed class Simulation : IDisposable {
        public required FlowmapData Flowmap { get; init; }
        public required Particle[] Particles { get; init; }
        public required WriteableBitmap Bitmap { get; init; }
        public required Random Random { get; init; }

        public void Dispose() => Bitmap.Dispose();
    }

    public Simulation Create(FlowmapData flowmap) {
        var random = new Random();
        var particles = new Particle[ParticleCount];
        for (var i = 0; i < particles.Length; i++) {
            particles[i] = Respawn(new Particle(), random);
        }

        return new Simulation {
            Flowmap = flowmap,
            Particles = particles,
            Bitmap = new WriteableBitmap(
                new PixelSize(FlowmapData.Size, FlowmapData.Size),
                new Vector(96, 96),
                PixelFormats.Bgra8888,
                AlphaFormat.Premul),
            Random = random,
        };
    }

    /// <summary>Advances the simulation by <paramref name="dt" /> seconds and redraws the trail bitmap.</summary>
    public unsafe void Tick(Simulation simulation, float dt) {
        using var framebuffer = simulation.Bitmap.Lock();
        var pixels = (byte*) framebuffer.Address;
        const int size = FlowmapData.Size;
        const int rowBytes = size * 4;

        // Fade existing trails
        for (var i = 0; i < size * rowBytes; i++) {
            pixels[i] = (byte) (pixels[i] * TrailFade);
        }

        // Advance particles and draw their movement as short trails
        var particleSpan = simulation.Particles.AsSpan();
        for (var i = 0; i < particleSpan.Length; i++) {
            ref var particle = ref particleSpan[i];
            particle.Age += dt;
            if (particle.Age > particle.MaxAge) {
                particle = Respawn(particle, simulation.Random);
            }

            // Particles live in screen space (row 0 = north), the flowmap's row 0 is south
            var index = FlowmapData.Index(
                Math.Clamp((int) particle.X, 0, size - 1),
                Math.Clamp(size - 1 - (int) particle.Y, 0, size - 1));
            var speed = simulation.Flowmap.Speed[index];
            if (speed <= 0.01f) {
                particle = Respawn(particle, simulation.Random);
                continue;
            }

            var direction = simulation.Flowmap.Direction[index];
            var scale = simulation.Flowmap.Scale[index];
            var velocity = speed * (0.4f + 0.6f * scale) * BaseSpeed * dt;

            // Texture Y grows downwards, so invert the Y component to flow north upwards
            var newX = particle.X + MathF.Cos(direction) * velocity;
            var newY = particle.Y - MathF.Sin(direction) * velocity;

            DrawLine(pixels, rowBytes, particle.X, particle.Y, newX, newY);

            particle.X = Wrap(newX);
            particle.Y = Wrap(newY);
        }
    }

    private static Particle Respawn(Particle particle, Random random) {
        particle.X = random.NextSingle() * FlowmapData.Size;
        particle.Y = random.NextSingle() * FlowmapData.Size;
        particle.Age = 0;
        particle.MaxAge = 1.5f + random.NextSingle() * 2.5f;
        return particle;
    }

    private static float Wrap(float value) {
        var wrapped = value % FlowmapData.Size;
        if (wrapped < 0) wrapped += FlowmapData.Size;
        return wrapped;
    }

    private static unsafe void DrawLine(byte* pixels, int rowBytes, float x1, float y1, float x2, float y2) {
        const uint color = 0xB4B4B4B4; // Premultiplied white with ~70% alpha

        var steps = (int) MathF.Ceiling(MathF.Max(MathF.Abs(x2 - x1), MathF.Abs(y2 - y1)));
        if (steps == 0) steps = 1;
        for (var i = 0; i <= steps; i++) {
            var t = i / (float) steps;
            var x = (int) (x1 + (x2 - x1) * t);
            var y = (int) (y1 + (y2 - y1) * t);
            if (x < 0 || x >= FlowmapData.Size || y < 0 || y >= FlowmapData.Size) continue;

            var address = pixels + y * rowBytes + x * 4;
            *(uint*) address = color;
        }
    }
}
