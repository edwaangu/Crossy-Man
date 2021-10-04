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
        3 - Train Tracks
         
         */
        public int y, type, id;

        public List<Tree> trees = new List<Tree>();

        public List<Car> cars = new List<Car>();
        public int carType = GameScreen.randGen.Next(1, 4);
        public int carDirection = GameScreen.randGen.Next(0, 2) == 1 ? 1 : -1;
        public int carSpeed = GameScreen.randGen.Next(5, 15);
        public int nextCar = 0;

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
        }

        public void makeCars()
        {
            nextCar-=carSpeed;
            if(nextCar < 0)
            {
                nextCar = GameScreen.randGen.Next(200, 600);
                cars.Add(new Car(carDirection == 1 ? -100 : 1380, y + 5, 100, carDirection, carSpeed, carType));
            }
        }
    }
}
