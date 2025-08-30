#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Extract parameters from a UnityEngine.ParticleSystem (in a prefab/GO)
/// into a UIParticleProfile ScriptableObject.
/// Menus:
/// - Tools ▸ UI Toolkit ▸ UIParticle ▸ Extract Profile From Selection
/// - Assets ▸ Create ▸ UI Toolkit ▸ UIParticle ▸ Extract Profile From Selection
/// </summary>
namespace MainraGames.Editor
{
	public static class UIParticleProfileExtractor
	{
		// Project Window "Create" menu wrapper (reuses the same logic)
		[MenuItem("CONTEXT/ParticleSystem/UI Toolkit/Extract to UIParticle Profile", false, 2000)]
		[MenuItem("GameObject/UI Toolkit/Extract to UIParticle Profile", false, 2000)]
		private static void ExtractFromSelection()
		{
			var objs = Selection.objects;
			if (objs == null || objs.Length == 0)
			{
				EditorUtility.DisplayDialog(
					"UIParticle Extractor",
					"Select a prefab asset or GameObject (Hierarchy/Project) that contains a ParticleSystem.",
					"OK");
				return;
			}

			int created = 0;

			foreach (var obj in objs)
			{
				GameObject go = obj as GameObject;

				if (!go)
				{
					var path = AssetDatabase.GetAssetPath(obj);
					if (!string.IsNullOrEmpty(path))
						go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
				}

				if (!go) continue;

				var particleSystems = go.GetComponentsInChildren<ParticleSystem>(true);
				if (particleSystems == null || particleSystems.Length == 0) continue;

				var rootPath = AssetDatabase.GetAssetPath(go);
				var dir = string.IsNullOrEmpty(rootPath) ? "Assets" : Path.GetDirectoryName(rootPath);
				if (string.IsNullOrEmpty(dir)) dir = "Assets";

				foreach (var ps in particleSystems)
				{
					var profile = CreateProfileFromParticleSystem(ps);
					if (!profile) continue;

					var safeName = SanitizeFileName($"{go.name}_{ps.gameObject.name}_UIParticleProfile.asset");
					var assetPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(dir, safeName));
					AssetDatabase.CreateAsset(profile, assetPath);
					EditorUtility.SetDirty(profile);
					created++;
				}
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			EditorUtility.DisplayDialog(
				"UIParticle Extractor",
				created > 0 ? $"Done. Created {created} UIParticleProfile asset(s)."
							: "No ParticleSystem found in the selection.",
				"OK");
		}

		// Validator so the Create menu item is only enabled for valid selections
		[MenuItem("Assets/Create/UI Toolkit/UIParticle/Extract Profile from Prefab", true)]
		[MenuItem("GameObject/UI Toolkit/Extract to UIParticle Profile", true)]
		private static bool ExtractFromSelection_CreateMenu_Validate()
		{
			var objs = Selection.objects;
			if (objs == null || objs.Length == 0) return false;

			foreach (var obj in objs)
			{
				GameObject go = obj as GameObject;
				if (!go)
				{
					var path = AssetDatabase.GetAssetPath(obj);
					if (!string.IsNullOrEmpty(path))
						go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
				}

				if (!go) continue;
				if (go.GetComponentInChildren<ParticleSystem>(true) != null)
					return true;
			}
			return false;
		}

		private static string SanitizeFileName(string s)
		{
			foreach (var c in Path.GetInvalidFileNameChars()) s = s.Replace(c, '_');
			return s;
		}

