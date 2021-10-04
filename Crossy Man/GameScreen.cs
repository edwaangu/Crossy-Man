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

        // Key presses
        bool[] keys = new bool[256];

        // Player
        Player p = new Player(620, 625, 40, 60);

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
                int lastLayer = 0;
                lastLayer += maxLayerType;
                while (lastLayer == maxLayerType)
                {
                    int theLayer = randGen.Next(1, 20);
                    if (theLayer == 20)
                    {
                        maxLayerType = 3;
                    }
                    else if (theLayer >= 15)
                    {
                        maxLayerType = 2;
                    }
                    else if (theLayer >= 10)
                    {
                        maxLayerType = 1;
                    }
                    else
                    {
                        maxLayerType = 0;
                    }
                    amtOfSameLayer = randGen.Next(1, 5);
                }
            }
            amtOfSameLayer--;
        }

        public GameScreen()
        {
            InitializeComponent();
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            maxLayerY = this.Height - 60;
            p.y = this.Height - 120 + 10;
            amtOfSameLayer = 7;
            while(maxLayerY > -60)
            {
                layers.Add(new Layer(maxLayerY, maxLayerType, maxLayerID));
                decideOnLayerType();
                maxLayerY -= 60;
                maxLayerID++;
            }

            if(layers[0].y + cameraY > this.Height)
            {
                layers.RemoveAt(0);
            }
        }

        private void gameTick_Tick(object sender, EventArgs e)
        {
            // Update booleans if player can move in a certain direction due to trees
            p.canMoveRight = true;
            p.canMoveLeft = true;
            p.canMoveUp = true;
            p.canMoveDown = true;

            foreach (Layer l in layers)
            {
                if(l.type == 0)
                {
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
                    l.makeCars();
                    foreach(Car c in l.cars)
                    {
                        c.Move();
                        Rectangle carRect = new Rectangle(c.x, c.y, c.width, 50);
                        if(carRect.IntersectsWith(new Rectangle(p.x, p.y, p.size, p.size))){
                            // Death
                        }
                    }
                }
            }

            // Update Camera
            cameraY++;
            while(cameraY + p.y < 300)
            {
                cameraY += 10;
            }

            // Add more layers if the camera changes things
            while (maxLayerY + cameraY > -60)
            {
                layers.Add(new Layer(maxLayerY, maxLayerType, maxLayerID));
                decideOnLayerType();
                maxLayerY -= 60;
                maxLayerID++;
            }

            // Refresh
            this.Refresh();
        }

        private void GameScreen_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush c = new SolidBrush(Color.White);
            foreach(Layer l in layers)
            {
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
                    case 3:
                        c.Color = Color.FromArgb(136, 136, 136);
                        break;
                }

                e.Graphics.FillRectangle(c, new Rectangle(0, l.y + cameraY, this.Width, 60));

                if(l.type == 0)
                {
                    foreach(Tree t in l.trees)
                    {
                        e.Graphics.FillRectangle(new SolidBrush(Color.ForestGreen), t.x, t.y + cameraY, 40, 40);
                    }
                }
                else if(l.type == 1)
                {
                    foreach(Car car in l.cars)
                    {
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
            }


            e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(p.x, p.y + cameraY, p.size, p.size));
        }

        private void GameScreen_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            keys[Convert.ToInt32(e.KeyCode)] = true;
            p.Move(keys[37], keys[39], keys[38], keys[40]);
            keys[Convert.ToInt32(e.KeyCode)] = false;
        }

        private void GameScreen_KeyUp(object sender, KeyEventArgs e)
        {
        }
    }
}
