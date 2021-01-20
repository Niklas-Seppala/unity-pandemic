#pragma warning disable CS0649

using CoronaGame.UI;

namespace CoronaGame.Items
{
    public class VaccineItem : WorldItem
    {
        public override void Collect()
        {
            ViewManager.Instance.ScreenFade(middleAction: () => {
                 ViewManager.Instance.Open<CreditsView>();
            });
        }
    }
}

