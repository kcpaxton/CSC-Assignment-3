using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Paxton3
{
	public partial class MainPage : ContentPage
	{
        const int errorDuration = 500;
        double[] frequencies = { 410.21, 523.25, 622.25, 739.99, 880 };
        

        const int sequenceTime = 750;
        protected const int flashDuration = 250;

        const double offLuminosity = 0.4;
        const double onLuminosity = 0.75;

        BoxView[] boxViews;
        Color[] colors = { Color.Fuchsia, Color.OrangeRed, Color.ForestGreen, Color.DodgerBlue, Color.Yellow };
        List<int> sequence = new List<int>();
        int sequenceIndex;
        bool awaitingTaps;
        bool gameEnded;
        Random random = new Random();
		public MainPage()
		{
			InitializeComponent();
            boxViews = new BoxView[] { boxview0, boxview1, boxview2, boxview3, boxview4 };
            InitializeBoxViewColors();
            
		}

        private void InitializeBoxViewColors()
        {
            for(int index = 0; index < 5; index++)
            {
                boxViews[index].Color = colors[index].WithLuminosity(offLuminosity);
            }
            
        }

        protected void OnStartGameButtonClicked(object sender, EventArgs args)
        {
            gameEnded = false;
            startGameButton.IsVisible = false;
            randomizeArray();
            InitializeBoxViewColors();
            sequence.Clear();
            StartSequence();
        }

        private void StartSequence()
        {
            sequence.Add(random.Next(5));
            sequenceIndex = 0;
            Device.StartTimer(TimeSpan.FromMilliseconds(sequenceTime), OnTimerTick);
        }

        bool OnTimerTick()
        {
            if (gameEnded)
            {
                return false;
            }
            FlashBoxView(sequence[sequenceIndex]);
            sequenceIndex++;
            awaitingTaps = sequenceIndex == sequence.Count;
            sequenceIndex = awaitingTaps ? 0 : sequenceIndex;
            return !awaitingTaps;
        }

        protected void FlashBoxView(int index)
        {
            SoundPlayer.PlaySound(frequencies[index], flashDuration);
            boxViews[index].Color = colors[index].WithLuminosity(onLuminosity);
            Device.StartTimer(TimeSpan.FromMilliseconds(flashDuration), () =>
            {
                if (gameEnded)
                {
                    return false;
                }
                boxViews[index].Color = colors[index].WithLuminosity(offLuminosity);
                return false;
            });
        }

        protected void OnBoxViewTapped(object sender, EventArgs args)
        {
            if (gameEnded)
            {
                return;
            }
            if (!awaitingTaps)
            {
                EndGame();
                return;
            }

            BoxView tappedBoxView = (BoxView)sender;
            int index = Array.IndexOf(boxViews, tappedBoxView);
            if(index != sequence[sequenceIndex])
            {
                EndGame();
                return;
            }

            FlashBoxView(index);

            sequenceIndex++;
            awaitingTaps = sequenceIndex < sequence.Count;

            if (!awaitingTaps)
            {
                StartSequence();
            }
        }

        protected virtual void EndGame()
        {
            SoundPlayer.PlaySound(65.4, errorDuration);
            gameEnded = true;
            for(int index = 0; index < 5; index++)
            {
                boxViews[index].Color = Color.Gray;
            }
            startGameButton.Text = "TryAgain?";
            startGameButton.IsVisible = true;

        }
         
        private void randomizeArray()
        {
            colors = colors.OrderBy(x => random.Next()).ToArray();
        }
    }
}
