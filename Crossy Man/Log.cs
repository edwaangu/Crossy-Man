using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossy_Man
{
    class Log
    {
        public int x, y, width, speed, direction;
        public Log(int _x, int _y, int _width, int _speed, int _direction)
        {
            x = _x;
            y = _y;
            width = _width;
            speed = _speed;
            direction = _direction;
        }

        public void Move()
        {
            x += speed * direction;
        }
    }
}
