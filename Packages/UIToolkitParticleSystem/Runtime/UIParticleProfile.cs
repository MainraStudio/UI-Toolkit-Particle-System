using UnityEngine;
using Alchemy.Inspector;

namespace MainraGames
{
    [CreateAssetMenu(fileName = "UIParticleProfile", menuName = "UI Toolkit/UIParticle/UIParticle Profile")]
    public class UIParticleProfile : ScriptableObject
    {
        // ===== hot-reload versioning (read by UIParticle) =====
        [HideInInspector] public int revision = 0;

        // ─────────────────────────────────────────────────────────────────────────
        // MAIN (arranged similar to Unity's Particle System "Main" module)
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Main")]
        [Group("Main/Split")]
        [Min(0.01f)]
        [Tooltip("System lifetime in seconds before looping or stopping.")]
        public float Duration = 5f;

        [Group("Main/Split")]
        [Tooltip("If enabled, the system restarts automatically after reaching Duration.")]
        public bool Looping = true;

        [Group("Main/Split")]
        [ShowIf("Looping")]
        [Tooltip("Start the simulation as if it has already been running (only meaningful when Looping is enabled).")]
        public bool Prewarm = false;

        [Group("Main/Split")]
        [LabelText("Play On Awake")]
        [Tooltip("Automatically begin playing when the component is attached/instantiated.")]
        public bool PlayOnAttach = true;

        [Group("Main/Split")]
        [LabelText("Start Delay")]
        [Min(0f)]
        [Tooltip("Delay in seconds before emission starts after play.")]
        public float StartDelay = 0f;

        [Group("Main/Split")]
        [LabelText("Start Lifetime")]
        [Min(0.01f)]
        [Tooltip("Base lifetime of each particle in seconds.")]
        public float StartLifetime = 5f;

        [Group("Main/Split")]
        [LabelText("± Lifetime Random (s)")]
        [Min(0f)]
        [Tooltip("Random lifetime variation added/subtracted from Start Lifetime.")]
        public float StartLifetimeRandom = 0f;

        [Group("Main/Split")]
        [LabelText("Start Speed")]
        [Tooltip("Initial particle speed in units/second.")]
        public float StartSpeed = 5f;

        [Group("Main/Split")]
        [LabelText("± Speed Random")]
        [Min(0f)]
        [Tooltip("Random speed variation added/subtracted from Start Speed.")]
        public float StartSpeedRandom = 0f;

        [Group("Main/Split")]
        [LabelText("Use 2D Size")]
        [Tooltip("Use separate X/Y start size instead of a single uniform size.")]
        public bool UseStartSize2D = false;

        [Group("Main/Split")]
        [HideIf("UseStartSize2D")]
        [LabelText("Start Size")]
        [Min(0.01f)]
        [Tooltip("Uniform initial size for each particle (pixels/units depending on renderer).")]
        public float StartSize = 16f;

        [Group("Main/Split")]
        [HideIf("UseStartSize2D")]
        [LabelText("± Size Random")]
        [Min(0f)]
        [Tooltip("Random size variation added/subtracted from Start Size.")]
        public float StartSizeRandom = 0f;

        [Group("Main/Split")]
        [ShowIf("UseStartSize2D")]
        [LabelText("Start Size X")]
        [Min(0.01f)]
        [Tooltip("Initial width of each particle.")]
        public float StartSizeX = 16f;

        [Group("Main/Split")]
        [ShowIf("UseStartSize2D")]
        [LabelText("Start Size Y")]
        [Min(0.01f)]
        [Tooltip("Initial height of each particle.")]
        public float StartSizeY = 16f;

        [Group("Main/Split")]
        [ShowIf("UseStartSize2D")]
        [LabelText("± Size Random X")]
        [Min(0f)]
        [Tooltip("Random width variation added/subtracted from Start Size X.")]
        public float StartSizeRandomX = 0f;

        [Group("Main/Split")]
        [ShowIf("UseStartSize2D")]
        [LabelText("± Size Random Y")]
        [Min(0f)]
        [Tooltip("Random height variation added/subtracted from Start Size Y.")]
        public float StartSizeRandomY = 0f;

        [Group("Main/Split")]
        [LabelText("Start Rotation (deg)")]
        [Tooltip("Initial rotation of each particle in degrees.")]
        public float StartRotation = 0f;

