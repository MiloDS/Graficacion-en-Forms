using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

/* Alexander Vargas Mejia
 * 23/10/2024
 * Graficacion en forms
 */

namespace GRAVEDAD_
{
    public partial class frmPrincipal : Form
    {
        private Random oRandom;        

        // Constantes para la simulación
        private const float fGravedad = 0.5f;
        private const float fReboteVertical = -0.8f;
        private const float fReboteHorizontal = -0.7f;
        private const float fFriccion = 0.97f;
        private const float fUmbralVelocidad = 0.1f;

        private int nFps = 120;

        // Lista para almacenar los círculos generados
        private List<(clsCirculo oCirculo, float fVelocidadX, float fVelocidadY)> lCirculos;

        public frmPrincipal()
        {
            InitializeComponent();
            lCirculos = new List<(clsCirculo, float, float)>();
            oRandom = new Random();
            // Configuración del timer        
            tmrMov.Interval = 1000 / nFps;
            this.DoubleBuffered = true; // Activar doble buffer para evitar parpadeos
        }

        // Generar un nuevo círculo
        private void btnGenerar_Click(object sender, EventArgs e)
        {
            // Generar un nuevo círculo con coordenadas aleatorias
            clsCirculo oCirculo = new clsCirculo(oRandom.Next(100, 700), oRandom.Next(80, 100), 30, Color.DeepPink,densidad:100.0f);

            // Generar velocidades iniciales
            float pfVelocidadX = (float)(oRandom.NextDouble() * 6 - 3); // Movimiento horizontal aleatorio
            float pfVelocidadY = 5;

            // Añadir el círculo y sus velocidades a la lista
            lCirculos.Add((oCirculo, pfVelocidadX, pfVelocidadY));

            // Forzar redibujado del formulario
            Invalidate(); // Asegura que se dibuje el nuevo círculo

            if (!tmrMov.Enabled)
            {
                tmrMov.Start(); // Iniciar el timer 
            }
        }

