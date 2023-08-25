using DG.Tweening;
using UnityEngine;

namespace Gameplay
{
    public class Player : MonoBehaviour, IUxSetup
    {
        [Header("Data")] 
        [SerializeField] public float initHealth;
        [SerializeField] public float currentHealth;

        [SerializeField] public float initArmor;
        [SerializeField] public float currentArmor;
        
        [Header("Components")]
        [SerializeField] public CardHolder cardHolderComponent;
        [SerializeField] public SpriteRenderer spriteRendererComponent;

        protected virtual void Start()
        {
            cardHolderComponent = GetComponentInChildren<CardHolder>();
            spriteRendererComponent = GetComponentInChildren<SpriteRenderer>();

            currentHealth = initHealth;
            currentArmor = initArmor;
        }

        public virtual void AddCardToHand(MasterCard card)
        {
            cardHolderComponent.AddCardToCardHolder(card.transform);
        }

        public virtual void UxSetup()
        {
            transform.position -= new Vector3(0, 3, 0);
            transform.DOMoveY(transform.position.y + 3, .2f);
        }

        public void ApplyAttack(float attackAmount)
        {

            spriteRendererComponent.DOColor(new Color(1, 1, 1, 0), .2f);
            spriteRendererComponent.DOColor(new Color(1, 1, 1, 1), .2f);

            float armorAfterChange = currentArmor - attackAmount;
            float healthAfterChange = currentHealth;

            if (armorAfterChange > 0)
            {
                currentArmor = armorAfterChange;
            }
            // Armor <= 0
            else
            {
                currentArmor = 0;
                healthAfterChange = currentHealth + armorAfterChange;
            }

            currentHealth = healthAfterChange;

            if (currentHealth <= 0)
            {
                Debug.Log(nameof(this.gameObject) + " Dead!");
            }

        }
        
        public void ApplyDefend(float defendAmount)
        {
            currentArmor += defendAmount;
        }

        public void ApplyTreat(float treatAmount)
        {
            float healthChanged = currentHealth + treatAmount;

            currentHealth = healthChanged >= initHealth ? initHealth : healthChanged;
        }
    }
}