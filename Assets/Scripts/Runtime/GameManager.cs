using System;
using UnityEngine;

namespace Runtime
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public CardBundle[] cardBank;
 
        private void Start()
        {
            Instance = this;
        }

        // TODO - Get card sprite in the sprite bank
        public Sprite GetCardSprite(int cardPoint, CardType cardType)
        {
            return null;
        }
    }

    public struct CardBundle
    {
        public Sprite attackSprite;
        public Sprite defendSprite;
        public Sprite treatSprite;
        public Sprite normalSprite;
    }
}
