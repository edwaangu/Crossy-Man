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
    public partial class Form1 : Form
    {

        /**
         * CROSSY BLOCK (Last minute name-change, old name is Crossy Man)
         *  Created by Ted Angus
         * 
         * - use the arrow keys to move
         * - don't get hit by cars!
         * - don't fall in the water! use the logs to tread safely
         * - don't get carried off screen by a log though
         * 
         * Lines of Code (TOTAL): 750+ (Counting duplicated system code: 810+)
             * GameScreen: 512
             * Layer: 124
             * MenuScreen: 52
             * Form1: 50+
             * Car: 31
             * Log: 29
             * Tree: 21
         * Time Taken: 6+ hours approx.
         * Date period: 6/08/2021 - 6/10/2021
         * Bugs squashed: many
         * Bugs remaining: some probably
         * 
         * Things I never added:
         * - Train obstacle
         * - Lilypads on the river to jump on
         * - An actual character that isn't a block
        */

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MenuScreen ms = new MenuScreen();
            this.Controls.Add(ms);
        }
    }
}
