using Renci.SshNet;
using System;
using System.Windows;
using System.Windows.Threading;

namespace TempReaderWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            InitializeComponent();
            // Set up timer on init to have it ready when needed
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 15, 0);
        }

        /// <summary>
        /// Connects to a Pi and gets the current temp
        /// </summary>
        /// <param name="ipAddress">IP Address of the pi</param>
        /// <returns>True if connection was successful false if it failed</returns>
        public Boolean updateTemp(String ipAddress)
        {
            String temp = "";
            // TODO: replace PASSWORD with the password for your pi
            SshClient pi = new SshClient(ipAddress.Trim(), "pi", "PASSWORD");
            try // Try connecting to the pi
            {
                pi.Connect();
            }
            catch (Exception)
            { // At this point either the password or IP is incorrect, we will assume it is the IP
                lblTemp.Content = "Invalid IP Address.";
                lblTime.Content = "";
                tbFilepath.IsReadOnly = false;
                tbIpAddress.IsReadOnly = false;
                btnVerify.IsEnabled = true;
                btnChange.IsEnabled = false;
                timer.Stop();
                return false;
            }
            try // After connectioning we will try to run the command
            {
                if(tbFilepath.Text.Trim() == "") // if no file was given
                {
                    throw new Exception(); // throw exception
                }
                temp = pi.RunCommand("python " + tbFilepath.Text.Trim()).Execute();
                if(temp == "") // If the command did not return anything
                {
                    throw new Exception(); // The file path was wrong
                }
                // The file path may still be wrong but the user would notice based on what was returned
                
            } catch(Exception) // Tell user the filepath was at fault
            {
                lblTemp.Content = "Filepath is incorrect.";
                tbFilepath.IsReadOnly = false;
                tbIpAddress.IsReadOnly = false;
                btnVerify.IsEnabled = true;
                btnChange.IsEnabled = false;
                timer.Stop();
                return false;
            }
            pi.Disconnect();
            lblTemp.Content = Math.Round(double.Parse(temp.Trim()), 2) + "°F" ;
            lblTime.Content = DateTime.Now;
            return true;
        }

        /// <summary>
        /// On click verify the IP and filepath and update them temp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            lblTemp.IsEnabled = true;
            if (updateTemp(tbIpAddress.Text))
            {
                timer.Start();
                tbFilepath.IsReadOnly = true;
                tbIpAddress.IsReadOnly = true;
                btnVerify.IsEnabled = false;
                btnChange.IsEnabled = true;
            }
        }

        /// <summary>
        /// Every 15 minutes after establishing a connection update the temp
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer_Tick(object sender, EventArgs e)
        {
            if(!updateTemp(tbIpAddress.Text))
            {
                lblTemp.Content = "Invalid IP Address.";
                lblTime.Content = "";
                tbFilepath.IsReadOnly = false;
                tbIpAddress.IsReadOnly = false;
                btnVerify.IsEnabled = true;
                btnChange.IsEnabled = false;
                timer.Stop();
            }
        }

        /// <summary>
        /// On click kill the timer and reenable the textboxes to allow user to change them
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            lblTemp.Content = "";
            lblTime.Content = "";
            tbFilepath.IsReadOnly = false;
            tbIpAddress.IsReadOnly = false;
            btnVerify.IsEnabled = true;
            btnChange.IsEnabled = false;
            timer.Stop();
        }
    }
}