		/// <summary>
		/// Main mapping from ParticleSystem -> UIParticleProfile
		/// </summary>
		private static UIParticleProfile CreateProfileFromParticleSystem(ParticleSystem ps)
		{
			if (!ps) return null;

			var p = ScriptableObject.CreateInstance<UIParticleProfile>();

			var main = ps.main;
			var emission = ps.emission;
			var shape = ps.shape;
			var vol = ps.velocityOverLifetime;
			var iv = ps.inheritVelocity;
			var fol = ps.forceOverLifetime;
			var lim = ps.limitVelocityOverLifetime;
			var sol = ps.sizeOverLifetime;
			var col = ps.colorOverLifetime;
			var rol = ps.rotationOverLifetime;
			var sbs = ps.sizeBySpeed;
			var cbs = ps.colorBySpeed;
			var rbs = ps.rotationBySpeed;
			var noise = ps.noise;
			var tsa = ps.textureSheetAnimation;
			var psr = ps.GetComponent<ParticleSystemRenderer>();

			// ─────────────────────────────────────────────────────────────────────
			// MAIN
			// ─────────────────────────────────────────────────────────────────────
			p.Duration = Mathf.Max(0.01f, main.duration);
			p.Looping = main.loop;
			p.Prewarm = main.prewarm;
			p.PlayOnAttach = main.playOnAwake;
			p.StartDelay = GetConstant(main.startDelay, 0f);

			GetAvgAndRandom(main.startLifetime, out var lifeAvg, out var lifeRand);
			p.StartLifetime = Mathf.Max(0.01f, lifeAvg);
			p.StartLifetimeRandom = Mathf.Max(0f, lifeRand);

			GetAvgAndRandom(main.startSpeed, out var spdAvg, out var spdRand);
			p.StartSpeed = spdAvg;
			p.StartSpeedRandom = Mathf.Max(0f, spdRand);

			if (main.startSize3D)
			{
				p.UseStartSize2D = true;
				GetAvgAndRandom(main.startSizeX, out var sx, out var srx);
				GetAvgAndRandom(main.startSizeY, out var sy, out var sry);
				p.StartSizeX = Mathf.Max(0.01f, sx);
				p.StartSizeY = Mathf.Max(0.01f, sy);
				p.StartSizeRandomX = Mathf.Max(0f, srx);
				p.StartSizeRandomY = Mathf.Max(0f, sry);
			}
			else
			{
				p.UseStartSize2D = false;
				GetAvgAndRandom(main.startSize, out var ss, out var ssr);
				p.StartSize = Mathf.Max(0.01f, ss);
				p.StartSizeRandom = Mathf.Max(0f, ssr);
			}

			// Start Rotation (radians -> degrees)
			float rotRad = GetConstant(main.startRotation, 0f);
			p.StartRotation = rotRad * Mathf.Rad2Deg;
			p.StartRotationRandom = 0f; // no direct PS counterpart for ±random start rotation

			// Start Color (used only when ColorOverLifetime is disabled)
			p.StartColor = GetColor(main.startColor, Color.white);

			p.GravityModifier = GetConstant(main.gravityModifier, 0f);
			p.MaxParticles = Mathf.Max(1, main.maxParticles);
			p.SimulateInEditor = true;
			p.PlaybackSpeed = Mathf.Max(0f, main.simulationSpeed);

			// [FIX] auto/random seed is on ParticleSystem (not MainModule)
			p.AutoRandomSeed = ps.useAutoRandomSeed;
			if (!p.AutoRandomSeed)
				p.RandomSeed = unchecked((int)ps.randomSeed);

			// ─────────────────────────────────────────────────────────────────────
			// EMISSION
			// ─────────────────────────────────────────────────────────────────────
			p.RateOverTime = emission.enabled ? GetConstant(emission.rateOverTime, 0f) : 0f;
			p.RateOverDistance = emission.enabled ? GetConstant(emission.rateOverDistance, 0f) : 0f;

			if (emission.enabled)
			{
				var bursts = new ParticleSystem.Burst[emission.burstCount];
				emission.GetBursts(bursts);
				if (bursts != null && bursts.Length > 0)
				{
					var b = bursts[0];
					p.BurstCount = Mathf.RoundToInt(b.count.constant);
					p.BurstTime = b.time;
					p.BurstCycles = Mathf.Max(1, b.cycleCount);
					p.BurstInterval = Mathf.Max(0f, b.repeatInterval);
				}
				else
				{
					p.BurstCount = 0;
				}
			}
			else
			{
				p.BurstCount = 0;
			}

			// ─────────────────────────────────────────────────────────────────────
			// SHAPE
			// ─────────────────────────────────────────────────────────────────────
			p.ShapeType = ConvertShape(shape.shapeType);

			switch (p.ShapeType)
			{
				case UIParticle.EmitterShapeType.Sphere:
				case UIParticle.EmitterShapeType.Hemisphere:
					p.SphereRadius = Mathf.Max(0f, shape.radius);
					break;

				case UIParticle.EmitterShapeType.Cone:
					p.ConeAngle = Mathf.Clamp(shape.angle, 0f, 89.9f);
					p.ConeRadius = Mathf.Max(0f, shape.radius);
					break;

				case UIParticle.EmitterShapeType.Box:
					// use scale as the box size (stable across versions)
					p.BoxSize = shape.scale;
					break;

				case UIParticle.EmitterShapeType.Circle:
					p.CircleRadius = Mathf.Max(0f, shape.radius);
					p.CircleArcDegrees = Mathf.Clamp(shape.arc, 0.01f, 360f);
					break;

				case UIParticle.EmitterShapeType.Edge:
					// use radius as a proxy for edge length
					p.EdgeLength = Mathf.Max(0f, shape.radius);
					break;
			}

			// ─────────────────────────────────────────────────────────────────────
			// VELOCITY & FORCES
			// ─────────────────────────────────────────────────────────────────────
			if (vol.enabled)
			{
				bool xCurve = IsCurve(vol.x);
				bool yCurve = IsCurve(vol.y);
				bool zCurve = IsCurve(vol.z);

				if (xCurve || yCurve || zCurve)
				{
					p.UseVelocityOverLifetimeCurves = true;
					p.velocityX = ToCurve(vol.x);
					p.velocityY = ToCurve(vol.y);
					p.velocityZ = ToCurve(vol.z);
				}
				else
				{
					p.UseVelocityOverLifetimeCurves = false;
					p.VelocityOverLifetime = new Vector3(
						GetConstant(vol.x, 0f),
						GetConstant(vol.y, 0f),
						GetConstant(vol.z, 0f)
					);
				}
			}
			else
			{
				p.UseVelocityOverLifetimeCurves = false;
				p.VelocityOverLifetime = Vector3.zero;
			}

			// Inherit Velocity
			p.InheritVelocityEnabled = iv.enabled;
			if (p.InheritVelocityEnabled)
			{
				p.InheritMode = iv.mode == ParticleSystemInheritVelocityMode.Current
					? UIParticle.InheritVelocityMode.Current
					: UIParticle.InheritVelocityMode.Initial;

				p.InheritVelocityMultiplier = GetConstant(iv.curve, 0f);
			}

			// Force Over Lifetime
			if (fol.enabled)
			{
				bool xCurve = IsCurve(fol.x);
				bool yCurve = IsCurve(fol.y);

				if (xCurve || yCurve)
				{
					p.UseForceOverLifetimeCurves = true;
					p.forceX = ToCurve(fol.x);
					p.forceY = ToCurve(fol.y);
				}
				else
				{
					p.UseForceOverLifetimeCurves = false;
					p.ForceOverLifetime = new Vector2(
						GetConstant(fol.x, 0f),
						GetConstant(fol.y, 0f));
				}
			}
			else
			{
				p.UseForceOverLifetimeCurves = false;
				p.ForceOverLifetime = Vector2.zero;
			}

			// ─────────────────────────────────────────────────────────────────────
			// LIMITS
			// ─────────────────────────────────────────────────────────────────────
			if (lim.enabled)
			{
				if (IsCurve(lim.limit))
				{
					p.UseLimitVelocityCurve = true;
					p.limitVelocityOverLifetime = ToCurve(lim.limit);
				}
				else
				{
					p.UseLimitVelocityCurve = false;
					p.LimitVelocity = Mathf.Max(0f, GetConstant(lim.limit, 0f));
				}
				p.LimitVelocityDampen = lim.dampen > 0f;
			}
			else
			{
				p.UseLimitVelocityCurve = false;
				p.LimitVelocity = 0f;
				p.LimitVelocityDampen = false;
			}

			// ─────────────────────────────────────────────────────────────────────
			// OVER LIFETIME
			// ─────────────────────────────────────────────────────────────────────
			if (sol.enabled)
			{
				p.UseSizeOverLifetimeCurve = true;
				p.sizeOverLifetime = ToCurve(sol.size);
			}
			else
			{
				p.UseSizeOverLifetimeCurve = false;
				p.SizeOverLifetimeStart = 1f;
				p.SizeOverLifetimeEnd = 0f;
			}

			if (col.enabled)
			{
				p.UseColorOverLifetimeCurve = true;
				p.colorOverLifetime = ToGradient(col.color);
			}
			else
			{
				p.UseColorOverLifetimeCurve = false;
				p.ColorOverLifetimeStart = Color.white;
				p.ColorOverLifetimeEnd = new Color(1, 1, 1, 0);
			}

			if (rol.enabled)
			{
				if (IsCurve(rol.z)) // 2D typically uses Z
				{
					p.UseAngularVelocityCurve = true;
					p.angularVelocity = ToCurve(rol.z);
				}
				else
				{
					p.UseAngularVelocityCurve = false;
					p.RotationOverLifetime = rol.z.constant * Mathf.Rad2Deg; // rad/s -> deg/s
					p.RotationRandom = 0f;
				}
			}
			else
			{
				p.UseAngularVelocityCurve = false;
				p.RotationOverLifetime = 0f;
				p.RotationRandom = 0f;
			}

			// ─────────────────────────────────────────────────────────────────────
			// BY SPEED
			// ─────────────────────────────────────────────────────────────────────
			p.SizeBySpeedEnabled = sbs.enabled;
			if (p.SizeBySpeedEnabled)
			{
				p.SizeBySpeedRangeMin = sbs.range.x;
				p.SizeBySpeedRangeMax = sbs.range.y;

				var c = ToCurve(sbs.size);
				p.SizeBySpeedMultiplierMin = c.Evaluate(0f);
				p.SizeBySpeedMultiplierMax = c.Evaluate(1f);
			}

			p.ColorBySpeedEnabled = cbs.enabled;
			if (p.ColorBySpeedEnabled)
			{
				p.ColorBySpeedRangeMin = cbs.range.x;
				p.ColorBySpeedRangeMax = cbs.range.y;

				var g = ToGradient(cbs.color);
				p.ColorBySpeedStart = g.Evaluate(0f);
				p.ColorBySpeedEnd = g.Evaluate(1f);
			}

			p.RotationBySpeedEnabled = rbs.enabled;
			if (p.RotationBySpeedEnabled)
			{
				p.RotationBySpeedRangeMin = rbs.range.x;
				p.RotationBySpeedRangeMax = rbs.range.y;

				var c = ToCurve(rbs.z);
				p.RotationBySpeedFactor = c.Evaluate(1f) * Mathf.Rad2Deg;
			}

			// ─────────────────────────────────────────────────────────────────────
			// NOISE
			// ─────────────────────────────────────────────────────────────────────
			p.NoiseEnabled = noise.enabled;
			if (p.NoiseEnabled)
			{
				p.NoiseStrength = GetConstant(noise.strength, 0f);
				p.NoiseFrequency = Mathf.Max(0f, noise.frequency);
				p.NoiseOctaves = Mathf.Clamp(noise.octaveCount, 1, 4);
				p.NoiseScrollSpeed = GetConstant(noise.scrollSpeed, 0f);
				p.NoiseDamping = noise.damping ? 1f : 0f;
			}

			// ─────────────────────────────────────────────────────────────────────
			// RENDERER & UV/TEXTURE
			// ─────────────────────────────────────────────────────────────────────
			p.RenderMode = ConvertRenderMode(psr);
			p.Sorting = UIParticle.RenderSort.None; // no direct counterpart
			p.YAxis = UIParticle.YAxisMode.UI_DownPositive; // UI default
			p.AlignToVelocity = (p.RenderMode == UIParticle.ParticleRenderMode.StretchedBillboard);

#if UNITY_2019_4_OR_NEWER
			if (p.RenderMode == UIParticle.ParticleRenderMode.StretchedBillboard && psr != null)
			{
				p.StretchLength = psr.lengthScale;
				p.StretchSpeedScale = psr.velocityScale;
			}
#endif
			p.MultiplyByElementOpacity = true;

			// Texture Sheet Animation
			if (tsa.enabled)
			{
				p.UVMode = UIParticle.UVSource.TextureSheet;
				p.TextureSheetEnabled = true;

				// Grid/Sprites mode + animation type WholeSheet/SingleRow
				bool isGrid = (tsa.mode == ParticleSystemAnimationMode.Grid);
				bool isWholeSheet = isGrid && (tsa.animation == ParticleSystemAnimationType.WholeSheet);

				p.SheetMode = isWholeSheet
					? UIParticle.TextureSheetMode.WholeSheet
					: UIParticle.TextureSheetMode.SingleRow;

				p.SheetTilesX = Mathf.Max(1, tsa.numTilesX);
				p.SheetTilesY = Mathf.Max(1, tsa.numTilesY);

#if UNITY_2019_4_OR_NEWER
				p.SheetCycles = Mathf.Max(0f, tsa.cycleCount);
#else
			p.SheetCycles = 1f;
#endif

#if UNITY_2018_3_OR_NEWER
				p.SheetRandomRow = (isGrid && tsa.rowMode == ParticleSystemAnimationRowMode.Random);
#else
			p.SheetRandomRow = tsa.useRandomRow;
#endif
				p.SheetRandomStartFrame = !isGrid || tsa.startFrame.mode != ParticleSystemCurveMode.Constant;
			}
			else
			{
				// Fallback: grab Texture from renderer material if available
				var tex = psr != null ? psr.sharedMaterial?.mainTexture as Texture2D : null;
				if (tex != null)
				{
					p.UVMode = UIParticle.UVSource.Texture;
					p.Texture = tex;
				}
				else
				{
					p.UVMode = UIParticle.UVSource.Texture; // safe default
				}
			}

			p.FlipU = false;
			p.FlipV = false;

			// ─────────────────────────────────────────────────────────────────────
			// POSITION & CULLING (no direct counterpart in PS)
			// ─────────────────────────────────────────────────────────────────────
			p.Anchor = UIParticle.ParticleEmitterAnchor.Center;
			p.EmitterOffset = Vector2.zero;
			p.PauseWhenInvisible = true;
			p.KillWhenOutside = false;
			p.CullPadding = 32f;

			// ─────────────────────────────────────────────────────────────────────
			// SYSTEM CURVES (GLOBAL)
			// ─────────────────────────────────────────────────────────────────────
			p.emissionRateMultiplier = Constant(1f);

			EditorUtility.SetDirty(p);
			return p;
		}

