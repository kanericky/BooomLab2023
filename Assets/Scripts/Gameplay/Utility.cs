namespace Gameplay
{
    public class Utility
    {
    
    }

    public enum CardType
    {
        Attack,
        Defend,
        Treat,
        Default,
        None,
    }

    public enum CardStatus
    {
        Complete,
        Separated,
        SeparateTop,
        SeparateBottom,
        Combined,
        Checkedout,
    }

    public enum Turn
    {
        Start,
        Player,
        Enemy,
        Win,
        Lose,
    }
}