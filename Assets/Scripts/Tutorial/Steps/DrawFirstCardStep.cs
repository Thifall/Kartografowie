using Kartografowie.Assets.Scripts.Tutorial.Steps.Abstract;
using Kartografowie.Cards;

namespace Kartografowie.Assets.Scripts.Tutorial.Steps
{
    public class DrawFirstCardStep : TutorialStep
    {
        private readonly TutorialController _controller;

        public DrawFirstCardStep(TutorialController controller)
        {
            _controller = controller;
        }
        public override void EnterStep()
        {
            // odblokuj draw button
            // pokaż highlight na przycisku
            // pokaż tekst
        }

        public override void ExitStep()
        {
            // usuń highlight
        }

        public override void HandleEvent(object eventData)
        {
            if (eventData is DiscoveryCard)
            {
                _controller.NextStep();
            }
        }
    }
}
