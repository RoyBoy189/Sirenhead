
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Sirenhead
{
	// Token: 0x0200000B RID: 11
	public class HediffCompProperties_Boom : HediffCompProperties
	{
		// Token: 0x0600000A RID: 10 RVA: 0x000026C0 File Offset: 0x000016C0
		public HediffCompProperties_Boom()
		{
			this.compClass = typeof(HediffCompProperties_Boom);
		}

		// Token: 0x0400002C RID: 44
		public ThingDef thingToSpawn;

		// Token: 0x0400002D RID: 45
		public int spawnCount = 1;

		// Token: 0x0400002E RID: 46
		public bool animalThing = false;
		
		// Token: 0x0400002F RID: 47
		public PawnKindDef animalToSpawn;

		// Token: 0x04000030 RID: 48
		public bool factionOfPlayerAnimal = false;

		// Token: 0x04000031 RID: 49
		public float minDaysB4Next = 1f;

		// Token: 0x04000032 RID: 50
		public float maxDaysB4Next = 2f;

		// Token: 0x04000033 RID: 51
		public float randomGrace = 0f;

		// Token: 0x04000034 RID: 52
		public float graceDays = 0.5f;

		// Token: 0x04000035 RID: 53
		public int spawnMaxAdjacent = -1;

		// Token: 0x04000036 RID: 54
		public bool spawnForbidden = false;

		// Token: 0x04000037 RID: 55
		public bool hungerRelative = false;

		// Token: 0x04000038 RID: 56
		public bool healthRelative = false;

		// Token: 0x04000039 RID: 57
		public bool ageWeightedQuantity = false;

		// Token: 0x0400003A RID: 58
		public bool ageWeightedPeriod = false;

		// Token: 0x0400003B RID: 59
		public bool olderSmallerPeriod = false;

		// Token: 0x0400003C RID: 60
		public bool olderBiggerQuantity = false;

		// Token: 0x0400003D RID: 61
		public bool exponentialQuantity = false;

		// Token: 0x0400003E RID: 62
		public int exponentialRatioLimit = 15;

		// Token: 0x0400003F RID: 63
		public string spawnVerb = "delivery";

		// Token: 0x04000040 RID: 64
		public bool debug = false;
	}
}





