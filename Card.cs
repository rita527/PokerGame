namespace PokerGame
{
    public class Card
    {
        public enum Suit { Spades = 0, Hearts = 1, Diamonds = 2, Clubs = 3 }
        public enum Rank
        {
            Two = 2, Three, Four, Five, Six, Seven,
            Eight, Nine, Ten, Jack = 11, Queen = 12, King = 13, Ace = 14
        }

        public Suit CardSuit { get; }
        public Rank CardRank { get; }

        public Card(Suit suit, Rank rank)
        {
            CardSuit = suit;
            CardRank = rank;
        }

        public string RankString => CardRank switch
        {
            Rank.Ace   => "A",
            Rank.King  => "K",
            Rank.Queen => "Q",
            Rank.Jack  => "J",
            Rank.Ten   => "10",
            _          => ((int)CardRank).ToString()
        };

        public string SuitSymbol => CardSuit switch
        {
            Suit.Spades   => "♠",
            Suit.Hearts   => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs    => "♣",
            _             => ""
        };

        public bool IsRed => CardSuit == Suit.Hearts || CardSuit == Suit.Diamonds;
    }
}
