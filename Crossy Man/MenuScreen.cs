using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crossy_Man
{
    public partial class MenuScreen : UserControl
    {
        public MenuScreen()
        {
            InitializeComponent();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            Form f = this.FindForm();
            f.Controls.Remove(this);
            GameScreen gs = new GameScreen();
            f.Controls.Add(gs);
            gs.Focus();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void MenuScreen_Load(object sender, EventArgs e)
        {

            Form f = this.FindForm();
            this.Focus(); // focus
            this.Location = new Point((f.Width - 1280) / 2, (f.Height - 720) / 2);
        }

        private void playButton_Enter(object sender, EventArgs e)
        {
            playButton.FlatAppearance.BorderSize = 10;
            exitButton.FlatAppearance.BorderSize = 5;
        }

        private void exitButton_Enter(object sender, EventArgs e)
        {
            playButton.FlatAppearance.BorderSize = 5;
            exitButton.FlatAppearance.BorderSize = 10;
        }
    }
}
