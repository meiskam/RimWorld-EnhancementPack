﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using Verse;
using RimWorld;
using RimWorld.Planet;
using Harmony;

namespace TD_Enhancement_Pack
{
	[StaticConstructorOnStartup]
	class BeautyOverlay : BaseOverlay
	{
		public BeautyOverlay(Map m) : base(m) { }

		public override bool ShowCell(int index)
		{
			return BeautyAt(map, index) != 0;
		}

		public override Color GetCellExtraColor(int index)
		{
			float amount = BeautyAt(map, index);

			bool good = amount > 0;
			amount = amount > 0 ? amount/50 : -amount/10;

			Color baseColor = good ? Color.green : Color.red;
			baseColor.a = 0;

			return good && amount > 1 ? Color.Lerp(Color.green, Color.white, amount - 1)
				: Color.Lerp(baseColor, good ? Color.green : Color.red, amount);
		}

		public static float BeautyAt(Map map, int index)
		{
			return BeautyUtility.CellBeauty(map.cellIndices.IndexToCell(index), map);
		}

		private static Texture2D icon = ContentFinder<Texture2D>.Get("Heart", true);
		public override Texture2D Icon() => icon;
		public override bool IconEnabled() => Settings.Get().showOverlayBeauty;//from Settings
		public override string IconTip() => "TD.ToggleBeauty".Translate();

	}

	[HarmonyPatch(typeof(TerrainGrid), "DoTerrainChangedEffects")]
	static class TerrainChangedSetDirty
	{
		public static void Postfix(Map ___map)
		{
			BaseOverlay.SetDirty(typeof(BeautyOverlay), ___map);
		}
	}

	[HarmonyPatch(typeof(ThingGrid), "Register")]
	public static class ThingDirtierRegister
	{
		public static void Postfix(Map ___map)
		{
			BaseOverlay.SetDirty(typeof(BeautyOverlay), ___map);
		}
	}

	[HarmonyPatch(typeof(ThingGrid), "Deregister")]
	public static class ThingDirtierDeregister
	{
		public static void Postfix(Map ___map)
		{
			BaseOverlay.SetDirty(typeof(BeautyOverlay), ___map);
		}
	}
}