        [Group("Main/Split")]
        [LabelText("± Rotation Random (deg)")]
        [Min(0f)]
        [Tooltip("Random rotation variation added/subtracted from Start Rotation.")]
        public float StartRotationRandom = 0f;

        [Group("Main/Split")]
        [LabelText("Start Color")]
        [Tooltip("Initial color of particles.")]
        public Color StartColor = Color.white;

        [Group("Main/Split")]
        [LabelText("Gravity Modifier")]
        [Tooltip("Amount of gravity applied to particles (uses UI particle's gravity interpretation).")]
        public float GravityModifier = 0f;

        [Group("Main/Split")]
        [LabelText("Max Particles")]
        [Min(1)]
        [Tooltip("Upper cap for the number of alive particles.")]
        public int MaxParticles = 1000;

        [Group("Main/Split")]
        [LabelText("Simulation Speed")]
        [Min(0f)]
        [Tooltip("Scales the entire simulation speed (time multiplier).")]
        public float PlaybackSpeed = 1f;

        [Group("Main/Split")]
        [LabelText("Auto Random Seed")]
        [Tooltip("If enabled, a random seed is generated automatically each play.")]
        public bool AutoRandomSeed = true;

        [Group("Main/Split")]
        [HideIf("AutoRandomSeed")]
        [LabelText("Random Seed")]
        [Tooltip("Manual random seed for deterministic playback.")]
        public int RandomSeed = 0;

        [Group("Main/Split")]
        [LabelText("Simulate In Editor")]
        [Tooltip("Run the particle simulation while in the Editor (not playing).")]
        public bool SimulateInEditor = true;

        // ─────────────────────────────────────────────────────────────────────────
        // EMISSION
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Emission")]
        [Group("Emission/Split")]
        [LabelText("Rate over Time")]
        [Min(0f)]
        [Tooltip("Continuous emission rate in particles per second.")]
        public float RateOverTime = 10f;

        [Group("Emission/Split")]
        [LabelText("Rate over Distance")]
        [Min(0f)]
        [Tooltip("Emit particles based on emitter movement distance.")]
        public float RateOverDistance = 0f;

        [Group("Emission/Split")]
        [LabelText("Bursts (Count)")]
        [Min(0)]
        [Tooltip("Number of particles emitted per burst. 0 disables bursts.")]
        public int BurstCount = 0;
        public bool IsBurst() => BurstCount > 0;

        [Group("Emission/Split")]
        [ShowIf("IsBurst")]
        [LabelText("Burst Time")]
        [Min(0f)]
        [Tooltip("Time (seconds since play) of the first burst.")]
        public float BurstTime = 0f;

        [Group("Emission/Split")]
        [ShowIf("IsBurst")]
        [LabelText("Burst Cycles")]
        [Min(0)]
        [Tooltip("Number of times the burst repeats.")]
        public int BurstCycles = 1;

        [Group("Emission/Split")]
        [ShowIf("IsBurst")]
        [LabelText("Burst Interval")]
        [Min(0f)]
        [Tooltip("Time between consecutive burst cycles in seconds.")]
        public float BurstInterval = 0.5f;

