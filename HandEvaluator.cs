using System.Collections.Generic;
using System.Linq;

namespace PokerGame
{
    public static class HandEvaluator
    {
        public enum HandRank
        {
            HighCard,
            OnePair,
            TwoPair,
            ThreeOfAKind,
            Straight,
            Flush,
            FullHouse,
            FourOfAKind,
            StraightFlush,
            RoyalFlush
        }

        public static HandRank Evaluate(List<Card> hand)
        {
            bool isFlush = hand.Select(c => c.CardSuit).Distinct().Count() == 1;

            var ranks = hand.Select(c => (int)c.CardRank).OrderBy(r => r).ToList();

            bool isStraight = false;
            if (ranks.Distinct().Count() == 5)
            {
                // 一般順子：最大 - 最小 == 4
                if (ranks[4] - ranks[0] == 4)
                    isStraight = true;

                // Ace-Low 順子：A-2-3-4-5（排序後為 2,3,4,5,14）
                if (ranks[0] == 2 && ranks[1] == 3 && ranks[2] == 4
                    && ranks[3] == 5 && ranks[4] == 14)
                    isStraight = true;
            }

            // 各點數分組數量（由多到少）
            var groups = hand
                .GroupBy(c => c.CardRank)
                .Select(g => g.Count())
                .OrderByDescending(c => c)
                .ToList();

            // 判斷順序：由高到低
            if (isFlush && isStraight && ranks[4] == 14 && ranks[0] == 10)
                return HandRank.RoyalFlush;        // 皇家同花順

            if (isFlush && isStraight)
                return HandRank.StraightFlush;     // 同花順

            if (groups[0] == 4)
                return HandRank.FourOfAKind;       // 四條

            if (groups[0] == 3 && groups[1] == 2)
                return HandRank.FullHouse;         // 葫蘆

            if (isFlush)
                return HandRank.Flush;             // 同花

            if (isStraight)
                return HandRank.Straight;          // 順子

            if (groups[0] == 3)
                return HandRank.ThreeOfAKind;      // 三條

            if (groups[0] == 2 && groups[1] == 2)
                return HandRank.TwoPair;           // 兩對

            if (groups[0] == 2)
                return HandRank.OnePair;           // 一對

            return HandRank.HighCard;              // 散牌
        }

        public static int GetMultiplier(HandRank rank)
        {
            return rank switch
            {
                HandRank.RoyalFlush    => 250,
                HandRank.StraightFlush => 50,
                HandRank.FourOfAKind   => 25,
                HandRank.FullHouse     => 9,
                HandRank.Flush         => 6,
                HandRank.Straight      => 4,
                HandRank.ThreeOfAKind  => 3,
                HandRank.TwoPair       => 2,
                HandRank.OnePair       => 1,
                _                      => 0
            };
        }

        public static string GetHandName(HandRank rank)
        {
            return rank switch
            {
                HandRank.RoyalFlush    => "皇家同花順",
                HandRank.StraightFlush => "同花順",
                HandRank.FourOfAKind   => "四條",
                HandRank.FullHouse     => "葫蘆",
                HandRank.Flush         => "同花",
                HandRank.Straight      => "順子",
                HandRank.ThreeOfAKind  => "三條",
                HandRank.TwoPair       => "兩對",
                HandRank.OnePair       => "一對",
                _                      => "散牌（無牌型）"
            };
        }
    }
}
