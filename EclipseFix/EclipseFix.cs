using BepInEx;
using RoR2;
using UnityEngine;
using HealthComponent = On.RoR2.HealthComponent;

namespace EclipseFix;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class EclipseFixPlugin : BaseUnityPlugin
{
    private const string PluginGUID = PluginAuthor + "." + PluginName;
    private const string PluginAuthor = "InkyaRev";
    private const string PluginName = "EclipseFix";
    private const string PluginVersion = "1.0.0";
    
    public void Awake()
    {
        Log.Init(Logger);
        HealthComponent.RepeatHealComponent.FixedUpdate += (_, selfMonoBehaviour) =>
        { 
            if (selfMonoBehaviour is RoR2.HealthComponent.RepeatHealComponent self)
            {
                self.timer -= Time.fixedDeltaTime;
                if (self.timer > 0.0)
                    return;
                self.timer = 0.2f;
                if (self.reserve <= 0.0)
                    return;
                var amount = Mathf.Min((float) (self.healthComponent.fullHealth * (double) self.healthFractionToRestorePerSecond * 0.2), self.reserve);
                self.reserve -= amount;
                var procChainMask = new ProcChainMask();
                procChainMask.AddProc(ProcType.RepeatHeal);
                if (self.healthComponent.body.teamComponent.teamIndex == TeamIndex.Player && Run.instance.selectedDifficulty >= DifficultyIndex.Eclipse5)
                {
                    amount *= 2f;
                }
                self.healthComponent.Heal(amount, procChainMask);
            }
            else
            {
                Log.Error("WHat the fuck");
            }
        };
        On.RoR2.EclipseRun.OverrideRuleChoices += (orig, self, include, exclude, seed) =>
        {
            orig(self, include, exclude, seed);
            self.ForceChoice(include, exclude, $"Items.{RoR2Content.Items.ShieldOnly.name}.Off");
        };
    }
}