        // ─────────────────────────────────────────────────────────────────────────
        // SHAPE
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Shape")]
        [Tooltip("Emitter geometry used to spawn particles.")]
        public UIParticle.EmitterShapeType ShapeType = UIParticle.EmitterShapeType.Sphere;
        public bool IsShapeTypeSphere() => this.ShapeType == UIParticle.EmitterShapeType.Sphere || this.ShapeType == UIParticle.EmitterShapeType.Hemisphere;
        public bool IsShapeTypeCone() => this.ShapeType == UIParticle.EmitterShapeType.Cone;
        public bool IsShapeTypeBox() => this.ShapeType == UIParticle.EmitterShapeType.Box;
        public bool IsShapeTypeCircle() => this.ShapeType == UIParticle.EmitterShapeType.Circle;
        public bool IsShapeTypeEdge() => this.ShapeType == UIParticle.EmitterShapeType.Edge;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeSphere")]
        [LabelText("Radius")]
        [Min(0f)]
        [Tooltip("Sphere/Hemisphere radius used for spawning positions.")]
        public float SphereRadius = 32f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeCone")]
        [LabelText("Angle (deg)")]
        [Range(0f, 89.9f)]
        [Tooltip("Cone spread angle in degrees.")]
        public float ConeAngle = 25f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeCone")]
        [LabelText("Radius")]
        [Min(0f)]
        [Tooltip("Cone base radius.")]
        public float ConeRadius = 8f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeBox")]
        [LabelText("Box Size")]
        [Tooltip("Size of the box shape used for emission.")]
        public Vector3 BoxSize = new Vector3(64, 64, 0);

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeCircle")]
        [LabelText("Radius")]
        [Min(0f)]
        [Tooltip("Circle radius used for emission.")]
        public float CircleRadius = 32f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeCircle")]
        [LabelText("Arc (deg)")]
        [Range(0.01f, 360f)]
        [Tooltip("Arc angle of the circle for partial ring emission.")]
        public float CircleArcDegrees = 360f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeEdge")]
        [LabelText("Edge Length")]
        [Min(0f)]
        [Tooltip("Length of the edge line used for emission.")]
        public float EdgeLength = 64f;

        // ─────────────────────────────────────────────────────────────────────────
        // VELOCITY OVER LIFETIME
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Velocity over Lifetime")]
        [LabelText("Use Curves")]
        [Tooltip("If enabled, velocity is controlled by curves over normalized lifetime.")]
        public bool UseVelocityOverLifetimeCurves = false;

        [BoxGroup("Velocity over Lifetime")]
        [HideIf("UseVelocityOverLifetimeCurves")]
        [LabelText("Velocity (constant)")]
        [Tooltip("Constant velocity applied over lifetime.")]
        public Vector3 VelocityOverLifetime = Vector3.zero;

        [BoxGroup("Velocity over Lifetime")]
        [ShowIf("UseVelocityOverLifetimeCurves")]
        [LabelText("Velocity X (curve)")]
        [Tooltip("X-axis velocity over lifetime.")]
        public AnimationCurve velocityX = Constant(0);

        [BoxGroup("Velocity over Lifetime")]
        [ShowIf("UseVelocityOverLifetimeCurves")]
        [LabelText("Velocity Y (curve)")]
        [Tooltip("Y-axis velocity over lifetime.")]
        public AnimationCurve velocityY = Constant(0);

        [BoxGroup("Velocity over Lifetime")]
        [ShowIf("UseVelocityOverLifetimeCurves")]
        [LabelText("Velocity Z (curve)")]
        [Tooltip("Z-axis velocity over lifetime.")]
        public AnimationCurve velocityZ = Constant(0);

        // ─────────────────────────────────────────────────────────────────────────
        // INHERIT VELOCITY
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Inherit Velocity")]
        [LabelText("Enabled")]
        [Tooltip("If enabled, particles inherit velocity from the emitter.")]
        public bool InheritVelocityEnabled = false;

        [BoxGroup("Inherit Velocity")]
        [ShowIf("InheritVelocityEnabled")]
        [LabelText("Mode")]
        [Tooltip("When to sample the emitter velocity: at spawn (Initial) or continuously (Current).")]
        public UIParticle.InheritVelocityMode InheritMode = UIParticle.InheritVelocityMode.Initial;

        [BoxGroup("Inherit Velocity")]
        [ShowIf("InheritVelocityEnabled")]
        [LabelText("Multiplier")]
        [Min(0f)]
        [Tooltip("Scale factor applied to inherited emitter velocity.")]
        public float InheritVelocityMultiplier = 0f;

        // ─────────────────────────────────────────────────────────────────────────
        // LIMIT VELOCITY OVER LIFETIME
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Limit Velocity over Lifetime")]
        [LabelText("Use Curve")]
        [Tooltip("If enabled, caps particle speed using a curve over lifetime.")]
        public bool UseLimitVelocityCurve = false;

        [BoxGroup("Limit Velocity over Lifetime")]
        [HideIf("UseLimitVelocityCurve")]
        [LabelText("Limit (constant)")]
        [Min(0f)]
        [Tooltip("Maximum speed allowed for particles (constant).")]
        public float LimitVelocity = 0f;

