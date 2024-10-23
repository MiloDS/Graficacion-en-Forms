using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRAVEDAD_
{
    public class clsCirculo
    {
        // Propiedades privadas
        private float x, y, radio;
        public float Masa { get; private set;}
        public float Densidad { get; private set;}
        private Color color;

        // Propiedades públicas para acceder a las posiciones y el radio
        public float X { get { return x; } set { x = value; } }
        public float Y { get { return y; } set { y = value; } }
        public float Radio { get { return radio; } }

        // Constructor del círculo
        public clsCirculo(float x, float y, float radio, Color color, float densidad)
        {
            this.x = x;
            this.y = y;
            this.radio = radio;
            this.color = color;
            this.Masa = densidad * (float)(Math.PI * Math.Pow(radio, 2));
            this.Densidad = densidad;
        }

        // Método para dibujar el círculo
        public void Draw(Graphics g)
        {
            g.FillEllipse(new SolidBrush(this.color), x, y, radio * 2, radio * 2);
        }

        // Método para obtener los límites del círculo
        public Rectangle GetBounds()
        {
            return new Rectangle((int)x, (int)y, (int)(radio * 2), (int)(radio * 2));
        }
    }
}
