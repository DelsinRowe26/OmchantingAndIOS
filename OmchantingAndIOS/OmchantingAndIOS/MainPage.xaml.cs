using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Plugin.AudioRecorder;
using static Xamarin.Essentials.Permissions;

namespace OmchantingAndIOS
{
    public partial class MainPage : ContentPage
    {
        int i = 0;
        AudioRecorderService recorder;
        AudioPlayer player;
        Microphone microphone;

        public MainPage()
        {
            InitializeComponent();
            
            microphone.CheckStatusAsync();
            recorder = new AudioRecorderService
            {
                StopRecordingAfterTimeout = true,
                TotalAudioTimeout = TimeSpan.FromSeconds(15),
                AudioSilenceTimeout = TimeSpan.FromSeconds(2)
            };

            player = new AudioPlayer();
            player.FinishedPlaying += Player_FinishedPlaying;
        }

        public async void ClickStart(object sender, EventArgs e)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            await RecordAudio();
            
            i++;
            lbCount.Text = i.ToString();
        }

        public void ClickPlay(object sender, EventArgs e) 
        {
            PlayAudio();
        }

        public void PlayAudio()
        {
            try
            {
                var filePath = recorder.GetAudioFilePath();

                if (filePath != null)
                {
                    btnPlay.IsEnabled = false;
                    btnStart.IsEnabled = false;

                    player.Play(filePath);
                }
            }
            catch (Exception ex)
            {
                //blow up the app!
                throw ex;
            }
        }

        async Task RecordAudio()
        {
            try
            {
                if (!recorder.IsRecording)
                {
                    recorder.StopRecordingOnSilence = TimeoutSwitch.IsToggled;

                    btnStart.IsEnabled = false;
                    btnPlay.IsEnabled = false;

                    var audioRecordTask = await recorder.StartRecording();

                    btnStart.Text = "Stop Recording";
                    btnStart.IsEnabled = true;

                    await audioRecordTask;

                    btnStart.Text = "Record";
                    btnPlay.IsEnabled = true;
                }
                else //Stop button clicked
                {
                    btnStart.IsEnabled = false;

                    //stop the recording...
                    await recorder.StopRecording();

                    btnStart.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                //blow up the app!
                throw ex;
            }
        }

        void Player_FinishedPlaying(object sender, EventArgs e)
        {
            btnPlay.IsEnabled = true;
            btnStart.IsEnabled = true;
        }
    }
}