        [BoxGroup("Limit Velocity over Lifetime")]
        [ShowIf("UseLimitVelocityCurve")]
        [LabelText("Limit (curve)")]
        [Tooltip("Maximum speed allowed for particles over lifetime.")]
        public AnimationCurve limitVelocityOverLifetime = Constant(0);

        [BoxGroup("Limit Velocity over Lifetime")]
        [LabelText("Dampen")]
        [Tooltip("If enabled, dampens velocity when exceeding the limit for smoother clamping.")]
        public bool LimitVelocityDampen = false;

        // ─────────────────────────────────────────────────────────────────────────
        // FORCE OVER LIFETIME
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Force over Lifetime")]
        [LabelText("Use Curves")]
        [Tooltip("If enabled, forces are controlled by curves over lifetime.")]
        public bool UseForceOverLifetimeCurves = false;

        [BoxGroup("Force over Lifetime")]
        [HideIf("UseForceOverLifetimeCurves")]
        [LabelText("Force (constant)")]
        [Tooltip("Constant force applied over lifetime (X/Y).")]
        public Vector2 ForceOverLifetime = Vector2.zero;

        [BoxGroup("Force over Lifetime")]
        [ShowIf("UseForceOverLifetimeCurves")]
        [LabelText("Force X (curve)")]
        [Tooltip("X-axis force over lifetime.")]
        public AnimationCurve forceX = Constant(0);

        [BoxGroup("Force over Lifetime")]
        [ShowIf("UseForceOverLifetimeCurves")]
        [LabelText("Force Y (curve)")]
        [Tooltip("Y-axis force over lifetime.")]
        public AnimationCurve forceY = Constant(0);

        // ─────────────────────────────────────────────────────────────────────────
        // COLOR OVER LIFETIME
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Color over Lifetime")]
        [LabelText("Use Gradient")]
        [Tooltip("If enabled, particle color is multiplied by this gradient over lifetime.")]
        public bool UseColorOverLifetimeCurve = true;

        [BoxGroup("Color over Lifetime")]
        [ShowIf("UseColorOverLifetimeCurve")]
        [Tooltip("Gradient multiplied with Start Color for each particle across its life.")]
        public Gradient colorOverLifetime = DefaultGradient();

        [BoxGroup("Color over Lifetime")]
        [HideIf("UseColorOverLifetimeCurve")]
        [LabelText("Start Color")]
        [Tooltip("Color at birth when gradient is disabled.")]
        public Color ColorOverLifetimeStart = Color.white;

        [BoxGroup("Color over Lifetime")]
        [HideIf("UseColorOverLifetimeCurve")]
        [LabelText("End Color")]
        [Tooltip("Color near death when gradient is disabled.")]
        public Color ColorOverLifetimeEnd = new Color(1, 1, 1, 0);

        // ─────────────────────────────────────────────────────────────────────────
        // SIZE OVER LIFETIME
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Size over Lifetime")]
        [LabelText("Use Curve")]
        [Tooltip("If enabled, scales particle size by a curve over lifetime.")]
        public bool UseSizeOverLifetimeCurve = true;

        [BoxGroup("Size over Lifetime")]
        [ShowIf("UseSizeOverLifetimeCurve")]
        [Tooltip("Multiplier curve applied to Start Size or Start Size 2D over lifetime.")]
        public AnimationCurve sizeOverLifetime = AnimationCurve.Linear(0, 1, 1, 0);

        [BoxGroup("Size over Lifetime")]
        [HideIf("UseSizeOverLifetimeCurve")]
        [LabelText("Start Multiplier")]
        [Tooltip("Size multiplier at birth when curve is disabled.")]
        public float SizeOverLifetimeStart = 1f;

        [BoxGroup("Size over Lifetime")]
        [HideIf("UseSizeOverLifetimeCurve")]
        [LabelText("End Multiplier")]
        [Tooltip("Size multiplier near death when curve is disabled.")]
        public float SizeOverLifetimeEnd = 0f;

        // ─────────────────────────────────────────────────────────────────────────
        // ROTATION OVER LIFETIME
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Rotation over Lifetime")]
        [LabelText("Use Angular Curve")]
        [Tooltip("If enabled, angular velocity is controlled by a curve over lifetime.")]
        public bool UseAngularVelocityCurve = false;

