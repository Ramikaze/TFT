using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Twitterizer;
using System.Diagnostics;
using System.IO;

namespace Twitter_Followings_Transfert
{
    public partial class MainForm : Form
    {
        private static OAuthTokens tokens;
        private static string ScreenName;
        private static decimal UserID;
        private static string requestToken;

        private static UserIdCollection followings;
        private static UserIdCollection myFollowings;

        private static TwitterUser transfertUser;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (Microsoft.VisualBasic.Information.IsNumeric(textBox1.Text) && textBox1.TextLength == 7)
            {
                button3.Enabled = true;
            }
            else button3.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            requestToken = OAuthUtility.GetRequestToken("CpcWcxvTAXoiM7ibIg", "Fellor49EGKkeCWiqvw9CPHktl1uiRjT2DiouBavA", "oob").Token;
            Process.Start(OAuthUtility.BuildAuthorizationUri(requestToken).AbsoluteUri);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string pin = textBox1.Text;
            if (pin.Length != 7)
            {
                return;
            }

            OAuthTokenResponse accessToken;
            try
            {
                accessToken = OAuthUtility.GetAccessToken("CpcWcxvTAXoiM7ibIg", "Fellor49EGKkeCWiqvw9CPHktl1uiRjT2DiouBavA", requestToken, pin);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            tokens = new OAuthTokens();
            tokens.AccessToken = accessToken.Token;
            tokens.AccessTokenSecret = accessToken.TokenSecret;
            tokens.ConsumerKey = "CpcWcxvTAXoiM7ibIg";
            tokens.ConsumerSecret = "Fellor49EGKkeCWiqvw9CPHktl1uiRjT2DiouBavA";
            ScreenName = accessToken.ScreenName;
            UserID = accessToken.UserId;

            pictureBox1.ImageLocation = TwitterUser.Show(UserID).ResponseObject.ProfileImageLocation;
            label2.Text = ScreenName;

            TwitterResponse<UserIdCollection> result = TwitterFriendship.FriendsIds(tokens);
            if (result.Result == RequestResult.Success)
            {
                myFollowings = result.ResponseObject;
            }
            else
            {
                MessageBox.Show("Can't find you. " + result.ErrorMessage);
            }

            groupBox4.Enabled = false;
            groupBox2.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox3.Text == "") return;
            UsersIdsOptions options = new UsersIdsOptions();
            options.UserId = transfertUser.Id;
            TwitterResponse<UserIdCollection> result = TwitterFriendship.FriendsIds(tokens, options);
            if (result.Result == RequestResult.Success)
            {
                followings = result.ResponseObject;
            }
            else
            {
                MessageBox.Show("Can't reach the followings : " + result.ErrorMessage);
                return;
            }

            groupBox5.Enabled = false;
            button1.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string userSearch = textBox2.Text;
            TwitterResponse<TwitterUser> result = TwitterUser.Show(textBox2.Text);
            if (result.Result == RequestResult.Success)
            {
                textBox3.Text = result.ResponseObject.ScreenName;
                transfertUser = result.ResponseObject;
            }
            else
            {
                MessageBox.Show("Can't find user : " + textBox2.Text);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (decimal id in followings)
            {
                if (myFollowings.Contains(id)) continue;
                try
                {
                    TwitterResponse<TwitterUser> result = TwitterFriendship.Create(tokens, id);
                    if (result.Result == RequestResult.Success)
                    {

                    }
                    else
                    {
                        MessageBox.Show("Can't follow the user " + id + " : " + result.ErrorMessage);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + " (if you see this error again restart the application)");
                }
            }
            MessageBox.Show("Transfert finished !");
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
        }
    }
}
