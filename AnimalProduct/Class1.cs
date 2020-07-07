using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace Sirenhead
{
	// Token: 0x02000016 RID: 22
	public class HediffComp_Poop : HediffComp
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00004D4C File Offset: 0x00003D4C
		public HediffCompProperties_Boom Props
		{
			get
			{
				return (HediffCompProperties_Boom)this.props;
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004D6C File Offset: 0x00003D6C
		public override void CompExposeData()
		{
			Scribe_Values.Look<int>(ref this.ticksUntilSpawn, "ticksUntilSpawn", 0, false);
			Scribe_Values.Look<int>(ref this.initialTicksUntilSpawn, "initialTicksUntilSpawn", 0, false);
			Scribe_Values.Look<float>(ref this.calculatedMinDaysB4Next, "calculatedMinDaysB4Next", 0f, false);
			Scribe_Values.Look<float>(ref this.calculatedMaxDaysB4Next, "calculatedMaxDaysB4Next", 0f, false);
			Scribe_Values.Look<int>(ref this.calculatedQuantity, "calculatedQuantity", 0, false);
			Scribe_Values.Look<int>(ref this.graceTicks, "graceTicks", 0, false);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004DF4 File Offset: 0x00003DF4
		public override void CompPostMake()
		{
			this.myDebug = this.Props.debug;
			Setup.Warn(string.Concat(new string[]
			{
				">>> ",
				this.parent.pawn.Label,
				" - ",
				this.parent.def.defName,
				" - CompPostMake start"
			}), this.myDebug);
			this.TraceProps();
			this.CheckProps();
			this.CalculateValues();
			this.CheckCalculatedValues();
			this.TraceCalculatedValues();
			bool flag = this.initialTicksUntilSpawn == 0;
			if (flag)
			{
				Setup.Warn("Reseting countdown bc initialTicksUntilSpawn == 0 (comppostmake)", this.myDebug);
				this.ResetCountdown();
			}
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00004EB4 File Offset: 0x00003EB4
		public override void CompPostTick(ref float severityAdjustment)
		{
			this.pawn = this.parent.pawn;
			bool flag = !Setup.OkPawn(this.pawn);
			checked
			{
				if (!flag)
				{
					bool flag2 = this.blockSpawn;
					if (!flag2)
					{
						bool flag3 = this.graceTicks > 0;
						if (flag3)
						{
							this.graceTicks--;
						}
						else
						{
							bool flag4 = this.Props.hungerRelative && Setup.IsHungry(this.pawn, this.myDebug);
							if (flag4)
							{
								int num = (int)(unchecked(this.RandomGraceDays() * 60000f));
								this.hungerReset++;
								this.graceTicks = num;
							}
							else
							{
								bool flag5 = this.Props.healthRelative && Setup.IsInjured(this.pawn, this.myDebug);
								if (flag5)
								{
									int num2 = (int)(unchecked(this.RandomGraceDays() * 60000f));
									this.healthReset++;
									this.graceTicks = num2;
								}
								else
								{
									this.hungerReset = (this.healthReset = 0);
									bool flag6 = this.CheckShouldSpawn();
									if (flag6)
									{
										Setup.Warn("Reseting countdown bc spawned thing", this.myDebug);
										this.CalculateValues();
										this.CheckCalculatedValues();
										this.ResetCountdown();
										bool flag7 = Rand.Chance(this.Props.randomGrace);
										if (flag7)
										{
											int num3 = (int)(unchecked(this.RandomGraceDays() * 60000f));
											this.graceTicks = num3;
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x0600005D RID: 93 RVA: 0x00005030 File Offset: 0x00004030
		private void TraceProps()
		{
			Setup.Warn(string.Concat(new object[]
			{
				"Props => minDaysB4Next: ",
				this.Props.minDaysB4Next,
				"; maxDaysB4Next: ",
				this.Props.maxDaysB4Next,
				"; randomGrace: ",
				this.Props.randomGrace,
				"; graceDays: ",
				this.Props.graceDays,
				"; hungerRelative: ",
				this.Props.hungerRelative.ToString(),
				"; healthRelative: ",
				this.Props.healthRelative.ToString(),
				"; "
			}), this.myDebug);
			bool animalThing = this.Props.animalThing;
			if (animalThing)
			{
				Setup.Warn(string.Concat(new string[]
				{
					"animalThing: ",
					this.Props.animalThing.ToString(),
					"; animalName: ",
					this.Props.animalToSpawn.defName,
					"; factionOfPlayerAnimal: ",
					this.Props.factionOfPlayerAnimal.ToString(),
					"; "
				}), this.myDebug);
			}
			bool ageWeightedQuantity = this.Props.ageWeightedQuantity;
			if (ageWeightedQuantity)
			{
				Setup.Warn(string.Concat(new string[]
				{
					"ageWeightedQuantity:",
					this.Props.ageWeightedQuantity.ToString(),
					"; olderBiggerQuantity:",
					this.Props.olderBiggerQuantity.ToString(),
					"; ",
					this.myDebug.ToString()
				}), false);
				bool exponentialQuantity = this.Props.exponentialQuantity;
				if (exponentialQuantity)
				{
					Setup.Warn(string.Concat(new object[]
					{
						"exponentialQuantity:",
						this.Props.exponentialQuantity.ToString(),
						"; exponentialRatioLimit:",
						this.Props.exponentialRatioLimit,
						"; "
					}), this.myDebug);
				}
			}
			bool ageWeightedPeriod = this.Props.ageWeightedPeriod;
			if (ageWeightedPeriod)
			{
			}
			Setup.Warn(string.Concat(new string[]
			{
				"ageWeightedPeriod:",
				this.Props.ageWeightedPeriod.ToString(),
				"; olderSmallerPeriod:",
				this.Props.olderSmallerPeriod.ToString(),
				"; ",
				this.myDebug.ToString()
			}), false);
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000052C8 File Offset: 0x000042C8
		private void CalculateValues()
		{
			float num = Setup.GetPawnAgeOverlifeExpectancyRatio(this.parent.pawn, this.myDebug);
			num = ((num > 1f) ? 1f : num);
			this.calculatedMinDaysB4Next = this.Props.minDaysB4Next;
			this.calculatedMaxDaysB4Next = this.Props.maxDaysB4Next;
			bool ageWeightedPeriod = this.Props.ageWeightedPeriod;
			if (ageWeightedPeriod)
			{
				float num2 = this.Props.olderSmallerPeriod ? (-num) : num;
				this.calculatedMinDaysB4Next = this.Props.minDaysB4Next * (1f + num2);
				this.calculatedMaxDaysB4Next = this.Props.maxDaysB4Next * (1f + num2);
				Setup.Warn(string.Concat(new object[]
				{
					" ageWeightedPeriod: ",
					this.Props.ageWeightedPeriod.ToString(),
					" ageRatio: ",
					num,
					" minDaysB4Next: ",
					this.Props.minDaysB4Next,
					" maxDaysB4Next: ",
					this.Props.maxDaysB4Next,
					" daysAgeRatio: ",
					num2,
					" calculatedMinDaysB4Next: ",
					this.calculatedMinDaysB4Next,
					";  calculatedMaxDaysB4Next: ",
					this.calculatedMaxDaysB4Next,
					"; "
				}), this.myDebug);
			}
			this.calculatedQuantity = this.Props.spawnCount;
			bool ageWeightedQuantity = this.Props.ageWeightedQuantity;
			if (ageWeightedQuantity)
			{
				float num3 = this.Props.olderBiggerQuantity ? num : (-num);
				Setup.Warn("quantityAgeRatio: " + num3, this.myDebug);
				this.calculatedQuantity = checked((int)Math.Round(unchecked((double)this.Props.spawnCount * (double)(1f + num3))));
				bool exponentialQuantity = this.Props.exponentialQuantity;
				if (exponentialQuantity)
				{
					num3 = 1f - num;
					bool flag = num3 == 0f;
					if (flag)
					{
						Setup.Warn(">ERROR< quantityAgeRatio is f* up : " + num3, this.myDebug);
						this.blockSpawn = true;
						Setup.DestroyParentHediff(this.parent, this.myDebug);
						return;
					}
					float num4 = this.Props.olderBiggerQuantity ? (1f / num3) : (num3 * num3);
					bool flag2 = false;
					bool flag3 = false;
					bool flag4 = num4 > (float)this.Props.exponentialRatioLimit;
					if (flag4)
					{
						num4 = (float)this.Props.exponentialRatioLimit;
						flag2 = true;
					}
					this.calculatedQuantity = checked((int)Math.Round(unchecked((double)this.Props.spawnCount * (double)num4)));
					bool flag5 = this.calculatedQuantity < 1;
					if (flag5)
					{
						this.calculatedQuantity = 1;
						flag3 = true;
					}
					Setup.Warn(string.Concat(new object[]
					{
						" exponentialQuantity: ",
						this.Props.exponentialQuantity.ToString(),
						"; expoFactor: ",
						num4,
						"; gotLimited: ",
						flag2.ToString(),
						"; gotAugmented: ",
						flag3.ToString()
					}), this.myDebug);
				}
				Setup.Warn(string.Concat(new object[]
				{
					"; Props.spawnCount: ",
					this.Props.spawnCount,
					"; calculatedQuantity: ",
					this.calculatedQuantity
				}), this.myDebug);
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00005654 File Offset: 0x00004654
		private void CheckCalculatedValues()
		{
			bool flag = this.calculatedQuantity > this.errorSpawnCount;
			if (flag)
			{
				Setup.Warn(string.Concat(new object[]
				{
					">ERROR< calculatedQuantity is too high: ",
					this.calculatedQuantity,
					"(>",
					this.errorSpawnCount,
					"), check and adjust your hediff props"
				}), this.myDebug);
				this.blockSpawn = true;
				Setup.DestroyParentHediff(this.parent, this.myDebug);
			}
			else
			{
				bool flag2 = this.calculatedMinDaysB4Next >= this.calculatedMaxDaysB4Next;
				if (flag2)
				{
					Setup.Warn(">ERROR< calculatedMinDaysB4Next should be lower than calculatedMaxDaysB4Next", this.myDebug);
					this.blockSpawn = true;
					Setup.DestroyParentHediff(this.parent, this.myDebug);
				}
				else
				{
					bool flag3 = this.calculatedMinDaysB4Next < this.errorMinDaysB4Next;
					if (flag3)
					{
						Setup.Warn(string.Concat(new object[]
						{
							">ERROR< calculatedMinDaysB4Next is too low: ",
							this.Props.minDaysB4Next,
							"(<",
							this.errorMinDaysB4Next,
							"), check and adjust your hediff props"
						}), this.myDebug);
						this.blockSpawn = true;
						Setup.DestroyParentHediff(this.parent, this.myDebug);
					}
				}
			}
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000057A0 File Offset: 0x000047A0
		private void TraceCalculatedValues()
		{
			Setup.Warn(string.Concat(new object[]
			{
				"<<< ",
				this.Props.ageWeightedPeriod ? ("Props.olderMoreOften: " + this.Props.olderSmallerPeriod.ToString() + "; ") : "",
				this.Props.ageWeightedQuantity ? ("Props.olderBiggerquantities: " + this.Props.olderBiggerQuantity.ToString() + "; ") : "",
				" Props.minDaysB4Next: ",
				this.Props.minDaysB4Next,
				"; Props.maxDaysB4Next: ",
				this.Props.maxDaysB4Next,
				";  calculatedMinDaysB4Next: ",
				this.calculatedMinDaysB4Next,
				"; calculatedMaxDaysB4Next: ",
				this.calculatedMaxDaysB4Next,
				";  Props.spawnCount: ",
				this.Props.spawnCount,
				"; CalculatedQuantity: ",
				this.calculatedQuantity,
				"; "
			}), this.myDebug);
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000058DC File Offset: 0x000048DC
		private void CheckProps()
		{
			bool flag = this.Props.spawnCount > this.errorSpawnCount;
			if (flag)
			{
				Setup.Warn(string.Concat(new object[]
				{
					"SpawnCount is too high: ",
					this.Props.spawnCount,
					"(>",
					this.errorSpawnCount,
					"),  some people just want to see the world burn"
				}), this.myDebug);
				this.blockSpawn = true;
				Setup.DestroyParentHediff(this.parent, this.myDebug);
			}
			else
			{
				bool flag2 = this.Props.minDaysB4Next >= this.Props.maxDaysB4Next;
				if (flag2)
				{
					Setup.Warn("minDaysB4Next should be lower than maxDaysB4Next", this.myDebug);
					this.blockSpawn = true;
					Setup.DestroyParentHediff(this.parent, this.myDebug);
				}
				else
				{
					bool flag3 = this.Props.minDaysB4Next < this.errorMinDaysB4Next;
					if (flag3)
					{
						Setup.Warn(string.Concat(new object[]
						{
							"minDaysB4Next is too low: ",
							this.Props.minDaysB4Next,
							"(<",
							this.errorMinDaysB4Next,
							"), some people just want to see the world burn"
						}), this.myDebug);
						this.blockSpawn = true;
						Setup.DestroyParentHediff(this.parent, this.myDebug);
					}
					else
					{
						bool animalThing = this.Props.animalThing;
						if (animalThing)
						{
							bool flag4 = this.Props.animalToSpawn == null || GenText.NullOrEmpty(this.Props.animalToSpawn.defName);
							if (flag4)
							{
								Setup.Warn("Props.animalThing=" + this.Props.animalThing.ToString() + "; but no Props.animalName", this.myDebug);
								this.blockSpawn = true;
								Setup.DestroyParentHediff(this.parent, this.myDebug);
								return;
							}
							Setup.Warn("Found animal PawnKindDef.defName=" + this.Props.animalToSpawn.defName, this.myDebug);
						}
						else
						{
							ThingDef thingDef = GenCollection.RandomElement<ThingDef>(from b in DefDatabase<ThingDef>.AllDefs
																					  where b == this.Props.thingToSpawn
																					  select b);
							bool flag5 = thingDef == null;
							if (flag5)
							{
								Setup.Warn("Could not find Props.thingToSpawn in DefDatabase", this.myDebug);
								this.blockSpawn = true;
								Setup.DestroyParentHediff(this.parent, this.myDebug);
								return;
							}
							Setup.Warn("Found ThingDef for " + this.Props.thingToSpawn.defName + "in DefDatabase", this.myDebug);
						}
						bool flag6 = !this.Props.ageWeightedPeriod;
						if (flag6)
						{
							bool olderSmallerPeriod = this.Props.olderSmallerPeriod;
							if (olderSmallerPeriod)
							{
								Setup.Warn("olderSmallerPeriod ignored since ageWeightedPeriod is false ", this.myDebug);
								this.blockSpawn = true;
								Setup.DestroyParentHediff(this.parent, this.myDebug);
								return;
							}
						}
						bool flag7 = !this.Props.ageWeightedQuantity;
						if (flag7)
						{
							bool olderBiggerQuantity = this.Props.olderBiggerQuantity;
							if (olderBiggerQuantity)
							{
								Setup.Warn("olderBiggerQuantity ignored since ageWeightedQuantity is false ", this.myDebug);
							}
							bool exponentialQuantity = this.Props.exponentialQuantity;
							if (exponentialQuantity)
							{
								Setup.Warn("exponentialQuantity ignored since ageWeightedQuantity is false ", this.myDebug);
							}
							bool flag8 = this.Props.olderBiggerQuantity || this.Props.exponentialQuantity;
							if (flag8)
							{
								this.blockSpawn = true;
								Setup.DestroyParentHediff(this.parent, this.myDebug);
								return;
							}
						}
						bool flag9 = this.Props.exponentialQuantity && this.Props.exponentialRatioLimit > this.errorExponentialLimit;
						if (flag9)
						{
							Setup.Warn(string.Concat(new object[]
							{
								"expoRatioLimit too low while expoQuantity is set: ",
								this.Props.exponentialRatioLimit,
								"(>",
								this.errorExponentialLimit,
								"), some people just want to see the world burn"
							}), this.myDebug);
							this.blockSpawn = true;
							Setup.DestroyParentHediff(this.parent, this.myDebug);
						}
					}
				}
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00005D0C File Offset: 0x00004D0C
		private bool CheckShouldSpawn()
		{
			this.pawn = this.parent.pawn;
			bool flag = !Setup.OkPawn(this.pawn);
			checked
			{
				bool result;
				if (flag)
				{
					Setup.Warn("CheckShouldSpawn pawn Null", this.myDebug);
					result = false;
				}
				else
				{
					this.ticksUntilSpawn--;
					bool flag2 = this.ticksUntilSpawn <= 0;
					if (flag2)
					{
						Setup.Warn("TryDoSpawn: " + this.TryDoSpawn().ToString(), this.myDebug);
						result = true;
					}
					else
					{
						result = false;
					}
				}
				return result;
			}
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00005DA0 File Offset: 0x00004DA0
		private PawnKindDef MyPawnKindDefNamed(string myDefName)
		{
			PawnKindDef result = null;
			foreach (PawnKindDef pawnKindDef in DefDatabase<PawnKindDef>.AllDefs)
			{
				bool flag = pawnKindDef.defName == myDefName;
				if (flag)
				{
					return pawnKindDef;
				}
			}
			return result;
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00005E08 File Offset: 0x00004E08
		public bool TryDoSpawn()
		{
			this.pawn = this.parent.pawn;
			bool flag = !Setup.OkPawn(this.pawn);
			checked
			{
				bool result;
				if (flag)
				{
					Setup.Warn("TryDoSpawn - pawn null", this.myDebug);
					result = false;
				}
				else
				{
					bool animalThing = this.Props.animalThing;
					if (animalThing)
					{
						Faction faction = this.Props.factionOfPlayerAnimal ? Faction.OfPlayer : null;
						PawnGenerationRequest pawnGenerationRequest;
						pawnGenerationRequest..ctor(this.Props.animalToSpawn, faction, 2, -1, false, true, false, false, true, false, 1f, false, true, true, true, false, false, false, false, 0f, null, 1f, null, null, null, null, null, null, null, null, null, null, null, null);
						for (int i = 0; i < this.calculatedQuantity; i++)
						{
							Pawn pawn = PawnGenerator.GeneratePawn(pawnGenerationRequest);
							GenSpawn.Spawn(pawn, this.parent.pawn.Position, this.parent.pawn.Map, 0);
							FilthMaker.TryMakeFilth(this.parent.pawn.Position, this.parent.pawn.Map, ThingDefOf.Filth_AmnioticFluid, 1, 0);
						}
						result = true;
					}
					else
					{
						bool flag2 = this.Props.spawnMaxAdjacent >= 0;
						if (flag2)
						{
							int num = 0;
							for (int j = 0; j < 9; j++)
							{
								IntVec3 intVec = this.pawn.Position + GenAdj.AdjacentCellsAndInside[j];
								bool flag3 = !GenGrid.InBounds(intVec, this.pawn.Map);
								if (!flag3)
								{
									List<Thing> thingList = GridsUtility.GetThingList(intVec, this.pawn.Map);
									for (int k = 0; k < thingList.Count; k++)
									{
										bool flag4 = thingList[k].def == this.Props.thingToSpawn;
										if (flag4)
										{
											num += thingList[k].stackCount;
											bool flag5 = num >= this.Props.spawnMaxAdjacent;
											if (flag5)
											{
												return false;
											}
										}
									}
								}
							}
						}
						int l = 0;
						int num2 = this.calculatedQuantity;
						int num3 = 0;
						while (l < this.calculatedQuantity)
						{
							IntVec3 intVec2;
							bool flag6 = this.TryFindSpawnCell(out intVec2);
							if (flag6)
							{
								Thing thing = ThingMaker.MakeThing(this.Props.thingToSpawn, null);
								thing.stackCount = num2;
								bool flag7 = thing.def.stackLimit > 0;
								if (flag7)
								{
									bool flag8 = thing.stackCount > thing.def.stackLimit;
									if (flag8)
									{
										thing.stackCount = thing.def.stackLimit;
									}
								}
								l += thing.stackCount;
								num2 -= thing.stackCount;
								Thing thing2;
								GenPlace.TryPlaceThing(thing, intVec2, this.pawn.Map, 0, ref thing2, null, null, default(Rot4));
								bool spawnForbidden = this.Props.spawnForbidden;
								if (spawnForbidden)
								{
									ForbidUtility.SetForbidden(thing2, true, true);
								}
							}
							bool flag9 = num3++ > 10;
							if (flag9)
							{
								Setup.Warn("Had to break the loop", this.myDebug);
								return false;
							}
						}
						bool flag10 = num2 <= 0;
						result = flag10;
					}
				}
				return result;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x000061A4 File Offset: 0x000051A4
		private bool TryFindSpawnCell(out IntVec3 result)
		{
			this.pawn = this.parent.pawn;
			bool flag = !Setup.OkPawn(this.pawn);
			checked
			{
				bool result2;
				if (flag)
				{
					result = IntVec3.Invalid;
					Setup.Warn("TryFindSpawnCell Null - pawn null", this.myDebug);
					result2 = false;
				}
				else
				{
					foreach (IntVec3 intVec in GenCollection.InRandomOrder<IntVec3>(GenAdj.CellsAdjacent8Way(this.pawn), null))
					{
						bool flag2 = GenGrid.Walkable(intVec, this.pawn.Map);
						if (flag2)
						{
							Building edifice = GridsUtility.GetEdifice(intVec, this.pawn.Map);
							bool flag3 = edifice == null || !EdificeUtility.IsEdifice(this.Props.thingToSpawn);
							if (flag3)
							{
								Building_Door building_Door;
								bool flag4 = (building_Door = (edifice as Building_Door)) == null || building_Door.FreePassage;
								if (flag4)
								{
									bool flag5 = GenSight.LineOfSight(this.pawn.Position, intVec, this.pawn.Map, false, null, 0, 0);
									if (flag5)
									{
										bool flag6 = false;
										List<Thing> thingList = GridsUtility.GetThingList(intVec, this.pawn.Map);
										for (int i = 0; i < thingList.Count; i++)
										{
											Thing thing = thingList[i];
											bool flag7 = thing.def.category == 2;
											if (flag7)
											{
												bool flag8 = thing.def != this.Props.thingToSpawn || thing.stackCount > this.Props.thingToSpawn.stackLimit - this.calculatedQuantity;
												if (flag8)
												{
													flag6 = true;
													break;
												}
											}
										}
										bool flag9 = !flag6;
										if (flag9)
										{
											result = intVec;
											return true;
										}
									}
								}
							}
						}
					}
					Setup.Warn("TryFindSpawnCell Null - no spawn cell found", this.myDebug);
					result = IntVec3.Invalid;
					result2 = false;
				}
				return result2;
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000063C4 File Offset: 0x000053C4
		private void ResetCountdown()
		{
			this.ticksUntilSpawn = (this.initialTicksUntilSpawn = checked((int)(unchecked(this.RandomDays2wait() * 60000f))));
		}

		// Token: 0x06000067 RID: 103 RVA: 0x000063F0 File Offset: 0x000053F0
		private float RandomDays2wait()
		{
			return Rand.Range(this.calculatedMinDaysB4Next, this.calculatedMaxDaysB4Next);
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00006418 File Offset: 0x00005418
		private float RandomGraceDays()
		{
			return this.Props.graceDays * Rand.Range(0f, 1f);
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000069 RID: 105 RVA: 0x00006448 File Offset: 0x00005448
		public override string CompTipStringExtra
		{
			get
			{
				string text = string.Empty;
				bool flag = this.graceTicks > 0;
				if (flag)
				{
					bool animalThing = this.Props.animalThing;
					if (animalThing)
					{
						text = " No " + this.Props.animalToSpawn.defName + " for " + GenDate.ToStringTicksToPeriod(this.graceTicks, true, false, true, true);
					}
					else
					{
						text = " No " + this.Props.thingToSpawn.label + " for " + GenDate.ToStringTicksToPeriod(this.graceTicks, true, false, true, true);
					}
					bool flag2 = this.hungerReset > 0;
					if (flag2)
					{
						text += "(hunger)";
					}
					else
					{
						bool flag3 = this.healthReset > 0;
						if (flag3)
						{
							text += "(injury)";
						}
						else
						{
							text += "(grace period)";
						}
					}
				}
				else
				{
					text = GenDate.ToStringTicksToPeriod(this.ticksUntilSpawn, true, false, true, true) + " before ";
					bool animalThing2 = this.Props.animalThing;
					if (animalThing2)
					{
						text += this.Props.animalToSpawn.defName;
					}
					else
					{
						text += this.Props.thingToSpawn.label;
					}
					text = string.Concat(new object[]
					{
						text,
						" ",
						this.Props.spawnVerb,
						"(",
						this.calculatedQuantity,
						"x)"
					});
				}
				return text;
			}
		}

		// Token: 0x0400007C RID: 124
		private int ticksUntilSpawn;

		// Token: 0x0400007D RID: 125
		private int initialTicksUntilSpawn = 0;

		// Token: 0x0400007E RID: 126
		private int hungerReset = 0;

		// Token: 0x0400007F RID: 127
		private int healthReset = 0;

		// Token: 0x04000080 RID: 128
		private int graceTicks = 0;

		// Token: 0x04000081 RID: 129
		private Pawn pawn = null;

		// Token: 0x04000082 RID: 130
		private float calculatedMaxDaysB4Next = 2f;

		// Token: 0x04000083 RID: 131
		private float calculatedMinDaysB4Next = 1f;

		// Token: 0x04000084 RID: 132
		private int calculatedQuantity = 1;

		// Token: 0x04000085 RID: 133
		private bool blockSpawn = false;

		// Token: 0x04000086 RID: 134
		private bool myDebug = false;

		// Token: 0x04000087 RID: 135
		private readonly float errorMinDaysB4Next = 0.001f;

		// Token: 0x04000088 RID: 136
		private readonly int errorExponentialLimit = 20;

		// Token: 0x04000089 RID: 137
		private readonly int errorSpawnCount = 750;
	}
}