        [BoxGroup("Rotation over Lifetime")]
        [ShowIf("UseAngularVelocityCurve")]
        [LabelText("Angular Velocity (deg/s)")]
        [Tooltip("Angular velocity in degrees per second over lifetime.")]
        public AnimationCurve angularVelocity = Constant(0);

        [BoxGroup("Rotation over Lifetime")]
        [HideIf("UseAngularVelocityCurve")]
        [LabelText("Rotation (deg/s)")]
        [Tooltip("Constant rotation speed in degrees per second.")]
        public float RotationOverLifetime = 0f;

        [BoxGroup("Rotation over Lifetime")]
        [HideIf("UseAngularVelocityCurve")]
        [LabelText("± Rotation Random")]
        [Min(0f)]
        [Tooltip("Random variation added/subtracted to rotation speed.")]
        public float RotationRandom = 0f;

        // ─────────────────────────────────────────────────────────────────────────
        // BY SPEED
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("By Speed")]
        [LabelText("Size by Speed")]
        [Tooltip("If enabled, particle size scales based on particle speed.")]
        public bool SizeBySpeedEnabled = false;

        [BoxGroup("By Speed")]
        [ShowIf("SizeBySpeedEnabled")]
        [LabelText("Range (Min/Max)")]
        [Tooltip("Speed range used for size scaling evaluation.")]
        public float SizeBySpeedRangeMin = 0f, SizeBySpeedRangeMax = 100f;

        [BoxGroup("By Speed")]
        [ShowIf("SizeBySpeedEnabled")]
        [LabelText("Multiplier (Min/Max)")]
        [Tooltip("Resulting size multiplier at min and max speed.")]
        public float SizeBySpeedMultiplierMin = 1f, SizeBySpeedMultiplierMax = 1f;

        [BoxGroup("By Speed")]
        [LabelText("Color by Speed")]
        [Tooltip("If enabled, particle color blends based on particle speed.")]
        public bool ColorBySpeedEnabled = false;

        [BoxGroup("By Speed")]
        [ShowIf("ColorBySpeedEnabled")]
        [LabelText("Range (Min/Max)")]
        [Tooltip("Speed range used for color blending evaluation.")]
        public float ColorBySpeedRangeMin = 0f, ColorBySpeedRangeMax = 100f;

        [BoxGroup("By Speed")]
        [ShowIf("ColorBySpeedEnabled")]
        [LabelText("Gradient (Start/End)")]
        [Tooltip("Colors used at min and max speed for interpolation.")]
        public Color ColorBySpeedStart = Color.white, ColorBySpeedEnd = new Color(1, 1, 1, 0);

        [BoxGroup("By Speed")]
        [LabelText("Rotation by Speed")]
        [Tooltip("If enabled, rotation speed scales with particle speed.")]
        public bool RotationBySpeedEnabled = false;

        [BoxGroup("By Speed")]
        [ShowIf("RotationBySpeedEnabled")]
        [LabelText("Range (Min/Max)")]
        [Tooltip("Speed range used for rotation scaling evaluation.")]
        public float RotationBySpeedRangeMin = 0f, RotationBySpeedRangeMax = 100f;

        [BoxGroup("By Speed")]
        [ShowIf("RotationBySpeedEnabled")]
        [LabelText("Rotation Factor (deg/s @ max)")]
        [Tooltip("Rotation speed at maximum speed within the range.")]
        public float RotationBySpeedFactor = 0f;

        // ─────────────────────────────────────────────────────────────────────────
        // NOISE
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Noise")]
        [LabelText("Enabled")]
        [Tooltip("If enabled, applies procedural turbulence to particle motion.")]
        public bool NoiseEnabled = false;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        [LabelText("Strength")]
        [Min(0f)]
        [Tooltip("Overall intensity of the noise force.")]
        public float NoiseStrength = 0f;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        [LabelText("Frequency")]
        [Min(0f)]
        [Tooltip("Spatial frequency of noise sampling.")]
        public float NoiseFrequency = 0.1f;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        [LabelText("Octaves")]
        [Range(1, 4)]
        [Tooltip("Number of layered noise octaves for richer detail.")]
        public int NoiseOctaves = 1;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        [LabelText("Scroll Speed")]
        [Tooltip("Temporal scrolling speed of the noise field.")]
        public float NoiseScrollSpeed = 0f;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        [LabelText("Damping")]
        [Range(0f, 1f)]
        [Tooltip("Damps high-frequency jitter for smoother motion.")]
        public float NoiseDamping = 0f;

