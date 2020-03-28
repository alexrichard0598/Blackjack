using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackCL
{
    public class Card
    {
        /// <summary>
        /// Generates a card
        /// </summary>
        /// <param name="suit">the suit of the card</param>
        /// <param name="value">the fave value of the card</param>
        public Card(Suit suit, FaceValue value)
        {
            FaceValue = value;
            Suit = suit;
        }

        public FaceValue FaceValue { get; }
        public Suit Suit { get; }
    }
}
