using UnityEngine;
using Alchemy.Inspector;

namespace UI_Tools
{
    [CreateAssetMenu(fileName = "UIParticleProfile", menuName = "UI Toolkit/UIParticle/UIParticle Profile")]
    public class UIParticleProfile : ScriptableObject
    {
        // ===== hot-reload versioning (dibaca UIParticle) =====
        [HideInInspector] public int revision = 0;

        // ─────────────────────────────────────────────────────────────────────────
        // MAIN
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Main")]
        [Group("Main/Split")]
        [Min(0.01f)]
        public float Duration = 5f;

        [Group("Main/Split")]
        public bool Looping = true;

        [Group("Main/Split")]
        [ShowIf("Looping")]
        public bool Prewarm = false;

        [Group("Main/Split")]
        [LabelText("Play On Attach")]
        public bool PlayOnAttach = true;

        [Group("Main/Split")]
        [Min(0f)]
        public float StartDelay = 0f;

        [Group("Main/Split")]
        [Min(0.01f)]
        public float StartLifetime = 5f;

        [Group("Main/Split")]
        [LabelText("± Lifetime Random (s)")]
        [Min(0f)]
        public float StartLifetimeRandom = 0f;

        [Group("Main/Split")]
        public float StartSpeed = 5f;

        [Group("Main/Split")]
        [LabelText("± Speed Random")]
        [Min(0f)]
        public float StartSpeedRandom = 0f;

        [Group("Main/Split")]
        [LabelText("Use 2D Size")]
        public bool UseStartSize2D = false;

        [Group("Main/Split")]
        [HideIf("UseStartSize2D")]
        [Min(0.01f)]
        public float StartSize = 16f;

        [Group("Main/Split")]
        [HideIf("UseStartSize2D")]
        [LabelText("± Size Random")]
        [Min(0f)]
        public float StartSizeRandom = 0f;

        [Group("Main/Split")]
        [ShowIf("UseStartSize2D")]
        [Min(0.01f)]
        public float StartSizeX = 16f;

        [Group("Main/Split")]
        [ShowIf("UseStartSize2D")]
        [Min(0.01f)]
        public float StartSizeY = 16f;

        [Group("Main/Split")]
        [ShowIf("UseStartSize2D")]
        [LabelText("± Size Random X")]
        [Min(0f)]
        public float StartSizeRandomX = 0f;

        [Group("Main/Split")]
        [ShowIf("UseStartSize2D")]
        [LabelText("± Size Random Y")]
        [Min(0f)]
        public float StartSizeRandomY = 0f;

        [Group("Main/Split")]
        [LabelText("Start Rotation (deg)")]
        public float StartRotation = 0f;

        [Group("Main/Split")]
        [LabelText("± Rotation Random (deg)")]
        [Min(0f)]
        public float StartRotationRandom = 0f;

        [Group("Main/Split")]
        [DisableIf("UseColorOverLifetimeCurve")] // UX: kalau pakai gradient, StartColor dinonaktifkan
        public Color StartColor = Color.white;

        [Group("Main/Split")]
        [LabelText("Gravity Modifier")]
        public float GravityModifier = 0f;

        [Group("Main/Split")]
        [Min(1)]
        public int MaxParticles = 1000;

        [Group("Main/Split")]
        public bool SimulateInEditor = true;

        [Group("Main/Split")]
        [Min(0f)]
        public float PlaybackSpeed = 1f;

        [Group("Main/Split")]
        public bool AutoRandomSeed = true;

        [Group("Main/Split")]
        [HideIf("AutoRandomSeed")]
        public int RandomSeed = 0;

        // ─────────────────────────────────────────────────────────────────────────
        // EMISSION
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Emission")]
        [Group("Emission/Split")]
        [Min(0f)]
        public float RateOverTime = 10f;

        [Group("Emission/Split")]
        [Min(0f)]
        public float RateOverDistance = 0f;

        [Group("Emission/Split")]
        [Min(0)]
        public int BurstCount = 0;
        public bool IsBurst() => BurstCount > 0;

        [Group("Emission/Split")]
        [ShowIf("IsBurst")]
        [Min(0f)]
        public float BurstTime = 0f;

        [Group("Emission/Split")]
        [ShowIf("IsBurst")]
        [Min(0)]
        public int BurstCycles = 1;

        [Group("Emission/Split")]
        [ShowIf("IsBurst")]
        [Min(0f)]
        public float BurstInterval = 0.5f;