        // ─────────────────────────────────────────────────────────────────────────
        // RENDERER (arranged similar to Unity's Particle System "Renderer" module)
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Renderer")]
        [Tooltip("Billboard, Stretched Billboard, or other render mode.")]
        public UIParticle.ParticleRenderMode RenderMode = UIParticle.ParticleRenderMode.Billboard;
        public bool IsRenderModeStretchedBillboard() => RenderMode == UIParticle.ParticleRenderMode.StretchedBillboard;

        [BoxGroup("Renderer")]
        [LabelText("Sorting")]
        [Tooltip("Optional sorting strategy for batched UI particles.")]
        public UIParticle.RenderSort Sorting = UIParticle.RenderSort.None;

        [BoxGroup("Renderer")]
        [LabelText("Y Axis Mode")]
        [Tooltip("Defines which direction is considered +Y for billboarding/orientation.")]
        public UIParticle.YAxisMode YAxis = UIParticle.YAxisMode.UI_DownPositive;

        [BoxGroup("Renderer")]
        [LabelText("Align To Velocity")]
        [Tooltip("Rotate particle quads to face the movement direction.")]
        public bool AlignToVelocity = false;

        [BoxGroup("Renderer")]
        [ShowIf("IsRenderModeStretchedBillboard")]
        [LabelText("Stretch Length")]
        [Tooltip("How long the stretched billboard appears along its velocity.")]
        public float StretchLength = 0.5f;

        [BoxGroup("Renderer")]
        [ShowIf("IsRenderModeStretchedBillboard")]
        [LabelText("Stretch Speed Scale")]
        [Tooltip("How strongly speed influences stretch length.")]
        public float StretchSpeedScale = 0.02f;

        [BoxGroup("Renderer")]
        [LabelText("Multiply by Element Opacity")]
        [Tooltip("Multiply particle color by the parent UI element's opacity.")]
        public bool MultiplyByElementOpacity = true;

        // ─────────────────────────────────────────────────────────────────────────
        // UV / TEXTURE
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Texture")]
        [Tooltip("Selects the UV source: plain Texture, Sprite, or Texture Sheet Animation.")]
        public UIParticle.UVSource UVMode = UIParticle.UVSource.Texture;
        public bool IsUVModeTextureOrTextureSheet() => UVMode == UIParticle.UVSource.Texture || UVMode == UIParticle.UVSource.TextureSheet;
        public bool IsUVModeSprite() => UVMode == UIParticle.UVSource.Sprite;
        public bool IsUVModeNotSprite() => UVMode != UIParticle.UVSource.Sprite;
        public bool IsUVModeTextureSheet() => UVMode == UIParticle.UVSource.TextureSheet;
        public bool IsUVModeTextureSheetAndTextureSheetEnabled() => UVMode == UIParticle.UVSource.TextureSheet && TextureSheetEnabled;
        public bool IsUVModeTextureSheetAndTextureSheetEnabledAndSheedModeSingleRow() => UVMode == UIParticle.UVSource.TextureSheet && TextureSheetEnabled && SheetMode == UIParticle.TextureSheetMode.SingleRow;

        // Texture (Texture/Sprite)
        [BoxGroup("Texture")]
        [ShowIf("IsUVModeTextureOrTextureSheet")]
        [Tooltip("Texture used when UV Mode is Texture or Texture Sheet.")]
        public Texture2D Texture;

        [BoxGroup("Texture")]
        [ShowIf("IsUVModeSprite")]
        [Tooltip("Sprite used when UV Mode is Sprite.")]
        public Sprite Sprite;

        [BoxGroup("Texture")]
        [ShowIf("IsUVModeNotSprite")]
        [Tooltip("Flip the U (horizontal) and/or V (vertical) UV coordinates.")]
        public bool FlipU = false, FlipV = false;

