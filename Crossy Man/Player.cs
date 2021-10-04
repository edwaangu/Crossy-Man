using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crossy_Man
{
    class Player
    {
        public int x, y, size, speed;
        public bool canMoveRight, canMoveLeft, canMoveUp, canMoveDown;

        public Player(int _x, int _y, int _size, int _speed)
        {
            x = _x;
            y = _y;
            size = _size;
            speed = _speed;
        }

        public void Move(bool leftward, bool rightward, bool upward, bool downward)
        {
            if (leftward && x - speed > 0 && canMoveLeft)
            {
                x -= speed;
            }
            if (rightward && x + speed < 1280 && canMoveRight)
            {
                x += speed;
            }
            if (upward && canMoveUp)
            {
                y -= speed;
            }
            if (downward && canMoveDown)
            {
                y += speed;
            }
        }
    }
}
