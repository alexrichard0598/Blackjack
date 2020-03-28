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
    }
}
