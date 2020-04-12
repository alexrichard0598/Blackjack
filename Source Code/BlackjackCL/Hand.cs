using System.Collections.Generic;
using System.Linq;

namespace BlackjackCL
{
    public class Hand:List<Card>
    {
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
