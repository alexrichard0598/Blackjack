using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using BlackjackCL;
using static BlackjackGame.BlackjackMethods;

namespace BlackjackGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region fields
        Deck deck;
        Hand playerHand;
        Hand dealerHand;
        Card holeCard;
        Hand dealerFullHand;
        bool isHoleRevealed = false;
        bool hasDealerStand = false;
        #endregion

        #region Methods
        /// <summary>
        /// Enable or disable all buttons
        /// </summary>
        /// <param name="enable">Whether the buttons should be enabled or disabled</param>
        private void ChangeButtonState(bool enable)
        {
            bttnHit.IsEnabled = enable;
            bttnStand.IsEnabled = enable;
            bttnHelp.IsEnabled = enable;
            bttnNewGame.IsEnabled = enable;
        }


        /// <summary>
        /// Clear game and set up
        /// </summary>
        private void SetupGame()
        {
            Reset();
            deck = SetupDeck(3);
            //Set up the hands
            playerHand = new Hand();
            dealerHand = new Hand();
            dealerFullHand = new Hand();

            //Deal the player two cards
            DealPlayerCard();
            DealPlayerCard();

            //Deal the dealer his card and the hole
            DealDealerCard();
            holeCard = deck.DrawOneCard();

            //Add the hole card to the full hand
            //which is already equal to the visible hand
            dealerFullHand.Add(holeCard);

            //If the player has Blackjack end the game.
            if (HandValue(playerHand) == 21)
            {
                EndGame();
            }

            //If the dealer has blackjack end the game.
            if (HandValue(dealerFullHand) == 21)
            {
                hasDealerStand = true;
                EndGame();
            }
        }

        /// <summary>
        /// Resets the form
        /// </summary>
        private void Reset()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                ChangeButtonState(true);
                isHoleRevealed = false;
                hasDealerStand = false;
                lblHandValue.Content = "0";
                lblDealerValue.Content = "0";
            });
        }

        /// <summary>
        /// Reveal the hole to the player
        /// </summary>
        private void RevealHole()
        {
            isHoleRevealed = true;
            DisplayCards();
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                lblDealerValue.Content = HandValue(dealerFullHand).ToString();
            });
        }

        /// <summary>
        /// Deal the player a card
        /// </summary>
        private void DealPlayerCard()
        {
            //Draw one card and add it to the players hand
            Card card = deck.DrawOneCard();
            playerHand.Add(card);

            DisplayCards();

            //Update the value of the players hand
            int playerHandValue = HandValue(playerHand);
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                lblHandValue.Content = playerHandValue.ToString();
            });

            if (playerHandValue > 21) //If the player has gone bust
            {
                EndGame();
            }
        }

        /// <summary>
        /// Deal a card to the dealer so long as the dealer's 
        /// total value is less then 17 and isn't a soft seventeen
        /// </summary>
        private void DealDealerCard()
        {
            //If the dealers hand is less and seventeen or 
            //the dealer has a soft seventeen deal the dealer a card
            bool dealerLessThanPlayer = HandValue(dealerFullHand) < HandValue(playerHand);

            if (IsSoftSeventeen(dealerFullHand) && dealerLessThanPlayer || HandValue(dealerFullHand) < 17 && dealerLessThanPlayer)
            {
                Card card = deck.DrawOneCard();
                dealerHand.Add(card);
                dealerFullHand.Add(card);
                System.Windows.Application.Current.Dispatcher.Invoke(delegate
                {
                    lblDealerValue.Content = HandValue(dealerFullHand).ToString();
                });
            }
            else //Dealser stands
            {
                hasDealerStand = true;
            }

            DisplayCards();

        }

        /// <summary>
        /// Update the displayed the cards
        /// </summary>
        private void DisplayCards()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                //Clear currently displayed cards
                pnlPlayerCards.Children.Clear();
                pnlDealerCards.Children.Clear();


                if (playerHand.Count <= 4)
                {
                    //Display all of the playercards
                    foreach (Card card in playerHand)
                    {
                        Image img = GenerateImage(card);
                        pnlPlayerCards.Children.Add(img);
                    }
                }
                else
                {
                    for (int i = 0; i < playerHand.Count; i++)
                    {
                        Image img = GenerateImage(playerHand[i], i != 0);
                        pnlPlayerCards.Children.Add(img);
                    }
                }



                if (dealerFullHand.Count <= 4)
                {
                    //Determines if the hole should be revealed or not
                    if (!isHoleRevealed)
                    {
                        //Display all of the cards except the hole card
                        foreach (Card card in dealerHand)
                        {
                            Image img = GenerateImage(card);

                            pnlDealerCards.Children.Add(img);
                        }

                        Image back = GenerateImage();
                        pnlDealerCards.Children.Add(back);
                    }
                    else
                    {
                        //Displays all the cards including the hole card
                        foreach (Card card in dealerFullHand)
                        {
                            Image img = GenerateImage(card);

                            pnlDealerCards.Children.Add(img);
                        }
                    }
                }
                else
                {
                    //Determines if the hole should be revealed or not
                    if (!isHoleRevealed)
                    {
                        //Display all of the cards except the hole card
                        for (int i = 0; i < dealerHand.Count; i++)
                        {
                            Image img = GenerateImage(dealerHand[i], i != 0);
                            pnlDealerCards.Children.Add(img);
                        }

                        Image back = GenerateImage(true);
                        pnlDealerCards.Children.Add(back);
                    }
                    else
                    {
                        //Displays all the cards including the hole card
                        for (int i = 0; i < dealerFullHand.Count; i++)
                        {
                            Image img = GenerateImage(dealerFullHand[i], i != 0);
                            pnlDealerCards.Children.Add(img);
                        }
                    }
                }

            });

        }

        /// <summary>
        /// Whether dealer is still uncertain or not
        /// </summary>
        /// <returns></returns>
        private bool UnknownWinner() { return !hasDealerStand && !(HandValue(playerHand) > 21) && HandValue(playerHand) != 21; }

        /// <summary>
        /// When the game is over
        /// </summary>
        private void EndGame()
        {
            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                //Disable Hit and Stand Controls
                bttnHit.IsEnabled = false;
                bttnStand.IsEnabled = false;
            });
                        
            RevealHole();
            if (UnknownWinner()) Thread.Sleep(1000);

            //While the winner is uncertain
            //deal new card to dealer
            while (UnknownWinner())
            {
                DealDealerCard();
                if (UnknownWinner()) Thread.Sleep(1000);
            }



            System.Windows.Application.Current.Dispatcher.Invoke(delegate
            {
                //If the player has Blackjack or
                //the player has more than the dealer 
                //or the dealer has gone bust and the 
                //player has not gone bust he wins. 
                //Otherwise the player looses.
                if (HandValue(playerHand) == 21)
                {
                    PlaySound(Sound.Win);
                    MessageBox.Show("Blackjack!", "You win!");
                }
                else if (HandValue(playerHand) > 21)
                {
                    PlaySound(Sound.Lose);
                    MessageBox.Show("You've gone bust.", "You lose!");
                }
                else if (HandValue(dealerFullHand) > 21 && HandValue(playerHand) <= 21)
                {
                    PlaySound(Sound.Win);
                    MessageBox.Show("Dealer goes bust. You Win!", "Congratulations!");
                }
                else if (HandValue(dealerFullHand) == 21)
                {
                    PlaySound(Sound.Lose);
                    MessageBox.Show("Dealer has Blackjack.", "You lose!");
                }
                else if (HandValue(dealerFullHand) == HandValue(playerHand))
                {
                    PlaySound(Sound.Lose);
                    MessageBox.Show("Dealer wins ties.", "You lose!");
                }
                else if (HandValue(dealerFullHand) >= HandValue(playerHand))
                {
                    PlaySound(Sound.Lose);
                    MessageBox.Show("Dealer has higher.", "You lose!");
                }
                else if (HandValue(dealerFullHand) < HandValue(playerHand))
                {
                    PlaySound(Sound.Win);
                    MessageBox.Show("Dealer has lower. You win!", "Congratulations!");
                }

                if (MessageBox.Show("Play again?", "New Game?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    SetupGame();
                else
                {
                    bttnHelp.IsEnabled = true;
                    bttnNewGame.IsEnabled = true;
                }
            });
        }

        
        #endregion

        #region Event Handlers

        private void ProgramStart(object sender, EventArgs e)
        {
            PlaySound(Sound.Win);
            if (MessageBox.Show("Start new game?", "New game?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                SetupGame();
        }

        private void bttnHit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangeButtonState(false);

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, e2) =>
                {
                    DealPlayerCard();
                    if (HandValue(playerHand) == 21) EndGame(); 
                };

                worker.RunWorkerAsync();
                ChangeButtonState(true);
            }
            catch (Exception ex)
            {
                ErrorHandling(ex);
            }

        }

        private void bttnStand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangeButtonState(false);

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, e2) =>
                {
                    EndGame();
                };

                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ErrorHandling(ex);
            }

        }

        private void bttnNewGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ChangeButtonState(false);

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, e2) =>
                {
                    SetupGame();
                };

                worker.RunWorkerAsync();

                ChangeButtonState(true);
            }
            catch (Exception ex)
            {
                ErrorHandling(ex);
            }
        }

        private void bttnHelp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string helpMsg = "The goal of Blackjack is to get as close to 21 without going over. " +
                                    "The player is dealt two cards, and then the dealer deals themselves two " +
                                    "cards, one face down, which is called the hole, and the other face up.\r\n\r\n" +
                                    "The scoring for Blackjack is as follows:\r\n" +
                                    "\u2022 Numbered cards are worth the amount on the card\r\n" +
                                    "\u2022 Kings, Queens and Jacks are worth 10\r\n" +
                                    "\u2022 Aces are worth 11 or 1 if 11 would put the player over 21\r\n\r\n" +
                                    "The player as two options, Hit or Stand. If you choose hit, the dealer deals " +
                                    "you another card. If you choose stand you end you turn and are not allowed any " +
                                    "more cards. After you stand the dealer will deal any additional card to " +
                                    "themselves until beat the player or reach 17, unless it is a Soft Seventeen " +
                                    "in which case the dealer hits again (unless the dealer has already one). " +
                                    "A Soft Seventeen is a seventeen in which one of the cards is an Ace " +
                                    "valued at 11.\r\n\r\n This varient of Blackjack uses three decks and the " +
                                    "dealer wins all ties except when the player has Blackjack.";

                MessageBox.Show(helpMsg, "Help", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorHandling(ex);
            }
        }
        #endregion

    }
}
