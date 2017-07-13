using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokerHandRecognition
{
    public partial class frmPoker : Form
    {
        //~Global variables
        //the deck containing all cards
        List<Card> deck = NewDeck();
        //indexes in deck will correspond to indexes in cardButtons, so 10 of Clubs card has same index as 10C button
        List<Control> cardButtons = new List<Control>();
        //list of result textboxes:
        List<TextBox> results = new List<TextBox>();
        //list of listboxes containing hands:
        List<ListBox> displayHands = new List<ListBox>();
        //list of card hands corresponding to diplayHands
        List<List<Card>> hands = new List<List<Card>>();
        //tracks current hand being added to:
        int currentHandIndex = 0;

        public frmPoker()
        {
            InitializeComponent();
        }

        private void frmPoker_Load(object sender, EventArgs e)
        {
            //adds reult textboxes to results list
            results.Add(txtHand1Result);
            results.Add(txtHand2Result);
            results.Add(txtHand3Result);
            results.Add(txtHand4Result);
            results.Add(txtHand5Result);
            results.Add(txtHand6Result);
            results.Add(txtHand7Result);
            results.Add(txtHand8Result);

            //adds listboxes to displayHands list
            displayHands.Add(lstHand1);
            displayHands.Add(lstHand2);
            displayHands.Add(lstHand3);
            displayHands.Add(lstHand4);
            displayHands.Add(lstHand5);
            displayHands.Add(lstHand6);
            displayHands.Add(lstHand7);
            displayHands.Add(lstHand8);

            //adds empty hands to hands list
            for (int i = 0; i < 8; i++)
            {
                hands.Add(new List<Card>());
            }

            //below loop adds controls to cardButtons in reverse order of how grpCardButtons orders them (reverse of order of control's addition to form)
            for (int i = 51; i > -1; i--) 
            {
                cardButtons.Add(grpCardButtons.Controls[i]);
            }
            //gives every button a generic click event
            foreach (Control button in cardButtons)
            {
                button.Click += cardButton_Click;
            }
            

        }

        //function for when a card button is clicked
        void cardButton_Click(Object sender, EventArgs e)
        {
            //finds what button the one clicked was
            Button clickedButton = (Button)sender;
            int buttonIndex = cardButtons.IndexOf(clickedButton);
            //adds the button's value to the current hand
            displayHands[currentHandIndex].Items.Add(deck[buttonIndex].name);
            hands[currentHandIndex].Add(deck[buttonIndex]);

            //if the hand is full move on to the next one, disable listbox to show user it's full
            if (displayHands[currentHandIndex].Items.Count==5)
            {
                displayHands[currentHandIndex].Enabled = false;
                currentHandIndex += 1;
            }
            //if the last hand is full, disable all card buttons
            if (currentHandIndex>7)
            {
                foreach (Control button in cardButtons)
                {
                    button.Enabled = false;
                }
            }
            //disable button to prevent double entry
            clickedButton.Enabled = false;
        }


        //function that evaluates hands and determines winner:
        private void btnEvaluate_Click(object sender, EventArgs e)
        {
            List<int[]> handsToEvaluate = new List<int[]>();
            int[] bestValue = new int[6] { 0, 0, 0, 0, 0, 0 };
            List<int> winnerIDs = new List<int>();
            int drawnHands = 0;
            //retrieves valid hands and puts them in handsToEvaluate
            foreach (List<Card> hand in hands)
            {
                if (hand.Count==5)
                {
                    handsToEvaluate.Add(Find5CardRank(hand));
                }
                else
                    break;
            }

            //says what kind of set each hand is
            for (int i = 0; i < handsToEvaluate.Count; i++)
            {
                switch (handsToEvaluate[i][0])
                {
                    case 1:
                        results[i].Text = "High Card";
                        break;
                    case 2:
                        results[i].Text = "Pair";
                        break;
                    case 3:
                        results[i].Text = "Two Pair";
                        break;
                    case 4:
                        results[i].Text = "3 of a Kind";
                        break;
                    case 5:
                        results[i].Text = "Straight";
                        break;
                    case 6:
                        results[i].Text = "Flush";
                        break;
                    case 7:
                        results[i].Text = "Full House";
                        break;
                    case 8:
                        results[i].Text = "4 of a Kind";
                        break;
                    case 9:
                        results[i].Text = "Straight Flush";
                        break;
                    default:
                        break;
                }
            }

            //Determines winning hand
            foreach (int[] cardRank in handsToEvaluate)
            {
                if (cardRank[0] > bestValue[0])
                    bestValue = cardRank;
                else if (cardRank[0] == bestValue[0])
                {
                    if (cardRank[1] > bestValue[1])
                        bestValue = cardRank;
                    else if (cardRank[1] == bestValue[1])
                    {
                        if (cardRank[2] > bestValue[2])
                            bestValue = cardRank;
                        else if (cardRank[2] == bestValue[2])
                        {
                            if (cardRank[3] > bestValue[3])
                                bestValue = cardRank;
                            else if (cardRank[3] == bestValue[3])
                            {
                                if (cardRank[4] > bestValue[4])
                                    bestValue = cardRank;
                                else if (cardRank[4] == bestValue[4])
                                {
                                    if (cardRank[5] > bestValue[5])
                                        bestValue = cardRank;
                                }
                            }
                        }
                    }
                }
            }

            //Determines if there was a tie and gets winners
            for (int i = 0; i < handsToEvaluate.Count; i++)
            {
                //compares values of array instead of object equivalence
                if (Enumerable.SequenceEqual(handsToEvaluate[i], bestValue))
                {
                    drawnHands += 1;
                    winnerIDs.Add(i + 1);
                }
                
            }
            //if there was no tie
            if (drawnHands<=1)
            {
                txtResultMessage.Text = string.Format("Hand {0} wins", winnerIDs[0]);
            }
            else 
            {
                txtResultMessage.Text = "Hands ";
                for (int i = 0; i < winnerIDs.Count-1; i++)
                {
                    txtResultMessage.Text += winnerIDs[i].ToString()+ ", ";
                }
                txtResultMessage.Text += winnerIDs[winnerIDs.Count - 1];
                txtResultMessage.Text += " draw";
            }
        }



        //function that creates a list to serve as the deck
        static List<Card> NewDeck() 
        {
            List<Card> deck = new List<Card>();
            for (int suit = 1; suit < 5; suit++)
            {
                for (int cardNumber = 1; cardNumber < 14; cardNumber++)
                {
                    deck.Add(new Card(cardNumber, suit));
                }
            }
            return deck;
        }


        /* Finds the value of the hand when added to the cards on the table at the final stage
         * of play: the turn. Values are as follows, from weakest to strongest:
         * High Card - 1
         * Pair - 2
         * Two Pair - 3
         * Three of a Kind - 4
         * Straight - 5
         * Flush - 6
         * Full House - 7
         * Four of a Kind - 8
         * Straight Flush - 9
         * further values in the array are for the events where there is a tie in hands
        */
        static int[] Find5CardRank(List<Card> hand)
        {
            int[] rankHighCard = new int[6] { 0, 0, 0, 0, 0, 0 }; //needs 5 values as high card hands can give 4 ties, last value is in case of ultimate tie, always 0
            bool foundValue = false;
            //below code checks for straight flush (highly improbable <0.01%)
            bool straightFlush = false;
            hand = hand.OrderBy(o => o.cardNum).ToList(); //sorts allCards in ascending order of cardValue
            int streak = 0;
            int previousSuit = 0;
            int previousNum = 69;
            foreach (Card card in hand)
            {
                if (card.cardNum == previousNum + 1 && card.suitNum == previousSuit)
                    streak += 1;
                else
                    streak = 0;
                previousNum = card.cardNum;
                previousSuit = card.suitNum;
                if (streak >= 4)
                {
                    straightFlush = true;
                    break;
                }
            }
            //loops through again to check if last cards can form straight with first cards
            if (!straightFlush)
            {
                previousNum = previousNum - 13;
                foreach (Card card in hand)
                {
                    if (card.cardNum == previousNum + 1 && card.suitNum == previousSuit)
                        streak += 1;
                    else
                        streak = 0;
                    previousNum = card.cardNum;
                    previousSuit = card.suitNum;
                    if (streak >= 4)
                    {
                        straightFlush = true;
                        break;
                    }
                }
            }
            //if the hand is a straight flush, return corresponding value and the highest ranking card as highcard
            if (straightFlush)
            {
                hand = hand.OrderBy(o => o.cardValue).ToList(); //sorts by value in case of ace being last value
                rankHighCard[0] = 9;
                rankHighCard[1] = hand[4].cardValue; //last card should have highest value as sorted numerically
                rankHighCard[2] = hand[3].cardValue; //if second highest value card is identical, so is the rest, no further comparison
                foundValue = true;
            }

            if (!foundValue) //checks for flush
            {
                bool sameSuit = false;
                int sameSuitCount = 0;
                for (int suit = 1; suit < 5; suit++)
                {
                    sameSuitCount = 0;
                    foreach (Card card in hand)
                    {
                        if (card.suitNum == suit)
                            sameSuitCount += 1;
                    }
                    if (sameSuitCount >= 5)
                    {
                        sameSuit = true;
                    }
                }
                if (sameSuit) //if flush
                {
                    hand = hand.OrderBy(o => o.cardValue).ToList(); //sorts by value in case of ace being last value
                    rankHighCard[0] = 6;
                    rankHighCard[1] = hand[4].cardValue; //last card should have highest value due to numerical sorting
                    rankHighCard[2] = hand[3].cardValue;
                    rankHighCard[3] = hand[2].cardValue;
                    rankHighCard[4] = hand[1].cardValue;
                    rankHighCard[5] = hand[0].cardValue;
                    foundValue = true;
                }
            }


            if (!foundValue) //checks for straight
            {
                bool straight = false;
                streak = 0;
                previousNum = 69;
                foreach (Card card in hand)
                {
                    if (card.cardNum == previousNum + 1)
                        streak += 1;
                    else
                        streak = 0;
                    previousNum = card.cardNum;
                    if (streak >= 4)
                    {
                        straight = true;
                        break;
                    }
                }
                if (!straight)
                {
                    previousNum = previousNum - 13;
                    foreach (Card card in hand)
                    {
                        if (card.cardNum == previousNum + 1)
                            streak += 1;
                        else
                            streak = 0;
                        previousNum = card.cardNum;
                        if (streak >= 4)
                        {
                            straight = true;
                            break;
                        }
                    }
                }
                if (straight) //straight
                {
                    hand = hand.OrderBy(o => o.cardValue).ToList(); //sorts by value in case of ace being highcard
                    rankHighCard[0] = 5;
                    rankHighCard[1] = hand[4].cardValue; //last card will have highest value due to numerical sorting
                    rankHighCard[2] = hand[3].cardValue; //no further comparison needed beyond second card
                    foundValue = true;
                }
            }

            //checks for values that aren't straights/flushes/straight flushes
            if (!foundValue)
            {
                hand = hand.OrderBy(o => o.cardValue).ToList(); //sorts by value in case of ace, numerical order isn't important for these hands
                List<Card> checkedNums = new List<Card>();
                bool isCheckedNum = false;
                int highestRepeat = 0;
                int repeat = 0;
                int currentNum = 0;
                int pairs = 0;
                foreach (Card card in hand)
                {
                    isCheckedNum = false;             //ensures there is no double count of pairs
                    foreach (Card checkingCard in checkedNums)
                    {
                        if (card.cardNum == checkingCard.cardNum)
                            isCheckedNum = true;
                    }
                    if (!isCheckedNum)
                    {
                        repeat = 0;
                        currentNum = card.cardNum;
                        foreach (Card card2 in hand)
                        {
                            if (currentNum == card2.cardNum)
                                repeat += 1;
                        }
                        if (repeat == 2)
                            pairs += 1;
                        if (repeat > highestRepeat)
                            highestRepeat = repeat;
                        checkedNums.Add(card);
                    }
                }

                if (highestRepeat == 4) //4 of a kind
                {
                    rankHighCard[0] = 8;
                    rankHighCard[1] = hand[2].cardValue; //third card guaranteed to be in the 4 of a kind due to sorting, cannot be remaining card as remaining card will be first or last
                    foreach (Card card in hand)
                    {
                        if (!(card.cardValue == hand[2].cardValue)) //remaining card will be found by checking if it is not equal to the 4 of a kind value
                        {
                            rankHighCard[2] = card.cardValue;
                            break;
                        }
                    }
                }

                else if (highestRepeat == 3 && pairs == 1) //full house
                {
                    rankHighCard[0] = 7;
                    rankHighCard[1] = hand[2].cardValue; //3rd card guarenteed to be in three of a kind due to sorting, cannot be remaining cards as they will be the first two or last two
                    foreach (Card card in hand)
                    {
                        if (!(card.cardValue == hand[2].cardValue)) //remaining card will be found by checking if it is not equal to the 3 of a kind value
                        {
                            rankHighCard[2] = card.cardValue;
                            break;
                        }
                    }
                }
                else if (highestRepeat == 3) //three of a kind
                {
                    rankHighCard[0] = 4;
                    rankHighCard[1] = hand[2].cardValue; //3rd card guarenteed to be in three of a kind due to sorting, cannot be remaining cards as they will never occupy the middle
                    List<Card> notTriple = new List<Card>(hand);
                    foreach (Card card in hand) //removes three of a kind from new list, leaving two cards and allowing for easy comparison as first card will be least valued, last most valued
                    {
                        if (card.cardValue == hand[2].cardValue)
                            notTriple.Remove(card);
                    }
                    rankHighCard[2] = notTriple[1].cardValue;
                    rankHighCard[3] = notTriple[0].cardValue;

                }

                else if (pairs == 2) //two pair
                {
                    rankHighCard[0] = 3;
                    int cardCount = 0;
                    int pairValue1 = 0;
                    int pairValue2 = 0;
                    foreach (Card card in hand) //finds the first pair and assigns pairValue1 their value
                    {
                        foreach (Card card2 in hand)
                        {
                            if (card.cardValue == card2.cardValue)
                                cardCount += 1;
                        }
                        if (cardCount == 2)
                        {
                            pairValue1 = card.cardValue;
                            break;
                        }
                        cardCount = 0;
                    }
                    List<Card> notTwoPair = new List<Card>(hand); //removes the first pair from notTwoPair
                    foreach (Card card in hand)
                    {
                        if (card.cardNum == pairValue1)
                            notTwoPair.Remove(card);
                    }
                    cardCount = 0;
                    foreach (Card card in notTwoPair) //finds the second pair and assigns pairValue2 their value
                    {
                        foreach (Card card2 in notTwoPair)
                        {
                            if (card.cardValue == card2.cardValue)
                                cardCount += 1;
                        }
                        if (cardCount == 2)
                        {
                            pairValue2 = card.cardValue;
                            break;
                        }
                        cardCount = 0;
                    }
                    //pair 1 should be less than pair 2 due to sorting, pair 1 would be found first and have the lower value
                    rankHighCard[1] = pairValue2;
                    rankHighCard[2] = pairValue1;
                    foreach (Card card in hand) //removes the second pair from notTwoPair, finding the last card and its value
                    {
                        if (card.cardNum == pairValue2)
                            notTwoPair.Remove(card);
                    }
                    rankHighCard[3] = notTwoPair[0].cardValue;

                }

                else if (pairs == 1) //pair
                {
                    rankHighCard[0] = 2;
                    int pairValue = 0;
                    int cardCount = 0;
                    foreach (Card card in hand) //finds the pair and assigns pairValue their value
                    {
                        foreach (Card card2 in hand)
                        {
                            if (card.cardValue == card2.cardValue)
                                cardCount += 1;
                        }
                        if (cardCount == 2)
                        {
                            pairValue = card.cardValue;
                            break;
                        }
                        cardCount = 0;
                    }
                    rankHighCard[1] = pairValue;
                    List<Card> notPair = new List<Card>(hand);
                    foreach (Card card in hand) //removes pair from notPair, leaving three cards and allowing for easy assignment due to sorting
                    {
                        if (card.cardValue == pairValue)
                            notPair.Remove(card);
                    }
                    rankHighCard[2] = notPair[2].cardValue;
                    rankHighCard[3] = notPair[1].cardValue;
                    rankHighCard[4] = notPair[0].cardValue;

                }

                else //high card
                {
                    rankHighCard[0] = 1;
                    //as values are sorted, first value will be lowest, last value highest
                    rankHighCard[1] = hand[4].cardValue;
                    rankHighCard[2] = hand[3].cardValue;
                    rankHighCard[3] = hand[2].cardValue;
                    rankHighCard[4] = hand[1].cardValue;
                    rankHighCard[5] = hand[0].cardValue;
                }
            }


            return rankHighCard;
        }

        
    }

    //an object that contains properties of cards, the suit, number and value
    class Card 
    {
        public int suitNum { get; set; }
        public int cardNum { get; set; }
        public int cardValue { get; set; }
        public string name { get; set; }
        //auto generates details based on inputted parameters, see NewDeck() for process
        public Card(int number, int suit)
        {
            name = "";
            suitNum = suit;
            cardNum = number;
            if (cardNum == 1) //sets aces to be worth more than kings
                cardValue = 14;
            else
                cardValue = cardNum;

            if (cardNum == 1)
                name += "A";
            else if (cardNum == 11)
                name += "J";
            else if (cardNum == 12)
                name += "Q";
            else if (cardNum == 13)
                name += "K";
            else
                name = cardNum.ToString();

            if (suitNum == 1)
                name += "♦";
            else if (suitNum == 2)
                name += "♥";
            else if (suitNum == 3)
                name += "♣";
            else if (suitNum == 4)
                name += "♠";
        }
    }
}