		// ─────────────────────────────────────────────────────────────────────────
		// Conversion helpers
		// ─────────────────────────────────────────────────────────────────────────

		private static float GetConstant(ParticleSystem.MinMaxCurve mmc, float fallback)
		{
			switch (mmc.mode)
			{
				case ParticleSystemCurveMode.Constant: return mmc.constant;
				case ParticleSystemCurveMode.TwoConstants: return (mmc.constantMin + mmc.constantMax) * 0.5f;
				case ParticleSystemCurveMode.Curve: return mmc.curve.Evaluate(0f);
				case ParticleSystemCurveMode.TwoCurves: return mmc.curveMax.Evaluate(0f);
				default: return fallback;
			}
		}

		private static void GetAvgAndRandom(ParticleSystem.MinMaxCurve mmc, out float avg, out float plusMinusRandom)
		{
			if (mmc.mode == ParticleSystemCurveMode.TwoConstants)
			{
				avg = (mmc.constantMin + mmc.constantMax) * 0.5f;
				plusMinusRandom = Mathf.Abs(mmc.constantMax - mmc.constantMin) * 0.5f;
			}
			else if (mmc.mode == ParticleSystemCurveMode.Constant)
			{
				avg = mmc.constant;
				plusMinusRandom = 0f;
			}
			else
			{
				avg = GetConstant(mmc, 0f); // t=0 is representative
				plusMinusRandom = 0f;
			}
		}

