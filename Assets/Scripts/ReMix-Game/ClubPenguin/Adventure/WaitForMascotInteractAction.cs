using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;  // Needed for Debug.Log

namespace ClubPenguin.Adventure
{
    [ActionCategory("Quest")]
    public class WaitForMascotInteractAction : FsmStateAction
    {
        public FsmEvent InteractEvent;
        public MascotDefinition Mascot;
        public bool DisableDefaultInteract = true;

        private Mascot mascotComponent;

        public override void OnEnter()
        {
            // Attempt to retrieve the Mascot component
            mascotComponent = Service.Get<MascotService>().GetMascot(Mascot.name);

            // Log whether mascotComponent was successfully retrieved
            if (mascotComponent == null)
            {
                Debug.LogError("MascotComponent is null in OnEnter");
            }
            else
            {
                mascotComponent.IsDefaultInteractDisabled = DisableDefaultInteract;
                mascotComponent.InteractionBehaviours.ZoomIn = false;
                mascotComponent.InteractionBehaviours.ZoomOut = false;
                mascotComponent.InteractionBehaviours.LowerTray = false;
                mascotComponent.InteractionBehaviours.RestoreTray = false;
                mascotComponent.InteractionBehaviours.ShowIndicator = false;
                mascotComponent.InteractionBehaviours.SuppressQuestNotifier = false;
                mascotComponent.InteractionBehaviours.RestoreQuestNotifier = false;
                mascotComponent.InteractionBehaviours.MoveToTalkSpot = false;
                mascotComponent.InteractionBehaviours.OverrideInteracteeTxform = false;
            }

            // Add the event listener
            Service.Get<EventDispatcher>().AddListener<QuestEvents.OnMascotInteract>(onMascotInteract);
        }

        public bool onMascotInteract(QuestEvents.OnMascotInteract evt)
        {
            // Check if the interacting mascot is the one we're interested in
            if (evt.Mascot.Name == Mascot.name)
            {
                base.Fsm.Event(InteractEvent);
            }
            return false;
        }

        public override void OnExit()
        {
            // Check if mascotComponent is null before modifying it
            if (mascotComponent == null)
            {
                Debug.LogError("MascotComponent is null in OnExit");
            }
            else
            {
                mascotComponent.IsDefaultInteractDisabled = false;
            }

            // Remove the event listener
            Service.Get<EventDispatcher>().RemoveListener<QuestEvents.OnMascotInteract>(onMascotInteract);
        }
    }
}