        // ─────────────────────────────────────────────────────────────────────────
        // SHAPE
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Shape")]
        public UIParticle.EmitterShapeType ShapeType = UIParticle.EmitterShapeType.Sphere;
        public bool IsShapeTypeSphere() => this.ShapeType == UI_Tools.UIParticle.EmitterShapeType.Sphere || this.ShapeType == UI_Tools.UIParticle.EmitterShapeType.Hemisphere;
        public bool IsShapeTypeCone() => this.ShapeType == UI_Tools.UIParticle.EmitterShapeType.Cone;
        public bool IsShapeTypeBox() => this.ShapeType == UI_Tools.UIParticle.EmitterShapeType.Box;
        public bool IsShapeTypeCircle() => this.ShapeType == UI_Tools.UIParticle.EmitterShapeType.Circle;
        public bool IsShapeTypeEdge() => this.ShapeType == UI_Tools.UIParticle.EmitterShapeType.Edge;
        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeSphere")]
        [Min(0f)]
        public float SphereRadius = 32f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeCone")]
        [LabelText("Cone Angle (deg)")]
        [Range(0f, 89.9f)]
        public float ConeAngle = 25f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeCone")]
        [Min(0f)]
        public float ConeRadius = 8f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeBox")]
        public Vector3 BoxSize = new Vector3(64, 64, 0);

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeCircle")]
        [Min(0f)]
        public float CircleRadius = 32f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeCircle")]
        [Range(0.01f, 360f)]
        public float CircleArcDegrees = 360f;

        [BoxGroup("Shape")]
        [ShowIf("IsShapeTypeEdge")]
        [Min(0f)]
        public float EdgeLength = 64f;

        // ─────────────────────────────────────────────────────────────────────────
        // VELOCITY & FORCES
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Velocity & Forces")]
        [LabelText("Use Velocity Curves")]
        public bool UseVelocityOverLifetimeCurves = false;

        [BoxGroup("Velocity & Forces")]
        [HideIf("UseVelocityOverLifetimeCurves")]
        [LabelText("Velocity (constant)")]
        public Vector3 VelocityOverLifetime = Vector3.zero;

        [BoxGroup("Velocity & Forces")]
        [ShowIf("UseVelocityOverLifetimeCurves")]
        [LabelText("Velocity X (curve)")]
        public AnimationCurve velocityX = Constant(0);

        [BoxGroup("Velocity & Forces")]
        [ShowIf("UseVelocityOverLifetimeCurves")]
        [LabelText("Velocity Y (curve)")]
        public AnimationCurve velocityY = Constant(0);

        [BoxGroup("Velocity & Forces")]
        [ShowIf("UseVelocityOverLifetimeCurves")]
        [LabelText("Velocity Z (curve)")]
        public AnimationCurve velocityZ = Constant(0);

        [BoxGroup("Velocity & Forces")]
        [LabelText("Inherit Velocity")]
        public bool InheritVelocityEnabled = false;

        [BoxGroup("Velocity & Forces")]
        [ShowIf("InheritVelocityEnabled")]
        public UIParticle.InheritVelocityMode InheritMode = UIParticle.InheritVelocityMode.Initial;

        [BoxGroup("Velocity & Forces")]
        [ShowIf("InheritVelocityEnabled")]
        [Min(0f)]
        public float InheritVelocityMultiplier = 0f;

        [BoxGroup("Velocity & Forces")]
        [LabelText("Use Force Curves")]
        public bool UseForceOverLifetimeCurves = false;

        [BoxGroup("Velocity & Forces")]
        [HideIf("UseForceOverLifetimeCurves")]
        [LabelText("Force (constant)")]
        public Vector2 ForceOverLifetime = Vector2.zero;

        [BoxGroup("Velocity & Forces")]
        [ShowIf("UseForceOverLifetimeCurves")]
        [LabelText("Force X (curve)")]
        public AnimationCurve forceX = Constant(0);

        [BoxGroup("Velocity & Forces")]
        [ShowIf("UseForceOverLifetimeCurves")]
        [LabelText("Force Y (curve)")]
        public AnimationCurve forceY = Constant(0);

        // ─────────────────────────────────────────────────────────────────────────
        // LIMITS
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Limits")]
        [LabelText("Use Limit Curve")]
        public bool UseLimitVelocityCurve = false;

        [BoxGroup("Limits")]
        [HideIf("UseLimitVelocityCurve")]
        [Min(0f)]
        public float LimitVelocity = 0f;

        [BoxGroup("Limits")]
        [ShowIf("UseLimitVelocityCurve")]
        [LabelText("Limit (curve)")]
        public AnimationCurve limitVelocityOverLifetime = Constant(0);

        [BoxGroup("Limits")]
        public bool LimitVelocityDampen = false;

        // ─────────────────────────────────────────────────────────────────────────
        // OVER LIFETIME
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Over Lifetime")]
        [LabelText("Use Size Curve")]
        public bool UseSizeOverLifetimeCurve = true;

