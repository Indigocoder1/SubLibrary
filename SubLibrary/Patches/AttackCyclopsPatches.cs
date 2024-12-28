using HarmonyLib;
using SubLibrary.Monobehaviors;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace SubLibrary.Patches;

[HarmonyPatch(typeof(AttackCyclops))]
internal class AttackCyclopsPatches
{
    [HarmonyPatch(nameof(AttackCyclops.UpdateAggression)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> UpdateAggression_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, GetMatch())
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(GetSubAttackableLikeCyclops));

        return matcher.InstructionEnumeration();
    }

    [HarmonyPatch(nameof(AttackCyclops.OnCollisionEnter)), HarmonyTranspiler]
    private static IEnumerable<CodeInstruction> OnCollisionEnter_Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var matcher = new CodeMatcher(instructions)
            .MatchForward(false, GetMatch())
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(GetSubAttackableLikeCyclops));

        return matcher.InstructionEnumeration();
    }

    private static CodeMatch GetMatch()
    {
        bool hasChameleon = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("Indigocoder.Chameleon");
        bool hasSeal = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("SealSub");

        if (hasChameleon)
        {
            return new CodeMatch(i => i.opcode == OpCodes.Call && i.operand is MethodInfo && ((MethodInfo)i.operand).Name == "IsCurrentSubCyclops");
        }
        else if (hasSeal)
        {
            return new CodeMatch(i => i.opcode == OpCodes.Call && i.operand is MethodInfo && ((MethodInfo)i.operand).Name == "IsCyclopsBool");
        }

        var isCyclops = typeof(SubRoot).GetField("isCyclops", BindingFlags.Instance | BindingFlags.Public);
        return new CodeMatch(i => i.opcode == OpCodes.Ldfld && (FieldInfo)i.operand == isCyclops);
    }

    public static bool GetSubAttackableLikeCyclops(bool wasCyclops)
    {
        var currentSub = Player.main.currentSub;
        if (!currentSub) return wasCyclops;

        if (currentSub.GetComponentInChildren<AttackableLikeCyclops>(true)) return true;

        return wasCyclops;
    }
}
