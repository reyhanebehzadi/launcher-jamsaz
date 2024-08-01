using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Jamsaz.Launcher.UI;
using Jamsaz.Launcher.Classes;
using Jamsaz.Launcher.BusinessObject.Data;
using Jamsaz.Common.UserAuthenticationManager;
using Jamsaz.Common;
using System.ServiceModel;
using UACServiceLibrary;
using Jamsaz.Common;

namespace Jamsaz.Launcher
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

        private JamsazERPLiteDataContext db;

        private UACServiceLibrary.CredentialProviderService credentialProviderServiceHost;

        //private ServiceHost serviceHost;

        private AuthenticationManager AuthenticationManager;

        private System.Timers.Timer timer = new System.Timers.Timer(1000);

        private Microsoft.Win32.RegistryKey Key;

        private Credential Credential;

        [DllImport("gdi32.dll")]
        static extern int AddFontResource(string filename);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

                timer.Enabled = true;

                this.LoadApplications();

                this.timeLabel.Content = FarsiLibrary.Utils.PersianDate.Now.ToWritten();

                this.IsEnabled = false;

                Login login = new Login();

                login.Parent = this;

                if (login.ShowDialog() == true)
                {

                    this.Credential = login.Credential;

                    this.useNameLabel.Content = "";

                    this.AuthenticationManager =
                        login.AuthenticationManager;

                    this.useNameCaption.Visibility = this.useNameLabel.Visibility =
                                                     this.departmentCaption.Visibility =
                                                     this.departmentLabel.Visibility =
                                                     this.fiscalyearCaption.Visibility =
                                                     this.fiscalyearLabel.Visibility =
                                                     System.Windows.Visibility.Visible;

                    this.useNameLabel.Content = this.AuthenticationManager.directoryServiceAuthenticatedUser.FullName;

                    this.departmentLabel.Content =
                        this.AuthenticationManager.directoryServiceAuthenticatedUser.Department;

                    this.fiscalyearLabel.Content = db.FiscalYears.Single(c => c.ID == login.SelectedFiscalYearID).Title;

                    this.UpdateLayout();

                    //ReserveHttpNamespace.ModifyReservation("http://+:8080/CredentialProviderService", "jamsaz", "alimardani", false);

                    try
                    {

                        if (AppHost.serviceHost != null && AppHost.serviceHost.State == CommunicationState.Opened)
                            AppHost.serviceHost.Close();


                        credentialProviderServiceHost = new CredentialProviderService();

                        AppHost.serviceHost = new ServiceHost(credentialProviderServiceHost);

                        AppHost.serviceHost.Open();

                        credentialProviderServiceHost.AddCredential(this.Credential);

                    }
                    catch { }

                    this.ArrangeApproachabilityApplications();

                    this.AddFonts();
                }
                else
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void AddFonts()
        {
            try
            {
                foreach (var font in System.IO.Directory.GetFiles(@"\\atlas\Fonts\"))
                {
                    AddFontResource(font);
                }
            }
            catch (Exeption ex)
            {
                MessageBox.Show(string.Format("{0}{1}{2}", "فونت سیستم را نصب کنید", "\n", ex.Message));
            }
        }

        private void ArrangeApproachabilityApplications()
        {
            try
            {


                string value =
                    this.AuthenticationManager.ApproachabilityApplications.ToCommaSepratedString("ID");

                Key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("ApproachedApplications");

                if (Key != null)
                {
                    Key.SetValue("AccessIDS", value.Encrypt());

                    Key.Close();
                }

                this.IsEnabled = true;

                foreach (var child in this.appsGrid.Children)
                {
                    if (child.GetType() == typeof(StackPanel))
                    {
                        BusinessApplication businessApplication = ((child as StackPanel).Tag as BusinessApplication);

                        if (
                            this.AuthenticationManager.ApproachabilityApplications.Exists(
                                c => c.ID == businessApplication.ID))
                        {
                            (child as StackPanel).IsEnabled = true;

                            (child as StackPanel).Cursor = Cursors.Hand;
                        }
                        else
                        {

                            Image image = new Image() { Source = new BitmapImage(new Uri("/Images/void.png", UriKind.Relative)) };

                            image.Width = 16;

                            image.Height = 16;

                            image.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

                            image.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                            (child as StackPanel).Children.Add(image);

                            (child as StackPanel).Children[0].Opacity = 0.25;

                            //GradientStop stop1 = new GradientStop() { Color = System.Windows.Media.Color.FromRgb(0, 0, 0), Offset = 1 };

                            //GradientStop stop2 = new GradientStop() { Color = System.Windows.Media.Color.FromRgb(32, 0, 0), Offset = 0.8 };

                            //GradientStop stop3 = new GradientStop() { Color = System.Windows.Media.Color.FromRgb(255, 0, 0), Offset = 0 };

                            //(child as StackPanel).Children[0].OpacityMask = new RadialGradientBrush() { Center = new Point(0.5, 0.5), GradientStops = new GradientStopCollection() { stop1, stop2, stop3 } };


                        }

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "ArrangeApproachabilityApplications");
            }

        }

        private void LoadApplications()
        {
            try
            {
                Key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("ApproachedApplications");

                string approchedApps = string.Empty;

                if (Key != null)
                {
                    if (Key.GetValue("AccessIDS") != null)
                        approchedApps = Key.GetValue("AccessIDS").ToString().Decrypt();

                    Key.Close();
                }

                db = new JamsazERPLiteDataContext();

                var apps = db.SelectBussinessApplications(approchedApps).ToList();

                int column = 0;

                int row = 0;



                foreach (var businessApplication in apps)
                {

                    StackPanel stackPanel = new StackPanel();

                    stackPanel.Tag = businessApplication;

                    stackPanel.Width = 80;

                    stackPanel.Height = 80;

                    stackPanel.MouseLeftButtonDown += new MouseButtonEventHandler(App_Click);

                    appsGrid.Children.Add(stackPanel);

                    Grid.SetColumn(stackPanel, column);

                    Grid.SetRow(stackPanel, row);


                    if (businessApplication.Icon != null)
                    {
                        BitmapImage bitmapImage = new BitmapImage();

                        bitmapImage.BeginInit();

                        bitmapImage.StreamSource = new System.IO.MemoryStream(businessApplication.Icon.ToArray());

                        bitmapImage.EndInit();

                        Image image = new Image() { Source = bitmapImage, Tag = businessApplication };

                        image.Width = 52;

                        image.Height = 52;

                        image.ToolTip = businessApplication.Title;

                        image.IsEnabled = false;

                        stackPanel.Children.Add(image);
                    }
                    else
                    {
                        BitmapImage bitmapImage = new BitmapImage();

                        bitmapImage = new BitmapImage(new Uri("/Images/Unknown.png", UriKind.Relative));

                        Image image = new Image() { Source = bitmapImage, Tag = businessApplication };

                        image.Width = 52;

                        image.Height = 52;

                        image.ToolTip = businessApplication.Title;

                        image.IsEnabled = false;

                        stackPanel.Children.Add(image);
                    }


                    TextBlock textBlock = new TextBlock();

                    textBlock.Text = businessApplication.Title;

                    textBlock.TextAlignment = TextAlignment.Center;

                    stackPanel.Children.Add(textBlock);

                    stackPanel.IsEnabled = false;

                    if (column < appsGrid.ColumnDefinitions.Count() - 1)
                        column++;

                    else
                    {
                        column = 0;

                        row++;
                    }

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "LoadApplications");
            }

        }

        private void App_Click(object sender, MouseButtonEventArgs e)
        {
            var app = ((sender as StackPanel).Tag as BusinessApplication);

            if ((bool)app.IsWebbase)
            {
                System.Diagnostics.Process.Start("iexplore", string.Format("{0}?UN={1}&PW={2}", app.InstallationPath, this.Credential.UserName.Encrypt(), this.Credential.Password.Encrypt()));

                return;
            }


            string appPath = string.Format("{0}{1}", app.InstallationPath, "Setup.exe");

            try
            {
                //System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();

                //if (!processes.Any(c => c.ProcessName.Contains(app.Name)))

                System.Diagnostics.Process.Start(appPath);
            }
            catch (Exception ex)
            {
                if (ex.Message == "The system cannot find the file specified")
                    Helper.ShowMessage("مسیر مورد نظر یافت نشد");

                else if (ex.Message == "Access is denied")
                    Helper.ShowMessage("شما به نرم افزار مورد نظر دسترسی ندارید");

                else
                    Helper.ShowMessage(ex.Message);
            }

        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = Cursors.Hand;

            this.DragMove();
        }

        private void clockGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, (System.Action)(() =>
            {
                secondHand.Angle = DateTime.Now.Second * -6;
                minuteHand.Angle = DateTime.Now.Minute * -6;
                hourHand.Angle = (DateTime.Now.Hour * -30) + (DateTime.Now.Minute * -0.5);
            }));
        }

        private void minimize(object sender, MouseButtonEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void close(object sender, MouseButtonEventArgs e)
        {
            if (Helper.Confirm("آیا مایل به بستن برنامه هستید؟"))
                this.Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                AppHost.serviceHost.Close();

                this.useNameCaption.Visibility = this.useNameLabel.Visibility =
                                                 this.departmentCaption.Visibility = this.departmentLabel.Visibility =
                                                                                     this.fiscalyearCaption.Visibility =
                                                                                     this.fiscalyearLabel.Visibility =
                                                                                     System.Windows.Visibility.Collapsed;

                appsGrid.Children.RemoveRange(0, appsGrid.Children.Count);

                this.LoadApplications();

                Login login = new Login();

                login.Parent = this;

                if (login.ShowDialog() == true)
                {
                    this.useNameLabel.Content = "";

                    this.AuthenticationManager =
                        login.AuthenticationManager;

                    this.useNameCaption.Visibility = this.useNameLabel.Visibility =
                                                     this.departmentCaption.Visibility =
                                                     this.departmentLabel.Visibility =
                                                     this.fiscalyearCaption.Visibility =
                                                     this.fiscalyearLabel.Visibility =
                                                     System.Windows.Visibility.Visible;

                    this.useNameLabel.Content = this.AuthenticationManager.directoryServiceAuthenticatedUser.FullName;

                    this.departmentLabel.Content =
                        this.AuthenticationManager.directoryServiceAuthenticatedUser.Department;

                    this.fiscalyearLabel.Content = db.FiscalYears.Single(c => c.ID == login.SelectedFiscalYearID).Title;

                    this.UpdateLayout();

                    if (AppHost.serviceHost != null && AppHost.serviceHost.State == CommunicationState.Opened)
                        AppHost.serviceHost.Close();

                    credentialProviderServiceHost = new CredentialProviderService();

                    AppHost.serviceHost = new ServiceHost(credentialProviderServiceHost);

                    AppHost.serviceHost.Open();

                    credentialProviderServiceHost.AddCredential(login.Credential);

                    this.ArrangeApproachabilityApplications();
                }
                else
                {
                    this.Close();
                }
            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message + "Image_MouseLeftButtonDown");
            }
        }

        private void main_Closed(object sender, EventArgs e)
        {
            if (AppHost.serviceHost != null)
                AppHost.serviceHost.Close();
        }
    }
}
