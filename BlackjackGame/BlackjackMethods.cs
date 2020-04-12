using BlackjackCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BlackjackGame
{
    public static class BlackjackMethods
    {
        /// <summary>
        /// Generates an image of the specified card
        /// that should lay ontop of other cards if 
        /// overlappingImage is set to true
        /// </summary>
        /// <param name="card">the card to generate image of</param>
        /// <param name="overlappingImage">Whether to overlap or just return normal image</param>
        /// <returns>Image of card</returns>
        public static Image GenerateImage(Card card, bool overlappingImage = false)
        {
            int x = 5;
            if (overlappingImage)
                x = -60;

            string filePath = $@"/images/{card.FaceValue.ToString()}{card.Suit.ToString()}.jpg";
            Uri path = new Uri(filePath, UriKind.Relative);
            BitmapImage image = new BitmapImage(path);
            Image img = new Image()
            {
                Source = image,
                Height = 100,
                Width = 75,
                Margin = new System.Windows.Thickness(x,0,0,0)
            };
            return img;
        }

        /// <summary>
        /// Generate a image of the back of the card overlapping or not
        /// </summary>
        /// <param name="overlappingImage">whether to overlap or return normal</param>
        /// <returns>Returns the image of the card back</returns>
        public static Image GenerateImage(bool overlappingImage = false)
        {
            int x = 5;
            if (overlappingImage)
                x = -60;

            string filePath = $@"/images/cardback.gif";
            Uri path = new Uri(filePath, UriKind.Relative);
            BitmapImage image = new BitmapImage(path);
            Image img = new Image()
            {
                Source = image,
                Height = 100,
                Width = 75,
                Margin = new System.Windows.Thickness(x,0,0,0)
            };
            return img;
        }

        public enum Sound
        {
            Win,
            Lose
        }

        /// <summary>
        /// Play a sound
        /// </summary>
        /// <param name="sound">The sound to play</param>
        public static void PlaySound(Sound sound)
        {
            MediaPlayer player = new MediaPlayer();
            if (sound == Sound.Lose)
            {
                player.Open(new Uri(@"audio/LoseSound.wav", UriKind.Relative));
                player.Play();
            } 
            else if (sound == Sound.Win)
            {
                player.Open(new Uri(@"audio/WinSound.wav", UriKind.Relative));
                player.Play();
            }
        }

        /// <summary>
        /// Reports the error to the user via a MessageBox
        /// </summary>
        /// <param name="ex">The Error</param>
        public static void ErrorHandling(Exception ex)
        {
            SystemSounds.Exclamation.Play();
            MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// Sorts the hand by the Card FaceValue Aces at the end Kings at the start
        /// </summary>
        /// <param name="hand">The hand to sort</param>
        /// <returns>The sorted hand</returns>
        public static Hand Sort(Hand hand)
        {
            Hand sorted = new Hand();
            sorted.AddRange(hand.OrderBy(c => c.FaceValue));
            sorted.Reverse();
            return sorted;
        }

        /// <summary>
        /// Determines the total face value of the hand in blackjack
        /// </summary>
        /// <param name="hand">The hand to calculate the value of</param>
        /// <returns>The total value of the hand</returns>
        public static int HandValue(Hand hand)
        {
            Hand handSorted = Sort(hand);
            int handValue = 0;

            //How many aces are there in the players hand
            int numAces = handSorted.Where(c => c.FaceValue == FaceValue.Ace).Count();
            foreach (Card pCard in handSorted)
            {
                FaceValue faceValue = pCard.FaceValue;
                int cardValue = (int)faceValue + 1;

                /* If there is more then one ace left or 11 would put over 21
                 * set the ace value to 1. If the ace wouldn't put it over 21
                 * and is the last ace, and thus last card, in the hand put the 
                 * ace valued at 11.
                */

                if (faceValue == FaceValue.Ace)
                {
                    numAces--; //subtract one of the number of aces left to calculate

                    if(numAces != 0 || handValue + 11 > 21)
                    {
                        handValue += 1;
                    }
                    else
                    {
                        handValue += 11;
                    }
                }
                else if (faceValue == FaceValue.Jack || faceValue == FaceValue.Queen || faceValue == FaceValue.King)
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

        /// <summary>
        /// Determines whether a hand is a soft seventeen or not
        /// </summary>
        /// <param name="hand">The hand to check</param>
        /// <returns></returns>
        public static bool IsSoftSeventeen(Hand hand)
        {
            Hand thisHand = new Hand();
            thisHand.AddRange(hand);

            if (thisHand.Where(c => c.FaceValue == FaceValue.Ace).ToList().Count == 0 || HandValue(hand) != 17)
                return false;

            //Hand without the aces
            Hand noAces = new Hand();
            noAces.AddRange(thisHand.Where(c => c.FaceValue != FaceValue.Ace).ToList());
            Hand onlyAces = new Hand();
            onlyAces.AddRange(thisHand.Where(c => c.FaceValue == FaceValue.Ace).ToList());

            int noAcesValue = HandValue(noAces);
            int onlyAcesValue = HandValue(onlyAces);
            return noAcesValue + onlyAcesValue == 17;


            throw new NotImplementedException();
        }

        /// <summary>
        /// Set up the deck using 3 standard decks,
        /// and shuffles the deck 7 times
        /// </summary>
        /// <param name="numDecks">The number of decks to make combine</param>
        /// <returns>The new deck appropriately shuffled</returns>
        public static Deck SetupDeck(int numDecks)
        {
            Deck deck = MakeBlackjackDeck(3);

            //Shuffle the deck 7 times which is the optimal number of times to shuffle.
            //Source: https://youtu.be/AxJubaijQbI
            for (int i = 0; i < 7; i++)
            {
                deck.Shuffle();
            }

            return deck;
        }

        /// <summary>
        /// Creates a Blackjack deck from the number of regular decks specified
        /// </summary>
        /// <param name="numDecks">The number of decks to use</param>
        public static Deck MakeBlackjackDeck(int numDecks)
        {
            List<Card> newDeck = new List<Card>();
            for (int i = 0; i < numDecks; i++)
            {
                Deck baseDeck = new Deck();
                for (int x = 0; x < 52; x++)
                {
                    newDeck.Add(baseDeck.DrawOneCard());
                }
            }
            return new Deck(newDeck);
        }
    }
}