        [BoxGroup("Over Lifetime")]
        [ShowIf("UseSizeOverLifetimeCurve")]
        [Tooltip("Multiplies StartSize/StartSize2D across life.")]
        public AnimationCurve sizeOverLifetime = AnimationCurve.Linear(0, 1, 1, 0);

        [BoxGroup("Over Lifetime")]
        [HideIf("UseSizeOverLifetimeCurve")]
        public float SizeOverLifetimeStart = 1f;

        [BoxGroup("Over Lifetime")]
        [HideIf("UseSizeOverLifetimeCurve")]
        public float SizeOverLifetimeEnd = 0f;

        [BoxGroup("Over Lifetime")]
        [LabelText("Use Color Gradient")]
        public bool UseColorOverLifetimeCurve = true;

        [BoxGroup("Over Lifetime")]
        [ShowIf("UseColorOverLifetimeCurve")]
        [Tooltip("Multiplied with StartColor per particle.")]
        public Gradient colorOverLifetime = DefaultGradient();

        [BoxGroup("Over Lifetime")]
        [HideIf("UseColorOverLifetimeCurve")]
        public Color ColorOverLifetimeStart = Color.white;

        [BoxGroup("Over Lifetime")]
        [HideIf("UseColorOverLifetimeCurve")]
        public Color ColorOverLifetimeEnd = new Color(1, 1, 1, 0);

        [BoxGroup("Over Lifetime")]
        [LabelText("Use Angular Curve")]
        public bool UseAngularVelocityCurve = false;

        [BoxGroup("Over Lifetime")]
        [ShowIf("UseAngularVelocityCurve")]
        [LabelText("Angular Velocity (deg/s)")]
        public AnimationCurve angularVelocity = Constant(0);

        [BoxGroup("Over Lifetime")]
        [HideIf("UseAngularVelocityCurve")]
        public float RotationOverLifetime = 0f;

        [BoxGroup("Over Lifetime")]
        [HideIf("UseAngularVelocityCurve")]
        [Min(0f)]
        public float RotationRandom = 0f;

        // ─────────────────────────────────────────────────────────────────────────
        // BY SPEED
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("By Speed")]
        public bool SizeBySpeedEnabled = false;

        [BoxGroup("By Speed")]
        [ShowIf("SizeBySpeedEnabled")]
        public float SizeBySpeedRangeMin = 0f, SizeBySpeedRangeMax = 100f;

        [BoxGroup("By Speed")]
        [ShowIf("SizeBySpeedEnabled")]
        public float SizeBySpeedMultiplierMin = 1f, SizeBySpeedMultiplierMax = 1f;

        [BoxGroup("By Speed")]
        public bool ColorBySpeedEnabled = false;

        [BoxGroup("By Speed")]
        [ShowIf("ColorBySpeedEnabled")]
        public float ColorBySpeedRangeMin = 0f, ColorBySpeedRangeMax = 100f;

        [BoxGroup("By Speed")]
        [ShowIf("ColorBySpeedEnabled")]
        public Color ColorBySpeedStart = Color.white, ColorBySpeedEnd = new Color(1, 1, 1, 0);

        [BoxGroup("By Speed")]
        public bool RotationBySpeedEnabled = false;

        [BoxGroup("By Speed")]
        [ShowIf("RotationBySpeedEnabled")]
        public float RotationBySpeedRangeMin = 0f, RotationBySpeedRangeMax = 100f;

        [BoxGroup("By Speed")]
        [ShowIf("RotationBySpeedEnabled")]
        [LabelText("Rotation Factor (deg/s @ max)")]
        public float RotationBySpeedFactor = 0f;

        // ─────────────────────────────────────────────────────────────────────────
        // NOISE
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Noise")]
        public bool NoiseEnabled = false;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        [Min(0f)]
        public float NoiseStrength = 0f;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        [Min(0f)]
        public float NoiseFrequency = 0.1f;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        [Range(1, 4)]
        public int NoiseOctaves = 1;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        public float NoiseScrollSpeed = 0f;

        [BoxGroup("Noise")]
        [ShowIf("NoiseEnabled")]
        [Range(0f, 1f)]
        public float NoiseDamping = 0f;

        // ─────────────────────────────────────────────────────────────────────────
        // RENDERER & UV/TEXTURE
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Renderer")]
        public UIParticle.ParticleRenderMode RenderMode = UIParticle.ParticleRenderMode.Billboard;
        public bool IsRenderModeStretchedBillboard() => RenderMode == UI_Tools.UIParticle.ParticleRenderMode.StretchedBillboard;
        [BoxGroup("Renderer")]
        public UIParticle.RenderSort Sorting = UIParticle.RenderSort.None;

