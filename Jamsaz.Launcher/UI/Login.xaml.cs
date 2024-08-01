using System;
using System.Linq;
using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Jamsaz.Common.UserAuthenticationManager;
using Jamsaz.Launcher.BusinessObject.Data;
using UACServiceLibrary;

namespace Jamsaz.Launcher.UI
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        #region Properties

        /// <summary>
        /// Should be provided by caller
        /// </summary>
        public string DomainName { get; set; }

        /// <summary>
        /// Should be provided by caller
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Provide selected fiscalyearID for the caller.
        /// </summary>
        public int SelectedFiscalYearID { get; set; }

        /// <summary>
        /// Should be provided by caller
        /// </summary>
        public bool IsShowWarehouse = false;

        /// <summary>
        /// Provide selected WarehouseID for the caller.
        /// </summary>
        public int SelectedWarehouseID { get; set; }

        /// <summary>
        /// Provide AuthenticationManager for the caller.
        /// </summary>
        public Jamsaz.Common.UserAuthenticationManager.AuthenticationManager AuthenticationManager { get; set; }

        /// <summary>
        /// Local Use.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Local Use.
        /// </summary>
        private string stackTrace;

        /// <summary>
        /// Provide UACServiceLibrary.Credential for the caller.
        /// </summary>
        public UACServiceLibrary.Credential Credential { get; set; }

        public DirectoryServiceAuthenticatedUser DirectoryServiceAuthenticatedUser { get; set; }

        public Window Parent { get; set; }

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            this.Left = this.Parent.Left + (this.Parent.Width / 2) - (this.Width / 2) + 50;

            this.Top = this.Parent.Top + this.Parent.Height / 2 - this.Height / 2 - 50;

            this.ConnectionString = "Data Source=atlas;Initial Catalog=JamsazERPLite;Integrated Security=True";

            this.DomainName = "jamsaz.org";

            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());

            string userName = windowsPrincipal.Identity.Name.Replace(@"JAMSAZ\", string.Empty).Trim();

            this.userNametextBox.Text = userName.Replace(@"JSIP\", string.Empty).Trim();

            JamsazERPLiteDataContext db = new JamsazERPLiteDataContext();

            CollectionViewSource fiscalYearSource = (CollectionViewSource)this.FindResource("fiscalYearViewSource");

            fiscalYearSource.Source = from c in db.FiscalYears orderby c.ID descending select c;
            
            fiscalyearComboBox.SelectedValue = db.FiscalYears.OrderByDescending(c => c.ID).First(c => c.Status == 2).ID;

            System.Windows.Data.CollectionViewSource fiscalYearViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("fiscalYearViewSource")));
        }

        private void cancelButon_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                this.ConnectionString = this.ConnectionString.Replace("JamsazERPLite", "JamsazERPLiteEX");
            }

            this.Authenticate();
        }

        private void Authenticate()
        {
            this.SelectedFiscalYearID = (int)this.fiscalyearComboBox.SelectedValue;

            this.AuthenticationManager = new AuthenticationManager(this.DomainName, this.ConnectionString, new object());

            try
            {
                bool returnValue = this.AuthenticationManager.AuthenticateUser1(this.userNametextBox.Text.Trim(), this.passwordTextbox.Password.Trim());

                if (!returnValue)
                {
                    this.messageLabel.Visibility = System.Windows.Visibility.Visible;

                    this.messageLabel.Content = string.Format("{0} : [{1}]", "نام کاربری یا رمز عبور صحیح نمی باشد", "1");

                    this.passwordTextbox.SelectAll();

                    this.passwordTextbox.Focus();

                    return;
                }
                if (this.AuthenticationManager.ApproachabilityApplications.Count == 0)
                {
                    this.messageLabel.Visibility = System.Windows.Visibility.Visible;

                    this.messageLabel.Content = string.Format("{0} : [{1}]", "شما مجاز به وارد شدن به هیچ کدام از برنامه ها نمی باشید", "2");
                }

                this.Credential = new Credential() { UserName = userNametextBox.Text, Password = passwordTextbox.Password, FiscalyearID = SelectedFiscalYearID };

                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                //this.erroeLinkLabel.Visible = true;
                errorMessage = ex.Message;
                stackTrace = ex.StackTrace;
            }
        }

        private void passwordTextbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.messageLabel.Content = string.Empty;

            this.messageLabel.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.okButton_Click(null, null);
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            //if (e.OriginalSource is TextBlock)
            //{
            //    if (!(((e.OriginalSource as TextBlock).Parent as DockPanel).Parent is Button))

            //        this.DragMove();

            //}
            //else if (e.OriginalSource is Image)
            //{
            //    if (!(((e.OriginalSource as Image).Parent as DockPanel).Parent is Button))

            //        this.DragMove();
            //}

            //else
            //    this.DragMove();

        }
    }
}
