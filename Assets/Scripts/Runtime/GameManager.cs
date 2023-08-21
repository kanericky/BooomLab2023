using System;
using UnityEngine;

namespace Runtime
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        public Color[] cardTypeColor;
        public Sprite[] cardTypeIcon;
 
        private void Start()
        {
            Instance = this;
        }

        public Color AssignColor(CardType cardType)
        {
            switch (cardType)
            {
                case CardType.Attack:
                    return cardTypeColor[1];
                    break;
            
                case CardType.Defend:
                    break;
            
                case CardType.Treat:
                    break;
            
                case CardType.Default:
                    break;
            }

            return Color.white;
        }

        public Sprite AssignIcon(CardType cardType)
        {
            return null;
        }
    }
}
