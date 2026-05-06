using System;
using System.Collections.Generic;

namespace PokerGame
{
    public class Deck
    {
        private List<Card> cards = new List<Card>();
        private Random rng = new Random();

        public Deck() { Reset(); }

        public void Reset()
        {
            cards.Clear();
            foreach (Card.Suit s in Enum.GetValues(typeof(Card.Suit)))
                foreach (Card.Rank r in Enum.GetValues(typeof(Card.Rank)))
                    cards.Add(new Card(s, r));
        }

        public void Shuffle()
        {
            for (int i = cards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (cards[i], cards[j]) = (cards[j], cards[i]);
            }
        }

        public Card Draw()
        {
            if (cards.Count == 0)
                throw new InvalidOperationException("牌組已空！");
            Card c = cards[0];
            cards.RemoveAt(0);
            return c;
        }
    }
}
