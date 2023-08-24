using System;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime
{
    public class Player : MonoBehaviour
    {
        [Header("Data")] 
        [SerializeField] private float initHealth;
        [SerializeField] private float currentHealth;

        [SerializeField] private float initArmor;
        [SerializeField] private float currentArmor;
        
        [Header("Components")]
        [SerializeField] public CardHolder cardHolderComponent;

        protected virtual void Start()
        {
            cardHolderComponent = GetComponentInChildren<CardHolder>();
        }

        public virtual void AddCardToHand(MasterCard card)
        {
            cardHolderComponent.AddCardToCardHolder(card.transform);
        }
    }
}