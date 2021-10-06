using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossy_Man
{
    class Car
    {
        // Initalize car variables
        public int x, y, width, direction, speed, type;

        public Car(int _x, int _y, int _width, int _direction, int _speed, int _type)
        {
            x = _x;
            y = _y;
            width = _width;
            direction = _direction;
            speed = _speed;
            type = _type;
        }
        
        public void Move()
        {
            // Move the car
            x += speed * direction;
        }
    }
}