        // Texture Sheet Animation
        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheet")]
        [LabelText("Enabled")]
        [Tooltip("Enables texture sheet animation (spritesheet).")]
        public bool TextureSheetEnabled = false;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        [LabelText("Mode")]
        [Tooltip("Animate over the whole sheet or a single row.")]
        public UIParticle.TextureSheetMode SheetMode = UIParticle.TextureSheetMode.WholeSheet;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        [LabelText("Tiles X")]
        [Min(1)]
        [Tooltip("Number of columns in the spritesheet.")]
        public int SheetTilesX = 1;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        [LabelText("Tiles Y")]
        [Min(1)]
        [Tooltip("Number of rows in the spritesheet.")]
        public int SheetTilesY = 1;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        [LabelText("Cycles")]
        [Min(0f)]
        [Tooltip("How many times to cycle through the sheet during particle lifetime.")]
        public float SheetCycles = 1f;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabledAndSheedModeSingleRow")]
        [LabelText("Random Row")]
        [Tooltip("Pick a random row for each particle (Single Row mode only).")]
        public bool SheetRandomRow = false;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        [LabelText("Random Start Frame")]
        [Tooltip("Start each particle on a random frame.")]
        public bool SheetRandomStartFrame = false;

        // ─────────────────────────────────────────────────────────────────────────
        // POSITION & CULLING
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Position & Culling")]
        [LabelText("Emitter Anchor")]
        [Tooltip("Anchor point inside the UI element used as the emission origin.")]
        public UIParticle.ParticleEmitterAnchor Anchor = UIParticle.ParticleEmitterAnchor.Center;

        [BoxGroup("Position & Culling")]
        [LabelText("Emitter Offset")]
        [Tooltip("Offset from the anchor position in local UI space.")]
        public Vector2 EmitterOffset = Vector2.zero;

        [BoxGroup("Position & Culling")]
        [LabelText("Pause When Invisible")]
        [Tooltip("Pause the simulation when the system is not visible by the UI renderer.")]
        public bool PauseWhenInvisible = true;

        [BoxGroup("Position & Culling")]
        [LabelText("Kill When Outside")]
        [Tooltip("Kill particles when they move outside the element bounds plus padding.")]
        public bool KillWhenOutside = false;

        [BoxGroup("Position & Culling")]
        [ShowIf("KillWhenOutside")]
        [LabelText("Cull Padding")]
        [Min(0f)]
        [Tooltip("Extra padding added to bounds checks before killing particles.")]
        public float CullPadding = 32f;

        // ─────────────────────────────────────────────────────────────────────────
        // SYSTEM CURVES (GLOBAL)
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("System Curves")]
        [Tooltip("Multiplier applied to Rate over Time over normalized system time (0..1).")]
        [LabelText("Emission Rate Multiplier")]
        public AnimationCurve emissionRateMultiplier = Constant(1);

#if UNITY_EDITOR
        private void OnValidate()
        {
            // clamps & guards (keep consistent with UIParticle)
            Duration       = Mathf.Max(0.01f, Duration);
            StartLifetime  = Mathf.Max(0.01f, StartLifetime);
            PlaybackSpeed  = Mathf.Max(0f, PlaybackSpeed);
            MaxParticles   = Mathf.Max(1, MaxParticles);

            if (UVMode == UIParticle.UVSource.TextureSheet)
            {
                SheetTilesX = Mathf.Max(1, SheetTilesX);
                SheetTilesY = Mathf.Max(1, SheetTilesY);
                SheetCycles = Mathf.Max(0f, SheetCycles);
            }

            CircleArcDegrees = Mathf.Clamp(CircleArcDegrees, 0.01f, 360f);
            SizeBySpeedRangeMax      = Mathf.Max(SizeBySpeedRangeMin,      SizeBySpeedRangeMax);
            ColorBySpeedRangeMax     = Mathf.Max(ColorBySpeedRangeMin,     ColorBySpeedRangeMax);
            RotationBySpeedRangeMax  = Mathf.Max(RotationBySpeedRangeMin,  RotationBySpeedRangeMax);

            // bump revision for live-sync in UIParticle
            unchecked { revision++; }

            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif

        // Helpers
        private static AnimationCurve Constant(float v) => new AnimationCurve(new Keyframe(0, v), new Keyframe(1, v));
        private static Gradient DefaultGradient()
        {
            var g = new Gradient();
            g.SetKeys(
                new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(0f, 1f) }
            );
            return g;
        }
    }
}
