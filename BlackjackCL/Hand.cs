using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackCL
{
    public class Hand:List<Card>
    {
        /// <summary>
        /// Sorts the hand by the Card FaceValue Aces at the end Kings at the start
        /// </summary>
        /// <returns>The sorted hand</returns>
        public new Hand Sort()
        {
            Hand sorted = new Hand();
            sorted.AddRange(this.OrderBy(c => c.FaceValue));
            sorted.Reverse();
            return sorted;
        }

        /// <summary>
        /// Determines the total face value of the hand in blackjack
        /// </summary>
        /// <returns>The total value of the hand</returns>
        public int HandValue()
        {
            Hand handSorted = Sort();
            int handValue = 0;

            //How many aces are there in the players hand
            int numAces = handSorted.Where(c => c.FaceValue == FaceValue.Ace).Count();
            foreach (Card pCard in handSorted)
            {
                FaceValue faceValue = pCard.FaceValue;
                int cardValue = (int)faceValue + 1;

                /* If there is only one Ace left then it would be the last card 
                 * in the hand to be calculated. So if it would be 21 or under
                 * when it's valued at 11 then it should be worth 11. If 11
                 * would push the player over 21 value the Ace at 1. If there 
                 * are not still Aces and 11 would put the player at 21 then
                 * it's worth 21. Otherwise if there are still aces and 11 
                 * would put the player at 21 value it at 1 because otherwise
                 * the player would be pushed over 21 with the next card.
                */
                if (faceValue == FaceValue.Ace)
                {
                    numAces--; //subtract one of the number of aces left to calculate

                    if (handValue + 11 == 21) //If 11 would make 21
                    {
                        if (numAces != 0) //and there are still cards
                        {
                            handValue += 1;
                        }
                        else
                        {
                            handValue += 11;
                        }
                    }
                    else if (handValue + 11 < 21) //if 11 would still make the hand value less then 21
                    {
                        handValue += 11;
                    }
                    else
                    {
                        handValue += 1;
                    }
                } 
                else if(faceValue == FaceValue.Jack || faceValue == FaceValue.Queen || faceValue == FaceValue.King)
                {
                    handValue += 10; 
                }
                else
                {
                    handValue += cardValue;
                }
            }

            return handValue;
        }

        public bool IsSoftSeventeen()
        {
            Hand thisHand = new Hand();
            thisHand.AddRange(this);

            if (thisHand.Where(c => c.FaceValue == FaceValue.Ace).ToList().Count == 0 || HandValue() != 17)
                return false;

            //Hand without the aces
            Hand noAces = new Hand();
            noAces.AddRange(thisHand.Where(c => c.FaceValue != FaceValue.Ace).ToList());
            Hand onlyAces = new Hand();
            onlyAces.AddRange(thisHand.Where(c => c.FaceValue == FaceValue.Ace).ToList());
            
            int noAcesValue = noAces.HandValue();
            int onlyAcesValue = onlyAces.HandValue();
            return noAcesValue + onlyAcesValue == 17;


            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks if the card is contianed within the hand
        /// </summary>
        /// <param name="cardToCheck">the card to check</param>
        /// <returns>true if hand contains card otherwise false</returns>
        public new bool Contains(Card cardToCheck)
        {
            return this.Where(card => card.FaceValue == cardToCheck.FaceValue && card.Suit == cardToCheck.Suit).ToList().Count != 0;
        }
    }
}
