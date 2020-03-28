using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackjackCL
{
    public class Deck
    {
        private List<Card> deck = new List<Card>();

        public Deck()
        {
            MakeDeck();
        }

        /// <summary>
        /// Creates a Blackjack deck from the number of regular decks specified
        /// </summary>
        /// <param name="numDecks">The number of decks to use</param>
        public Deck(int numDecks)
        {
            for (int i = 0; i < numDecks; i++)
            {
                Deck baseDeck = new Deck();
                for (int x = 0; x < 52; x++)
                {
                    deck.Add(baseDeck.DrawOneCard());
                }
            }
        }

        /// <summary>
        /// Creates a deck
        /// </summary>
        private void MakeDeck()
        {
            // there are 4 suits
            foreach (int s in Enum.GetValues(typeof(Suit)))
            {
                // there are 13 cards per suit
                foreach (int v in Enum.GetValues(typeof(FaceValue)))
                {
                    // create a card for the current suit and value
                    Card newCard = new Card((Suit)s, (FaceValue)v);

                    // add the card to the deck
                    deck.Add(newCard);
                }
            }
        }

        /// <summary>
        /// Shuffles the deck
        /// </summary>
        public void Shuffle()
        {
            List<Card> newDeck = new List<Card>();
            Random rGen = new Random();

            while (deck.Count > 0)
            {
                int removeIndex = rGen.Next(0, deck.Count);
                Card cardToRemove = deck[removeIndex];
                deck.RemoveAt(removeIndex);
                newDeck.Add(cardToRemove);
            }

            // replace the old deck with the next deck
            deck = newDeck;
        }


        /// <summary>
        /// Draws one card and removes it from the deck
        /// </summary>
        /// <returns>Card drawn</returns>
        public Card DrawOneCard()
        {
            Card topCard;
            if (deck.Count > 0)
            {
                topCard = deck[0];
                deck.RemoveAt(0);

                return topCard;
            }
            else
            {
                throw new ArgumentException("There are no cards in the deck - deal again.");
            }
        }
    }
}
