using UnityEngine;

namespace Gameplay
{
    public class EnemyCard : MasterCard
    {
        public Sprite[] hiddenSprites;

        protected override void SpawnMainCard(CardData cardData, CardType cardType)
        {
            base.SpawnMainCard(cardData, cardType);
            
            _completeCard.CoverCard(hiddenSprites[(int) cardType]);
        }

        public void RevealCard()
        {
            _completeCard.RefreshCardSprite(masterCardType);
        }

        protected override void OnMouseExit() { }

        protected override void OnMouseDown() { }

        protected override void OnMouseDrag() { }

        protected override void OnMouseUp()
        {
            if (_canCheckout)
            {
                LevelManager.Instance.CheckoutCard(this);
                KillIdleTweenAnimation();
                masterCardStatus = CardStatus.Checkedout;
            }
        }

        protected override void OnMouseOver() { }
    }
}