		private static bool IsCurve(ParticleSystem.MinMaxCurve mmc)
		{
			return mmc.mode == ParticleSystemCurveMode.Curve || mmc.mode == ParticleSystemCurveMode.TwoCurves;
		}

		private static AnimationCurve ToCurve(ParticleSystem.MinMaxCurve mmc)
		{
			switch (mmc.mode)
			{
				case ParticleSystemCurveMode.Curve: return new AnimationCurve(mmc.curve.keys);
				case ParticleSystemCurveMode.TwoCurves: return new AnimationCurve(mmc.curveMax.keys);
				case ParticleSystemCurveMode.Constant: return Constant(mmc.constant);
				case ParticleSystemCurveMode.TwoConstants: return Constant((mmc.constantMin + mmc.constantMax) * 0.5f);
				default: return Constant(0f);
			}
		}

		private static Gradient ToGradient(ParticleSystem.MinMaxGradient mmg)
		{
			var g = new Gradient();
			switch (mmg.mode)
			{
				case ParticleSystemGradientMode.Color:
					g.SetKeys(
						new[] { new GradientColorKey(mmg.color, 0f), new GradientColorKey(mmg.color, 1f) },
						new[] { new GradientAlphaKey(mmg.color.a, 0f), new GradientAlphaKey(mmg.color.a, 1f) }
					);
					break;

				case ParticleSystemGradientMode.Gradient:
					g.SetKeys(mmg.gradient.colorKeys, mmg.gradient.alphaKeys);
					break;

				case ParticleSystemGradientMode.TwoColors:
					g.SetKeys(
						new[] { new GradientColorKey(mmg.colorMin, 0f), new GradientColorKey(mmg.colorMax, 1f) },
						new[] { new GradientAlphaKey(mmg.colorMin.a, 0f), new GradientAlphaKey(mmg.colorMax.a, 1f) }
					);
					break;

				case ParticleSystemGradientMode.TwoGradients:
					g.SetKeys(mmg.gradientMax.colorKeys, mmg.gradientMax.alphaKeys);
					break;

				default:
					g.SetKeys(
						new[] { new GradientColorKey(Color.white, 0f), new GradientColorKey(Color.white, 1f) },
						new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
					);
					break;
			}
			return g;
		}

