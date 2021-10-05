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
        public int y, type, id;

        public List<Tree> trees = new List<Tree>();

        public List<Car> cars = new List<Car>();
        public int carType = GameScreen.randGen.Next(1, 4);
        public int carDirection = GameScreen.randGen.Next(0, 2) == 1 ? 1 : -1;
        public int carSpeed = GameScreen.randGen.Next(5, 15);
        public int nextCar = 0;

        public List<Log> logs = new List<Log>();
        public int logDirection = GameScreen.randGen.Next(0, 2) == 1 ? 1 : -1;
        public int logSpeed = GameScreen.randGen.Next(1, 8);
        public int nextLog = 0;

        public Layer(int _y, int _type, int _id)
        {
            y = _y;
            type = _type;
            id = _id;

            if (type == 0)
            {
                int treesLeft = GameScreen.randGen.Next(1, 4);
                int treesRight = GameScreen.randGen.Next(1, 4);

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
                int theCarX = 0;
                while(theCarX < 1280)
                {
                    cars.Add(new Car(theCarX, y + 5, 100, carDirection, carSpeed, carType));
                    theCarX += GameScreen.randGen.Next(200, 600);
                }
            }
            else if (type == 2)
            {
                int theLogX = 0;
                while (theLogX < 1280)
                {
                    int logWidth = GameScreen.randGen.Next(70, 190);
                    logs.Add(new Log(theLogX, y + 10, logWidth, logSpeed, logDirection));
                    theLogX += GameScreen.randGen.Next(30 + logWidth, 300 + logWidth);
                }
            }
        }

        public void makeCars()
        {
            nextCar-=carSpeed;
            if(nextCar < 0)
            {
                nextCar = GameScreen.randGen.Next(200, 600);
                cars.Add(new Car(carDirection == 1 ? -100 : 1380, y + 5, 100, carDirection, carSpeed, carType));
            }

            if(cars[0].x < -200 || cars[0].x > 1480)
            {
                cars.RemoveAt(0);
            }
        }

        public void makeLogs()
        {
            nextLog -= logSpeed;
            if (nextLog < 0)
            {
                int logWidth = GameScreen.randGen.Next(70, 190);
                nextLog = GameScreen.randGen.Next(30 + logWidth, 300 + logWidth);
                logs.Add(new Log(logDirection == 1 ? -logWidth : 1280 + logWidth, y + 10, logWidth, logSpeed, logDirection));
            }

            if (logs[0].x < -300 || logs[0].x > 1580)
            {
                logs.RemoveAt(0);
            }
        }
    }
}
