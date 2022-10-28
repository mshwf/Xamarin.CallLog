using App1.Interfaces;
using System;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace App1
{
    public partial class MainPage : ContentPage
    {
        private const string APP_NAME = "App Log";
        public MainPage()
        {
            InitializeComponent();

            LoadCallLog();

            // Attach Events
            CallLogList.Refreshing += CallLogList_Refreshing;
        }

        public async void LoadCallLog()
        {
            activity_indicator.IsRunning = true;
            activity_indicator.IsVisible = true;

            try
            {
                var statusContact = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
                var statusPhone = await Permissions.CheckStatusAsync<Permissions.Phone>();
                
                if (statusContact != PermissionStatus.Granted)
                    statusContact = await Permissions.RequestAsync<Permissions.ContactsRead>();
                
                if (statusPhone != PermissionStatus.Granted)
                    statusPhone = await Permissions.RequestAsync<Permissions.Phone>();

                if (statusContact == PermissionStatus.Granted && statusPhone == PermissionStatus.Granted)
                {
                    CallLogList.IsRefreshing = true;

                    var Logg = DependencyService.Get<ICallLog>().GetCallLogs();

                    CallLogList.IsRefreshing = false;
                    CallLogList.ItemsSource = Logg;
                }
                else if (statusContact != PermissionStatus.Unknown || statusPhone == PermissionStatus.Unknown)
                {
                    await DisplayAlert(APP_NAME, "El permiso fue negado. No podemos continuar, intente nuevamente.", "OK");
                }
            }
            catch (Exception es)
            {
                activity_indicator.IsRunning = false;
                activity_indicator.IsVisible = false;

                await DisplayAlert("Call Log", "Ha ocurrido un problema, comuniquese con el soporte al cliente. Reporte Técnico: " + es.Message, "OK");
            }
            finally
            {
                activity_indicator.IsRunning = false;
                activity_indicator.IsVisible = false;
            }
        }

        private void CallLogList_Refreshing(object sender, EventArgs e)
        {
            LoadCallLog();
        }
    }
}