		private static Color GetColor(ParticleSystem.MinMaxGradient mmg, Color fallback)
		{
			switch (mmg.mode)
			{
				case ParticleSystemGradientMode.Color: return mmg.color;
				case ParticleSystemGradientMode.TwoColors: return (mmg.colorMin + mmg.colorMax) * 0.5f;
				case ParticleSystemGradientMode.Gradient: return mmg.gradient.Evaluate(0f);
				case ParticleSystemGradientMode.TwoGradients: return mmg.gradientMax.Evaluate(0f);
				default: return fallback;
			}
		}

		private static UIParticle.EmitterShapeType ConvertShape(ParticleSystemShapeType s)
		{
			switch (s)
			{
				case ParticleSystemShapeType.Sphere: return UIParticle.EmitterShapeType.Sphere;
				case ParticleSystemShapeType.Hemisphere: return UIParticle.EmitterShapeType.Hemisphere;
				case ParticleSystemShapeType.Cone: return UIParticle.EmitterShapeType.Cone;
				case ParticleSystemShapeType.Box: return UIParticle.EmitterShapeType.Box;
				case ParticleSystemShapeType.Circle: return UIParticle.EmitterShapeType.Circle;

#if UNITY_2018_1_OR_NEWER
				// some Unity versions don’t have Edge; use SingleSidedEdge
				case ParticleSystemShapeType.SingleSidedEdge: return UIParticle.EmitterShapeType.Edge;
#endif
				default: return UIParticle.EmitterShapeType.Sphere;
			}
		}

		private static UIParticle.ParticleRenderMode ConvertRenderMode(ParticleSystemRenderer psr)
		{
			if (!psr) return UIParticle.ParticleRenderMode.Billboard;

			// Unity enum is "Stretch", not "StretchedBillboard"
			switch (psr.renderMode)
			{
				case ParticleSystemRenderMode.Billboard:
					return UIParticle.ParticleRenderMode.Billboard;

				case ParticleSystemRenderMode.Stretch:
					return UIParticle.ParticleRenderMode.StretchedBillboard;

				case ParticleSystemRenderMode.HorizontalBillboard:
				case ParticleSystemRenderMode.VerticalBillboard:
				case ParticleSystemRenderMode.Mesh:
				default:
					return UIParticle.ParticleRenderMode.Billboard;
			}
		}

		private static AnimationCurve Constant(float v)
		{
			return new AnimationCurve(new Keyframe(0, v), new Keyframe(1, v));
		}
	}
#endif
}
