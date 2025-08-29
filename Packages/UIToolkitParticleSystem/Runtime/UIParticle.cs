// -----------------------------------------------------------------------------
// UI_Tools.UIParticle (PROFILE-ONLY)
// - Menghapus seluruh sistem binding/konversi dari ParticleSystem
// - Sumber konfigurasi HANYA dari UIParticleProfile
// - Mendukung: emisi (time/distance), single-burst (dari Profile), prewarm,
//   shape (Sphere/Hemisphere/Cone/Box/Circle/Edge), velocity/force curves,
//   limit velocity, color/size/rotation over lifetime, by-speed modifiers,
//   noise, render modes (billboard/horizontal/vertical/stretched),
//   texture/sprite/texture sheet (whole/single row), sorting, culling, Y-axis
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace MainraGames
{
    [UxmlElement]
    public partial class UIParticle : VisualElement
    {
        public enum EmitterShapeType { Sphere, Hemisphere, Cone, Box, Circle, Edge }
        public enum ParticleRenderMode { Billboard, StretchedBillboard, HorizontalBillboard, VerticalBillboard }
        public enum ParticleEmitterAnchor { Center, TopLeft, TopRight, BottomLeft, BottomRight }
        public enum YAxisMode { UI_DownPositive, World_UpPositive }
        public enum RenderSort { None, OldestOnTop, NewestOnTop, LargestOnTop, SmallestOnTop }
        public enum UVSource { Texture, Sprite, TextureSheet }
        public enum InheritVelocityMode { Initial, Current }
        public enum TextureSheetMode { WholeSheet, SingleRow }

        [UxmlAttribute, Tooltip("Assign a UIParticleProfile ScriptableObject.")]
        public UIParticleProfile Profile;

        [Serializable]
        struct Config
        {
            // --- MAIN ---
            public float Duration;
            public bool Looping;
            public bool Prewarm;
            public bool PlayOnAttach;
            public float StartDelay;
            public float StartLifetime;
            public float StartLifetimeRandom;
            public float StartSpeed;
            public float StartSpeedRandom;

            public bool UseStartSize2D;
            public float StartSize;
            public float StartSizeRandom;
            public float StartSizeX;
            public float StartSizeY;
            public float StartSizeRandomX;
            public float StartSizeRandomY;

            public float StartRotation;
            public float StartRotationRandom;
            public Color StartColor;
            public float GravityModifier;
            public int   MaxParticles;
            public bool  SimulateInEditor;
            public float PlaybackSpeed;
            public bool  AutoRandomSeed;
            public int   RandomSeed;

            // --- EMISSION ---
            public float RateOverTime;
            public float RateOverDistance;
            public int   BurstCount;
            public float BurstTime;
            public int   BurstCycles;
            public float BurstInterval;

            // --- SHAPE ---
            public EmitterShapeType ShapeType;
            public float SphereRadius;
            public float ConeAngle;
            public float ConeRadius;
            public Vector3 BoxSize;
            public float CircleRadius;
            public float CircleArcDegrees;
            public float EdgeLength;

            // --- VELOCITY & FORCE ---
            public Vector3 VelocityOverLifetime;
            public bool InheritVelocityEnabled;
            public InheritVelocityMode InheritMode;
            public float InheritVelocityMultiplier;
            public Vector2 ForceOverLifetime;

            // --- LIMITS ---
            public float LimitVelocity;
            public bool  LimitVelocityDampen;

            // --- OVER LIFETIME ---
            public Color ColorOverLifetimeStart;
            public Color ColorOverLifetimeEnd;
            public float SizeOverLifetimeStart;
            public float SizeOverLifetimeEnd;
            public float RotationOverLifetime;
            public float RotationRandom;

            // --- BY SPEED ---
            public bool  SizeBySpeedEnabled;
            public float SizeBySpeedRangeMin, SizeBySpeedRangeMax;
            public float SizeBySpeedMultiplierMin, SizeBySpeedMultiplierMax;

            public bool  ColorBySpeedEnabled;
            public float ColorBySpeedRangeMin, ColorBySpeedRangeMax;
            public Color ColorBySpeedStart, ColorBySpeedEnd;

            public bool  RotationBySpeedEnabled;
            public float RotationBySpeedRangeMin, RotationBySpeedRangeMax;
            public float RotationBySpeedFactor;

            // --- NOISE ---
            public bool  NoiseEnabled;
            public float NoiseStrength;
            public float NoiseFrequency;
            public int   NoiseOctaves;
            public float NoiseScrollSpeed;
            public float NoiseDamping;

            // --- RENDERER ---
            public ParticleRenderMode RenderMode;
            public bool AlignToVelocity;
            public float StretchLength;
            public float StretchSpeedScale;
            public RenderSort Sorting;
            public bool MultiplyByElementOpacity;

            // --- TEXTURE/UV ---
            public UVSource UVMode;
            public Texture2D Texture;
            public Sprite Sprite;
            public bool FlipU, FlipV;

            // Texture Sheet
            public bool TextureSheetEnabled;
            public TextureSheetMode SheetMode;
            public int  SheetTilesX, SheetTilesY;
            public float SheetCycles;
            public bool SheetRandomRow, SheetRandomStartFrame;

            // opsional start-frame control (default: 0..1 random jika RandomStartFrame true)
            public float SheetStartFrameConst;
            public float SheetStartFrameMin, SheetStartFrameMax;

            // --- POSITION/CULL ---
            public YAxisMode YAxis;
            public ParticleEmitterAnchor Anchor;
            public Vector2 EmitterOffset;
            public bool PauseWhenInvisible;
            public bool KillWhenOutside;
            public float CullPadding;

            // --- CURVES ---
            public AnimationCurve sizeOverLifetime;
            public Gradient       colorOverLifetime;
            public AnimationCurve velocityX, velocityY, velocityZ;
            public AnimationCurve forceX, forceY;
            public AnimationCurve angularVelocity;
            public AnimationCurve emissionRateMultiplier;
            public AnimationCurve limitVelocityOverLifetime;
        }

        struct Particle
        {
            public Vector3 position, velocity;
            public float lifetime, startLifetime;
            public float startSize, size;
            public float startSizeX, startSizeY;
            public float sizeX, sizeY;
            public float rotation, angularVelocity;
            public Color32 baseColor;
            public Color32 tint;
            public int sheetRow;
            public float sheetStart;
            public uint spawnIndex;
            public float noiseSeedX, noiseSeedY;
        }

        readonly List<Particle> _particles = new();
        readonly List<int> _drawOrder = new();

        Config _cfg;
        int _lastProfileRevision = -1;

        System.Random _rng = new();
        float _emitAccumulator;
        float _emitDistanceAccumulator;
        float _playbackTime;
        bool  _isPlaying;
        bool  _isSetup;
        IVisualElementScheduledItem _tickItem;
        double _lastTickTime;
        uint _spawnCounter;

        Vector2 _lastEmitterOrigin;
        bool _hasLastEmitterOrigin;
        Vector2 _emitterVelocityUI;

        float _nextBurstTime;
        int _burstCyclesLeft;
        bool _burstInit;

        public UIParticle()
        {
            AddToClassList("ui-particle");
            generateVisualContent += OnGenerateVisualContent;
            pickingMode = PickingMode.Ignore;

            RegisterCallback<AttachToPanelEvent>(_ =>
            {
                ApplySource(force: true);
                Setup();
                if (_cfg.PlayOnAttach) Start();
            });
            RegisterCallback<DetachFromPanelEvent>(_ => Stop());
        }

        // PROFILE ONLY
        void ApplySource(bool force = false) => ApplyFromProfile(force);

        void ApplyFromProfile(bool force)
        {
            if (Profile == null) return;
            if (!force && _lastProfileRevision == Profile.revision) return;

            _cfg.Duration              = Profile.Duration;
            _cfg.Looping               = Profile.Looping;
            _cfg.Prewarm               = Profile.Prewarm;
            _cfg.PlayOnAttach          = Profile.PlayOnAttach;
            _cfg.StartDelay            = Profile.StartDelay;

            _cfg.StartLifetime         = Profile.StartLifetime;
            _cfg.StartLifetimeRandom   = Profile.StartLifetimeRandom;
            _cfg.StartSpeed            = Profile.StartSpeed;
            _cfg.StartSpeedRandom      = Profile.StartSpeedRandom;

            _cfg.UseStartSize2D        = Profile.UseStartSize2D;
            _cfg.StartSize             = Profile.StartSize;
            _cfg.StartSizeRandom       = Profile.StartSizeRandom;
            _cfg.StartSizeX            = Profile.StartSizeX;
            _cfg.StartSizeY            = Profile.StartSizeY;
            _cfg.StartSizeRandomX      = Profile.StartSizeRandomX;
            _cfg.StartSizeRandomY      = Profile.StartSizeRandomY;

            _cfg.StartRotation         = Profile.StartRotation;
            _cfg.StartRotationRandom   = Profile.StartRotationRandom;
            _cfg.StartColor            = Profile.StartColor;
            _cfg.GravityModifier       = Profile.GravityModifier;
            _cfg.MaxParticles          = Profile.MaxParticles;
            _cfg.SimulateInEditor      = Profile.SimulateInEditor;
            _cfg.PlaybackSpeed         = Profile.PlaybackSpeed;
            _cfg.AutoRandomSeed        = Profile.AutoRandomSeed;
            _cfg.RandomSeed            = Profile.RandomSeed;

            _cfg.RateOverTime          = Profile.RateOverTime;
            _cfg.RateOverDistance      = Profile.RateOverDistance;
            _cfg.BurstCount            = Profile.BurstCount;
            _cfg.BurstTime             = Profile.BurstTime;
            _cfg.BurstCycles           = Profile.BurstCycles;
            _cfg.BurstInterval         = Profile.BurstInterval;

            _cfg.ShapeType             = Profile.ShapeType;
            _cfg.SphereRadius          = Profile.SphereRadius;
            _cfg.ConeAngle             = Profile.ConeAngle;
            _cfg.ConeRadius            = Profile.ConeRadius;
            _cfg.BoxSize               = Profile.BoxSize;
            _cfg.CircleRadius          = Profile.CircleRadius;
            _cfg.CircleArcDegrees      = Profile.CircleArcDegrees;
            _cfg.EdgeLength            = Profile.EdgeLength;

            _cfg.VelocityOverLifetime  = Profile.UseVelocityOverLifetimeCurves ? Vector3.zero : Profile.VelocityOverLifetime;
            _cfg.InheritVelocityEnabled= Profile.InheritVelocityEnabled;
            _cfg.InheritMode           = Profile.InheritMode;
            _cfg.InheritVelocityMultiplier = Profile.InheritVelocityMultiplier;
            _cfg.ForceOverLifetime     = Profile.UseForceOverLifetimeCurves ? Vector2.zero : Profile.ForceOverLifetime;

            _cfg.LimitVelocity         = Profile.UseLimitVelocityCurve ? 0f : Profile.LimitVelocity;
            _cfg.LimitVelocityDampen   = Profile.LimitVelocityDampen;

            _cfg.ColorOverLifetimeStart= Profile.UseColorOverLifetimeCurve ? Color.white : Profile.ColorOverLifetimeStart;
            _cfg.ColorOverLifetimeEnd  = Profile.UseColorOverLifetimeCurve ? new Color(1,1,1,1) : Profile.ColorOverLifetimeEnd;
            _cfg.SizeOverLifetimeStart = Profile.UseSizeOverLifetimeCurve ? 1f : Profile.SizeOverLifetimeStart;
            _cfg.SizeOverLifetimeEnd   = Profile.UseSizeOverLifetimeCurve ? 1f : Profile.SizeOverLifetimeEnd;
            _cfg.RotationOverLifetime  = Profile.UseAngularVelocityCurve ? 0f : Profile.RotationOverLifetime;
            _cfg.RotationRandom        = Profile.UseAngularVelocityCurve ? 0f : Profile.RotationRandom;

            _cfg.SizeBySpeedEnabled    = Profile.SizeBySpeedEnabled;
            _cfg.SizeBySpeedRangeMin   = Profile.SizeBySpeedRangeMin;
            _cfg.SizeBySpeedRangeMax   = Profile.SizeBySpeedRangeMax;
            _cfg.SizeBySpeedMultiplierMin = Profile.SizeBySpeedMultiplierMin;
            _cfg.SizeBySpeedMultiplierMax = Profile.SizeBySpeedMultiplierMax;

            _cfg.ColorBySpeedEnabled   = Profile.ColorBySpeedEnabled;
            _cfg.ColorBySpeedRangeMin  = Profile.ColorBySpeedRangeMin;
            _cfg.ColorBySpeedRangeMax  = Profile.ColorBySpeedRangeMax;
            _cfg.ColorBySpeedStart     = Profile.ColorBySpeedStart;
            _cfg.ColorBySpeedEnd       = Profile.ColorBySpeedEnd;

            _cfg.RotationBySpeedEnabled= Profile.RotationBySpeedEnabled;
            _cfg.RotationBySpeedRangeMin= Profile.RotationBySpeedRangeMin;
            _cfg.RotationBySpeedRangeMax= Profile.RotationBySpeedRangeMax;
            _cfg.RotationBySpeedFactor = Profile.RotationBySpeedFactor;

            _cfg.NoiseEnabled          = Profile.NoiseEnabled;
            _cfg.NoiseStrength         = Profile.NoiseStrength;
            _cfg.NoiseFrequency        = Profile.NoiseFrequency;
            _cfg.NoiseOctaves          = Profile.NoiseOctaves;
            _cfg.NoiseScrollSpeed      = Profile.NoiseScrollSpeed;
            _cfg.NoiseDamping          = Profile.NoiseDamping;

            _cfg.RenderMode            = Profile.RenderMode;
            _cfg.AlignToVelocity       = Profile.AlignToVelocity;
            _cfg.StretchLength         = Profile.StretchLength;
            _cfg.StretchSpeedScale     = Profile.StretchSpeedScale;
            _cfg.Sorting               = Profile.Sorting;
            _cfg.MultiplyByElementOpacity = Profile.MultiplyByElementOpacity;

            _cfg.UVMode                = Profile.UVMode;
            _cfg.Texture               = Profile.Texture;
            _cfg.Sprite                = Profile.Sprite;
            _cfg.FlipU                 = Profile.FlipU;
            _cfg.FlipV                 = Profile.FlipV;

            _cfg.TextureSheetEnabled   = Profile.TextureSheetEnabled;
            _cfg.SheetMode             = Profile.SheetMode;
            _cfg.SheetTilesX           = Profile.SheetTilesX;
            _cfg.SheetTilesY           = Profile.SheetTilesY;
            _cfg.SheetCycles           = Profile.SheetCycles;
            _cfg.SheetRandomRow        = Profile.SheetRandomRow;
            _cfg.SheetRandomStartFrame = Profile.SheetRandomStartFrame;
            _cfg.SheetStartFrameConst  = 0f;
            _cfg.SheetStartFrameMin    = 0f;
            _cfg.SheetStartFrameMax    = 1f;

            _cfg.YAxis                 = Profile.YAxis;
            _cfg.Anchor                = Profile.Anchor;
            _cfg.EmitterOffset         = Profile.EmitterOffset;
            _cfg.PauseWhenInvisible    = Profile.PauseWhenInvisible;
            _cfg.KillWhenOutside       = Profile.KillWhenOutside;
            _cfg.CullPadding           = Profile.CullPadding;

            _cfg.sizeOverLifetime      = Profile.UseSizeOverLifetimeCurve ? Profile.sizeOverLifetime : null;
            _cfg.colorOverLifetime     = Profile.UseColorOverLifetimeCurve ? Profile.colorOverLifetime : null;
            _cfg.velocityX             = Profile.UseVelocityOverLifetimeCurves ? Profile.velocityX : null;
            _cfg.velocityY             = Profile.UseVelocityOverLifetimeCurves ? Profile.velocityY : null;
            _cfg.velocityZ             = Profile.UseVelocityOverLifetimeCurves ? Profile.velocityZ : null;
            _cfg.forceX                = Profile.UseForceOverLifetimeCurves ? Profile.forceX : null;
            _cfg.forceY                = Profile.UseForceOverLifetimeCurves ? Profile.forceY : null;
            _cfg.angularVelocity       = Profile.UseAngularVelocityCurve ? Profile.angularVelocity : null;
            _cfg.emissionRateMultiplier= Profile.emissionRateMultiplier;
            _cfg.limitVelocityOverLifetime = Profile.UseLimitVelocityCurve ? Profile.limitVelocityOverLifetime : null;

            _lastProfileRevision = Profile.revision;

            ReinitRng();
            HandlePrewarmOrInitialBurst();
            MarkDirtyRepaint();
        }

        // ----------------- LIFECYCLE -----------------
        public void Setup()
        {
            if (_isSetup) return;
            _isSetup = true;
            _isPlaying = false;
        }

        void Start()
        {
#if UNITY_EDITOR
            if (panel?.contextType == ContextType.Editor && !_cfg.SimulateInEditor)
            {
                _tickItem ??= schedule.Execute(() =>
                {
                    ApplySource();
                    MarkDirtyRepaint();
                }).Every(250);
                return;
            }
#endif
            if (_tickItem == null)
            {
                _lastTickTime = Time.realtimeSinceStartupAsDouble;
                _tickItem = schedule.Execute(Tick).Every(16).StartingIn(Mathf.Max(0, (int)(_cfg.StartDelay * 1000)));
            }
            else
            {
                _tickItem.Resume();
            }

            _isPlaying = true;
        }

        void Stop() => _tickItem?.Pause();

        public bool IsAlive() =>
            _particles.Count > 0 || (_isPlaying && (_cfg.RateOverTime > 0f || _cfg.RateOverDistance > 0f));

        public new void Clear()
        {
            base.Clear();
            _particles.Clear();
            MarkDirtyRepaint();
        }

        public void Burst(int count)
        {
            if (count <= 0) return;
            int room = Mathf.Max(0, _cfg.MaxParticles - _particles.Count);
            count = Mathf.Min(count, room);
            for (int i = 0; i < count; i++)
                _particles.Add(CreateParticle());
            MarkDirtyRepaint();
        }

        void Tick()
        {
            ApplySource();

            if (_cfg.PauseWhenInvisible && (!visible || resolvedStyle.display == DisplayStyle.None))
            {
                _lastTickTime = Time.realtimeSinceStartupAsDouble;
                MarkDirtyRepaint();
                return;
            }

            double now = Time.realtimeSinceStartupAsDouble;
            float dt = Mathf.Clamp((float)(now - _lastTickTime), 0f, 0.1f) * Mathf.Max(0f, _cfg.PlaybackSpeed);
            _lastTickTime = now;

            Vector2 origin = GetEmitterOrigin(contentRect);
            if (_hasLastEmitterOrigin && dt > 0f) _emitterVelocityUI = (origin - _lastEmitterOrigin) / dt;
            _lastEmitterOrigin = origin;
            _hasLastEmitterOrigin = true;

            if (_isPlaying) _playbackTime += dt;

            if (_isPlaying && !_cfg.Looping && _playbackTime >= _cfg.Duration)
                _isPlaying = false;

            float rateMul = 1f;
            if (_cfg.emissionRateMultiplier != null && _cfg.Duration > 0.0001f)
            {
                float sys01 = _cfg.Looping ? Mathf.Repeat(_playbackTime, _cfg.Duration) / _cfg.Duration : Mathf.Clamp01(_playbackTime / _cfg.Duration);
                rateMul = Mathf.Max(0f, _cfg.emissionRateMultiplier.Evaluate(sys01));
            }

            // Emission over time
            if (_isPlaying && _cfg.RateOverTime > 0f)
            {
                _emitAccumulator += _cfg.RateOverTime * rateMul * dt;
                while (_emitAccumulator >= 1f && _particles.Count < _cfg.MaxParticles)
                {
                    _particles.Add(CreateParticle());
                    _emitAccumulator -= 1f;
                }
            }

            // Emission over distance
            if (_isPlaying && _cfg.RateOverDistance > 0f && _hasLastEmitterOrigin)
            {
                float moved = _emitterVelocityUI.magnitude * dt;
                _emitDistanceAccumulator += moved * _cfg.RateOverDistance;
                while (_emitDistanceAccumulator >= 1f && _particles.Count < _cfg.MaxParticles)
                {
                    _particles.Add(CreateParticle());
                    _emitDistanceAccumulator -= 1f;
                }
            }

            // Single-burst (dari Profile)
            if (_isPlaying && _cfg.BurstCount > 0 && _cfg.BurstCycles != 0)
            {
                if (!_burstInit)
                {
                    _nextBurstTime = Mathf.Max(0f, _cfg.BurstTime);
                    _burstCyclesLeft = Mathf.Max(0, _cfg.BurstCycles);
                    _burstInit = true;
                }

                float localT = _cfg.Looping && _cfg.Duration > 0f ? Mathf.Repeat(_playbackTime, _cfg.Duration) : _playbackTime;

                while (_burstCyclesLeft > 0 && localT >= _nextBurstTime && _particles.Count < _cfg.MaxParticles)
                {
                    Burst(_cfg.BurstCount);
                    _burstCyclesLeft--;
                    _nextBurstTime += Mathf.Max(0.001f, _cfg.BurstInterval);

                    if (_cfg.Looping && _cfg.Duration > 0f)
                    {
                        while (_nextBurstTime >= _cfg.Duration) _nextBurstTime -= _cfg.Duration;
                        if (_burstCyclesLeft == 0) _burstCyclesLeft = Mathf.Max(0, _cfg.BurstCycles);
                    }
                }
            }

            Rect rect = contentRect;
            Rect cullRect = new Rect(rect.xMin - _cfg.CullPadding, rect.yMin - _cfg.CullPadding, rect.width + 2 * _cfg.CullPadding, rect.height + 2 * _cfg.CullPadding);

            Vector3 accConst = new Vector3(_cfg.ForceOverLifetime.x, _cfg.ForceOverLifetime.y + (-9.81f * _cfg.GravityModifier), 0f);

            for (int i = _particles.Count - 1; i >= 0; i--)
            {
                var p = _particles[i];
                p.lifetime -= dt;
                if (p.lifetime <= 0f) { _particles.RemoveAt(i); continue; }

                float age01 = 1f - (p.lifetime / p.startLifetime);

                if (_cfg.InheritVelocityEnabled && _cfg.InheritMode == InheritVelocityMode.Current)
                    p.velocity += new Vector3(_emitterVelocityUI.x, UnmapYFromUI(_emitterVelocityUI.y), 0) * _cfg.InheritVelocityMultiplier * dt;

                Vector3 acc = accConst;
                if (_cfg.forceX != null) acc.x += _cfg.forceX.Evaluate(age01);
                if (_cfg.forceY != null) acc.y += _cfg.forceY.Evaluate(age01);
                if (_cfg.velocityX != null) p.velocity.x += _cfg.velocityX.Evaluate(age01) * dt;
                if (_cfg.velocityY != null) p.velocity.y += _cfg.velocityY.Evaluate(age01) * dt;
                if (_cfg.velocityZ != null) p.velocity.z += _cfg.velocityZ.Evaluate(age01) * dt;

                if (_cfg.NoiseEnabled && _cfg.NoiseStrength > 0f)
                {
                    float t = (float)(_playbackTime) * _cfg.NoiseFrequency + _cfg.NoiseScrollSpeed * 0.01f;
                    float nx = (Mathf.PerlinNoise(p.noiseSeedX + t, p.noiseSeedY) - 0.5f) * 2f;
                    float ny = (Mathf.PerlinNoise(p.noiseSeedY - t, p.noiseSeedX) - 0.5f) * 2f;

                    float amp = 1f, sumX = nx, sumY = ny, totalAmp = 1f;
                    for (int o = 1; o < Mathf.Max(1, _cfg.NoiseOctaves); o++)
                    {
                        float tt = t * (o + 1);
                        float ax = (Mathf.PerlinNoise(p.noiseSeedX + tt, p.noiseSeedY + tt) - 0.5f) * 2f;
                        float ay = (Mathf.PerlinNoise(p.noiseSeedY - tt, p.noiseSeedX - tt) - 0.5f) * 2f;
                        amp *= 0.5f; totalAmp += amp;
                        sumX += ax * amp; sumY += ay * amp;
                    }
                    sumX /= totalAmp; sumY /= totalAmp;

                    p.velocity.x += sumX * _cfg.NoiseStrength * dt;
                    p.velocity.y += sumY * _cfg.NoiseStrength * dt;

                    if (_cfg.NoiseDamping > 0f) p.velocity *= Mathf.Clamp01(1f - _cfg.NoiseDamping * dt);
                }

                p.velocity += acc * dt;
                p.velocity += _cfg.VelocityOverLifetime * dt;

                float vLimit = _cfg.LimitVelocity;
                if (_cfg.limitVelocityOverLifetime != null)
                    vLimit = _cfg.limitVelocityOverLifetime.Evaluate(age01);

                if (vLimit > 0f)
                {
                    float spd = p.velocity.magnitude;
                    if (spd > vLimit)
                        p.velocity = (_cfg.LimitVelocityDampen && spd > 1e-6f) ? p.velocity * (vLimit / spd) : p.velocity.normalized * vLimit;
                }

                p.position += p.velocity * dt;

                float ang = _cfg.RotationOverLifetime;
                if (_cfg.angularVelocity != null) ang += _cfg.angularVelocity.Evaluate(age01);
                if (_cfg.RotationBySpeedEnabled)
                {
                    float spd2D = new Vector2(p.velocity.x, p.velocity.y).magnitude;
                    float s01 = Mathf.InverseLerp(_cfg.RotationBySpeedRangeMin, _cfg.RotationBySpeedRangeMax, spd2D);
                    ang += _cfg.RotationBySpeedFactor * s01;
                }
                p.angularVelocity = ang;
                p.rotation += p.angularVelocity * dt;

                float sizeMul = Mathf.Lerp(_cfg.SizeOverLifetimeStart, _cfg.SizeOverLifetimeEnd, age01);
                if (_cfg.sizeOverLifetime != null) sizeMul = _cfg.sizeOverLifetime.Evaluate(age01);

                if (_cfg.SizeBySpeedEnabled)
                {
                    float spd2D = new Vector2(p.velocity.x, p.velocity.y).magnitude;
                    float s01 = Mathf.InverseLerp(_cfg.SizeBySpeedRangeMin, _cfg.SizeBySpeedRangeMax, spd2D);
                    sizeMul *= Mathf.Lerp(_cfg.SizeBySpeedMultiplierMin, _cfg.SizeBySpeedMultiplierMax, s01);
                }

                if (_cfg.UseStartSize2D)
                {
                    p.sizeX = Mathf.Max(0.01f, p.startSizeX * sizeMul);
                    p.sizeY = Mathf.Max(0.01f, p.startSizeY * sizeMul);
                }
                else
                {
                    p.size = Mathf.Max(0.01f, p.startSize * sizeMul);
                    p.sizeX = p.sizeY = p.size;
                }

                Color colr = Color.Lerp(_cfg.ColorOverLifetimeStart, _cfg.ColorOverLifetimeEnd, age01);
                if (_cfg.colorOverLifetime != null) colr = _cfg.colorOverLifetime.Evaluate(age01);
                Color finalCol = colr * (Color)p.baseColor;

                if (_cfg.ColorBySpeedEnabled)
                {
                    float spd2D = new Vector2(p.velocity.x, p.velocity.y).magnitude;
                    float s01 = Mathf.InverseLerp(_cfg.ColorBySpeedRangeMin, _cfg.ColorBySpeedRangeMax, spd2D);
                    finalCol *= Color.Lerp(_cfg.ColorBySpeedStart, _cfg.ColorBySpeedEnd, s01);
                }
                p.tint = (Color32)finalCol;

                if (_cfg.KillWhenOutside)
                {
                    Vector2 world = origin + new Vector2(p.position.x, MapYForUI(p.position.y));
                    if (!cullRect.Contains(world)) { _particles.RemoveAt(i); continue; }
                }

                _particles[i] = p;
            }

            MarkDirtyRepaint();
        }

        // ----------------- RENDER -----------------
        void OnGenerateVisualContent(MeshGenerationContext mgc)
        {
            if (_particles.Count == 0) return;

            var rect = contentRect;
            if (rect.width <= 0f || rect.height <= 0f) return;

            Texture2D tex = ResolveTexture();
            if (tex == null) tex = Texture2D.whiteTexture;

            Vector2 emitterOrigin = GetEmitterOrigin(rect);

            int quadCount = _particles.Count;
            var mesh = mgc.Allocate(quadCount * 4, quadCount * 6, tex);

            _drawOrder.Clear();
            _drawOrder.Capacity = Mathf.Max(_drawOrder.Capacity, _particles.Count);
            for (int i = 0; i < _particles.Count; i++) _drawOrder.Add(i);

            switch (_cfg.Sorting)
            {
                case RenderSort.OldestOnTop:   _drawOrder.Sort((a, b) => _particles[a].spawnIndex.CompareTo(_particles[b].spawnIndex)); break;
                case RenderSort.NewestOnTop:   _drawOrder.Sort((a, b) => _particles[b].spawnIndex.CompareTo(_particles[a].spawnIndex)); break;
                case RenderSort.LargestOnTop:  _drawOrder.Sort((a, b) => (_particles[b].sizeX * _particles[b].sizeY).CompareTo(_particles[a].sizeX * _particles[a].sizeY)); break;
                case RenderSort.SmallestOnTop: _drawOrder.Sort((a, b) => (_particles[a].sizeX * _particles[a].sizeY).CompareTo(_particles[b].sizeX * _particles[b].sizeY)); break;
            }

            float elementOpacity = _cfg.MultiplyByElementOpacity ? Mathf.Clamp01(resolvedStyle.opacity) : 1f;
            Rect baseUV = GetBaseUVRect(tex);

            int vtxBase = 0;
            for (int idx = 0; idx < _drawOrder.Count; idx++)
            {
                var p = _particles[_drawOrder[idx]];
                Vector2 worldPos = emitterOrigin + new Vector2(p.position.x, MapYForUI(p.position.y));

                Vector2 right, up;
                switch (_cfg.RenderMode)
                {
                    default:
                    case ParticleRenderMode.Billboard:
                    case ParticleRenderMode.VerticalBillboard:
                    {
                        if (_cfg.AlignToVelocity)
                        {
                            Vector2 dir = new Vector2(p.velocity.x, MapYForUI(p.velocity.y));
                            if (dir.sqrMagnitude < 1e-6f) dir = Vector2.right;
                            dir.Normalize();
                            Vector2 nRight = dir;
                            Vector2 nUp = new Vector2(-dir.y, dir.x);
                            right = nRight * (p.sizeX * 0.5f);
                            up    = nUp    * (p.sizeY * 0.5f);
                        }
                        else
                        {
                            float rad = p.rotation * Mathf.Deg2Rad;
                            float cs = Mathf.Cos(rad);
                            float sn = Mathf.Sin(rad);
                            Vector2 xAxis = new Vector2(cs, sn);
                            Vector2 yAxis = new Vector2(-sn, cs);
                            right = xAxis * (p.sizeX * 0.5f);
                            up    = yAxis * (p.sizeY * 0.5f);
                        }
                        break;
                    }
                    case ParticleRenderMode.HorizontalBillboard:
                    {
                        right = new Vector2(p.sizeX * 0.5f, 0);
                        up    = new Vector2(0, p.sizeY * 0.5f);
                        break;
                    }
                    case ParticleRenderMode.StretchedBillboard:
                    {
                        Vector2 dir = new Vector2(p.velocity.x, MapYForUI(p.velocity.y));
                        if (dir.sqrMagnitude < 1e-6f) dir = Vector2.right;
                        dir.Normalize();

                        Vector2 nRight = dir;
                        Vector2 nUp = new Vector2(-dir.y, dir.x);

                        float widthHalf  = Mathf.Max(0.5f, p.sizeY * 0.5f);
                        float lengthHalf = (p.sizeX * (1f + _cfg.StretchLength + _cfg.StretchSpeedScale * new Vector2(p.velocity.x, p.velocity.y).magnitude)) * 0.5f;

                        right = nRight * lengthHalf;
                        up    = nUp    * widthHalf;
                        break;
                    }
                }

                Vector3 v0 = new(worldPos.x - right.x - up.x, worldPos.y - right.y - up.y, Vertex.nearZ);
                Vector3 v1 = new(worldPos.x + right.x - up.x, worldPos.y + right.y - up.y, Vertex.nearZ);
                Vector3 v2 = new(worldPos.x + right.x + up.x, worldPos.y + right.y + up.y, Vertex.nearZ);
                Vector3 v3 = new(worldPos.x - right.x + up.x, worldPos.y - right.y + up.y, Vertex.nearZ);

                Vector2 uv0, uv1, uv2, uv3;
                if (_cfg.TextureSheetEnabled && _cfg.UVMode == UVSource.TextureSheet && _cfg.SheetTilesX > 0 && _cfg.SheetTilesY > 0)
                    GetSheetUV(p, baseUV, out uv0, out uv1, out uv2, out uv3);
                else
                    GetBaseUV(baseUV, out uv0, out uv1, out uv2, out uv3);

                Color32 tint = p.tint;
                if (elementOpacity < 1f)
                {
                    byte a = (byte)Mathf.RoundToInt(tint.a * elementOpacity);
                    tint.a = a;
                }

                mesh.SetNextVertex(new Vertex { position = v0, tint = tint, uv = uv0 });
                mesh.SetNextVertex(new Vertex { position = v1, tint = tint, uv = uv1 });
                mesh.SetNextVertex(new Vertex { position = v2, tint = tint, uv = uv2 });
                mesh.SetNextVertex(new Vertex { position = v3, tint = tint, uv = uv3 });

                mesh.SetNextIndex((ushort)(vtxBase + 0));
                mesh.SetNextIndex((ushort)(vtxBase + 1));
                mesh.SetNextIndex((ushort)(vtxBase + 2));
                mesh.SetNextIndex((ushort)(vtxBase + 0));
                mesh.SetNextIndex((ushort)(vtxBase + 2));
                mesh.SetNextIndex((ushort)(vtxBase + 3));

                vtxBase += 4;
            }
        }

        // ---------- particle creation / prewarm ----------
        Vector3 RandomOnUnitSphere()
        {
            double u = _rng.NextDouble() * 2 - 1;
            double v = _rng.NextDouble() * 2 - 1;
            double s = u * u + v * v;
            while (s >= 1 || s == 0)
            {
                u = _rng.NextDouble() * 2 - 1;
                v = _rng.NextDouble() * 2 - 1;
                s = u * u + v * v;
            }
            double mul = 2.0 * Math.Sqrt(1 - s);
            return new Vector3((float)(u * mul), (float)(v * mul), (float)((1 - 2 * s)));
        }

        Particle CreateParticle()
        {
            float lifetime = Mathf.Max(0.01f, _cfg.StartLifetime + ((float)_rng.NextDouble() * 2 - 1) * _cfg.StartLifetimeRandom);
            float speed    = Mathf.Max(0f, _cfg.StartSpeed + ((float)_rng.NextDouble() * 2 - 1) * _cfg.StartSpeedRandom);

            float ss  = Mathf.Max(0.01f, _cfg.StartSize  + ((float)_rng.NextDouble() * 2 - 1) * _cfg.StartSizeRandom);
            float ssx = Mathf.Max(0.01f, _cfg.StartSizeX + ((float)_rng.NextDouble() * 2 - 1) * _cfg.StartSizeRandomX);
            float ssy = Mathf.Max(0.01f, _cfg.StartSizeY + ((float)_rng.NextDouble() * 2 - 1) * _cfg.StartSizeRandomY);

            float rotation        = _cfg.StartRotation + ((float)_rng.NextDouble() * 2 - 1) * _cfg.StartRotationRandom;
            float angularVelocity = _cfg.RotationOverLifetime + ((float)_rng.NextDouble() * 2 - 1) * _cfg.RotationRandom;

            Vector3 position = Vector3.zero;
            Vector3 velocity = Vector3.zero;

            switch (_cfg.ShapeType)
            {
                case EmitterShapeType.Sphere:
                {
                    Vector3 n = RandomOnUnitSphere();
                    position = n * _cfg.SphereRadius;
                    velocity = n * speed;
                    break;
                }
                case EmitterShapeType.Hemisphere:
                {
                    Vector3 n = RandomOnUnitSphere();
                    if (n.y < 0) n.y = -n.y;
                    position = n * _cfg.SphereRadius;
                    velocity = n * speed;
                    break;
                }
                case EmitterShapeType.Cone:
                {
                    float angle = Mathf.Deg2Rad * Mathf.Clamp(_cfg.ConeAngle, 0f, 89.9f);
                    float az = (float)(_rng.NextDouble() * 2.0 * Math.PI);
                    float r = (float)_rng.NextDouble() * _cfg.ConeRadius;
                    position = new Vector3(Mathf.Cos(az) * r, 0, Mathf.Sin(az) * r);
                    Vector3 dir = new(
                        Mathf.Cos(az) * Mathf.Sin(angle),
                        Mathf.Cos(angle),
                        Mathf.Sin(az) * Mathf.Sin(angle)
                    );
                    velocity = dir.normalized * speed;
                    break;
                }
                case EmitterShapeType.Box:
                {
                    position = new Vector3(
                        ((float)_rng.NextDouble() - 0.5f) * _cfg.BoxSize.x,
                        ((float)_rng.NextDouble() - 0.5f) * _cfg.BoxSize.y,
                        ((float)_rng.NextDouble() - 0.5f) * _cfg.BoxSize.z
                    );
                    Vector3 dir = position.sqrMagnitude > 1e-6f ? position.normalized : Vector3.up;
                    velocity = dir * speed;
                    break;
                }
                case EmitterShapeType.Circle:
                {
                    float arc = Mathf.Clamp(_cfg.CircleArcDegrees, 0.01f, 360f) * Mathf.Deg2Rad;
                    float a = (float)_rng.NextDouble() * arc - arc * 0.5f;
                    float r = _cfg.CircleRadius;
                    position = new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, 0);
                    velocity = position.sqrMagnitude > 1e-6f ? position.normalized * speed : Vector3.right * speed;
                    break;
                }
                case EmitterShapeType.Edge:
                {
                    float t = (float)_rng.NextDouble() - 0.5f;
                    position = new Vector3(t * _cfg.EdgeLength, 0, 0);
                    velocity = Vector3.up * speed;
                    break;
                }
            }

            if (_cfg.InheritVelocityEnabled && _cfg.InheritMode == InheritVelocityMode.Initial && _hasLastEmitterOrigin)
            {
                Vector2 vUI = _emitterVelocityUI;
                velocity += new Vector3(vUI.x, UnmapYFromUI(vUI.y), 0) * _cfg.InheritVelocityMultiplier;
            }

            int row = 0;
            if (_cfg.TextureSheetEnabled && _cfg.SheetMode == TextureSheetMode.SingleRow && _cfg.SheetRandomRow && _cfg.SheetTilesY > 0)
                row = _rng.Next(0, Mathf.Max(1, _cfg.SheetTilesY));

            float startOffset = 0f;
            if (_cfg.TextureSheetEnabled)
            {
                if (_cfg.SheetRandomStartFrame)
                {
                    float min = Mathf.Min(_cfg.SheetStartFrameMin, _cfg.SheetStartFrameMax);
                    float max = Mathf.Max(_cfg.SheetStartFrameMin, _cfg.SheetStartFrameMax);
                    startOffset = Mathf.Lerp(min, max, (float)_rng.NextDouble());
                }
                else
                {
                    startOffset = Mathf.Clamp01(_cfg.SheetStartFrameConst);
                }
            }

            float seedX = (float)_rng.NextDouble() * 1000f + _spawnCounter * 0.1234f;
            float seedY = (float)_rng.NextDouble() * 1000f + _spawnCounter * 0.5678f;

            var baseCol = (Color32)_cfg.StartColor;

            var p = new Particle
            {
                position = position,
                velocity = velocity,
                lifetime = lifetime,
                startLifetime = lifetime,
                startSize = ss,
                size = ss,
                startSizeX = ssx,
                startSizeY = ssy,
                sizeX = _cfg.UseStartSize2D ? ssx : ss,
                sizeY = _cfg.UseStartSize2D ? ssy : ss,
                rotation = rotation,
                angularVelocity = angularVelocity,
                baseColor = baseCol,
                tint = baseCol,
                sheetRow = row,
                sheetStart = startOffset,
                spawnIndex = _spawnCounter++,
                noiseSeedX = seedX,
                noiseSeedY = seedY
            };
            return p;
        }

        void PrewarmApproximate()
        {
            int expected = Mathf.Clamp(Mathf.FloorToInt(_cfg.RateOverTime * _cfg.Duration) + _cfg.BurstCount * Mathf.Max(0, _cfg.BurstCycles), 0, _cfg.MaxParticles);
            float g = -9.81f * _cfg.GravityModifier;

            for (int i = 0; i < expected; i++)
            {
                var p = CreateParticle();
                float age = UnityEngine.Random.Range(0f, p.startLifetime);

                Vector3 a = new Vector3(_cfg.ForceOverLifetime.x, _cfg.ForceOverLifetime.y + g, 0f);
                Vector3 v0 = p.velocity + _cfg.VelocityOverLifetime * age;
                p.position += v0 * age + 0.5f * a * age * age;
                p.rotation += p.angularVelocity * age;
                p.lifetime = Mathf.Max(0.02f, p.startLifetime - age);
                _particles.Add(p);
                if (_particles.Count >= _cfg.MaxParticles) break;
            }
        }

        void ReinitRng()
        {
            _rng = (!_cfg.AutoRandomSeed && _cfg.RandomSeed != 0)
                ? new System.Random(_cfg.RandomSeed)
                : new System.Random(Guid.NewGuid().GetHashCode());
        }

        void HandlePrewarmOrInitialBurst()
        {
            _particles.Clear();
            _emitAccumulator = 0f;
            _emitDistanceAccumulator = 0f;
            _playbackTime = 0f;
            _burstInit = false;
            _spawnCounter = 0;

            if (_cfg.Prewarm)
                PrewarmApproximate();
            else if (_cfg.BurstCount > 0 && _cfg.BurstCycles > 0 && Mathf.Approximately(_cfg.BurstTime, 0f))
                Burst(_cfg.BurstCount);
        }

        // ----------------- UTILS -----------------
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float MapYForUI(float y) => (_cfg.YAxis == YAxisMode.UI_DownPositive) ? -y : y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float UnmapYFromUI(float uiY) => (_cfg.YAxis == YAxisMode.UI_DownPositive) ? -uiY : uiY;

        Texture2D ResolveTexture()
        {
            if (_cfg.UVMode == UVSource.Sprite && _cfg.Sprite != null && _cfg.Sprite.texture != null) return _cfg.Sprite.texture;
            if (_cfg.Texture != null) return _cfg.Texture;
            if (_cfg.Sprite != null && _cfg.Sprite.texture != null) return _cfg.Sprite.texture;
            return null;
        }

        Rect GetBaseUVRect(Texture2D tex)
        {
            if (_cfg.UVMode == UVSource.Sprite && _cfg.Sprite != null && tex != null && _cfg.Sprite.texture == tex)
            {
                Rect tr = _cfg.Sprite.textureRect;
                return new Rect(tr.x / tex.width, tr.y / tex.height, tr.width / tex.width, tr.height / tex.height);
            }
            return new Rect(0, 0, 1, 1);
        }

        void GetBaseUV(in Rect baseUV, out Vector2 uv0, out Vector2 uv1, out Vector2 uv2, out Vector2 uv3)
        {
            float u0 = baseUV.xMin, v0 = baseUV.yMin, u1 = baseUV.xMax, v1 = baseUV.yMax;

            float fu0 = _cfg.FlipU ? u1 : u0;
            float fu1 = _cfg.FlipU ? u0 : u1;
            float fv0 = _cfg.FlipV ? v1 : v0;
            float fv1 = _cfg.FlipV ? v0 : v1;

            uv0 = new Vector2(fu0, fv0);
            uv1 = new Vector2(fu1, fv0);
            uv2 = new Vector2(fu1, fv1);
            uv3 = new Vector2(fu0, fv1);
        }

        void GetSheetUV(in Particle p, in Rect baseUV, out Vector2 uv0, out Vector2 uv1, out Vector2 uv2, out Vector2 uv3)
        {
            int tilesX = Mathf.Max(1, _cfg.SheetTilesX);
            int tilesY = Mathf.Max(1, _cfg.SheetTilesY);
            int totalFrames = tilesX * tilesY;

            float age01 = 1f - (p.lifetime / p.startLifetime);
            float f = (age01 + p.sheetStart) * Mathf.Max(0f, _cfg.SheetCycles) * totalFrames;

            int frame = Mathf.FloorToInt(f) % totalFrames;
            int col, row;

            if (_cfg.SheetMode == TextureSheetMode.SingleRow)
            {
                row = Mathf.Clamp(p.sheetRow, 0, tilesY - 1);
                col = frame % tilesX;
            }
            else
            {
                row = frame / tilesX;
                col = frame % tilesX;
            }

            float du = baseUV.width / tilesX;
            float dv = baseUV.height / tilesY;

            float u0 = baseUV.x + col * du;
            float v0 = baseUV.y + row * dv;
            float u1 = u0 + du;
            float v1 = v0 + dv;

            float fu0 = _cfg.FlipU ? u1 : u0;
            float fu1 = _cfg.FlipU ? u0 : u1;
            float fv0 = _cfg.FlipV ? v1 : v0;
            float fv1 = _cfg.FlipV ? v0 : v1;

            uv0 = new Vector2(fu0, fv0);
            uv1 = new Vector2(fu1, fv0);
            uv2 = new Vector2(fu1, fv1);
            uv3 = new Vector2(fu0, fv1);
        }

        Vector2 GetEmitterOrigin(Rect rect)
        {
            Vector2 origin = rect.center;
            switch (_cfg.Anchor)
            {
                // UIElements: yMin = top, yMax = bottom
                case ParticleEmitterAnchor.TopLeft:     origin = new Vector2(rect.xMin, rect.yMin); break;
                case ParticleEmitterAnchor.TopRight:    origin = new Vector2(rect.xMax, rect.yMin); break;
                case ParticleEmitterAnchor.BottomLeft:  origin = new Vector2(rect.xMin, rect.yMax); break;
                case ParticleEmitterAnchor.BottomRight: origin = new Vector2(rect.xMax, rect.yMax); break;
                case ParticleEmitterAnchor.Center:      origin = rect.center; break;
            }
            return origin + _cfg.EmitterOffset;
        }

        static AnimationCurve Constant(float v) => new AnimationCurve(new Keyframe(0, v), new Keyframe(1, v));
    }
}
