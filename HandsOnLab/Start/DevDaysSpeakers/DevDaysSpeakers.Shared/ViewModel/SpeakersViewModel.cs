using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using DevDaysSpeakers.Model;
using DevDaysSpeakers.Services;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace DevDaysSpeakers.ViewModel
{
    public class SpeakersViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<Speaker> Speakers { get; set; }

        public SpeakersViewModel()
        {
            Speakers = new ObservableCollection<Speaker>();

            GetSpeakersCommand = new Command(
                async () => await GetSpeakers(),
                () => !IsBusy);
        }

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            var changed = PropertyChanged;

            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
            set
            {
                isBusy = value;
                OnPropertyChanged();
            }
        }

        private async Task GetSpeakers()
        {
            if (IsBusy)
                return;

            Exception error = null;
            try
            {
                IsBusy = true;

                using (var client = new HttpClient())
                {
                    // Grab json from the server
                    var json = await client.GetStringAsync("http://demo4404797.mockable.io/speakers");

                    var items = JsonConvert.DeserializeObject<List<Speaker>>(json);

                    Speakers.Clear();
                    foreach (var item in items)
                        Speakers.Add(item);
                }

            }
            catch (Exception ex)
            {
                error = ex;

            }
            finally
            {
                IsBusy = false;
            }

            if (error != null)
                await Application.Current.MainPage.DisplayAlert("Error!", error.Message, "OK");

        }

        public Command GetSpeakersCommand { get; set; }
    }
}
