using Drafts;
using GuildMasterIsekai.Model;
using GuildMasterIsekai.Systems;
using GuildMasterIsekai.Visitor.Spot;
using UnityEngine;

namespace GuildMasterIsekai.Visitor
{
    public class VisitorController : MonoBehaviour
    {
        [SerializeField] private TimerComponent adventurerTimer;
        [SerializeField] private TimerComponent costumerTimer;
        [SerializeField] private HallSpots spots;
        [SerializeField, Prefab] private VisitorAI visitorAiPrefab;

        private Guild guild;
        private GuildHall hall;

        private void Start()
        {
            guild = Game.SaveMngr.Get<Guild>();
            hall = Game.SaveMngr.Get<GuildHall>();
            adventurerTimer.OnTrigger.AddListener(OnTriggerAdventurer);
            costumerTimer.OnTrigger.AddListener(OnTriggerCostumer);
        }

        private void OnDestroy()
        {
            adventurerTimer.OnTrigger.RemoveListener(OnTriggerAdventurer);
            costumerTimer.OnTrigger.RemoveListener(OnTriggerCostumer);
        }

        private void OnTriggerAdventurer() => SpawnVisitor(guild.Generator.NewAdventurer());
        private void OnTriggerCostumer() => SpawnVisitor(guild.Generator.NewCostumer());

        public void SpawnVisitor(IVisitor visitor)
        {
            var (pos, rot) = spots.Entrance;
            var controller = Instantiate(visitorAiPrefab, pos, rot, transform);
            Instantiate(visitor.Prefab, controller.transform);
            controller.SetVisitor(visitor.GetAI(controller.Agent, spots));
        }
    }
}