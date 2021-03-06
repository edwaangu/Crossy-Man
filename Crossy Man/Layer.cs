using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossy_Man
{
    class Layer
    {
        /* 
         Types:
        0 - Regular Ground
        1 - Road
        2 - River
         
         */

        // Basic layer variables
        public int y, type, id;


        // type 0: grass plains variables
        public List<Tree> trees = new List<Tree>();

        // type 1: roadway variables
        public List<Car> cars = new List<Car>();
        public int carType = GameScreen.randGen.Next(1, 4);
        public int carDirection = GameScreen.randGen.Next(0, 2) == 1 ? 1 : -1;
        public int carSpeed = GameScreen.randGen.Next(5, 15);
        public int nextCar = 150;

        // typ 2: log variables
        public List<Log> logs = new List<Log>();
        public int logDirection = GameScreen.randGen.Next(0, 2) == 1 ? 1 : -1;
        public int logSpeed = GameScreen.randGen.Next(1, 8);
        public int nextLog = 0;

        public Layer(int _y, int _type, int _id)
        {
            // Initalize variables
            y = _y;
            type = _type;
            id = _id;

            if (type == 0)
            {
                // Add trees on both sides
                int treesLeft = GameScreen.randGen.Next(1, 5);
                int treesRight = GameScreen.randGen.Next(1, 5);

                for (int i = 0; i < treesLeft; i++)
                {
                    trees.Add(new Tree(i * 60 + 10, y + 10));
                }
                for (int i = 0; i < treesRight; i++)
                {
                    trees.Add(new Tree(1280 - 60 - i * 60 + 10, y + 10));
                }
            }
            else if(type == 1)
            {
                // Add some cars to start so the player can't just dash across a temporarily cleared roadway
                int theCarX = 0;
                while(theCarX < 1280)
                {
                    cars.Add(new Car(theCarX, y + 5, (carType == 1 ? 77 : 105), carDirection, carSpeed, carType));
                    theCarX += GameScreen.randGen.Next(250, 600) + (carType == 1 ? 77 : 105);
                }
            }
            else if (type == 2)
            {
                // Add some logs to start so the player doesn't have to wait long
                int theLogX = 0;
                while (theLogX < 1280)
                {
                    int logWidth = GameScreen.randGen.Next(1, 5);
                    logWidth = logWidth == 1 ? 140 : (logWidth == 2 ? 105 : (logWidth == 3 ? 77 : 49));
                    logs.Add(new Log(theLogX, y + 10, logWidth, logSpeed, logDirection));
                    theLogX += GameScreen.randGen.Next(30 + logWidth, 300 + logWidth);
                }
            }
        }

        public void makeCars()
        {
            // Decrease the amount of time until the next car can be initalized
            nextCar-=carSpeed;
            if(nextCar < 0)
            {
                // Create a new car
                nextCar = GameScreen.randGen.Next(250, 600) + (carType == 1 ? 77 : 105);
                cars.Add(new Car(carDirection == 1 ? -100 : 1380, y + 5, (carType == 1 ? 77 : 105), carDirection, carSpeed, carType));
            }

            // Remove cars that are off the screen
            if(cars[0].x < -200 || cars[0].x > 1480)
            {
                cars.RemoveAt(0);
            }
        }

        public void makeLogs()
        {
            // Decrease the amount of time until the next log can be initalized
            nextLog -= logSpeed;
            if (nextLog < 0)
            {
                // Create a new log of a certain width
                int logWidth = GameScreen.randGen.Next(1, 5);
                logWidth = logWidth == 1 ? 140 : (logWidth == 2 ? 105 : (logWidth == 3 ? 77 : 49));
                nextLog = GameScreen.randGen.Next(30 + logWidth, 300 + logWidth);
                logs.Add(new Log(logDirection == 1 ? -logWidth : 1280 + logWidth, y + 10, logWidth, logSpeed, logDirection));
            }

            // Remove logs that are off the screen
            if (logs[0].x < -300 || logs[0].x > 1580)
            {
                logs.RemoveAt(0);
            }
        }
    }
}
