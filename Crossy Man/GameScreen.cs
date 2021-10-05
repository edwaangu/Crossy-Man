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
    public partial class GameScreen : UserControl
    {
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

        public GameScreen()
        {
            InitializeComponent();
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
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
                if(l.type == 0)
                {
                    // Check to make sure the player can't run into trees
                    foreach(Tree t in l.trees)
                    {
                        Rectangle treeRect = new Rectangle(t.x, t.y, 40, 40);
                        if(treeRect.IntersectsWith(new Rectangle(p.x + p.speed, p.y, p.size, p.size)))
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
                else if(l.type == 1)
                {
                    // Create more cars if necessary
                    l.makeCars();
                    foreach(Car c in l.cars)
                    {
                        // Move the car
                        c.Move();

                        // Check if the car collides with the player, resulting in a gameover
                        Rectangle carRect = new Rectangle(c.x, c.y, c.width, 50);
                        if(carRect.IntersectsWith(new Rectangle(p.x, p.y, p.size, p.size))){
                            gameover = true;
                        }
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
                        Rectangle logRect = new Rectangle(log.x, log.y, log.width, 40);
                        if (logRect.IntersectsWith(new Rectangle(p.x, p.y, p.size, p.size)))
                        {
                            if (deadFromWater)
                            {
                                p.x += log.speed * log.direction;
                            }
                            deadFromWater = false;
                        }
                    }

                    Rectangle layerRect = new Rectangle(0, l.y, this.Width, 60);
                    if (layerRect.IntersectsWith(new Rectangle(p.x, p.y, p.size, p.size)) && deadFromWater)
                    {
                        // If the player is not colliding with a log but with the water, then result in a game over
                        gameover = true;
                    }
                }
            }

            // Update Camera and Game
            if (!gameover)
            {
                // Move the camera down slowly
                cameraY++;

                // If the player exceeds a certain distance move the camera down quickly to keep up
                while(cameraY + p.y < 300)
                {
                    cameraY += 10;
                }

                // Result in a game-over if the player's middle touches the bottom
                if(cameraY + p.y >= this.Height - (p.size / 2))
                {
                    gameover = true;
                }

                // Result in a game-over if the player goes off the screen horizontally (only happens when on a log)
                if(p.x + p.size < 0 || p.x > this.Width)
                {
                    gameover = true;
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
            if (layers[0].y + cameraY > this.Height)
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

                if(l.type == 0)
                {
                    // Trees
                    foreach(Tree t in l.trees)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.ForestGreen), t.x, t.y + cameraY, 40, 40);
                    }
                }
                else if(l.type == 1)
                {
                    // Cars
                    foreach(Car car in l.cars)
                    {
                        // Show a different color for each type of car
                        SolidBrush carBrush = new SolidBrush(Color.Red);
                        if(car.type == 1)
                        {
                            carBrush.Color = Color.Blue;
                        }
                        else if (car.type == 2)
                        {
                            carBrush.Color = Color.Yellow;
                        }
                        e.Graphics.FillRectangle(carBrush, car.x, car.y + cameraY, car.width, 50);
                    }
                }
                else if(l.type == 2)
                {
                    // Logs
                    foreach(Log log in l.logs)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.Brown), log.x, log.y + cameraY, log.width, 40);
                    }
                }
            }

            if (!gameover || (gameover && cameraY + p.y >= this.Height - (p.size / 2) && birdY < p.y + cameraY))
            {
                // Player Rectangle
                e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(p.x, p.y + cameraY, p.size, p.size));
            }

            // Bird:
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(p.x - 10, birdY, 20, 80));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(p.x - 40, birdY + 40, 80, 20));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(p.x - 40, birdY + 20, 20, 20));
            e.Graphics.FillRectangle(new SolidBrush(Color.Black), new Rectangle(p.x + 20, birdY + 20, 20, 20));
            e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(p.x - 10, birdY + 60, 20, 20));
            e.Graphics.FillRectangle(new SolidBrush(Color.Orange), new Rectangle(p.x - 5, birdY + 80, 10, 15));
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (!gameover)
            {
                keys[Convert.ToInt32(e.KeyCode)] = true;
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
