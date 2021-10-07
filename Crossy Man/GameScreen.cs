using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Media;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crossy_Man
{
    public partial class GameScreen : UserControl
    {
        // Sounds
        SoundPlayer jumpSound = new SoundPlayer(Properties.Resources.jump);
        SoundPlayer deathSound = new SoundPlayer(Properties.Resources.death);
        int deathSoundPlayed = 0;

        // Randgen
        public static Random randGen = new Random();

        // Main game variables
        int score = 0;
        bool gameover = false;

        // Key presses
        bool[] keys = new bool[256];

        // Player
        Player p = new Player(620, 625, 40, 60);
        int maxPlayerY = 625;
        bool jumping = false;
        int pgravity = -10;
        int pgravityY = 0;
        int distY = 0;
        int distX = 0;
        int distXdir = 0;
        int distYdir = 0;

        // Bird
        int birdY = -100;

        // Layers and Obstacles
        List<Layer> layers = new List<Layer>();
        int cameraY = 0;
        int maxLayerY = 0;
        int maxLayerID = 0;
        int maxLayerType = 0;
        int amtOfSameLayer = 0;

        void decideOnLayerType()
        {
            // Only change the type of layer once enough of same layer has passed
            if(amtOfSameLayer <= 0)
            {
                // Make sure the next set of layers is different than the last
                int lastLayer = maxLayerType;
                while (lastLayer == maxLayerType)
                {
                    int theLayer = randGen.Next(1, 20);
                    if (theLayer >= 15)
                    { //40% chance for logs if road was last, 54% if grass was last
                        maxLayerType = 2;
                    }
                    else if (theLayer >= 10)
                    { //35% chance for road if logs were last, 45% if grass was last
                        maxLayerType = 1;
                    }
                    else
                    { //65% for grass if logs were last, 60% if road was last
                        maxLayerType = 0;
                    }

                    // Allow the amount of same layers to be between 1 and 5 plus the amount of score by 50s
                    amtOfSameLayer = randGen.Next(1, 6 + Convert.ToInt32(Math.Floor(Convert.ToDouble(score) / 50)));
                }
            }
            amtOfSameLayer--;
        }

        void setupGame()
        {
            this.Focus();
            deathSoundPlayed = 0;

            // Reset layers
            maxLayerY = this.Height - 60;
            maxLayerID = 0;
            maxLayerType = 0;
            layers.Clear();
            amtOfSameLayer = 7;
            while (maxLayerY > -60)
            {
                // Add layers until they reach the top of the screen beyond view
                layers.Add(new Layer(maxLayerY, maxLayerType, maxLayerID));
                decideOnLayerType();
                maxLayerY -= 60;
                maxLayerID++;
            }

            // Reset Camera
            cameraY = 0;
            birdY = -100;

            // Reset player position and player variables
            p.y = this.Height - 120 + 10;
            p.x = this.Width / 2 - 10;
            maxPlayerY = this.Height - 120 + 10;

            // Reset main game variables
            score = 0;
            gameover = false;

            // Hide any gameover buttons
            retryButton.Visible = false;
            menuButton.Visible = false;
        }

        bool checkForDie()
        {
            bool hasDied = false;

            foreach (Layer l in layers)
            {
                if (l.type == 1)
                {
                    // Create more cars if necessary
                    foreach (Car c in l.cars)
                    {
                        // Check if the car collides with the player
                        Rectangle carRect = new Rectangle(c.x, c.y, c.width, 50);
                        if (carRect.IntersectsWith(new Rectangle(p.x, p.y, p.size, p.size)))
                        {
                            hasDied = true;
                        }
                    }
                }
                else if (l.type == 2)
                {
                    // Create more logs if necessary
                    bool deadWater = true;
                    foreach (Log log in l.logs)
                    {
                        // Check if the log is colliding with the player, if so, move the player along
                        if (!gameover)
                        {
                            Rectangle logRect = new Rectangle(log.x, log.y, log.width, 40);
                            if (logRect.IntersectsWith(new Rectangle(p.x, p.y, p.size, p.size)))
                            {
                                deadWater = false;
                            }
                        }
                    }
                    Rectangle layerRect = new Rectangle(0, l.y, this.Width, 60);
                    if (layerRect.IntersectsWith(new Rectangle(p.x, p.y, p.size, p.size)) && deadWater){
                        // If the player is not colliding with a log but with the water, then result in a game over
                        hasDied = true;
                    }
                }
            }

            return hasDied;
        }

        public GameScreen()
        {
            InitializeComponent();
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            // Center the screen
            Form f = this.FindForm();
            this.Focus(); // focus
            this.Location = new Point((f.Width - 1280) / 2, (f.Height - 720) / 2);

            // Run the setupGame function
            setupGame();
        }

        private void gameTick_Tick(object sender, EventArgs e)
        {
            // Update booleans if player can move in a certain direction due to trees
            p.canMoveRight = true;
            p.canMoveLeft = true;
            p.canMoveUp = true;
            p.canMoveDown = true;

            // Update layers
            foreach (Layer l in layers)
            {
                if (l.type == 0 && !gameover)
                {
                    // Check to make sure the player can't run into trees
                    foreach (Tree t in l.trees)
                    {
                        Rectangle treeRect = new Rectangle(t.x, t.y, 40, 40);
                        if (treeRect.IntersectsWith(new Rectangle(p.x + p.speed, p.y, p.size, p.size)))
                        {
                            p.canMoveRight = false;
                        }
                        if (treeRect.IntersectsWith(new Rectangle(p.x - p.speed, p.y, p.size, p.size)))
                        {
                            p.canMoveLeft = false;
                        }
                        if (treeRect.IntersectsWith(new Rectangle(p.x, p.y + p.speed, p.size, p.size)))
                        {
                            p.canMoveDown = false;
                        }
                        if (treeRect.IntersectsWith(new Rectangle(p.x, p.y - p.speed, p.size, p.size)))
                        {
                            p.canMoveUp = false;
                        }
                    }
                }
                else if (l.type == 1)
                {
                    // Create more cars if necessary
                    l.makeCars();
                    foreach (Car c in l.cars)
                    {
                        // Move the car
                        c.Move();
                    }
                }
                else if (l.type == 2)
                {
                    // Create more logs if necessary
                    l.makeLogs();
                    bool deadFromWater = true;
                    foreach (Log log in l.logs)
                    {
                        // Move the logs
                        log.Move();

                        // Check if the log is colliding with the player, if so, move the player along
                        if (!gameover)
                        {
                            Rectangle logRect = new Rectangle(log.x, log.y, log.width, 40);
                            if (logRect.IntersectsWith(new Rectangle(p.x, p.y, p.size, p.size)))
                            {
                                if (deadFromWater && distXdir == 0 && distYdir == 0)
                                {
                                    p.x += log.speed * log.direction;
                                }
                                deadFromWater = false;
                            }
                        }
                    }
                }
            }

            // Update Camera and Game
            if (!gameover)
            {
                // Move the camera down slowly
                cameraY++;

                // If the player exceeds a certain distance move the camera down quickly to keep up
                if(cameraY + p.y < 300)
                {
                    cameraY += 10;
                }
                if (cameraY + p.y < 50)
                {
                    cameraY += 60;
                }

                // Result in a game-over if the player's middle touches the bottom
                if (cameraY + p.y >= this.Height - (p.size / 2))
                {
                    gameover = true;
                    if (deathSoundPlayed == 0)
                    {
                        deathSoundPlayed = 1;
                    }
                }

                // Result in a game-over if the player goes off the screen horizontally (only happens when on a log)
                if(p.x + p.size < 0 || p.x > this.Width)
                {
                    gameover = true;
                    if (deathSoundPlayed == 0)
                    {
                        deathSoundPlayed = 1;
                    }
                }
            }

            // Add more layers if the camera changes position to exceed a certain amount
            while (maxLayerY + cameraY > -60)
            {
                layers.Add(new Layer(maxLayerY, maxLayerType, maxLayerID));
                decideOnLayerType();
                maxLayerY -= 60;
                maxLayerID++;
            }

            // Remove a layer if beyond the screen
            if (layers[0].y + cameraY > this.Height + 120)
            {
                layers.RemoveAt(0);
            }

            // Scoring
            scoreLabel.Text = $"{score}";
            if(p.y < maxPlayerY)
            {
                maxPlayerY = p.y;
                score++;
            }

            // Game-Over related stuff
            if (gameover)
            {
                // Check if we could use the bird due to the player going off the screen vertically
                if (cameraY + p.y >= this.Height - (p.size / 2))
                {
                    birdY += 50;
                    if(birdY > this.Height)
                    {
                        retryButton.Visible = true;
                        menuButton.Visible = true;
                    }
                    else
                    {
                        retryButton.Visible = false;
                        menuButton.Visible = false;
                    }
                }
                else
                {
                    retryButton.Visible = true;
                    menuButton.Visible = true;
                }
                retryButton.Enabled = true;
                menuButton.Enabled = true;
            }
            else
            {
                retryButton.Enabled = false;
                menuButton.Enabled = false;
            }

            if (jumping)
            {
                pgravityY += pgravity;
                pgravity++;
                distX += 6 * distXdir;
                distY += 6 * distYdir;

                if(pgravityY > 0)
                {
                    pgravity = -5;
                    pgravityY = 0;
                    jumping = false;
                    distXdir = 0;
                    distYdir = 0;
                    distX = 0;
                    distY = 0;
                }
            }
            if((distXdir == 0 && distYdir == 0) && checkForDie())
            {
                gameover = true;
                if (deathSoundPlayed == 0)
                {
                    deathSoundPlayed = 1;
                }
            }

            if(deathSoundPlayed == 1)
            {
                deathSoundPlayed = 2;
                deathSound.Play();
            }
            

            // Refresh
            this.Refresh();
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush c = new SolidBrush(Color.White);
            foreach(Layer l in layers)
            {
                // Base layer color (greens for grass, dark grat for roads, light blue for logs
                switch (l.type)
                {
                    case 0:
                        c.Color = l.id % 2 == 0 ? Color.FromArgb(60, 236, 9) : Color.FromArgb(50, 196, 8);
                        break;
                    case 1:
                        c.Color = Color.FromArgb(36, 36, 36);
                        break;
                    case 2:
                        c.Color = Color.SkyBlue;
                        break;
                }

                e.Graphics.FillRectangle(c, new Rectangle(0, l.y + cameraY, this.Width, 60));

            }

            for(int i = layers.Count - 1;i > 0;i--)
            {
                if (layers[i].type == 0)
                {
                    // Trees
                    foreach (Tree t in layers[i].trees)
                    {
                        e.Graphics.DrawImage(Properties.Resources.tree, t.x + 3, t.y + cameraY - 37);
                    }
                }
                else if (layers[i].type == 1)
                {
                    // Cars
                    foreach (Car car in layers[i].cars)
                    {
                        // Show a different car for each type of car
                        if (car.type == 1)
                        {
                            e.Graphics.DrawImage(Properties.Resources.bluecar, new Rectangle(car.x + (77 * (car.direction == 1 ? 0 : 1)), car.y - 32 + cameraY, 77 * car.direction, 77));
                        }
                        else if (car.type == 2)
                        {
                            e.Graphics.DrawImage(Properties.Resources.yellowcar, new Rectangle(car.x + (105 * (car.direction == 1 ? 0 : 1)), car.y - 25 + cameraY, 105 * car.direction, 70));
                        }
                        else
                        {
                            e.Graphics.DrawImage(Properties.Resources.redcar, new Rectangle(car.x + (105 * (car.direction == 1 ? 0 : 1)), car.y - 25 + cameraY, 105 * car.direction, 70));
                        }

                    }
                }
                else if (layers[i].type == 2)
                {
                    // Logs
                    foreach (Log log in layers[i].logs)
                    {
                        // Show different logs for each log width type
                        if(log.width == 49)
                        {
                            e.Graphics.DrawImage(Properties.Resources.logveryshort, log.x, log.y + cameraY + 10);
                        }
                        else if (log.width == 77)
                        {
                            e.Graphics.DrawImage(Properties.Resources.logshort, log.x, log.y + cameraY + 10);
                        }
                        else if (log.width == 105)
                        {
                            e.Graphics.DrawImage(Properties.Resources.log, log.x, log.y + cameraY + 10);
                        }
                        else if (log.width == 140)
                        {
                            e.Graphics.DrawImage(Properties.Resources.loglong, log.x, log.y + cameraY + 10);
                        }
                    }
                }
            }

            if (!gameover || (gameover && cameraY + p.y >= this.Height - (p.size / 2) && birdY < p.y + cameraY) || (distXdir != 0 || distYdir != 0))
            {
                // Player Rectangle
                e.Graphics.DrawImage(Properties.Resources.player, p.x + distX, p.y + distY - 20 + cameraY + pgravityY);
            }

            // Bird:
            if (gameover)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(p.x - 10, birdY, 20, 80));
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(p.x - 40, birdY + 40, 80, 20));
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(p.x - 40, birdY + 20, 20, 20));
                e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(p.x + 20, birdY + 20, 20, 20));
                e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(p.x - 10, birdY + 60, 20, 20));
                e.Graphics.FillRectangle(new SolidBrush(Color.Orange), new Rectangle(p.x - 5, birdY + 80, 10, 15));
            }
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (!gameover)
            {
                keys[Convert.ToInt32(e.KeyCode)] = true;
                if ((keys[38] || keys[39] || keys[37] || keys[40]) && !checkForDie())
                {
                    jumping = true;
                    jumpSound.Play();
                    pgravity = -5;
                    pgravityY = 0;
                    if (keys[37]) { distX = 60; distY = 0; distYdir = 0; distXdir = -1; }
                    if (keys[39]) { distX = -60; distY = 0; distYdir = 0; distXdir = 1; }
                    if (keys[38]) { distY = 60; distX = 0; distXdir = 0; distYdir = -1; }
                    if (keys[40]) { distY = -60; distX = 0; distXdir = 0; distYdir = 1; }

                    if ((!p.canMoveLeft && keys[37]) || (!p.canMoveRight && keys[39]))
                    {
                        distX = 0;
                        distXdir = 0;
                    }
                    if ((!p.canMoveUp && keys[38]) || (!p.canMoveDown && keys[40]))
                    {
                        distY = 0;
                        distYdir = 0;
                    }
                }
                else if((keys[38] || keys[39] || keys[37] || keys[40]) && checkForDie())
                {
                    gameover = true;
                    if (deathSoundPlayed == 0)
                    {
                        deathSoundPlayed = 1;
                    }
                }
                p.Move(keys[37], keys[39], keys[38], keys[40]);
                keys[Convert.ToInt32(e.KeyCode)] = false;
            }
        }


        private void retryButton_Click(object sender, EventArgs e)
        {
            setupGame();
            this.Focus();
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            Form f = this.FindForm();
            f.Controls.Remove(this);
            MenuScreen ms = new MenuScreen();
            f.Controls.Add(ms);
        }

        private void retryButton_Enter(object sender, EventArgs e)
        {
            retryButton.FlatAppearance.BorderSize = 10;
            menuButton.FlatAppearance.BorderSize = 5;
        }

        private void menuButton_Enter(object sender, EventArgs e)
        {
            retryButton.FlatAppearance.BorderSize = 5;
            menuButton.FlatAppearance.BorderSize = 10;
        }
    }
}