        // Manejar las colisiones y el movimiento
        private void tmr_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < lCirculos.Count; i++)
            {
                var (oCirculo, fVelocidadX, fVelocidadY) = lCirculos[i];

                // Agregar la gravedad Solo en el eje Y
                fVelocidadY += fGravedad;

                // Movimiento del círculo
                oCirculo.Y += fVelocidadY;
                oCirculo.X += fVelocidadX;

                // Verificar colisiones con los bordes del formulario
                if (oCirculo.Y + oCirculo.Radio * 2 >= this.ClientSize.Height) // Colisión con el fondo
                {
                    oCirculo.Y = this.ClientSize.Height - oCirculo.Radio * 2;
                    fVelocidadY *= fReboteVertical;
                    fVelocidadX *= fFriccion; // Reducir velocidad en X por fricción
                }
                if (oCirculo.Y <= 0) // Colisión con el techo
                {
                    oCirculo.Y = 0;
                    fVelocidadY *= fReboteVertical;
                }
                if (oCirculo.X <= 0) // Colisión con la pared izquierda
                {
                    oCirculo.X = 0;
                    fVelocidadX *= fReboteHorizontal;
                }
                if (oCirculo.X + oCirculo.Radio * 2 >= this.ClientSize.Width) // Colisión con la pared derecha
                {
                    oCirculo.X = this.ClientSize.Width - oCirculo.Radio * 2;
                    fVelocidadX *= fReboteHorizontal;
                }

                // Verificar colisiones entre círculos
                for (int j = 0; j < lCirculos.Count; j++)
                {
                    if (i != j)
                    {
                        var (otroCirculo, velocidadOtroX, velocidadOtroY) = lCirculos[j];

                        // Detectar colisión
                        if (VerificarColision(oCirculo, otroCirculo))
                        {
                            // Calcular la distancia entre los centros de los dos círculos
                            float distX = otroCirculo.X - oCirculo.X;
                            float distY = otroCirculo.Y - oCirculo.Y;
                            float distancia = (float)Math.Sqrt(distX * distX + distY * distY);

                            // Si la distancia es 0, evitamos división por cero
                            if (distancia == 0)
                                distancia = 1;

                            // Vector normalizado de colisión
                            float nx = distX / distancia;
                            float ny = distY / distancia;

                            // Separar los círculos para evitar que se queden pegados
                            float superposicion = (oCirculo.Radio + otroCirculo.Radio) - distancia;

                            if (superposicion < 0)
                                superposicion = 0;

                            oCirculo.X -= nx * superposicion / 2;
                            oCirculo.Y -= ny * superposicion / 2;
                            otroCirculo.X += nx * superposicion / 2;
                            otroCirculo.Y += ny * superposicion / 2;


                            // Calcular la nueva velocidad en el eje X tras la colisión
                            float fNewVelocidadX = ((oCirculo.Masa - otroCirculo.Masa) * fVelocidadX + 2 * otroCirculo.Masa * velocidadOtroX) / (oCirculo.Masa + otroCirculo.Masa);
                            float NewVelocidadOtroX = ((otroCirculo.Masa - oCirculo.Masa) * velocidadOtroX + 2 * oCirculo.Masa * fVelocidadX) / (oCirculo.Masa + otroCirculo.Masa);

                            // Calcular la nueva velocidad en el eje Y tras la colisión
                            float fNewVelocidadY = ((oCirculo.Masa - otroCirculo.Masa) * fVelocidadY + 2 * otroCirculo.Masa * velocidadOtroY) / (oCirculo.Masa + otroCirculo.Masa);
                            float NewVelocidadOtroY = ((otroCirculo.Masa - oCirculo.Masa) * velocidadOtroY + 2 * oCirculo.Masa * fVelocidadY) / (oCirculo.Masa + otroCirculo.Masa);

                            //intercambiar velocidades para simular el rebote
                            (fVelocidadX, velocidadOtroX) = (velocidadOtroX, fVelocidadX);
                            (fVelocidadY, velocidadOtroY) = (velocidadOtroY, fVelocidadY);
                            
                            fVelocidadX = fNewVelocidadX;
                            velocidadOtroX = NewVelocidadOtroX;
                            fVelocidadY = fNewVelocidadY;
                            velocidadOtroY = NewVelocidadOtroY;

                            // Actualizar la lista con las nuevas velocidades
                            lCirculos[i] = (oCirculo, fVelocidadX, fVelocidadY);
                            lCirculos[j] = (otroCirculo, velocidadOtroX, velocidadOtroY);
                        }
                    }
                }

                // Limitar las velocidades para evitar rebotes indefinidos
                if (Math.Abs(fVelocidadX) < fUmbralVelocidad) fVelocidadX = 0;
                if (Math.Abs(fVelocidadY) < fUmbralVelocidad) fVelocidadY = 0;

                // Actualizar las velocidades en la lista
                lCirculos[i] = (oCirculo, fVelocidadX, fVelocidadY);
            }

            // Redibujar la pantalla
            Invalidate(); // Redibuja el formulario con los círculos en nuevas posiciones

            // Detener el timer si todas las bolas están quietas
            if (lCirculos.TrueForAll(c => Math.Abs(c.fVelocidadX) < fUmbralVelocidad && Math.Abs(c.fVelocidadY) < fUmbralVelocidad))
            {
                tmrMov.Stop();
            }
        }

        // Método para detectar colisión entre dos círculos
        private bool VerificarColision(clsCirculo c1, clsCirculo c2)
        {
            float distancia = (float)Math.Sqrt(Math.Pow(c2.X - c1.X, 2) + Math.Pow(c2.Y - c1.Y, 2));
            return distancia < (c1.Radio + c2.Radio);
        }

        // Sobrescribir el método OnPaint para dibujar los círculos
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Dibujar todos los círculos de la lista
            foreach (var (circulo, _, _) in lCirculos)
            {
                circulo.Draw(e.Graphics); // Dibuja el círculo actual
            }
        }
    }
}

