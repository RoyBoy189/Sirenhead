
using System;
using System.Collections.Generic;
using System.Linq;
using AlienRace;
using Verse;

namespace Sirenhead
{
	// Token: 0x0200001A RID: 26
	public static class Setup
	{
		// Token: 0x06000077 RID: 119 RVA: 0x00006A4C File Offset: 0x00005A4C
		public static void DestroyParentHediff(Hediff parentHediff, bool debug = false)
		{
			bool flag = parentHediff.pawn != null && parentHediff.def.defName != null;
			if (flag)
			{
				Setup.Warn(parentHediff.pawn.Label + "'s Hediff: " + parentHediff.def.defName + " says goodbye.", debug);
			}
			parentHediff.Severity = 0f;
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00006AB0 File Offset: 0x00005AB0
		public static float GetPawnAgeOverlifeExpectancyRatio(Pawn pawn, bool debug = false)
		{
			float num = 1f;
			bool flag = pawn == null;
			float result;
			if (flag)
			{
				Setup.Warn("GetPawnAgeOverlifeExpectancyRatio pawn NOT OK", debug);
				result = num;
			}
			else
			{
				num = pawn.ageTracker.AgeBiologicalYearsFloat / pawn.RaceProps.lifeExpectancy;
				Setup.Warn(string.Concat(new object[]
				{
					pawn.Label,
					" Age: ",
					pawn.ageTracker.AgeBiologicalYearsFloat,
					"; lifeExpectancy: ",
					pawn.RaceProps.lifeExpectancy,
					"; ratio:",
					num
				}), debug);
				result = num;
			}
			return result;
		}

		// Token: 0x06000079 RID: 121 RVA: 0x00006B5C File Offset: 0x00005B5C
		public static float GetPawnAdultRatio(Pawn pawn, bool debug = false)
		{
			float num = 1f;
			bool flag = !Setup.OkPawn(pawn);
			float result;
			if (flag)
			{
				Setup.Warn("GetPawnAgeOverlifeExpectancyRatio pawn NOT OK", debug);
				result = num;
			}
			else
			{
				num = (pawn.ageTracker.AgeBiologicalYearsFloat - pawn.RaceProps.lifeStageAges.Last<LifeStageAge>().minAge) / (pawn.RaceProps.lifeExpectancy - pawn.RaceProps.lifeStageAges.Last<LifeStageAge>().minAge);
				result = num;
			}
			return result;
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00006BD8 File Offset: 0x00005BD8
		public static bool IsInjured(Pawn pawn, bool debug = false)
		{
			bool flag = pawn == null;
			bool result;
			if (flag)
			{
				Setup.Warn("pawn is null - wounded ", debug);
				result = false;
			}
			else
			{
				float num = 0f;
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i = checked(i + 1))
				{
					bool flag2 = hediffs[i] is Hediff_Injury && !HediffUtility.IsPermanent(hediffs[i]);
					if (flag2)
					{
						num += hediffs[i].Severity;
					}
				}
				Setup.Warn(pawn.Label + " is wounded ", debug && num > 0f);
				result = (num > 0f);
			}
			return result;
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00006CA0 File Offset: 0x00005CA0
		public static bool IsHungry(Pawn pawn, bool debug = false)
		{
			bool flag = pawn == null;


			bool result;
			if (flag)
			{
				Setup.Warn("pawn is null - IsHungry ", debug);
				result = false;
			}
			else
			{
				bool flag2 = pawn.needs.food != null && pawn.needs.food.CurCategory == RimWorld.HungerCategory.Starving;
				Setup.Warn(pawn.Label + " is hungry ", debug && flag2);
				result = flag2;
			}
			return result;
		}

		// Token: 0x0600007C RID: 124 RVA: 0x00006D0C File Offset: 0x00005D0C
		public static BodyPartRecord GetBPRecord(this Pawn pawn, BodyPartDef bodyPartDef, bool myDebug = false)
		{
			IEnumerable<BodyPartDef> enumerable = from b in DefDatabase<BodyPartDef>.AllDefs
												  where b == bodyPartDef
												  select b;
			bool flag = GenCollection.EnumerableNullOrEmpty<BodyPartDef>(enumerable);
			BodyPartRecord result;
			if (flag)
			{
				Setup.Warn(pawn.Label + " - GetBPRecord - did not find any " + bodyPartDef.defName, myDebug);
				result = null;
			}
			else
			{
				BodyPartDef bodyPartDef2 = GenCollection.RandomElement<BodyPartDef>(enumerable);
				BodyPartRecord bodyPartRecord;
				GenCollection.TryRandomElement<BodyPartRecord>(pawn.RaceProps.body.GetPartsWithDef(bodyPartDef2), out bodyPartRecord);
				Setup.Warn(pawn.Label + "GetBPRecord - DID find " + bodyPartDef.defName, myDebug);
				result = bodyPartRecord;
			}
			return result;
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00006DBC File Offset: 0x00005DBC
		public static bool OkPawn(Pawn pawn)
		{
			return pawn != null && pawn.Map != null;
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00006DE0 File Offset: 0x00005DE0
		public static void Warn(string warning, bool debug = false)
		{
			if (debug)
			{
				Log.Warning(warning, false);
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00006DFC File Offset: 0x00005DFC
		public static AlienPartGenerator.AlienComp GetAlien(Pawn pawn = null)
		{
			return (pawn != null) ? ThingCompUtility.TryGetComp<AlienPartGenerator.AlienComp>(pawn) : null;
		}
	}
}