        [BoxGroup("Renderer")]
        public UIParticle.YAxisMode YAxis = UIParticle.YAxisMode.UI_DownPositive;

        [BoxGroup("Renderer")]
        [LabelText("Align To Velocity")]
        public bool AlignToVelocity = false;

        [BoxGroup("Renderer")]
        [ShowIf("IsRenderModeStretchedBillboard")]
        public float StretchLength = 0.5f;

        [BoxGroup("Renderer")]
        [ShowIf("IsRenderModeStretchedBillboard")]
        public float StretchSpeedScale = 0.02f;

        [BoxGroup("Renderer")]
        public bool MultiplyByElementOpacity = true;

        [BoxGroup("Renderer")]
        public UIParticle.UVSource UVMode = UIParticle.UVSource.Texture;
        public bool IsUVModeTextureOrTextureSheet() => UVMode == UI_Tools.UIParticle.UVSource.Texture || UVMode == UI_Tools.UIParticle.UVSource.TextureSheet;
        public bool IsUVModeSprite() => UVMode == UI_Tools.UIParticle.UVSource.Sprite;
        public bool IsUVModeNotSprite() => UVMode != UI_Tools.UIParticle.UVSource.Sprite;
        public bool IsUVModeTextureSheet() => UVMode == UI_Tools.UIParticle.UVSource.TextureSheet;
        public bool IsUVModeTextureSheetAndTextureSheetEnabled() => UVMode == UI_Tools.UIParticle.UVSource.TextureSheet && TextureSheetEnabled;
        public bool IsUVModeTextureSheetAndTextureSheetEnabledAndSheedModeSingleRow() => UVMode == UI_Tools.UIParticle.UVSource.TextureSheet && TextureSheetEnabled && SheetMode == UI_Tools.UIParticle.TextureSheetMode.SingleRow;
        // Texture (Texture/Sprite)
        [BoxGroup("Texture")]
        [ShowIf("IsUVModeTextureOrTextureSheet")]
        public Texture2D Texture;

        [BoxGroup("Texture")]
        [ShowIf("IsUVModeSprite")]
        public Sprite Sprite;

        [BoxGroup("Texture")]
        [ShowIf("IsUVModeNotSprite")]
        public bool FlipU = false, FlipV = false;

        // Texture Sheet
        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheet")]
        public bool TextureSheetEnabled = false;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        public UIParticle.TextureSheetMode SheetMode = UIParticle.TextureSheetMode.WholeSheet;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        [Min(1)] public int SheetTilesX = 1;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        [Min(1)] public int SheetTilesY = 1;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        [Min(0f)] public float SheetCycles = 1f;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabledAndSheedModeSingleRow")]
        public bool SheetRandomRow = false;

        [BoxGroup("Texture Sheet Animation")]
        [ShowIf("IsUVModeTextureSheetAndTextureSheetEnabled")]
        public bool SheetRandomStartFrame = false;

        // ─────────────────────────────────────────────────────────────────────────
        // POSITION & CULLING
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("Position & Culling")]
        public UIParticle.ParticleEmitterAnchor Anchor = UIParticle.ParticleEmitterAnchor.Center;

        [BoxGroup("Position & Culling")]
        public Vector2 EmitterOffset = Vector2.zero;

        [BoxGroup("Position & Culling")]
        [LabelText("Pause When Invisible")]
        public bool PauseWhenInvisible = true;

        [BoxGroup("Position & Culling")]
        public bool KillWhenOutside = false;

        [BoxGroup("Position & Culling")]
        [ShowIf("KillWhenOutside")]
        [Min(0f)]
        public float CullPadding = 32f;

        // ─────────────────────────────────────────────────────────────────────────
        // SYSTEM CURVES (GLOBAL)
        // ─────────────────────────────────────────────────────────────────────────
        [BoxGroup("System Curves")]
        [Tooltip("Multiplier to RateOverTime over system normalized time (0..1).")]
        public AnimationCurve emissionRateMultiplier = Constant(1);

#if UNITY_EDITOR
        private void OnValidate()
        {
            // clamps & guards (agar konsisten dgn UIParticle)
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
            SizeBySpeedRangeMax   = Mathf.Max(SizeBySpeedRangeMin,   SizeBySpeedRangeMax);
            ColorBySpeedRangeMax  = Mathf.Max(ColorBySpeedRangeMin,  ColorBySpeedRangeMax);
            RotationBySpeedRangeMax = Mathf.Max(RotationBySpeedRangeMin, RotationBySpeedRangeMax);

            // bump revision untuk live-sync di UIParticle
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
